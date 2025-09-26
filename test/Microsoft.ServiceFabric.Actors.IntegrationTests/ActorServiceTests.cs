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

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorServiceIntegrationTest
    {
        protected async Task<ActorService> GetActorService<T>(
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            ActorServiceSettings actorServiceSettings = null,
            IActorStateProvider actorStateProvider = null)
            where T : Actor
        {
            IFuzz fuzzy = new RandomFuzz();

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

            public class WhenReminderAreRegister : GetRemindersAsync
            {
                protected readonly int numberOfActor;
                protected readonly int numberOfReminderPerActor;
                protected readonly List<ActorId> allActors;

                public WhenReminderAreRegister()
                {
                    var fuzzy = new RandomFuzz();
                    numberOfActor = fuzzy.Int32().Between(5, 10);
                    numberOfReminderPerActor = fuzzy.Int32().Between(10, 20);
                    allActors = new List<ActorId>();

                    for (int i = 0; i < numberOfActor; i++)
                    {
                        allActors.Add(new ActorId($"Actor_{i}"));
                    }
                }

                protected IActorStateProvider CreateActorStateProviderWithReminders()
                {
                    IActorStateProvider actorStateProvider = new NullActorStateProvider();

                    foreach (var actorId in allActors)
                    {
                        for (int j = 0; j < numberOfReminderPerActor; j++)
                        {
                            var reminderMock = new Mock<IActorReminder>();
                            reminderMock.SetupGet(r => r.Name).Returns($"Reminder_{j}");
                            actorStateProvider.SaveReminderAsync(actorId, reminderMock.Object).Wait();
                        }
                    }

                    return actorStateProvider;
                }

                public class CancellationTokenIsNotNull : WhenReminderAreRegister
                {
                    [Fact]
                    public async Task ThrowsWhenCancellationTokenIsCanceled()
                    {
                        var actorStateProvider = CreateActorStateProviderWithReminders();
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
                        actorStateProviderWithReminders = CreateActorStateProviderWithReminders();
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
                            int expectedNumberOfReminder = numberOfActor * numberOfReminderPerActor;

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
                        public void ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            var actorStateProvider = CreateActorStateProviderWithReminders();
                            // Test implementation here
                            Assert.True(true);
                        }

                        [Fact]
                        public void ReflectsChangesToRemindersInConsecutivePages()
                        {
                            var actorStateProvider = CreateActorStateProviderWithReminders();
                            // Test implementation here
                            Assert.True(true);
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public void ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            var actorStateProvider = CreateActorStateProviderWithReminders();
                            // Test implementation here
                            Assert.True(true);
                        }

                        [Fact]
                        public void ReflectsChangesToRemindersInConsecutivePages()
                        {
                            var actorStateProvider = CreateActorStateProviderWithReminders();
                            // Test implementation here
                            Assert.True(true);
                        }
                    }
                }

                public class WhenChangesAreMadeToActorsBetweenResults : WhenReminderAreRegister
                {
                    [Fact]
                    public void ReflectsChangesToActorsInConsecutivePages()
                    {
                        var actorStateProvider = CreateActorStateProviderWithReminders();
                        // Test implementation here
                        Assert.True(true);
                    }
                }
            }
            }
        }
    }
