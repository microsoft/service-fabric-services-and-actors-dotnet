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
    internal sealed class ActorStateMigrationInProgressException : FabricTransientException
    {
        public ActorStateMigrationInProgressException()
            : base()
        {
        }

        public ActorStateMigrationInProgressException(string message)
            : base(message)
        {
        }

        public ActorStateMigrationInProgressException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private ActorStateMigrationInProgressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
