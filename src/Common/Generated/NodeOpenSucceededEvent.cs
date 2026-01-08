// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Node Opened Succeeded event.
    /// </summary>
    public partial class NodeOpenSucceededEvent : NodeEvent
    {
        /// <summary>
        /// Initializes a new instance of the NodeOpenSucceededEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstance">Id of Node instance.</param>
        /// <param name="nodeId">Id of Node.</param>
        /// <param name="upgradeDomain">Upgrade domain of Node.</param>
        /// <param name="faultDomain">Fault domain of Node.</param>
        /// <param name="ipAddressOrFQDN">IP address or FQDN.</param>
        /// <param name="hostname">Name of Host.</param>
        /// <param name="isSeedNode">Indicates if it is seed node.</param>
        /// <param name="nodeVersion">Version of Node.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public NodeOpenSucceededEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            NodeName nodeName,
            long? nodeInstance,
            string nodeId,
            string upgradeDomain,
            string faultDomain,
            string ipAddressOrFQDN,
            string hostname,
            bool? isSeedNode,
            string nodeVersion,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.NodeOpenSucceeded,
                nodeName,
                category,
                hasCorrelatedEvents)
        {
            nodeInstance.ThrowIfNull(nameof(nodeInstance));
            nodeId.ThrowIfNull(nameof(nodeId));
            upgradeDomain.ThrowIfNull(nameof(upgradeDomain));
            faultDomain.ThrowIfNull(nameof(faultDomain));
            ipAddressOrFQDN.ThrowIfNull(nameof(ipAddressOrFQDN));
            hostname.ThrowIfNull(nameof(hostname));
            isSeedNode.ThrowIfNull(nameof(isSeedNode));
            nodeVersion.ThrowIfNull(nameof(nodeVersion));
            this.NodeInstance = nodeInstance;
            this.NodeId = nodeId;
            this.UpgradeDomain = upgradeDomain;
            this.FaultDomain = faultDomain;
            this.IpAddressOrFQDN = ipAddressOrFQDN;
            this.Hostname = hostname;
            this.IsSeedNode = isSeedNode;
            this.NodeVersion = nodeVersion;
        }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public long? NodeInstance { get; }

        /// <summary>
        /// Gets id of Node.
        /// </summary>
        public string NodeId { get; }

        /// <summary>
        /// Gets upgrade domain of Node.
        /// </summary>
        public string UpgradeDomain { get; }

        /// <summary>
        /// Gets fault domain of Node.
        /// </summary>
        public string FaultDomain { get; }

        /// <summary>
        /// Gets IP address or FQDN.
        /// </summary>
        public string IpAddressOrFQDN { get; }

        /// <summary>
        /// Gets name of Host.
        /// </summary>
        public string Hostname { get; }

        /// <summary>
        /// Gets indicates if it is seed node.
        /// </summary>
        public bool? IsSeedNode { get; }

        /// <summary>
        /// Gets version of Node.
        /// </summary>
        public string NodeVersion { get; }
    }
}
