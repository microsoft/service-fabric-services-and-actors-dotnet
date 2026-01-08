// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric service replica deployed on a node.
    /// </summary>
    public abstract partial class DeployedServiceReplicaDetailInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedServiceReplicaDetailInfo class.
        /// </summary>
        /// <param name="serviceKind">The kind of service (Stateless or Stateful).</param>
        /// <param name="serviceName">Full hierarchical name of the service in URI format starting with `fabric:`.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="currentServiceOperation">Specifies the current active life-cycle operation on a stateful service
        /// replica or stateless service instance. Possible values include: 'Unknown', 'None', 'Open', 'ChangeRole', 'Close',
        /// 'Abort'</param>
        /// <param name="currentServiceOperationStartTimeUtc">The start time of the current service operation in UTC
        /// format.</param>
        /// <param name="reportedLoad">List of load reported by replica.</param>
        protected DeployedServiceReplicaDetailInfo(
            ServiceKind? serviceKind,
            ServiceName serviceName = default(ServiceName),
            PartitionId partitionId = default(PartitionId),
            ServiceOperationName? currentServiceOperation = default(ServiceOperationName?),
            DateTime? currentServiceOperationStartTimeUtc = default(DateTime?),
            IEnumerable<LoadMetricReportInfo> reportedLoad = default(IEnumerable<LoadMetricReportInfo>))
        {
            serviceKind.ThrowIfNull(nameof(serviceKind));
            this.ServiceKind = serviceKind;
            this.ServiceName = serviceName;
            this.PartitionId = partitionId;
            this.CurrentServiceOperation = currentServiceOperation;
            this.CurrentServiceOperationStartTimeUtc = currentServiceOperationStartTimeUtc;
            this.ReportedLoad = reportedLoad;
        }

        /// <summary>
        /// Gets full hierarchical name of the service in URI format starting with `fabric:`.
        /// </summary>
        public ServiceName ServiceName { get; }

        /// <summary>
        /// Gets an internal ID used by Service Fabric to uniquely identify a partition. This is a randomly generated GUID when
        /// the service was created. The partition ID is unique and does not change for the lifetime of the service. If the
        /// same service was deleted and recreated the IDs of its partitions would be different.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <summary>
        /// Gets specifies the current active life-cycle operation on a stateful service replica or stateless service instance.
        /// Possible values include: 'Unknown', 'None', 'Open', 'ChangeRole', 'Close', 'Abort'
        /// </summary>
        public ServiceOperationName? CurrentServiceOperation { get; }

        /// <summary>
        /// Gets the start time of the current service operation in UTC format.
        /// </summary>
        public DateTime? CurrentServiceOperationStartTimeUtc { get; }

        /// <summary>
        /// Gets list of load reported by replica.
        /// </summary>
        public IEnumerable<LoadMetricReportInfo> ReportedLoad { get; }

        /// <summary>
        /// Gets the kind of service (Stateless or Stateful).
        /// </summary>
        public ServiceKind? ServiceKind { get; }
    }
}
