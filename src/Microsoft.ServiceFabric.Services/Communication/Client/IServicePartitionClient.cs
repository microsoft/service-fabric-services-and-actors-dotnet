// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Client;

    /// <summary>
    /// Defines the interface for the client that can communicate with replicas of a particular service partition.
    /// </summary>
    /// <typeparam name="TCommunicationClient">Type of ICommunicationClient</typeparam>
    public interface IServicePartitionClient<TCommunicationClient> where TCommunicationClient : ICommunicationClient
    {
        /// <summary>
        /// Gets the name of the service
        /// </summary>
        /// <value>Name of the service</value>
        Uri ServiceUri { get; }

        /// <summary>
        /// Gets the key of the partition the client is communicating with. 
        /// </summary>
        /// <value>Partition key</value>
        ServicePartitionKey PartitionKey { get; }

        /// <summary>
        /// Gets the information about which replica in the partition the client should connect to.
        /// </summary>
        /// <value>A <see cref="Microsoft.ServiceFabric.Services.Communication.Client.TargetReplicaSelector"/></value>
        TargetReplicaSelector TargetReplicaSelector { get; }

        /// <summary>
        /// Gets the name of the listener in the replica to which the client should connect to.
        /// </summary>
        /// <value>Listener name</value>
        string ListenerName { get; }

        /// <summary>
        /// Gets the communication client factory
        /// </summary>
        /// <value>Communication client factory</value>
        ICommunicationClientFactory<TCommunicationClient> Factory { get; }

        /// <summary>
        /// Gets the resolved service partition that was set on the client.
        /// </summary>
        /// <param name="resolvedServicePartition">previous ResolvedServicePartition</param>
        /// <returns>true if a ResolvedServicePartition was set</returns>
        bool TryGetLastResolvedServicePartition(out ResolvedServicePartition resolvedServicePartition);
    }
}
