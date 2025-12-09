using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.TestFramework;
using Moq;
using Xunit;
using Xunit.Internal;

namespace Microsoft.ServiceFabric.Actors.ActorServiceIntegrationTests
{
    public class GetRemindersAsync : MockedMetricsTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz();

        public class WhenNoReminderIsRegistered : GetRemindersAsync
        {
            [Fact]
            public async Task ReturnEmptyResult()
            {
                IActorService actorService = await TestableActorService.GetActorService<TestActor>();

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

            public class WithCancellationToken : WhenRemindersAreRegistered
            {
                public WithCancellationToken(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                [Fact]
                public async Task ThrowsWhenCancelled()
                {
                    // Arrange
                    var (actorStateProvider, _, _) = CreateActorStateProviderWithReminders();
                    IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                    var cts = new CancellationTokenSource();

                    // Act
                    cts.Cancel();

                    //Assert
                    await Assert.ThrowsAsync<OperationCanceledException>(() => actorService.GetRemindersAsync(null, null, cts.Token));
                }
            }

            public class WhenNoChangesBetweenResults : WhenRemindersAreRegistered
            {
                protected readonly IActorStateProvider actorStateProviderWithReminders;
                protected readonly Dictionary<ActorId, List<IActorReminder>> registeredRemindersPerActor;
                protected readonly IEnumerable<ActorId> allActors;

                public WhenNoChangesBetweenResults(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture)
                {
                    (actorStateProviderWithReminders, allActors, registeredRemindersPerActor) = CreateActorStateProviderWithReminders();
                }

                [Fact]
                public async Task ReturnsTheSameReminderPerActor()
                {
                    // Arrange
                    IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProviderWithReminders);

                    foreach (ActorId actorId in allActors)
                    {
                        // Arrange
                        Dictionary<ActorId, List<IActorReminder>> expectedQueryResult = registeredRemindersPerActor
                            .Where(kvp => kvp.Key == actorId)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                        // Act
                        Dictionary<ActorId, List<IActorReminder>> actualQueryResult = await QueryReminders(actorService, actorId);

                        // Assert
                        Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, actualQueryResult));
                    }
                }

                [Fact]
                public async Task ReturnsSameReminder()
                {
                    // Arrange 
                    IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProviderWithReminders);
                    var expectedQueryResult = registeredRemindersPerActor;

                    // Act
                    Dictionary<ActorId, List<IActorReminder>> actualQueryResult = await QueryReminders(actorService, null);

