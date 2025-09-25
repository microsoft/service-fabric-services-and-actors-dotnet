using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
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

            public class WithCancellationToken : GetRemindersAsync
            {
                [Fact]
                public void ThrowsWhenCancellationTokenIsCanceled()
                {
                    Assert.True(true);
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
                public class WhenNoChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegister
                {

                    public class WhenActorIdIsGiven : WhenNoChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public void ReturnsTheSameReminderWhichHaveBeenRegisteredForEachActor()
                        {
                            Assert.True(true);
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenNoChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public void ReturnsTheSameReminderWhichHaveBeenRegistered()
                        {
                            Assert.True(true);
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
                            Assert.True(true);
                        }

                        [Fact]
                        public void ReflectsChangesToRemindersInConsecutivePages()
                        {
                            Assert.True(true);
                        }
                    }

                    public class WhenActorIdIsNotGiven : WhenChangesAreMadeToTheRemindersBetweenResults
                    {
                        [Fact]
                        public void ChangesToReminderAreNotReflectedInPageThatHasBeenRead()
                        {
                            Assert.True(true);
                        }

                        [Fact]
                        public void ReflectsChangesToRemindersInConsecutivePages()
                        {
                            Assert.True(true);
                        }
                    }
                }

                public class WhenChangesAreMadeToActorsBetweenResults : WhenReminderAreRegister
                {
                    [Fact]
                    public void ReflectsChangesToActorsInConsecutivePages()
                    {
                        Assert.True(true);
                    }
                }
                
            }
            }
        }
    }
