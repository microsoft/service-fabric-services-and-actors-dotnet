// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;

    /// <summary>
    /// Interface for ActorStateProviders internal implementations.
    /// </summary>
    internal interface IActorStateProviderInternal
    {
        /// <summary>
        /// Gets TraceType used for tracing.
        /// </summary>
        string TraceType { get; }

        /// <summary>
        /// Gets trace id used for tracing.
        /// </summary>
        string TraceId { get; }

        /// <summary>
        /// Gets current replica role.
        /// </summary>
        ReplicaRole CurrentReplicaRole { get; }

        /// <summary>
        /// Gets retry delay time for transient error.
        /// </summary>
        TimeSpan TransientErrorRetryDelay { get; }

        /// <summary>
        /// Gets timeout for the operations.
        /// </summary>
        TimeSpan OperationTimeout { get; }

        /// <summary>
        /// Gets current logical time.
        /// </summary>
        TimeSpan CurrentLogicalTime { get; }

        /// <summary>
        /// Gets a long to track replica role changes.
        /// </summary>
        long RoleChangeTracker { get; }
    }
}
