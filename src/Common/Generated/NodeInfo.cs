// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a node in Service Fabric cluster.
    /// </summary>
    public partial class NodeInfo
    {
        /// <summary>
        /// Initializes a new instance of the NodeInfo class.
        /// </summary>
        /// <param name="name">The name of a Service Fabric node.</param>
        /// <param name="ipAddressOrFQDN">The IP address or fully qualified domain name of the node.</param>
        /// <param name="type">The type of the node.</param>
        /// <param name="codeVersion">The version of Service Fabric binaries that the node is running.</param>
        /// <param name="configVersion">The version of Service Fabric cluster manifest that the node is using.</param>
        /// <param name="nodeStatus">The status of the node. Possible values include: 'Invalid', 'Up', 'Down', 'Enabling',
        /// 'Disabling', 'Disabled', 'Unknown', 'Removed'</param>
        /// <param name="nodeUpTimeInSeconds">Time in seconds since the node has been in NodeStatus Up. Value zero indicates
        /// that the node is not Up.</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="isSeedNode">Indicates if the node is a seed node or not. Returns true if the node is a seed node,
        /// otherwise false. A quorum of seed nodes are required for proper operation of Service Fabric cluster.</param>
        /// <param name="upgradeDomain">The upgrade domain of the node.</param>
        /// <param name="faultDomain">The fault domain of the node.</param>
        /// <param name="id">An internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically
        /// generated from node name.</param>
        /// <param name="instanceId">The ID representing the node instance. While the ID of the node is deterministically
        /// generated from the node name and remains same across restarts, the InstanceId changes every time node
        /// restarts.</param>
        /// <param name="nodeDeactivationInfo">Information about the node deactivation. This information is valid for a node
        /// that is undergoing deactivation or has already been deactivated.</param>
        /// <param name="isStopped">Indicates if the node is stopped by calling stop node API or not. Returns true if the node
        /// is stopped, otherwise false.</param>
        /// <param name="nodeDownTimeInSeconds">Time in seconds since the node has been in NodeStatus Down. Value zero
        /// indicates node is not NodeStatus Down.</param>
        /// <param name="nodeUpAt">Date time in UTC when the node came up. If the node has never been up then this value will
        /// be zero date time.</param>
        /// <param name="nodeDownAt">Date time in UTC when the node went down. If node has never been down then this value will
        /// be zero date time.</param>
        /// <param name="nodeTags">List that contains tags, which will be applied to the nodes.</param>
        /// <param name="isNodeByNodeUpgradeInProgress">Indicates if a node-by-node upgrade is currently being performed on
        /// this node.</param>
        /// <param name="infrastructurePlacementID">PlacementID used by the InfrastructureService.</param>
        public NodeInfo(
            NodeName name = default(NodeName),
            string ipAddressOrFQDN = default(string),
            string type = default(string),
            string codeVersion = default(string),
            string configVersion = default(string),
            NodeStatus? nodeStatus = default(NodeStatus?),
            string nodeUpTimeInSeconds = default(string),
            HealthState? healthState = default(HealthState?),
            bool? isSeedNode = default(bool?),
            string upgradeDomain = default(string),
            string faultDomain = default(string),
            NodeId id = default(NodeId),
            string instanceId = default(string),
            NodeDeactivationInfo nodeDeactivationInfo = default(NodeDeactivationInfo),
            bool? isStopped = default(bool?),
            string nodeDownTimeInSeconds = default(string),
            DateTime? nodeUpAt = default(DateTime?),
            DateTime? nodeDownAt = default(DateTime?),
            IEnumerable<string> nodeTags = default(IEnumerable<string>),
            bool? isNodeByNodeUpgradeInProgress = default(bool?),
            string infrastructurePlacementID = default(string))
        {
            this.Name = name;
            this.IpAddressOrFQDN = ipAddressOrFQDN;
            this.Type = type;
            this.CodeVersion = codeVersion;
            this.ConfigVersion = configVersion;
            this.NodeStatus = nodeStatus;
            this.NodeUpTimeInSeconds = nodeUpTimeInSeconds;
            this.HealthState = healthState;
            this.IsSeedNode = isSeedNode;
            this.UpgradeDomain = upgradeDomain;
            this.FaultDomain = faultDomain;
            this.Id = id;
            this.InstanceId = instanceId;
            this.NodeDeactivationInfo = nodeDeactivationInfo;
            this.IsStopped = isStopped;
            this.NodeDownTimeInSeconds = nodeDownTimeInSeconds;
            this.NodeUpAt = nodeUpAt;
            this.NodeDownAt = nodeDownAt;
            this.NodeTags = nodeTags;
            this.IsNodeByNodeUpgradeInProgress = isNodeByNodeUpgradeInProgress;
            this.InfrastructurePlacementID = infrastructurePlacementID;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName Name { get; }

        /// <summary>
        /// Gets the IP address or fully qualified domain name of the node.
        /// </summary>
        public string IpAddressOrFQDN { get; }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the version of Service Fabric binaries that the node is running.
        /// </summary>
        public string CodeVersion { get; }

        /// <summary>
        /// Gets the version of Service Fabric cluster manifest that the node is using.
        /// </summary>
        public string ConfigVersion { get; }

        /// <summary>
        /// Gets the status of the node. Possible values include: 'Invalid', 'Up', 'Down', 'Enabling', 'Disabling', 'Disabled',
        /// 'Unknown', 'Removed'
        /// </summary>
        public NodeStatus? NodeStatus { get; }

        /// <summary>
        /// Gets time in seconds since the node has been in NodeStatus Up. Value zero indicates that the node is not Up.
        /// </summary>
        public string NodeUpTimeInSeconds { get; }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; }

        /// <summary>
        /// Gets indicates if the node is a seed node or not. Returns true if the node is a seed node, otherwise false. A
        /// quorum of seed nodes are required for proper operation of Service Fabric cluster.
        /// </summary>
        public bool? IsSeedNode { get; }

        /// <summary>
        /// Gets the upgrade domain of the node.
        /// </summary>
        public string UpgradeDomain { get; }

        /// <summary>
        /// Gets the fault domain of the node.
        /// </summary>
        public string FaultDomain { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a node. Node Id is deterministically generated from
        /// node name.
        /// </summary>
        public NodeId Id { get; }

        /// <summary>
        /// Gets the ID representing the node instance. While the ID of the node is deterministically generated from the node
        /// name and remains same across restarts, the InstanceId changes every time node restarts.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// Gets information about the node deactivation. This information is valid for a node that is undergoing deactivation
        /// or has already been deactivated.
        /// </summary>
        public NodeDeactivationInfo NodeDeactivationInfo { get; }

        /// <summary>
        /// Gets indicates if the node is stopped by calling stop node API or not. Returns true if the node is stopped,
        /// otherwise false.
        /// </summary>
        public bool? IsStopped { get; }

        /// <summary>
        /// Gets time in seconds since the node has been in NodeStatus Down. Value zero indicates node is not NodeStatus Down.
        /// </summary>
        public string NodeDownTimeInSeconds { get; }

        /// <summary>
        /// Gets date time in UTC when the node came up. If the node has never been up then this value will be zero date time.
        /// </summary>
        public DateTime? NodeUpAt { get; }

        /// <summary>
        /// Gets date time in UTC when the node went down. If node has never been down then this value will be zero date time.
        /// </summary>
        public DateTime? NodeDownAt { get; }

        /// <summary>
        /// Gets list that contains tags, which will be applied to the nodes.
        /// </summary>
        public IEnumerable<string> NodeTags { get; }

        /// <summary>
        /// Gets indicates if a node-by-node upgrade is currently being performed on this node.
        /// </summary>
        public bool? IsNodeByNodeUpgradeInProgress { get; }

        /// <summary>
        /// Gets placementID used by the InfrastructureService.
        /// </summary>
        public string InfrastructurePlacementID { get; }
    }
}
