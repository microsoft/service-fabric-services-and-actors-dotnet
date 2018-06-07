// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using Microsoft.ServiceFabric.Services;

    internal static class ActorTrace
    {
        static ActorTrace()
        {
            Source = ActorEventSource.Instance;
        }

        internal static ActorEventSource Source { get; }

        internal static string GetTraceIdForActor(Guid partitionId, long replicaId, ActorId actorId)
        {
            return string.Concat(
                GetTraceIdForReplica(partitionId, replicaId),
                ":",
                actorId.GetStorageKey());
        }

        internal static string GetTraceIdForReplica(Guid partitionId, long replicaId)
        {
            return ServiceTrace.GetTraceIdForReplica(partitionId, replicaId);
        }
    }
}
