// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Text;

    /// <summary>
    /// Configures the replicator used by the <see cref="IReliableStateManager"/>, adding copy-batching, stable-read, and replication-batching settings.
    /// </summary>
    public class ReliableStateManagerReplicatorSettings2 : ReliableStateManagerReplicatorSettings
    {
        /// <summary>
        /// Gets or sets the size, in kilobytes, of the copy log message used to build a replica. A higher value copies more log records in each message.
        /// </summary>
        /// <value>
        /// The default is 0, which copies one log record at a time. The value must be greater than or equal to 0.
        /// </value>
        public long? CopyBatchSizeInKB { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether stable reads are enabled. Stable reads allow every replica to return only values that are quorum acknowledged on a read.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if stable reads are enabled; otherwise, <see langword="false" />. The default is <see langword="false" />.
        /// </value>
        public bool? EnableStableReads { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a build can be canceled when the log is full.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if a build can be canceled when the log is full; otherwise, <see langword="false" />. The default is <see langword="false" />.
        /// </value>
        public bool? ShouldAbortCopyForTruncation { get; set; }
        
        /// <summary>
        /// Gets or sets the number of operations in a replication batch.
        /// </summary>
        /// <value>
        /// The default is 1.
        /// </value>
        public long? ReplicationBatchSize { get; set; }

        /// <summary>
        /// Gets or sets the interval at which a replication batch is force sent even if it hasn't reached <see cref="ReplicationBatchSize" />.
        /// </summary>
        /// <value>
        /// The default is 0.015 seconds (15 milliseconds).
        /// </value>
        public TimeSpan? ReplicationBatchSendInterval { get; set; }

        /// <summary>
        /// Determines whether the specified delta settings equal the current settings, comparing only the properties that are set on <paramref name="obj"/>, and returns <see langword="true" /> if every set property matches; otherwise, <see langword="false" />.
        /// </summary>
        /// <param name="obj">The delta settings to compare with the current settings.</param>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ReliableStateManagerReplicatorSettings2 updated = obj as ReliableStateManagerReplicatorSettings2;
            if (updated == null)
            {
                // this means that delta does not change any of ReliableStateManagerReplicatorSettings2.
                if (obj is ReliableStateManagerReplicatorSettings)
                {
                    return BaseInternalEquals(obj as ReliableStateManagerReplicatorSettings);
                }

                // some wrong object type passed for Equality check.
                return false;
            }
            else
            {
                return InternalEquals(updated);
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            if (this.CopyBatchSizeInKB.HasValue)
            {
                builder.AppendFormat("CopyBatchSizeInKB = {0}" + Environment.NewLine, this.CopyBatchSizeInKB);
            }

            if (this.EnableStableReads.HasValue)
            {
                builder.AppendFormat("EnableStableReads = {0}" + Environment.NewLine, this.EnableStableReads);
            }

            if (this.ShouldAbortCopyForTruncation.HasValue)
            {
                builder.AppendFormat("ShouldAbortCopyForTruncation = {0}" + Environment.NewLine, this.ShouldAbortCopyForTruncation);
            }
            
            if (this.ReplicationBatchSize.HasValue)
            {
                builder.AppendFormat("ReplicationBatchSize = {0}" + Environment.NewLine, this.ReplicationBatchSize);
            }

            if (this.ReplicationBatchSendInterval.HasValue)
            {
                builder.AppendFormat("ReplicationBatchSendInterval = {0}" + Environment.NewLine, this.ReplicationBatchSendInterval);
            }

            return builder.ToString();
        }

        private bool InternalEquals(ReliableStateManagerReplicatorSettings2 updated)
        {
            bool isEqual = true;
            if (updated.CopyBatchSizeInKB.HasValue)
            {
                isEqual = this.CopyBatchSizeInKB == updated.CopyBatchSizeInKB;
            }

            if (updated.EnableStableReads.HasValue)
            {
                isEqual = this.EnableStableReads == updated.EnableStableReads;
            }

            if (updated.ShouldAbortCopyForTruncation.HasValue)
            {
                isEqual = this.ShouldAbortCopyForTruncation == updated.ShouldAbortCopyForTruncation;
            }
            
            if (updated.ReplicationBatchSize.HasValue)
            {
                isEqual = this.ReplicationBatchSize == updated.ReplicationBatchSize;
            }

            if (updated.ReplicationBatchSendInterval.HasValue)
            {
                isEqual = this.ReplicationBatchSendInterval == updated.ReplicationBatchSendInterval;
            }

            return isEqual && BaseInternalEquals(updated as ReliableStateManagerReplicatorSettings);
        }

        // Copied from ReliableStateManagerReplicatorSettings.InternalEquals because:
        // * InternalEquals is private, so it can't be called directly.
        // * Calling it via reflection on .NET Core (production coreclr apps) would be ugly.
        // * ReliableStateManagerReplicatorSettings is frozen, so this copy won't go out of sync.
        // * base.Equals can't be used either: its GetType() runtime check fails when a ReliableStateManagerReplicatorSettings2 is passed.
        private bool BaseInternalEquals(ReliableStateManagerReplicatorSettings updated)
        {
            bool areEqual = true;

            if (!string.IsNullOrEmpty(updated.SharedLogId))
            {
                areEqual = !string.IsNullOrEmpty(this.SharedLogId) && (this.SharedLogId == updated.SharedLogId);
            }

            if (areEqual && !string.IsNullOrEmpty(updated.SharedLogPath))
            {
                areEqual = !string.IsNullOrEmpty(this.SharedLogPath) && (this.SharedLogPath == updated.SharedLogPath);
            }

            if (areEqual && updated.MaxStreamSizeInMB.HasValue)
            {
                areEqual = this.MaxStreamSizeInMB.HasValue && (this.MaxStreamSizeInMB.Value == updated.MaxStreamSizeInMB.Value);
            }

            if (areEqual && updated.MaxRecordSizeInKB.HasValue)
            {
                areEqual = this.MaxRecordSizeInKB.HasValue && (this.MaxRecordSizeInKB.Value == updated.MaxRecordSizeInKB.Value);
            }

            if (areEqual && updated.MaxMetadataSizeInKB.HasValue)
            {
                areEqual = this.MaxMetadataSizeInKB.HasValue && (this.MaxMetadataSizeInKB.Value == updated.MaxMetadataSizeInKB.Value);
            }

            if (areEqual && updated.OptimizeForLocalSSD.HasValue)
            {
                areEqual = this.OptimizeForLocalSSD.HasValue && (this.OptimizeForLocalSSD.Value == updated.OptimizeForLocalSSD.Value);
            }

            if (areEqual && updated.OptimizeLogForLowerDiskUsage.HasValue)
            {
                areEqual = this.OptimizeLogForLowerDiskUsage.HasValue && (this.OptimizeLogForLowerDiskUsage.Value == updated.OptimizeLogForLowerDiskUsage.Value);
            }

            if (areEqual && updated.CheckpointThresholdInMB.HasValue)
            {
                areEqual = this.CheckpointThresholdInMB.HasValue && (this.CheckpointThresholdInMB.Value == updated.CheckpointThresholdInMB.Value);
            }

            if (areEqual && updated.MaxAccumulatedBackupLogSizeInMB.HasValue)
            {
                areEqual = this.MaxAccumulatedBackupLogSizeInMB.HasValue && (this.MaxAccumulatedBackupLogSizeInMB.Value == updated.MaxAccumulatedBackupLogSizeInMB.Value);
            }

            if (areEqual && updated.MinLogSizeInMB.HasValue)
            {
                areEqual = this.MinLogSizeInMB.HasValue && (this.MinLogSizeInMB.Value == updated.MinLogSizeInMB.Value);
            }

            if (areEqual && updated.TruncationThresholdFactor.HasValue)
            {
                areEqual = this.TruncationThresholdFactor.HasValue && (this.TruncationThresholdFactor.Value == updated.TruncationThresholdFactor.Value);
            }

            if (areEqual && updated.ThrottlingThresholdFactor.HasValue)
            {
                areEqual = this.ThrottlingThresholdFactor.HasValue && (this.ThrottlingThresholdFactor.Value == updated.ThrottlingThresholdFactor.Value);
            }

            if (areEqual && updated.SlowApiMonitoringDuration.HasValue)
            {
                areEqual = this.SlowApiMonitoringDuration.HasValue && (this.SlowApiMonitoringDuration == updated.SlowApiMonitoringDuration);
            }

#if NETFRAMEWORK
            // 12529905 - Disable new configuration for LogTruncationIntervalSeconds in CoreCLR
            if (areEqual && updated.LogTruncationIntervalSeconds.HasValue)
            {
                areEqual = this.LogTruncationIntervalSeconds.HasValue && (this.LogTruncationIntervalSeconds == updated.LogTruncationIntervalSeconds);
            }

            if (areEqual && updated.EnableIncrementalBackupsAcrossReplicas.HasValue)
            {
                areEqual = this.EnableIncrementalBackupsAcrossReplicas.HasValue && (this.EnableIncrementalBackupsAcrossReplicas == updated.EnableIncrementalBackupsAcrossReplicas);
            }

            if (areEqual && updated.EnableSendWindowSizeInBytes.HasValue)
            {
                areEqual = this.EnableSendWindowSizeInBytes.HasValue && (this.EnableSendWindowSizeInBytes == updated.EnableSendWindowSizeInBytes);
            }

            if (areEqual && updated.MaxReplicationQueueSendWindowSizeInBytes.HasValue)
            {
                areEqual = this.MaxReplicationQueueSendWindowSizeInBytes.HasValue && (this.MaxReplicationQueueSendWindowSizeInBytes == updated.MaxReplicationQueueSendWindowSizeInBytes);
            }

            if (areEqual && updated.MaxCopyQueueSendWindowSizeInBytes.HasValue)
            {
                areEqual = this.MaxCopyQueueSendWindowSizeInBytes.HasValue && (this.MaxCopyQueueSendWindowSizeInBytes == updated.MaxCopyQueueSendWindowSizeInBytes);
            }

            if (areEqual && updated.UseIndividualHeapPerReplica.HasValue)
            {
                areEqual = this.UseIndividualHeapPerReplica.HasValue && (this.UseIndividualHeapPerReplica == updated.UseIndividualHeapPerReplica);
            }
#endif
            return areEqual;
        }
    }
}
