// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Context for request forwarder when the current service is unable to service the request.
    /// </summary>
    public class RequestForwarderContext
    {
        /// <summary>
        /// Gets the remote service uri.
        /// </summary>
        public Uri ServiceUri { get; internal set; }

        /// <summary>
        /// Gets the remote partition key.
        /// </summary>
        public ServicePartitionKey ServicePartitionKey { get; internal set; }

        /// <summary>
        /// Gets the target replica selector.
        /// </summary>
        public TargetReplicaSelector ReplicaSelector { get; internal set; }

        /// <summary>
        /// Gets the trace id for logging.
        /// </summary>
        public string TraceId { get; internal set; }
    }
}
