using System;
using System.Fabric;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Xunit;
using Fuzzy;

namespace Microsoft.ServiceFabric.Actors
{
    public class ActorIntegrationTest
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

        interface ITestableActor : IActor
        {
            Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period);
        }

        class TestableActor : Actor, ITestableActor, IRemindable
        {
            ActorMethodContext currentMethodContext;
            Action<string, string, ActorCallType, byte[], TimeSpan, TimeSpan> receiveReminderCallback;

            public TestableActor(ActorService actorService, ActorId actorId, Action<string, string, ActorCallType, byte[], TimeSpan, TimeSpan> receiveReminderCallback)
                : base(actorService, actorId)
            {
                this.receiveReminderCallback = receiveReminderCallback;
            }

            protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
            {
                currentMethodContext = actorMethodContext;
                return base.OnPreActorMethodAsync(actorMethodContext);
            }

            public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
            {
                receiveReminderCallback(reminderName, currentMethodContext.MethodName, currentMethodContext.CallType, state, dueTime, period);
                return Task.CompletedTask;
            }

            public new Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
            {
                return base.RegisterReminderAsync(reminderName, state, dueTime, period);
            }
        }

        public class RegisterReminderAsync : ActorIntegrationTest
        {
            [Fact]
            public async Task ReminderParametersPersistBetweenRegisteringAndReceivingReminder()
            {
                int expectedCallbackInvocationCounter = 1;
                string expectedReminderName = "TestReminder";
                string expectedActorMethodName = "ReceiveReminderAsync";
                ActorCallType expectedActorCallType = ActorCallType.ReminderMethod;
                byte[] expectedState = UTF8Encoding.UTF8.GetBytes("TestReminderState");
                TimeSpan expectedDueTime = TimeSpan.FromSeconds(1);
                TimeSpan expectedPeriod = TimeSpan.FromMinutes(1);

                int reminderCallbackInvocationCounter = 0;
                string actualReminderName = "";
                string actualActorMethodName = "";
                ActorCallType actualActorCallType = default;
                byte[] actualState = UTF8Encoding.UTF8.GetBytes("");
                TimeSpan actualDueTime = TimeSpan.Zero;
                TimeSpan actualPeriod = TimeSpan.Zero;

                Func<ActorBase, CancellationToken, Task<IActorReminder>> registerActorReminder = async (actorBase, cacnelationToken) =>
                {
                    var testActor = (ITestableActor)actorBase;
                    return await testActor.RegisterReminderAsync(expectedReminderName, expectedState, expectedDueTime, expectedPeriod);
                };

                Action<string, string, ActorCallType, byte[], TimeSpan, TimeSpan> receiveReminderCallback = (reminderName, actorMethodName, actorCallType, state, dueTime, period) =>
                {
                    actualReminderName = reminderName;
                    actualActorMethodName = actorMethodName;
                    actualActorCallType = actorCallType;
                    actualState = state;
                    actualDueTime = dueTime;
                    actualPeriod = period;

                    reminderCallbackInvocationCounter += 1;
                };

                Func<ActorService, ActorId, ActorBase> actorFactory = (actorService, actorId) => new TestableActor(actorService, actorId, receiveReminderCallback);

                ActorService actorService = await GetActorService<TestableActor>(actorFactory);

                IActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                    actorId: new ActorId("RemiderTestActor1"),
                    actorMethodContext: new ActorMethodContext(),
                    createIfRequired: true,
                    registerActorReminder,
                    callContext: "TestCallContext",
                    timerCall: false,
                    cancellationToken: new CancellationToken());

                Assert.NotNull(reminderResult);
                Assert.Equal(expectedReminderName, reminderResult.Name);
                Assert.Equal(expectedState, reminderResult.State);
                Assert.Equal(expectedDueTime, reminderResult.DueTime);
                Assert.Equal(expectedPeriod, reminderResult.Period);

                // Wait enough time for reminder to fire
                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.Equal(expectedCallbackInvocationCounter, reminderCallbackInvocationCounter);
                Assert.Equal(expectedReminderName, actualReminderName);
                Assert.Equal(expectedActorMethodName, actualActorMethodName);
                Assert.Equal(expectedActorCallType, actualActorCallType);
                Assert.Equal(expectedState, actualState);
                Assert.Equal(expectedDueTime, actualDueTime);
                Assert.Equal(expectedPeriod, actualPeriod);
            }
        }
    }
}
