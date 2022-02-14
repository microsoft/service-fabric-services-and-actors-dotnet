// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception to represent State Migration is in progress.
    /// </summary>
    [Serializable]
    internal sealed class ActorStateMigrationInProgressException : FabricTransientException
    {
        // TODO: Revisit exception handling.
        // TODO: Add exception conversion handlers.
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
