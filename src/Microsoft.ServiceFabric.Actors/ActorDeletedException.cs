// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class ActorDeletedException : FabricTransientException
    {
        public ActorDeletedException()
            : base()
        {
        }

        public ActorDeletedException(string message)
            : base(message)
        {
        }

        public ActorDeletedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private ActorDeletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
