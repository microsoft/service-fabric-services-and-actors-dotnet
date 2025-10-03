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

            [Collection(typeof(ServiceStateCollectionFixture))]
            public class WhenReminderAreRegister : GetRemindersAsync
            {
                protected readonly ServiceStateFixture serviceStateFixture;

                public WhenReminderAreRegister(ServiceStateFixture serviceStateFixture)
                {
                    this.serviceStateFixture = serviceStateFixture;
                }

                protected (IActorStateProvider, IEnumerable<ActorId>, Dictionary<ActorId, List<IActorReminder>>) CreateActorStateProviderWithReminders()
                {
                    IActorStateProvider actorStateProvider = new NullActorStateProvider();
                    var registeredReminders = new Dictionary<ActorId, List<IActorReminder>>();

                    for (int i = 0; i < serviceStateFixture.NumberOfActors; i++)
                    {
                        var actorId = new ActorId($"Actor_{i}");
                        registeredReminders[actorId] = new List<IActorReminder>();

                        for (int j = 0; j < serviceStateFixture.NumberOfReminderPerActor; j++)
                        {
                            var reminderMock = new Mock<IActorReminder>();
                            reminderMock.SetupGet(r => r.Name).Returns($"Reminder_{j}");
                            actorStateProvider.SaveReminderAsync(actorId, reminderMock.Object).Wait();

                            registeredReminders[actorId].Add(reminderMock.Object);
                        }
                    }

                    return (actorStateProvider, registeredReminders.Keys, registeredReminders);
                }

                public class CancellationTokenIsNotNull : WhenReminderAreRegister
                {
                    public CancellationTokenIsNotNull(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                    [Fact]
                    public async Task ThrowsWhenCancellationTokenIsCanceled()
                    {
                        var (actorStateProvider, _, _) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                        var cts = new CancellationTokenSource();
                        cts.Cancel();

                        await Assert.ThrowsAsync<OperationCanceledException>(() => actorService.GetRemindersAsync(null, null, cts.Token));
                    }
                }

                public class WhenNoChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegister
                {
                    protected readonly IActorStateProvider actorStateProviderWithReminders;
                    protected readonly Dictionary<ActorId, List<IActorReminder>> registeredRemindersPerActor;
                    protected readonly IEnumerable<ActorId> allActors;

                    public WhenNoChangesAreMadeToTheRemindersBetweenResults(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture)
                    {
                        (actorStateProviderWithReminders, allActors, registeredRemindersPerActor) = CreateActorStateProviderWithReminders();
                    }

                    public class WhenActorIdIsGiven : WhenNoChangesAreMadeToTheRemindersBetweenResults
                    {
                        public WhenActorIdIsGiven(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

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
                                    var page = await actorService.GetRemindersAsync(actorId, continuationToken, TestContext.Current.CancellationToken);
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

                                Assert.Equal(serviceStateFixture.NumberOfReminderPerActor, allQueriedRemindersPerActor.Count());
                                Assert.Empty(actorsNotMatchingQuery);
                                Assert.Empty(duplicateReminders);
                            }
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenNoChangesAreMadeToTheRemindersBetweenResults
                    {
                        public WhenActorIdIsNotGiven(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                        [Fact]
                        public async Task ReturnsTheSameReminderWhichHaveBeenRegistered()
                        {
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProviderWithReminders);

                            ContinuationToken continuationToken = null;
                            int expectedNumberOfReminder = serviceStateFixture.NumberOfActors * serviceStateFixture.NumberOfReminderPerActor;

                            var allReminders = new List<ActorReminderState>();

                            do
                            {
                                var page = await actorService.GetRemindersAsync(null, continuationToken, TestContext.Current.CancellationToken);
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
                    public WhenChangesAreMadeToTheRemindersBetweenResults(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                    public class WhenActorIdIsGiven : WhenChangesAreMadeToTheRemindersBetweenResults
                    {
                        public WhenActorIdIsGiven(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                        [Fact]
                        public async Task ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            var (actorStateProvider, allActors, _) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                            var expectedRemindersPerPage = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();
                            var targetActorId = fuzzy.Element(allActors);

                            var page = await actorService.GetRemindersAsync(targetActorId, null, TestContext.Current.CancellationToken);
                            var allQueriedReminders = page.Items.First().Value.Select(r => r.Name); // Only one key-value pair is returned when querying for a specific actor
                            var targetReminderState = fuzzy.Element(allQueriedReminders);
                            await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminderState, TestContext.Current.CancellationToken);

                            Assert.Equal(expectedRemindersPerPage, allQueriedReminders.Count());
                            Assert.Contains(targetReminderState, allQueriedReminders);
                        }

                        [Fact]
                        public async Task ReflectsChangesToRemindersInConsecutivePages()
                        {
                            var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                            var targetActorId = fuzzy.Element(allActors);
                            var targetReminder = "";
                            var allNamesOfQueriedReminders = new List<string>();

                            ContinuationToken continuationToken = null;
                            bool firstPage = true;

                            do
                            {
                                var page = await actorService.GetRemindersAsync(targetActorId, continuationToken, TestContext.Current.CancellationToken);
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

                                    await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);
                                    await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object, TestContext.Current.CancellationToken);

                                    firstPage = false;
                                }

                                allNamesOfQueriedReminders.AddRange(queriedReminders.Select(rs => rs.Name));
                            }
                            while (continuationToken != null);

                            Assert.Equal(serviceStateFixture.NumberOfReminderPerActor, allNamesOfQueriedReminders.Count());
                            Assert.Contains("Reminder_new", allNamesOfQueriedReminders);
                            Assert.DoesNotContain(targetReminder, allNamesOfQueriedReminders);
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenChangesAreMadeToTheRemindersBetweenResults
                    {
                        public WhenActorIdIsNotGiven(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                        [Fact]
                        public async Task ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            var (actorStateProvider, _, _) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                            var expectedRemindersPerPage = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();

                            var page = await actorService.GetRemindersAsync(null, null, TestContext.Current.CancellationToken);
                            IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);

                            var targetActorId = fuzzy.Element(queriedActors);
                            var namesOfAllQueriedReminders = page.Items
                                .Where(kvp => kvp.Key == targetActorId)
                                .SelectMany(kvp => kvp.Value)
                                .Select(reminder => reminder.Name);
                            var targetReminder = fuzzy.Element(namesOfAllQueriedReminders);

                            await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);

                            Assert.Equal(expectedRemindersPerPage, namesOfAllQueriedReminders.Count());
                            Assert.Contains(targetReminder, namesOfAllQueriedReminders);
                        }

                        [Fact]
                        public async Task ReflectsChangesToRemindersInConsecutivePages()
                        {
                            var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                            IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                            var targetActorId = new ActorId("");
                            var targetReminder = "";
                            Dictionary<ActorId, List<string>> allNamesOfQueriedReminders = allActors.ToDictionary(id => id, _ => new List<string>());

                            int expectedNumberOfRegisteredReminders = serviceStateFixture.NumberOfActors * serviceStateFixture.NumberOfReminderPerActor;

                            ContinuationToken continuationToken = null;
                            bool firstPage = true;

                            do
                            {
                                var page = await actorService.GetRemindersAsync(null, continuationToken, TestContext.Current.CancellationToken);
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

                                    await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);
                                    await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object, TestContext.Current.CancellationToken);

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

                            Assert.Equal(expectedNumberOfRegisteredReminders, actualNumberOfQueriedReminders);
                            Assert.Contains("Reminder_new", allNamesOfQueriedReminders[targetActorId]);
                            Assert.DoesNotContain(targetReminder, allNamesOfQueriedReminders[targetActorId]);
                        }
                    }
                }

                public class WhenChangesAreMadeToActorsBetweenResults : WhenReminderAreRegister
                {
                    public WhenChangesAreMadeToActorsBetweenResults(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                    [Fact]
                    public async Task ReflectsChangesToActorsInConsecutivePages()
                    {
                        var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                        var targetActorId = new ActorId("");
                        var numberOfReminderForTargetActorInFirstPage = 0;
                        Dictionary<ActorId, List<string>> allNamesOfQueriedReminders = allActors.ToDictionary(id => id, _ => new List<string>());

                        ContinuationToken continuationToken = null;
                        bool firstPage = true;

                        do
                        {
                            var page = await actorService.GetRemindersAsync(null, continuationToken, TestContext.Current.CancellationToken);
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

                                await actorService.DeleteActorAsync(targetActorId, TestContext.Current.CancellationToken);
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

            public class ServiceStateFixture : IDisposable
            {
                private readonly int numberOfActors;
                private readonly int numberOfReminderPerActor;
                private readonly int defaultPageSize;
                public int NumberOfActors { get => numberOfActors; }
                public int NumberOfReminderPerActor { get => numberOfReminderPerActor; }

                public ServiceStateFixture()
                {
                    numberOfActors = fuzzy.Int32().Between(5, 10);
                    numberOfReminderPerActor = fuzzy.Int32().Between(10, 20);
                    defaultPageSize = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();

                    // Reminder for a particular actor should be divided in at least two result pages
                    int newPageSize = fuzzy.Int32().Between(1, numberOfReminderPerActor - 1);
                    ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.SetDefaultPageSize(newPageSize);
                }

                public void Dispose()
                {
                    ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.SetDefaultPageSize(defaultPageSize);
                }
            }

            [Collection("Service State Collection")]
            public class ServiceStateCollectionFixture : ICollectionFixture<ServiceStateFixture> { }

        }
    }
}
