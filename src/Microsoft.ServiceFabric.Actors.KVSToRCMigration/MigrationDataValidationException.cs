// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
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
    }
}
