using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.ActorIntegrationTests;

public class RegisterReminderAsync
{
    static readonly TimeSpan signalTimeout = TimeSpan.FromSeconds(30);

    public class WithNewReminderName : RegisterReminderAsync
    {
        [Fact]
        public async Task ReminderParametersPersistBetweenRegisteringAndReceivingReminder()
        {
            string expectedReminderName = "TestReminder";
            string expectedActorMethodName = "ReceiveReminderAsync";
            ActorCallType expectedActorCallType = ActorCallType.ReminderMethod;
            byte[] expectedState = UTF8Encoding.UTF8.GetBytes("TestReminderState");
            TimeSpan expectedDueTime = TimeSpan.FromSeconds(1);
            TimeSpan expectedPeriod = TimeSpan.FromMinutes(1);

            string actualReminderName = "";
            string actualActorMethodName = "";
            ActorCallType actualActorCallType = default;
            byte[] actualState = UTF8Encoding.UTF8.GetBytes("");
            TimeSpan actualDueTime = TimeSpan.Zero;
            TimeSpan actualPeriod = TimeSpan.Zero;

            using var signal = new SemaphoreSlim(0);

            Func<ActorBase, CancellationToken, Task<ActorReminder>> registerActorReminder = async (actorBase, cancellationToken) =>
            {
                var testActor = (ITestableActor)actorBase;
                IActorReminder reminderResult = await testActor.RegisterReminderAsync(expectedReminderName, expectedState, expectedDueTime, expectedPeriod);
                return (ActorReminder)reminderResult;
            };

            Action<ReminderCallbackInfo> receiveReminderCallback = (reminderCallbackInfo) =>
            {
                actualReminderName = reminderCallbackInfo.ReminderName;
                actualActorMethodName = reminderCallbackInfo.MethodContext.MethodName;
                actualActorCallType = reminderCallbackInfo.MethodContext.CallType;
                actualState = reminderCallbackInfo.State;
                actualDueTime = reminderCallbackInfo.DueTime;
                actualPeriod = reminderCallbackInfo.Period;
                signal.Release();
            };

            Func<ActorService, ActorId, ActorBase> actorFactory = (actorService, actorId) => new TestableActor(actorService, actorId, receiveReminderCallback);

            ActorService actorService = await TestableActorService.GetActorService<TestableActor>(actorFactory);

            ActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor1"),
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
            Assert.True(reminderResult.IsValid());

            Assert.True(await signal.WaitAsync(signalTimeout, TestContext.Current.CancellationToken), "Reminder callback was not invoked");

            Func<ActorBase, CancellationToken, Task<ActorReminder>> unregister = async (actorBase, cancellationToken) =>
            {
                await ((ITestableActor)actorBase).UnregisterReminderAsync(reminderResult);
                return reminderResult;
            };

            await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor1"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                unregister,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

            Assert.Equal(expectedReminderName, actualReminderName);
            Assert.Equal(expectedActorMethodName, actualActorMethodName);
            Assert.Equal(expectedActorCallType, actualActorCallType);
            Assert.Equal(expectedState, actualState);
            Assert.Equal(expectedDueTime, actualDueTime);
            Assert.Equal(expectedPeriod, actualPeriod);
        }

        [Fact]
        public async Task ReminderFiresInExpectedTimeIntervals()
        {
            IFuzz fuzzy = new RandomFuzz();

            TimeSpan reminderDueTime = TimeSpan.FromSeconds(2);
            TimeSpan reminderPeriod = TimeSpan.FromSeconds(1);
            TimeSpan tolerance = TimeSpan.FromSeconds(5);
            TimeSpan earlyFireTolerance = TimeSpan.FromMilliseconds(15); // https://learn.microsoft.com/dotnet/api/system.threading.timer

            using var signal = new SemaphoreSlim(0);

            Func<ActorBase, CancellationToken, Task<IActorReminder>> registerActorReminder = async (actorBase, cancellationToken) =>
            {
                var testActor = (ITestableActor)actorBase;
                return await testActor.RegisterReminderAsync(fuzzy.String(Length.Between(5, 10)), UTF8Encoding.UTF8.GetBytes(fuzzy.String(Length.Between(5, 10))), reminderDueTime, reminderPeriod);
            };

            Action<ReminderCallbackInfo> receiveReminderCallback = _ => signal.Release();

            Func<ActorService, ActorId, ActorBase> actorFactory = (actorService, actorId) => new TestableActor(actorService, actorId, receiveReminderCallback);

            ActorService actorService = await TestableActorService.GetActorService<TestableActor>(actorFactory);

            var stopwatch = Stopwatch.StartNew();

            IActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor2"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                registerActorReminder,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

            Assert.True(await signal.WaitAsync(signalTimeout, TestContext.Current.CancellationToken), "Reminder was not invoked after due time");
            TimeSpan firstCallbackElapsed = stopwatch.Elapsed;

            Assert.True(await signal.WaitAsync(signalTimeout, TestContext.Current.CancellationToken), "Reminder was not invoked after period");
            TimeSpan secondCallbackElapsed = stopwatch.Elapsed;

            Func<ActorBase, CancellationToken, Task<IActorReminder>> unregister = async (actorBase, cancellationToken) =>
            {
                await ((ITestableActor)actorBase).UnregisterReminderAsync(reminderResult);
                return reminderResult;
            };

            await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor2"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                unregister,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

            Assert.InRange(firstCallbackElapsed, reminderDueTime - earlyFireTolerance, reminderDueTime + tolerance);
            Assert.InRange(secondCallbackElapsed - firstCallbackElapsed, reminderPeriod - earlyFireTolerance, reminderPeriod + tolerance);
        }
    }

    public class WithExistingReminderName : RegisterReminderAsync
    {
        [Fact]
        public async Task NewReminderIsCreatedAndOldReminderIsInvalidated()
        {
            IFuzz fuzzy = new RandomFuzz();

            string fuzzyReminderName = fuzzy.String(Length.Between(5, 10));

            Func<ActorBase, CancellationToken, Task<ActorReminder>> registerActorReminder = async (actorBase, cancellationToken) =>
            {
                var testActor = (ITestableActor)actorBase;

                byte[] fuzzyReminderState = UTF8Encoding.UTF8.GetBytes(fuzzy.String(Length.Between(5, 10)));
                TimeSpan fuzzyDueTime = TimeSpan.FromSeconds(fuzzy.Int32().Between(1, 5));
                TimeSpan fuzzyPeriod = TimeSpan.FromSeconds(fuzzy.Int32().Between(1, 5));

                IActorReminder reminderResult = await testActor.RegisterReminderAsync(fuzzyReminderName, fuzzyReminderState, fuzzyDueTime, fuzzyPeriod);
                return (ActorReminder)reminderResult;
            };

            ActorService actorService = await TestableActorService.GetActorService<TestableActor>();

            ActorReminder firstReminder = await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor3"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                registerActorReminder,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

            ActorReminder secondReminder = await actorService.ActorManager.DispatchToActorAsync(
                actorId: new ActorId("TestableActor3"),
                actorMethodContext: new ActorMethodContext(),
                createIfRequired: true,
                registerActorReminder,
                callContext: "TestCallContext",
                timerCall: false,
                cancellationToken: new CancellationToken());

            Assert.False(firstReminder.IsValid());
            Assert.True(secondReminder.IsValid());
        }
    }
}

