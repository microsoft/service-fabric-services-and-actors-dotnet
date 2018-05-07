// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class ReminderLoadInProgressException : FabricTransientException
    {
        public ReminderLoadInProgressException()
            : base()
        {
        }

        public ReminderLoadInProgressException(string message)
            : base(message)
        {
        }

        public ReminderLoadInProgressException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private ReminderLoadInProgressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
