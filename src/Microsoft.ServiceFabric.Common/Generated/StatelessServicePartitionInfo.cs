// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a partition of a stateless Service Fabric service.
    /// </summary>
    public partial class StatelessServicePartitionInfo : ServicePartitionInfo
    {
        /// <summary>
        /// Initializes a new instance of the StatelessServicePartitionInfo class.
        /// </summary>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="partitionStatus">The status of the service fabric service partition. Possible values include:
        /// 'Invalid', 'Ready', 'NotReady', 'InQuorumLoss', 'Reconfiguring', 'Deleting'</param>
        /// <param name="partitionInformation">Information about the partition identity, partitioning scheme and keys supported
        /// by it.</param>
        /// <param name="instanceCount">Number of instances of this partition.</param>
        /// <param name="minInstanceCount">MinInstanceCount is the minimum number of instances that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </param>
        /// <param name="minInstancePercentage">MinInstancePercentage is the minimum percentage of InstanceCount that must be
        /// up to meet the EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </param>
        public StatelessServicePartitionInfo(
            HealthState? healthState = default(HealthState?),
            ServicePartitionStatus? partitionStatus = default(ServicePartitionStatus?),
            PartitionInformation partitionInformation = default(PartitionInformation),
            long? instanceCount = default(long?),
            int? minInstanceCount = default(int?),
            int? minInstancePercentage = default(int?))
            : base(
                Common.ServiceKind.Stateless,
                healthState,
                partitionStatus,
                partitionInformation)
        {
            this.InstanceCount = instanceCount;
            this.MinInstanceCount = minInstanceCount;
            this.MinInstancePercentage = minInstancePercentage;
        }

        /// <summary>
        /// Gets number of instances of this partition.
        /// </summary>
        public long? InstanceCount { get; }

        /// <summary>
        /// Gets minInstanceCount is the minimum number of instances that must be up to meet the EnsureAvailability safety
        /// check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstanceCount computation -1 is first converted into the number of
        /// nodes on which the instances are allowed to be placed according to the placement constraints on the service.
        /// </summary>
        public int? MinInstanceCount { get; }

        /// <summary>
        /// Gets minInstancePercentage is the minimum percentage of InstanceCount that must be up to meet the
        /// EnsureAvailability safety check during operations like upgrade or deactivate node.
        /// The actual number that is used is max( MinInstanceCount, ceil( MinInstancePercentage/100.0 * InstanceCount) ).
        /// Note, if InstanceCount is set to -1, during MinInstancePercentage computation, -1 is first converted into the
        /// number of nodes on which the instances are allowed to be placed according to the placement constraints on the
        /// service.
        /// </summary>
        public int? MinInstancePercentage { get; }
    }
}
