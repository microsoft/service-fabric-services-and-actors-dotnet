// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System;
    using System.Globalization;

    internal static class ServiceTrace
    {
        private static ServiceEventSource source;

        static ServiceTrace()
        {
            Source = ServiceEventSource.Instance;
        }

        internal static ServiceEventSource Source
        {
            get
            {
                return source;
            }

            private set
            {
                source = value;
            }
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
