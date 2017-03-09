// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    /// <summary>
    /// An <see cref="ActorMethodContext"/> contains information about the method that is invoked by actor runtime and
    /// is passed as an argument to <see cref="ActorBase.OnPreActorMethodAsync"/> and <see cref="ActorBase.OnPostActorMethodAsync"/>.
    /// </summary>
    public struct ActorMethodContext
    {
        private readonly string actorMethodName;
        private readonly ActorCallType actorCallType;

        private ActorMethodContext(string methodName, ActorCallType callType)
        {
            this.actorMethodName = methodName;
            this.actorCallType = callType;
        }

        internal static ActorMethodContext CreateForActor(string methodName)
        {
            return new ActorMethodContext(methodName, ActorCallType.ActorInterfaceMethod);
        }

        internal static ActorMethodContext CreateForTimer(string methodName)
        {
            return new ActorMethodContext(methodName, ActorCallType.TimerMethod);
        }

        internal static ActorMethodContext CreateForReminder(string methodName)
        {
            return new ActorMethodContext(methodName, ActorCallType.ReminderMethod);
        }

        /// <summary>
        /// Name of the method invoked by actor runtime.
        /// </summary>
        /// <value>A string representing the name of method.</value>
        public string MethodName
        {
            get { return this.actorMethodName; }
        }

        /// <summary>
        /// Type of call by actor runtime (e.g. actor interface method, timer callback etc.).
        /// </summary>
        /// <value>
        /// An <see cref="ActorCallType"/> representing the call type.
        /// </value>
        public ActorCallType CallType
        {
            get { return this.actorCallType; }
        }
    }
}