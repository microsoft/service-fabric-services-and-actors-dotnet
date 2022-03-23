// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class ActorStateMigratedDataValidationFailedException : FabricException
    {
        public ActorStateMigratedDataValidationFailedException()
            : base()
        {
        }

        public ActorStateMigratedDataValidationFailedException(string message)
            : base(message)
        {
        }

        public ActorStateMigratedDataValidationFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private ActorStateMigratedDataValidationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
