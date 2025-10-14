using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.ActorIntegrationTests
{
    public class UnregisterReminderAsync
    {
        [Fact]
        public async Task ReminderIsInvalidatedAndDoesNotFireCallback()
        {
            IFuzz fuzzy = new RandomFuzz();

            int reminderCallbackInvocationCounter = 0;
            int callbackInvocationCounterBeforeUnregistering = 0;
            int callbackInvocationCounterAfterUnregistering = 0;

            ActorReminder reminderToUnregister = null;

            string fuzzyReminderName = fuzzy.String(Length.Between(5, 10));
            byte[] fuzzyReminderState = UTF8Encoding.UTF8.GetBytes(fuzzy.String(Length.Between(5, 10)));
            TimeSpan fuzzyDueTime = TimeSpan.FromSeconds(fuzzy.Int32().Between(1, 5));
            TimeSpan fuzzyPeriod = TimeSpan.FromSeconds(fuzzy.Int32().Between(1, 5));
            TimeSpan allowedTimeVariation = TimeSpan.FromMilliseconds(100);

            Func<ActorBase, CancellationToken, Task<ActorReminder>> registerActorReminder = async (actorBase, cancellationToken) =>
            {
                var testActor = (ITestableActor)actorBase;

                IActorReminder reminderResult = await testActor.RegisterReminderAsync(fuzzyReminderName, fuzzyReminderState, fuzzyDueTime, fuzzyPeriod);
                return (ActorReminder)reminderResult;
            };

            Func<ActorBase, CancellationToken, Task<ActorReminder>> unregisterActorReminder = async (actorBase, cancellationToke) =>
            {
                var testActor = (ITestableActor)actorBase;
                await testActor.UnregisterReminderAsync(reminderToUnregister);

                return reminderToUnregister;
            };

            Action<ReminderCallbackInfo> receiveReminderCallback = (reminderCallbackInfo) =>
            {
                reminderCallbackInvocationCounter += 1;
            };

            Func<ActorService, ActorId, ActorBase> actorFactory = (actorService, actorId) => new TestableActor(actorService, actorId, receiveReminderCallback);

            ActorService actorService = await TestableActorService.GetActorService<TestableActor>(actorFactory);

            ActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor4"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                registerActorReminder,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

            reminderToUnregister = reminderResult;
            callbackInvocationCounterBeforeUnregistering = reminderCallbackInvocationCounter;

            await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor4"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                unregisterActorReminder,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());


            await Task.Delay(fuzzyDueTime + fuzzyPeriod + allowedTimeVariation, TestContext.Current.CancellationToken);

            callbackInvocationCounterAfterUnregistering = reminderCallbackInvocationCounter;

            Assert.False(reminderToUnregister.IsValid());
            Assert.Equal(callbackInvocationCounterBeforeUnregistering, callbackInvocationCounterAfterUnregistering);
        }
    }
}