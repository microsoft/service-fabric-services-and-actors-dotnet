// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Text;

    /// <summary>
    /// Settings that configure the replicator
    /// </summary>
    public class ReliableStateManagerReplicatorSettings2 : ReliableStateManagerReplicatorSettings
    {
        /// <summary>
        /// Controls the size of copy log message that is used in building a replica. Higher value will copy more log records in each message.
        /// Default value is 0 which means copy one log record at a time.
        /// </summary>
        public long? CopyBatchSizeInKB { get; set; }

        /// <summary>
        /// Flag controls Stable reads feature. Stable reads allows every replica to only return values on read which are quorum acked.
        /// Default is false
        /// </summary>
        public bool? EnableStableReads { get; set; }

        /// <summary>
        /// Determines whether build can be canceled if the log is full.
        /// Default is 0
        /// </summary>
        public bool? ShouldAbortCopyForTruncation { get; set; }
        
        /// <summary>
        /// Size of a ReplicationBatch.
        /// Default is 1
        /// </summary>
        public long? ReplicationBatchSize { get; set; }

        /// <summary>
        /// Interval at which we force send Replication Batch even if it hasn't reach ReplicationBatchSize.
        /// Default value is 0.015 Seconds (15 milliseconds)
        /// </summary>
        public TimeSpan? ReplicationBatchSendInterval { get; set; }

        /// <summary>
        /// Equals is used for delta comparison of current this object with passed in delta obj.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/> representing the hash code.
        /// </returns>
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>

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

        /// <summary>
        /// Copied from ReliableStateManagerReplicatorSettings's InternalEquals.
        /// We can use base.InternalEquals but
        /// * base.InternalEquals is private in ReliableStateManagerReplicatorSettings
        /// * Since base.InternalEquals is private, we will have to use reflection to call in dotnet core (for production coreclr apps), which looks ugly.
        /// * ReliableStateManagerReplicatorSettings will never change as Data.Interfaces is frozen now. So, this code will not go out of sync.
        /// * We can't use base.Equals as that checks GetType() runtime checks which fails if we pass ReliableStateManagerReplicatorSettings2 object as argument.
        /// </summary>
        /// <param name="updated"></param>
        /// <returns></returns>
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