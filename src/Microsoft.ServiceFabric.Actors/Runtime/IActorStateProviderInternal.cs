// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;

    internal interface IActorStateProviderInternal
    {
        string TraceType { get; }

        string TraceId { get; }

        ReplicaRole CurrentReplicaRole { get; }

        TimeSpan TransientErrorRetryDelay { get; }

        TimeSpan CurrentLogicalTime { get; }
    }
}