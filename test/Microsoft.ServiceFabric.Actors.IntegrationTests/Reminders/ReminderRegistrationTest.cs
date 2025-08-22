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
    public class ReminderRegistrationTest : ActorIntegrationTest
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
                currentMethodContext = actorMethodContext;
                return base.OnPreActorMethodAsync(actorMethodContext);
            }

            public async Task<IActorReminder> InitStateAndCreateReminder(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
            {
                await StateManager.SetStateAsync(expectedReminderStateKey, state);

                return await RegisterReminderAsync(reminderName, state, dueTime, period);
            }

            public async Task<bool> ValidateActorState()
            {
                int fireCount = await StateManager.GetStateAsync<int>(fireCountKey);
                bool isValidCallContext = (await StateManager.TryGetStateAsync<bool>(isValidCallContextKey)).Value;
                bool isValidReminderState = (await StateManager.TryGetStateAsync<bool>(isValidReminderStateKey)).Value;

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
                if (currentMethodContext.MethodName == "ReceiveReminderAsync" &&
                    currentMethodContext.CallType == ActorCallType.ReminderMethod)
                {
                    await StateManager.SetStateAsync(isValidCallContextKey, true);
                }

                byte[] expectedReminderState = await StateManager.GetStateAsync<byte[]>(expectedReminderStateKey);

                string receivedState = UTF8Encoding.UTF8.GetString(state);
                string expectedState = UTF8Encoding.UTF8.GetString(expectedReminderState);

                if (String.Equals(receivedState, expectedState))
                {
                    await StateManager.SetStateAsync(isValidReminderStateKey, true);
                }

                await StateManager.AddOrUpdateStateAsync(fireCountKey, 1, (stateName, oldValue) => { return oldValue + 1; });
            }
        }

        [Fact]
        public async Task ReminderSuccessfullyRegisteredAndFired()
        {
            ActorService actorService = await GetActorService<RemiderTestActor>();

            string expectedReminderName = "TestReminder";
            byte[] expectedState = UTF8Encoding.UTF8.GetBytes("TestReminderState");
            TimeSpan expectedDueTime = TimeSpan.FromSeconds(1);
            TimeSpan expectedPeriod = TimeSpan.FromMinutes(1);

            IActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("RemiderTestActor1"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                actorFunc: (actorBase, cacnelationToken) =>
                {
                    return ((RemiderTestActor)actorBase).InitStateAndCreateReminder(
                        reminderName: expectedReminderName,
                        state: expectedState,
                        dueTime: expectedDueTime,
                        period: expectedPeriod);
                },
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

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
