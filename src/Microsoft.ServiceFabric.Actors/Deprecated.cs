// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// Contains deprecation messages for obsolete APIs.
    /// </summary>
    internal static class Deprecated
    {
        /// <summary>
        /// Deprecation message for Microsoft.ServiceFabric.Actors.Migration APIs.
        /// </summary>
        internal const string MigrationApis = "Microsoft.ServiceFabric.Actors.Migration APIs are being removed in a future release.";

        /// <summary>
        /// Deprecation message for Microsoft.ServiceFabric.Actors.KVSToRCMigration APIs.
        /// </summary>
        internal const string KVSToRCMigrationApis = "Microsoft.ServiceFabric.Actors.KVSToRCMigration APIs are being removed in a future release.";
    }
}