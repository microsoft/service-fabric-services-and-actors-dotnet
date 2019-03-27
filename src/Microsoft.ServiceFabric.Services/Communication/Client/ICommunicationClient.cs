// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System.Fabric;

    /// <summary>
    /// Defines the interface that represents the communication client for reliable services.
    /// </summary>
    public interface ICommunicationClient
    {
        /// <summary>
        /// Gets or Sets the Resolved service partition which was used when this client was created.
        /// </summary>
        /// <value><see cref="System.Fabric.ResolvedServicePartition" /> object</value>
        ResolvedServicePartition ResolvedServicePartition { get; set; }

        /// <summary>
        /// Gets or Sets the name of the listener in the replica or instance to which the client is
        /// connected to.
        /// </summary>
        /// <value>Name of the listener</value>
        string ListenerName { get; set; }

        /// <summary>
        /// Gets or Sets the service endpoint to which the client is connected to.
        /// </summary>
        /// <value><see cref="System.Fabric.ResolvedServiceEndpoint" /></value>
        ResolvedServiceEndpoint Endpoint { get; set; }
    }
}
