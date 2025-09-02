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

        class ReminderCallbackInfo
        {
            public ReminderCallbackInfo(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period, ActorMethodContext methodContext)
            {
                ReminderName = reminderName;
                State = state;
                DueTime = dueTime;
                Period = period;
                MethodContext = methodContext;
            }

            public string ReminderName { get; set; }
            public byte[] State { get; set; }
            public TimeSpan DueTime { get; set; }
            public TimeSpan Period { get; set; }
            public ActorMethodContext MethodContext { get; set; }
        }

        interface ITestableActor : IActor
        {
            Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period);
            Task UnregisterReminderAsync(IActorReminder reminder);
        }

        class TestableActor : Actor, ITestableActor, IRemindable
        {
            ActorMethodContext currentMethodContext;
            Action<ReminderCallbackInfo> receiveReminderCallback;

            public TestableActor(ActorService actorService, ActorId actorId)
                : base(actorService, actorId)
            {
                this.receiveReminderCallback = (reminderCallbackInfo) => { return; };
            }

            public TestableActor(ActorService actorService, ActorId actorId, Action<ReminderCallbackInfo> receiveReminderCallback)
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
                var reminderCallbackInfo = new ReminderCallbackInfo(reminderName, state, dueTime, period, currentMethodContext);
                receiveReminderCallback(reminderCallbackInfo);
                return Task.CompletedTask;
            }

            public new Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
            {
                return base.RegisterReminderAsync(reminderName, state, dueTime, period);
            }

            public new Task UnregisterReminderAsync(IActorReminder reminder)
            {
                return base.UnregisterReminderAsync(reminder);
            }
        }

        public class RegisterReminderAsync : ActorIntegrationTest
        {

            public class WithNewReminderName : RegisterReminderAsync
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

                        reminderCallbackInvocationCounter += 1;
                    };

                    Func<ActorService, ActorId, ActorBase> actorFactory = (actorService, actorId) => new TestableActor(actorService, actorId, receiveReminderCallback);

                    ActorService actorService = await GetActorService<TestableActor>(actorFactory);

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

                [Fact]
                public async Task ReminderFiresInExpectedTimeIntervals()
                {
                    IFuzz fuzzy = new RandomFuzz();

                    int expectedReminderInvocationCounterAfterReminderDueTime = 1;
                    int expectedReminderInvocationCounterAfterReminderPeriod = 2;

                    int reminderCallbackInvocationCounter = 0;
                    int reminderInvocationCounterAfterReminderDueTime = 0;
                    int reminderInvocationCounterAfterReminderPeriod = 0;

                    TimeSpan reminderDueTime = TimeSpan.FromSeconds(2);
                    TimeSpan reminderPeriod = TimeSpan.FromSeconds(1);
                    TimeSpan allowedTimeVariation = TimeSpan.FromMilliseconds(100);

                    Func<ActorBase, CancellationToken, Task<IActorReminder>> registerActorReminder = async (actorBase, cancellationToken) =>
                    {
                        var testActor = (ITestableActor)actorBase;
                        return await testActor.RegisterReminderAsync(fuzzy.String(Length.Between(5, 10)), UTF8Encoding.UTF8.GetBytes(fuzzy.String(Length.Between(5, 10))), reminderDueTime, reminderPeriod);
                    };

                    Action<ReminderCallbackInfo> receiveReminderCallback = (reminderCallbackInfo) =>
                    {
                        reminderCallbackInvocationCounter += 1;
                    };

                    Func<ActorService, ActorId, ActorBase> actorFactory = (actorService, actorId) => new TestableActor(actorService, actorId, receiveReminderCallback);

                    ActorService actorService = await GetActorService<TestableActor>(actorFactory);

                    IActorReminder reminderResult = await actorService.ActorManager.DispatchToActorAsync(
                        actorId: new ActorId("TestableActor2"),
                        actorMethodContext: new ActorMethodContext(),
                        createIfRequired: true,
                        registerActorReminder,
                        callContext: "TestCallContext",
                        timerCall: false,
                        cancellationToken: new CancellationToken());

                    await Task.Delay(reminderDueTime + allowedTimeVariation);

                    reminderInvocationCounterAfterReminderDueTime = reminderCallbackInvocationCounter;

                    await Task.Delay(reminderPeriod + allowedTimeVariation);

                    reminderInvocationCounterAfterReminderPeriod = reminderCallbackInvocationCounter;

                    Assert.Equal(expectedReminderInvocationCounterAfterReminderDueTime, reminderInvocationCounterAfterReminderDueTime);
                    Assert.Equal(expectedReminderInvocationCounterAfterReminderPeriod, reminderInvocationCounterAfterReminderPeriod);
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

                    ActorService actorService = await GetActorService<TestableActor>();

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

        public class UnregisterReminderAsync : ActorIntegrationTest
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

                ActorService actorService = await GetActorService<TestableActor>(actorFactory);

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


                await Task.Delay(fuzzyDueTime + fuzzyPeriod + allowedTimeVariation);

                callbackInvocationCounterAfterUnregistering = reminderCallbackInvocationCounter;

                Assert.False(reminderToUnregister.IsValid());
                Assert.Equal(callbackInvocationCounterBeforeUnregistering, callbackInvocationCounterAfterUnregistering);
            }
        }
    }
}
