using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Fuzzy.Implementation;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorServiceIntegrationTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz();

        protected async Task<ActorService> GetActorService<T>(
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            ActorServiceSettings actorServiceSettings = null,
            IActorStateProvider actorStateProvider = null)
            where T : Actor
        {
            ActorService actorService = new ActorService(
                fuzzy.StatefulServiceContext(),
                ActorTypeInformation.Get(typeof(T)),
                actorFactory,
                null,
                actorStateProvider ?? new NullActorStateProvider(),
                actorServiceSettings);

            IStatefulUserServiceReplica statefulServiceReplica = actorService;
            await statefulServiceReplica.OnOpenAsync(ReplicaOpenMode.New, CancellationToken.None);
            await statefulServiceReplica.OnChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None);
            await statefulServiceReplica.RunAsync(CancellationToken.None);

            return actorService;
        }

        public class GetRemindersAsync : ActorServiceIntegrationTest
        {
            interface ITestableActor : IActor
            { }

            class TestActor : Actor, ITestableActor
            {
                public TestActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
                {
                }
            }

            public class WhenNoReminderIsRegistered : GetRemindersAsync
            {
                [Fact]
                public async Task ReturnEmptyResultIfNoRemindersAreRegistered()
                {
                    IActorService actorService = await GetActorService<TestActor>();

                    ContinuationToken continuationToken = null;
                    var page = await actorService.GetRemindersAsync(null, continuationToken, CancellationToken.None);

                    Assert.Empty(page.Items);
                    Assert.Null(continuationToken);
                }
            }

            public class WhenReminderAreRegister : GetRemindersAsync, IDisposable
            {
                static protected readonly int numberOfActors = fuzzy.Int32().Between(5, 10);
                static protected readonly int numberOfReminderPerActor = fuzzy.Int32().Between(10, 20);
                protected readonly List<ActorId> allActors;
                static readonly int newPageSize = fuzzy.Int32().Between(1, numberOfReminderPerActor - 1); // Reminder for a particular actor should be divided in at least two result pages
                static readonly int defaultPageSize = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();

                public WhenReminderAreRegister()
                {
                    allActors = new List<ActorId>();

                    for (int i = 0; i < numberOfActors; i++)
                    {
                        allActors.Add(new ActorId($"Actor_{i}"));
                    }
                    
                    ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.SetDefaultPageSize(newPageSize);
                }
                
                public virtual void Dispose()
                {
                    ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.SetDefaultPageSize(defaultPageSize);
                }

                protected (IActorStateProvider, Dictionary<ActorId, List<IActorReminder>>) CreateActorStateProviderWithReminders()
                {
                    IActorStateProvider actorStateProvider = new NullActorStateProvider();
                    var registeredReminders = new Dictionary<ActorId, List<IActorReminder>>();

                    foreach (var actorId in allActors)
                    {
                        registeredReminders[actorId] = new List<IActorReminder>();

                        for (int j = 0; j < numberOfReminderPerActor; j++)
                        {
                            var reminderMock = new Mock<IActorReminder>();
                            reminderMock.SetupGet(r => r.Name).Returns($"Reminder_{j}");
                            actorStateProvider.SaveReminderAsync(actorId, reminderMock.Object).Wait();

                            registeredReminders[actorId].Add(reminderMock.Object);
                        }
                    }

                    return (actorStateProvider, registeredReminders);
                }

                public class CancellationTokenIsNotNull : WhenReminderAreRegister
                {
                    [Fact]
                    public async Task ThrowsWhenCancellationTokenIsCanceled()
                    {
                        var (actorStateProvider, _) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                        var cts = new CancellationTokenSource();
                        cts.Cancel();

                        await Assert.ThrowsAsync<OperationCanceledException>(() => actorService.GetRemindersAsync(null, null, cts.Token));
                    }
                }

                public class WhenNoChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegister
                {
                    protected readonly IActorStateProvider actorStateProviderWithReminders;

                    public WhenNoChangesAreMadeToTheRemindersBetweenResults()
                    {
                        (actorStateProviderWithReminders, _) = CreateActorStateProviderWithReminders();
                    }

                    public class WhenActorIdIsGiven : WhenNoChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public async Task ReturnsTheSameReminderWhichHaveBeenRegisteredForEachActor()
                        {
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProviderWithReminders);

                            foreach (ActorId actorId in allActors)
                            {
                                ContinuationToken continuationToken = null;
                                var allQueriedActors = new List<ActorId>();
                                var allQueriedRemindersPerActor = new List<ActorReminderState>();

                                do
                                {
                                    var page = await actorService.GetRemindersAsync(actorId, continuationToken, default);
                                    continuationToken = page.ContinuationToken;

                                    foreach (var kvp in page.Items)
                                    {
                                        ActorId queriedActor = kvp.Key;
                                        List<ActorReminderState> queriedReminderList = kvp.Value;

                                        allQueriedActors.Add(queriedActor);
                                        allQueriedRemindersPerActor.AddRange(queriedReminderList);
                                    }
                                }
                                while (continuationToken != null);

                                var actorsNotMatchingQuery = allQueriedActors.Where(aid => aid != actorId);
                                var duplicateReminders = allQueriedRemindersPerActor
                                    .GroupBy(r => r) // Group by ActorReminderState
                                    .Where(g => g.Count() > 1) // Filter out group which have 1 or less ActorReminderStates
                                    .SelectMany(g => g) // Flattens groups
                                    .ToList();

                                Assert.Equal(numberOfReminderPerActor, allQueriedRemindersPerActor.Count());
                                Assert.Empty(actorsNotMatchingQuery);
                                Assert.Empty(duplicateReminders);
                            }
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenNoChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public async Task ReturnsTheSameReminderWhichHaveBeenRegistered()
                        {
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProviderWithReminders);

                            ContinuationToken continuationToken = null;
                            int expectedNumberOfReminder = numberOfActors * numberOfReminderPerActor;

                            var allReminders = new List<ActorReminderState>();

                            do
                            {
                                var page = await actorService.GetRemindersAsync(null, continuationToken, default);
                                continuationToken = page.ContinuationToken;

                                foreach (var kvp in page.Items)
                                {
                                    ActorId actorId = kvp.Key;
                                    List<ActorReminderState> reminderList = kvp.Value;

                                    allReminders.AddRange(reminderList);
                                }
                            }
                            while (continuationToken != null);

                            var duplicateReminders = allReminders
                                .GroupBy(r => r) // Group by ActorReminderState
                                .Where(g => g.Count() > 1) // Filter out group which have 1 or less ActorReminderStates
                                .SelectMany(g => g) // Flattens groups
                                .ToList();

                            Assert.Equal(expectedNumberOfReminder, allReminders.Count());
                            Assert.Empty(duplicateReminders);
                        }
                    }
                }

                public class WhenChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegister
                {
                    public class WhenActorIdIsGiven : WhenChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public async Task ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            var (actorStateProvider, _) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                            var expectedRemindersPerPage = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();
                            var targetActorId = fuzzy.Element(allActors);

                            var page = await actorService.GetRemindersAsync(targetActorId, null, default);
                            var allQueriedReminders = page.Items.First().Value.Select(r => r.Name); // Only one key-value pair is returned when querying for a specific actor
                            var targetReminderState = fuzzy.Element(allQueriedReminders);
                            await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminderState);

                            Assert.Equal(expectedRemindersPerPage, allQueriedReminders.Count());
                            Assert.Contains(targetReminderState, allQueriedReminders);
                        }

                        [Fact]
                        public async Task ReflectsChangesToRemindersInConsecutivePages()
                        {
                            var (actorStateProvider, registeredReminders) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                            var targetActorId = fuzzy.Element(allActors);
                            var targetReminder = "";
                            var allNamesOfQueriedReminders = new List<string>();

                            ContinuationToken continuationToken = null;
                            bool firstPage = true;

                            do
                            {
                                var page = await actorService.GetRemindersAsync(targetActorId, continuationToken, default);
                                continuationToken = page.ContinuationToken;

                                IEnumerable<ActorReminderState> queriedReminders = page.Items.First().Value;

                                if (firstPage)
                                {
                                    var namesOfQueriedReminders = queriedReminders.Select(r => r.Name);
                                    var namesOfRegisteredReminders = registeredReminders[targetActorId].Select(r => r.Name);
                                    var namesOfNotQueriedReminders = namesOfRegisteredReminders.Except(namesOfQueriedReminders, StringComparer.OrdinalIgnoreCase);

                                    targetReminder = fuzzy.Element(namesOfNotQueriedReminders.ToList());

                                    var newReminderMock = new Mock<IActorReminder>();
                                    newReminderMock.Setup(r => r.Name).Returns("Reminder_new");

                                    await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder);
                                    await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object);

                                    firstPage = false;
                                }

                                allNamesOfQueriedReminders.AddRange(queriedReminders.Select(rs => rs.Name));
                            }
                            while (continuationToken != null);

                            Assert.Equal(numberOfReminderPerActor, allNamesOfQueriedReminders.Count());
                            Assert.Contains("Reminder_new", allNamesOfQueriedReminders);
                            Assert.DoesNotContain(targetReminder, allNamesOfQueriedReminders);
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public async Task ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            var (actorStateProvider, _) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                            var expectedRemindersPerPage = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();

                            var page = await actorService.GetRemindersAsync(null, null, default);
                            IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);

                            var targetActorId = fuzzy.Element(queriedActors);
                            var namesOfAllQueriedReminders = page.Items
                                .Where(kvp => kvp.Key == targetActorId)
                                .SelectMany(kvp => kvp.Value)
                                .Select(reminder => reminder.Name);
                            var targetReminder = fuzzy.Element(namesOfAllQueriedReminders);
                            
                            await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder);

                            Assert.Equal(expectedRemindersPerPage, namesOfAllQueriedReminders.Count());
                            Assert.Contains(targetReminder, namesOfAllQueriedReminders);
                        }

                        [Fact]
                        public async Task ReflectsChangesToRemindersInConsecutivePages()
                        {
                            var (actorStateProvider, registeredReminders) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                            var targetActorId = new ActorId("");
                            var targetReminder = "";
                            Dictionary<ActorId, List<string>> allNamesOfQueriedReminders = allActors.ToDictionary(id => id, _ => new List<string>());

                            ContinuationToken continuationToken = null;
                            bool firstPage = true;

                            do
                            {
                                var page = await actorService.GetRemindersAsync(null, continuationToken, default);
                                continuationToken = page.ContinuationToken;

                                IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);

                                if (firstPage)
                                {
                                    var actorsNotInFirstPage = allActors.Except(queriedActors);

                                    targetActorId = actorsNotInFirstPage.Any()
                                        ? fuzzy.Element(actorsNotInFirstPage)
                                        : fuzzy.Element(queriedActors);

                                    targetReminder = fuzzy.Element(registeredReminders[targetActorId]).Name;

                                    var newReminderMock = new Mock<IActorReminder>();
                                    newReminderMock.Setup(r => r.Name).Returns("Reminder_new");

                                    await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder);
                                    await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object);

                                    firstPage = false;
                                }

                                foreach (var kvp in page.Items)
                                {
                                    ActorId actorId = kvp.Key;
                                    IEnumerable<ActorReminderState> reminders = kvp.Value;
                                    allNamesOfQueriedReminders[actorId].AddRange(reminders.Select(reminder => reminder.Name));
                                }
                            }
                            while (continuationToken != null);

                            int actualNumberOfQueriedReminders = allNamesOfQueriedReminders.Sum(kvp => kvp.Value.Count());

                            Assert.Equal(numberOfActors * numberOfReminderPerActor, actualNumberOfQueriedReminders);
                            Assert.Contains("Reminder_new", allNamesOfQueriedReminders[targetActorId]);
                            Assert.DoesNotContain(targetReminder, allNamesOfQueriedReminders[targetActorId]);
                        }
                    }
                }

                public class WhenChangesAreMadeToActorsBetweenResults : WhenReminderAreRegister
                {
                    [Fact]
                    public async Task ReflectsChangesToActorsInConsecutivePages()
                    {
                        var (actorStateProvider, registeredReminders) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                        var targetActorId = new ActorId("");
                        var numberOfReminderForTargetActorInFirstPage = 0;
                        Dictionary<ActorId, List<string>> allNamesOfQueriedReminders = allActors.ToDictionary(id => id, _ => new List<string>());

                        ContinuationToken continuationToken = null;
                        bool firstPage = true;

                        do
                        {
                            var page = await actorService.GetRemindersAsync(null, continuationToken, default);
                            continuationToken = page.ContinuationToken;

                            IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);

                            if (firstPage)
                            {
                                var actorsNotInFirstPage = allActors.Except(queriedActors);
                                if (actorsNotInFirstPage.Any())
                                {
                                    targetActorId = fuzzy.Element(actorsNotInFirstPage);
                                }
                                else
                                {
                                    targetActorId = fuzzy.Element(allActors);
                                    numberOfReminderForTargetActorInFirstPage = page.Items
                                        .Where(kvp => kvp.Key == targetActorId)
                                        .Sum(kvp => kvp.Value.Count);
                                }

                                await actorService.DeleteActorAsync(targetActorId, default);
                                firstPage = false;
                            }

                            foreach (var kvp in page.Items)
                            {
                                ActorId actorId = kvp.Key;
                                IEnumerable<ActorReminderState> reminders = kvp.Value;
                                allNamesOfQueriedReminders[actorId].AddRange(reminders.Select(reminder => reminder.Name));
                            }
                        }
                        while (continuationToken != null);

                        Assert.Equal(numberOfReminderForTargetActorInFirstPage, allNamesOfQueriedReminders[targetActorId].Count);
                    }
                }
            }
        }
    }
}
