// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Partition Reconfiguration event.
    /// </summary>
    public partial class PartitionReconfiguredEvent : PartitionEvent
    {
        /// <summary>
        /// Initializes a new instance of the PartitionReconfiguredEvent class.
        /// </summary>
        /// <param name="eventInstanceId">The identifier for the FabricEvent instance.</param>
        /// <param name="timeStamp">The time event was logged.</param>
        /// <param name="partitionId">An internal ID used by Service Fabric to uniquely identify a partition. This is a
        /// randomly generated GUID when the service was created. The partition ID is unique and does not change for the
        /// lifetime of the service. If the same service was deleted and recreated the IDs of its partitions would be
        /// different.</param>
        /// <param name="nodeName">The name of a Service Fabric node.</param>
        /// <param name="nodeInstanceId">Id of Node instance.</param>
        /// <param name="serviceType">Type of Service.</param>
        /// <param name="ccEpochDataLossVersion">CcEpochDataLoss version.</param>
        /// <param name="ccEpochConfigVersion">CcEpochConfig version.</param>
        /// <param name="reconfigType">Type of reconfiguration.</param>
        /// <param name="result">Describes reconfiguration result.</param>
        /// <param name="phase0DurationMs">Duration of Phase0 in milli-seconds.</param>
        /// <param name="phase1DurationMs">Duration of Phase1 in milli-seconds.</param>
        /// <param name="phase2DurationMs">Duration of Phase2 in milli-seconds.</param>
        /// <param name="phase3DurationMs">Duration of Phase3 in milli-seconds.</param>
        /// <param name="phase4DurationMs">Duration of Phase4 in milli-seconds.</param>
        /// <param name="totalDurationMs">Total duration in milli-seconds.</param>
        /// <param name="category">The category of event.</param>
        /// <param name="hasCorrelatedEvents">Shows there is existing related events available.</param>
        public PartitionReconfiguredEvent(
            Guid? eventInstanceId,
            DateTime? timeStamp,
            PartitionId partitionId,
            NodeName nodeName,
            string nodeInstanceId,
            string serviceType,
            long? ccEpochDataLossVersion,
            long? ccEpochConfigVersion,
            string reconfigType,
            string result,
            double? phase0DurationMs,
            double? phase1DurationMs,
            double? phase2DurationMs,
            double? phase3DurationMs,
            double? phase4DurationMs,
            double? totalDurationMs,
            string category = default(string),
            bool? hasCorrelatedEvents = default(bool?))
            : base(
                eventInstanceId,
                timeStamp,
                Common.FabricEventKind.PartitionReconfigured,
                partitionId,
                category,
                hasCorrelatedEvents)
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            nodeInstanceId.ThrowIfNull(nameof(nodeInstanceId));
            serviceType.ThrowIfNull(nameof(serviceType));
            ccEpochDataLossVersion.ThrowIfNull(nameof(ccEpochDataLossVersion));
            ccEpochConfigVersion.ThrowIfNull(nameof(ccEpochConfigVersion));
            reconfigType.ThrowIfNull(nameof(reconfigType));
            result.ThrowIfNull(nameof(result));
            phase0DurationMs.ThrowIfNull(nameof(phase0DurationMs));
            phase1DurationMs.ThrowIfNull(nameof(phase1DurationMs));
            phase2DurationMs.ThrowIfNull(nameof(phase2DurationMs));
            phase3DurationMs.ThrowIfNull(nameof(phase3DurationMs));
            phase4DurationMs.ThrowIfNull(nameof(phase4DurationMs));
            totalDurationMs.ThrowIfNull(nameof(totalDurationMs));
            this.NodeName = nodeName;
            this.NodeInstanceId = nodeInstanceId;
            this.ServiceType = serviceType;
            this.CcEpochDataLossVersion = ccEpochDataLossVersion;
            this.CcEpochConfigVersion = ccEpochConfigVersion;
            this.ReconfigType = reconfigType;
            this.Result = result;
            this.Phase0DurationMs = phase0DurationMs;
            this.Phase1DurationMs = phase1DurationMs;
            this.Phase2DurationMs = phase2DurationMs;
            this.Phase3DurationMs = phase3DurationMs;
            this.Phase4DurationMs = phase4DurationMs;
            this.TotalDurationMs = totalDurationMs;
        }

        /// <summary>
        /// Gets the name of a Service Fabric node.
        /// </summary>
        public NodeName NodeName { get; }

        /// <summary>
        /// Gets id of Node instance.
        /// </summary>
        public string NodeInstanceId { get; }

        /// <summary>
        /// Gets type of Service.
        /// </summary>
        public string ServiceType { get; }

        /// <summary>
        /// Gets ccEpochDataLoss version.
        /// </summary>
        public long? CcEpochDataLossVersion { get; }

        /// <summary>
        /// Gets ccEpochConfig version.
        /// </summary>
        public long? CcEpochConfigVersion { get; }

        /// <summary>
        /// Gets type of reconfiguration.
        /// </summary>
        public string ReconfigType { get; }

        /// <summary>
        /// Gets reconfiguration result.
        /// </summary>
        public string Result { get; }

        /// <summary>
        /// Gets duration of Phase0 in milli-seconds.
        /// </summary>
        public double? Phase0DurationMs { get; }

        /// <summary>
        /// Gets duration of Phase1 in milli-seconds.
        /// </summary>
        public double? Phase1DurationMs { get; }

        /// <summary>
        /// Gets duration of Phase2 in milli-seconds.
        /// </summary>
        public double? Phase2DurationMs { get; }

        /// <summary>
        /// Gets duration of Phase3 in milli-seconds.
        /// </summary>
        public double? Phase3DurationMs { get; }

        /// <summary>
        /// Gets duration of Phase4 in milli-seconds.
        /// </summary>
        public double? Phase4DurationMs { get; }

        /// <summary>
        /// Gets total duration in milli-seconds.
        /// </summary>
        public double? TotalDurationMs { get; }
    }
}
