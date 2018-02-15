// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    //extern alias Internal;
    using System;
    using System.Globalization;

    internal static class ServiceTrace
    {
        internal static ServiceEventSource Source;

        static ServiceTrace()
        {
            Source = ServiceEventSource.Instance;
        }

        internal static string GetTraceIdForReplica(Guid partitionId, long replicaId)
        {
            return string.Concat(
                partitionId.ToString("B"),
                ":",
                replicaId.ToString(CultureInfo.InvariantCulture));
        }
    }
}
