// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;

    [Flags]
    internal enum MigrationMode
    {
        Auto,

        Manual,

        ManualDowntime,
    }
}
