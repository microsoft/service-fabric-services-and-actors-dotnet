using System;
using System.Fabric;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.IntegrationTests
{
    public class ReminderRegistrationTest
    {
        interface IReminderTestActor : IActor
        {
            Task<IActorReminder> InitStateAndCreateReminder(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period);
            Task<bool> ValidateActorState();
        }

        class RemiderTestActor : Actor, IReminderTestActor, IRemindable
        {
            string expectedReminderStateKey = "ExpectedReminderState";
            string isValidCallContextKey = "IsValidCallContext";
            string isValidReminderStateKey = "IsValidReminderState";
            string fireCountKey = "FireCount";

            ActorMethodContext currentMethodContext;

            public RemiderTestActor(ActorService actorService, ActorId actorId)
                : base(actorService, actorId)
            { }

            protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
            {
                this.currentMethodContext = actorMethodContext; 
                return base.OnPreActorMethodAsync(actorMethodContext);
            }

            public async Task<IActorReminder> InitStateAndCreateReminder(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
            {
                await this.StateManager.SetStateAsync(this.expectedReminderStateKey, state);

                return await this.RegisterReminderAsync(reminderName, state, dueTime, period);
            }

            public async Task<bool> ValidateActorState()
            {
                int fireCount = await this.StateManager.GetStateAsync<int>(this.fireCountKey);
                bool isValidCallContext = (await this.StateManager.TryGetStateAsync<bool>(this.isValidCallContextKey)).Value;
                bool isValidReminderState = (await this.StateManager.TryGetStateAsync<bool>(this.isValidReminderStateKey)).Value;

                if (fireCount == 0)
                {
                    throw new Exception("Reminder has not been fired");
                }
                if (!isValidCallContext)
                {
                    throw new Exception("Reminder callback has incorect method context");
                }
                if (!isValidReminderState)
                {
                    throw new Exception("Reminder callback has received incorect state");
                }

                return true;
            }

            public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
            {
                if (this.currentMethodContext.MethodName == "ReceiveReminderAsync" &&
                    this.currentMethodContext.CallType == ActorCallType.ReminderMethod)
                {
                    await this.StateManager.SetStateAsync(this.isValidCallContextKey, true);
                }

                byte[] expectedReminderState = await this.StateManager.GetStateAsync<byte[]>(this.expectedReminderStateKey);

                string receivedState = UTF8Encoding.UTF8.GetString(state);
                string expectedState = UTF8Encoding.UTF8.GetString(expectedReminderState);

                if (String.Equals(receivedState, expectedState))
                {
                    await this.StateManager.SetStateAsync(this.isValidReminderStateKey, true);
                }

                await this.StateManager.AddOrUpdateStateAsync(this.fireCountKey, 1, (stateName, oldValue) => { return oldValue + 1; });
            }
        }

        [Fact]
        public async Task ReminderSuccessfullyRegisteredAndFired()
        {
            ActorService actorService = await TestMocksRepository.GetActorService<RemiderTestActor>();

            string expectedReminderName = "TestReminder";
            byte[] expectedState = UTF8Encoding.UTF8.GetBytes("TestReminderState");
            TimeSpan expectedDueTime = TimeSpan.FromSeconds(1);
            TimeSpan expectedPeriod = TimeSpan.FromMinutes(1);

            IActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                actorId : new ActorId("RemiderTestActor1"),
                actorMethodContext : new ActorMethodContext(),
                createIfRequired : true,
                actorFunc : (actorBase, cacnelationToken) => {
                    return ((RemiderTestActor)actorBase).InitStateAndCreateReminder(
                        reminderName: expectedReminderName,
                        state: expectedState,
                        dueTime: expectedDueTime,
                        period: expectedPeriod);
                },
                callContext : "TestCallContext",
                timerCall : false,
                cancellationToken : new CancellationToken());

            Assert.NotNull(reminderResult);
            Assert.Equal(expectedReminderName, reminderResult.Name);
            Assert.Equal(expectedState, reminderResult.State);
            Assert.Equal(expectedDueTime, reminderResult.DueTime);
            Assert.Equal(expectedPeriod, reminderResult.Period);

            // Wait enough time for reminder to fire
            await Task.Delay(TimeSpan.FromSeconds(5));

            var exception = await Record.ExceptionAsync(async () =>
            {
                await actorService.ActorManager.DispatchToActorAsync(
                    actorId: new ActorId("RemiderTestActor1"),
                    actorMethodContext: new ActorMethodContext(),
                    createIfRequired: true,
                    actorFunc: (actorBase, cancellationToken) =>
                    {
                        return ((RemiderTestActor)actorBase).ValidateActorState();
                    },
                    callContext: "TestCallContext",
                    timerCall: false,
                    cancellationToken: new CancellationToken());
            });

            Assert.Null(exception);
        }
    }
}
