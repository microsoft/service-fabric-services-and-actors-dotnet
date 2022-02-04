// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class ActorStateInvalidMigrationConfigException : FabricException
    {
        public ActorStateInvalidMigrationConfigException()
            : base()
        {
        }

        public ActorStateInvalidMigrationConfigException(string message)
            : base(message)
        {
        }

        public ActorStateInvalidMigrationConfigException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private ActorStateInvalidMigrationConfigException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
