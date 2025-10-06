using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Moq;
using Xunit;
using Xunit.Internal;

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
            public class WhenRemindersAreRegistered : GetRemindersAsync
            {
                protected readonly ServiceStateFixture serviceStateFixture;

                public WhenRemindersAreRegistered(ServiceStateFixture serviceStateFixture)
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

                protected bool ReminderDictionariesAreEqual(
                    Dictionary<ActorId, List<IActorReminder>> dict1,
                    Dictionary<ActorId, List<IActorReminder>> dict2
                )
                {
                    if (dict1.Count != dict2.Count)
                        return false;

                    foreach (var kvp in dict1)
                    {
                        if (!dict2.TryGetValue(kvp.Key, out var reminders2))
                            return false;

                        var names1 = kvp.Value.Select(r => r.Name).ToHashSet();
                        var names2 = reminders2.Select(r => r.Name).ToHashSet();

                        if (!names1.SetEquals(names2))
                            return false;
                    }

                    return true;
                }

                protected void ReadReminderPage(
                    ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>> resultPage,
                    Dictionary<ActorId, List<IActorReminder>> output
                )
                {
                    foreach (var kvp in resultPage.Items)
                    {
                       if (!output.ContainsKey(kvp.Key))
                            output[kvp.Key] = new List<IActorReminder>();

                        output[kvp.Key].AddRange(kvp.Value);
                    }
                }

                public class CancellationTokenIsNotNull : WhenReminderAreRegistered
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

                public class WhenNoChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegistered
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
                                Dictionary<ActorId, List<IActorReminder>> expectedQueryResult = registeredRemindersPerActor
                                    .Where(kvp => kvp.Key == actorId)
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                                ContinuationToken continuationToken = null;
                                var queryResult = new Dictionary<ActorId, List<IActorReminder>>();

                                do
                                {
                                    var page = await actorService.GetRemindersAsync(actorId, continuationToken, TestContext.Current.CancellationToken);
                                    continuationToken = page.ContinuationToken;

                                    ReadReminderPage(page, queryResult);
                                }
                                while (continuationToken != null);

                                Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, queryResult));
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
                            var queryResult = new Dictionary<ActorId, List<IActorReminder>>();

                            do
                            {
                                var page = await actorService.GetRemindersAsync(null, continuationToken, TestContext.Current.CancellationToken);
                                continuationToken = page.ContinuationToken;

                                ReadReminderPage(page, queryResult);
                            }
                            while (continuationToken != null);

                            Assert.True(ReminderDictionariesAreEqual(registeredRemindersPerActor, queryResult));
                        }
                    }
                }

                public class WhenChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegistered
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
                            var targetReminder = ""; // Target reminder is determined when reading the first result page
                            var expectedQueryResult = registeredReminders
                                .Where(kvp => kvp.Key == targetActorId)
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                            var queryResult = new Dictionary<ActorId, List<IActorReminder>>();

                            ContinuationToken continuationToken = null;
                            bool firstPage = true;

                            do
                            {
                                var page = await actorService.GetRemindersAsync(targetActorId, continuationToken, TestContext.Current.CancellationToken);
                                continuationToken = page.ContinuationToken;

                                if (firstPage)
                                {
                                    IEnumerable<ActorReminderState> queriedReminders = page.Items.First().Value;
                                    var namesOfQueriedReminders = queriedReminders.Select(r => r.Name);
                                    var namesOfRegisteredReminders = registeredReminders[targetActorId].Select(r => r.Name);
                                    var namesOfNotQueriedReminders = namesOfRegisteredReminders.Except(namesOfQueriedReminders, StringComparer.OrdinalIgnoreCase);

                                    targetReminder = fuzzy.Element(namesOfNotQueriedReminders.ToList());

                                    var newReminderMock = new Mock<IActorReminder>();
                                    newReminderMock.Setup(r => r.Name).Returns("Reminder_new");

                                    await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);
                                    await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object, TestContext.Current.CancellationToken);

                                    expectedQueryResult[targetActorId].RemoveAll(r => r.Name == targetReminder);
                                    expectedQueryResult[targetActorId].Add(newReminderMock.Object);

                                    firstPage = false;
                                }

                                ReadReminderPage(page, queryResult);

                            }
                            while (continuationToken != null);

                            Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, queryResult));
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
                            
                            // Target actor and reminder is determined when reading the first result page
                            var targetActorId = new ActorId("");
                            var targetReminder = "";
                            var expectedQueryResult = registeredReminders;

                            var queryResult = new Dictionary<ActorId, List<IActorReminder>>();

                            ContinuationToken continuationToken = null;
                            bool firstPage = true;

                            do
                            {
                                var page = await actorService.GetRemindersAsync(null, continuationToken, TestContext.Current.CancellationToken);
                                continuationToken = page.ContinuationToken;

                                if (firstPage)
                                {
                                    IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);
                                    var actorsNotInFirstPage = allActors.Except(queriedActors);
                                    targetActorId = actorsNotInFirstPage.Any()
                                        ? fuzzy.Element(actorsNotInFirstPage)
                                        : fuzzy.Element(queriedActors);
                                    targetReminder = fuzzy.Element(registeredReminders[targetActorId]).Name;

                                    var newReminderMock = new Mock<IActorReminder>();
                                    newReminderMock.Setup(r => r.Name).Returns("Reminder_new");

                                    await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);
                                    await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object, TestContext.Current.CancellationToken);

                                    expectedQueryResult[targetActorId].RemoveAll(r => r.Name == targetReminder);
                                    expectedQueryResult[targetActorId].Add(newReminderMock.Object);

                                    firstPage = false;
                                }

                                ReadReminderPage(page, queryResult);
                            }
                            while (continuationToken != null);

                            Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, queryResult));
                        }
                    }
                }

                public class WhenChangesAreMadeToActorsBetweenResults : WhenReminderAreRegistered
                {
                    public WhenChangesAreMadeToActorsBetweenResults(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                    [Fact]
                    public async Task ReflectsChangesToActorsInConsecutivePages()
                    {
                        var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                        var targetActorId = new ActorId(""); // Target actor is determined while reading the first page
                        var expectedQueryResult = registeredReminders; // Will change while reading first page

                        var queryResult = new Dictionary<ActorId, List<IActorReminder>>();

                        ContinuationToken continuationToken = null;
                        bool firstPage = true;

                        do
                        {
                            var page = await actorService.GetRemindersAsync(null, continuationToken, TestContext.Current.CancellationToken);
                            continuationToken = page.ContinuationToken;

                            if (firstPage)
                            {
                                IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);
                                var actorsNotInFirstPage = allActors.Except(queriedActors);
                                targetActorId = actorsNotInFirstPage.Any()
                                    ? fuzzy.Element(actorsNotInFirstPage)
                                    : fuzzy.Element(allActors);

                                await actorService.DeleteActorAsync(targetActorId, TestContext.Current.CancellationToken);

                                IEnumerable<string> reminderNamesForTargetActorInFirstPage = page.Items
                                    .Where(kvp => kvp.Key == targetActorId)
                                    .SelectMany(kvp => kvp.Value)
                                    .Select(r => r.Name);

                                if (reminderNamesForTargetActorInFirstPage.Count() == 0)
                                {
                                    expectedQueryResult.Remove(targetActorId);
                                }
                                else
                                {
                                    expectedQueryResult
                                        .Where(kvp => kvp.Key == targetActorId)
                                        .Select(kvp => kvp.Value)
                                        .ForEach(list => list.RemoveAll(r => reminderNamesForTargetActorInFirstPage.Contains(r.Name)));
                                }
                                firstPage = false;
                            }

                            ReadReminderPage(page, queryResult);
                        }
                        while (continuationToken != null);

                        Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, queryResult));
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
