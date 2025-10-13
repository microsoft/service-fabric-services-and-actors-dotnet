using Microsoft.ServiceFabric.Actors.Runtime;
using System;

namespace Microsoft.ServiceFabric.Actors.ActorIntegrationTests
{
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
}