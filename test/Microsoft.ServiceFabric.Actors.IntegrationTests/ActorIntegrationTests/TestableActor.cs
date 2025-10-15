using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Actors.ActorIntegrationTests
{
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
}