                    // Assert
                    Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, actualQueryResult));
                }
            }

            public class WhenChangesBetweenResults : WhenRemindersAreRegistered
            {
                public WhenChangesBetweenResults(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                public class WhenChangingCurrentPage : WhenChangesBetweenResults
                {
                    public WhenChangingCurrentPage(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                    [Fact]
                    public async Task ReturnsSameRemindersPerActor()
                    {
                        // Arrange
                        var (actorStateProvider, allActors, _) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                        var expectedRemindersPerPage = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();
                        var targetActorId = fuzzy.Element(allActors);

                        // Act
                        var page = await actorService.GetRemindersAsync(targetActorId, null, TestContext.Current.CancellationToken);
                        var allQueriedReminders = page.Items.First().Value.Select(r => r.Name); // Only one key-value pair is returned when querying for a specific actor
                        var targetReminderState = fuzzy.Element(allQueriedReminders);
                        await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminderState, TestContext.Current.CancellationToken);

                        // Assert
                        Assert.Equal(expectedRemindersPerPage, allQueriedReminders.Count());
                        Assert.Contains(targetReminderState, allQueriedReminders);
                    }

                    [Fact]
                    public async Task ReturnsSameReminders()
                    {
                        // Arrange
                        var (actorStateProvider, _, _) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProvider);
                        var expectedRemindersPerPage = ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>.GetDefaultPageSize();

                        // Act
                        var page = await actorService.GetRemindersAsync(null, null, TestContext.Current.CancellationToken);
                        IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);

                        var targetActorId = fuzzy.Element(queriedActors);
                        var namesOfAllQueriedReminders = page.Items
                            .Where(kvp => kvp.Key == targetActorId)
                            .SelectMany(kvp => kvp.Value)
                            .Select(reminder => reminder.Name);
                        var targetReminder = fuzzy.Element(namesOfAllQueriedReminders);

                        await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);

                        // Assert
                        Assert.Equal(expectedRemindersPerPage, namesOfAllQueriedReminders.Count());
                        Assert.Contains(targetReminder, namesOfAllQueriedReminders);
                    }
                }

                public class WhenChangingUpcomingPage : WhenChangesBetweenResults
                {
                    public WhenChangingUpcomingPage(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                    [Fact]
                    public async Task ReflectsChangesInConsecutivePagesPerActor()
                    {
                        // Arrange
                        var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                        var targetActorId = fuzzy.Element(allActors);
                        var expectedQueryResult = registeredReminders
                            .Where(kvp => kvp.Key == targetActorId)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                        Func<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>, Task> editStateWhenFirstPageIsRead = async (page) =>
                        {
                            IEnumerable<ActorReminderState> queriedReminders = page.Items.First().Value; // Only one key-value pair is returned when querying for a specific actor

                            var namesOfQueriedReminders = queriedReminders.Select(r => r.Name);
                            var namesOfRegisteredReminders = registeredReminders[targetActorId].Select(r => r.Name);
                            var namesOfNotQueriedReminders = namesOfRegisteredReminders.Except(namesOfQueriedReminders);

                            var targetReminder = fuzzy.Element(namesOfNotQueriedReminders.ToList());

                            var newReminderMock = new Mock<IActorReminder>();
                            newReminderMock.Setup(r => r.Name).Returns("Reminder_new");

                            // Edit the state which is outside of the page that was last read
                            await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminder, TestContext.Current.CancellationToken);
                            await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object, TestContext.Current.CancellationToken);

                            // Edit expected query results based on the changes to the actor state provider
                            expectedQueryResult[targetActorId].RemoveAll(r => r.Name == targetReminder);
                            expectedQueryResult[targetActorId].Add(newReminderMock.Object);
                        };

                        // Act
                        Dictionary<ActorId, List<IActorReminder>> actualQueryResult = await QueryRemindersWithChangesOnFirstPage(actorService, targetActorId, editStateWhenFirstPageIsRead);

                        // Assert
                        Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, actualQueryResult));
                    }

                    [Fact]
                    public async Task ReflectsChangesInConsecutivePages()
                    {
                        // Arrange 
                        var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                        IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                        var expectedQueryResult = registeredReminders; // Will change while reading first page

                        Func<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>, Task> editStateWhenFirstPageIsRead = async (page) =>
                        {
                            IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);
                            var actorsNotInFirstPage = allActors.Except(queriedActors);

                            ActorId targetActorId = actorsNotInFirstPage.Any()
                                ? fuzzy.Element(actorsNotInFirstPage)
                                : fuzzy.Element(queriedActors);
                            string targetReminderName = fuzzy.Element(registeredReminders[targetActorId]).Name;

                            var newReminderMock = new Mock<IActorReminder>();
                            newReminderMock.Setup(r => r.Name).Returns("Reminder_new");

                            // Edit the state which is outside of the page that was last read
                            await actorStateProvider.DeleteReminderAsync(targetActorId, targetReminderName, TestContext.Current.CancellationToken);
                            await actorStateProvider.SaveReminderAsync(targetActorId, newReminderMock.Object, TestContext.Current.CancellationToken);

                            // Edit expected query results based on the changes to the actor state provider
                            expectedQueryResult[targetActorId].RemoveAll(r => r.Name == targetReminderName);
                            expectedQueryResult[targetActorId].Add(newReminderMock.Object);
                        };

                        // Act
                        Dictionary<ActorId, List<IActorReminder>> actualQueryResult = await QueryRemindersWithChangesOnFirstPage(actorService, null, editStateWhenFirstPageIsRead);

                        // Assert
                        Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, actualQueryResult));
                    }
                }
            }

            public class WhenActorDeleted : WhenRemindersAreRegistered
            {
                public WhenActorDeleted(ServiceStateFixture serviceStateFixture) : base(serviceStateFixture) { }

                [Fact]
                public async Task ReflectsChangesInConsecutivePages()
                {
                    // Arrange
                    var (actorStateProvider, allActors, registeredReminders) = CreateActorStateProviderWithReminders();
                    IActorService actorService = await TestableActorService.GetActorService<TestActor>(actorStateProvider: actorStateProvider);

                    var expectedQueryResult = registeredReminders; // Will change while reading first page

                    Func<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>, Task> editState = async (page) =>
                    {
                        IEnumerable<ActorId> queriedActors = page.Items.Select(kvp => kvp.Key);
                        var actorsNotInFirstPage = allActors.Except(queriedActors);

                        ActorId targetActorId = actorsNotInFirstPage.Any()
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
                    };

                    // Act
                    Dictionary<ActorId, List<IActorReminder>> actualQueryResult = await QueryRemindersWithChangesOnFirstPage(actorService, null, editState);

                    // Assert
                    Assert.True(ReminderDictionariesAreEqual(expectedQueryResult, actualQueryResult));
                }
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

            protected async Task<Dictionary<ActorId, List<IActorReminder>>> QueryReminders(IActorService actorService, ActorId actorId)
            {
                var output = new Dictionary<ActorId, List<IActorReminder>>();

                ContinuationToken continuationToken = null;
                do
                {
                    var page = await actorService.GetRemindersAsync(actorId, continuationToken, TestContext.Current.CancellationToken);
                    continuationToken = page.ContinuationToken;

                    ReadReminderPage(page, output);
                }
                while (continuationToken != null);

                return output;
            }

            protected async Task<Dictionary<ActorId, List<IActorReminder>>> QueryRemindersWithChangesOnFirstPage(
                IActorService actorService, ActorId actorId,
                Func<ReminderPagedResult<KeyValuePair<ActorId, List<ActorReminderState>>>, Task> makeChanges
            )
            {
                var output = new Dictionary<ActorId, List<IActorReminder>>();

                ContinuationToken continuationToken = null;
                bool firstPage = true;
                do
                {
                    var page = await actorService.GetRemindersAsync(actorId, continuationToken, TestContext.Current.CancellationToken);
                    continuationToken = page.ContinuationToken;

                    if (firstPage)
                    {
                        await makeChanges(page);
                        firstPage = false;
                    }

                    ReadReminderPage(page, output);
                }
                while (continuationToken != null);

                return output;
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
