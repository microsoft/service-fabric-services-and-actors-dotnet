using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorServiceIntegrationTest
    {
        protected async Task<ActorService> GetActorService<T>(
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            ActorServiceSettings actorServiceSettings = null)
            where T : Actor
        {
            IFuzz fuzzy = new RandomFuzz();

            ActorService actorService = new ActorService(
                fuzzy.StatefulServiceContext(),
                ActorTypeInformation.Get(typeof(T)),
                actorFactory,
                null,
                new NullActorStateProvider(),
                actorServiceSettings);

            IStatefulUserServiceReplica statefulServiceReplica = actorService;
            await statefulServiceReplica.OnOpenAsync(ReplicaOpenMode.New, CancellationToken.None);
            await statefulServiceReplica.OnChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None);
            await statefulServiceReplica.RunAsync(CancellationToken.None);

            return actorService;
        }

        public class GetReminderAsync : ActorServiceIntegrationTest
        {

            public class WhenNoReminderIsRegistered : GetReminderAsync
            {
                [Fact]
                public void ReturnEmptyResultIfNoRemindersAreRegistered()
                {
                    Assert.True(true);
                }
            }

            public class WhenReminderAreRegister : GetReminderAsync
            {
                public class WhenNoChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegister
                {
                    [Fact]
                    public void ReturnsTheSameReminderWhichHaveBeenRegistered()
                    {
                        Assert.True(true);
                    }
                }

                public class WhenChangesAreMadeToTheRemindersBetweenResults : WhenReminderAreRegister
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