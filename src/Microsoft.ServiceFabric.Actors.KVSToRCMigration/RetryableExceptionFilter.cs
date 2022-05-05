// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;

    internal static class RetryableExceptionFilter
    {
        private static IList<string> list = new List<string>
        {
            typeof(FabricNotPrimaryException).Name,
        };

        public static bool Contains(Exception ex)
        {
            return list.Contains(ex.GetType().Name);
        }
    }
}
