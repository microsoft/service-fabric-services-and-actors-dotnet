// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


#pragma warning disable CS0618 // Type or member is obsolete (KVSToRCMigration uses deprecated Migration APIs)

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class MigrationDataValidationException : FabricException
    {
        public MigrationDataValidationException()
            : base()
        {
        }

        public MigrationDataValidationException(string message)
            : base(message)
        {
        }

        public MigrationDataValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private MigrationDataValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
