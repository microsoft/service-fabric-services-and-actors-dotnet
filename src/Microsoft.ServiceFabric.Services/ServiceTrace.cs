// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services
{
    //extern alias Internal;
    using System;
    using System.Globalization;
    using System.Fabric.Common.Tracing;

    internal static class ServiceTrace
    {
        internal static FabricEvents.ExtensionsEvents Source;

        static ServiceTrace()
        {
            TraceConfig.InitializeFromConfigStore();
            Source = new FabricEvents.ExtensionsEvents(FabricEvents.Tasks.ServiceFramework);
        }

        internal static string GetTraceIdForReplica(Guid partitionId, long replicaId)
        {
            return String.Concat(
                partitionId.ToString("B"),
                ":",
                replicaId.ToString(CultureInfo.InvariantCulture));
        }
    }
}
