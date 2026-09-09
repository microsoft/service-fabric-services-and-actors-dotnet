// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Fabric;
    using System.Text;

    /// <summary>
    /// Configures the replicator used by the <see cref="IReliableStateManager"/>.
    /// </summary>
    public class ReliableStateManagerReplicatorSettings
    {
        /// <summary>
        /// Gets or sets how long the replicator waits after it transmits a message from the primary to the secondary for
        /// the secondary to acknowledge that it has received the message and, if no acknowledgement is received,
        /// retransmits the message.
        /// </summary>
        /// <value>
        /// The default is 5 seconds.
        /// </value>
        public TimeSpan? RetryInterval { get; set; }

        /// <summary>
        /// Gets or sets the amount of time that the replicator waits after receiving an operation before sending back
        /// an acknowledgment.
        /// </summary>
        /// <value>
        /// The default is 15 milliseconds.
        /// </value>
        public TimeSpan? BatchAcknowledgementInterval { get; set; }

        /// <summary>
        /// Gets or sets the address in <c>{ip}:{port}</c> format that this replicator uses when communicating with other
        /// replicators.
        /// </summary>
        /// <value>
        /// The default is <c>"localhost:0"</c>, which picks a dynamic port number at runtime.
        /// </value>
        /// <remarks>
        /// If the replicator runs inside a container, set <see cref="ReplicatorListenAddress"/> and <see cref="ReplicatorPublishAddress"/>
        /// instead.
        /// </remarks>
        public string ReplicatorAddress { get; set; }

        /// <summary>
        /// Gets or sets the address in <c>{ip}:{port}</c> format that this replicator uses to receive information from other
        /// replicators.
        /// </summary>
        /// <value>
        /// The default is the empty string, which causes the replicator to use <see cref="ReplicatorAddress"/>
        /// for receiving information from other replicators.
        /// </value>
        /// <remarks>
        /// The <c>{ip}</c> part of the listen address can be obtained from <see cref="CodePackageActivationContext.ServiceListenAddress"/>.
        /// </remarks>
        public string ReplicatorListenAddress { get; set; }

        /// <summary>
        /// Gets or sets the address in <c>{ip}:{port}</c> format that this replicator uses to send information to other
        /// replicators.
        /// </summary>
        /// <value>
        /// The default is the empty string, which causes the replicator to use <see cref="ReplicatorAddress"/>
        /// for sending information to other replicators.
        /// </value>
        /// <remarks>
        /// The <c>{ip}</c> part of the publish address can be obtained from <see cref="CodePackageActivationContext.ServicePublishAddress"/>.
        /// </remarks>
        public string ReplicatorPublishAddress { get; set; }

        /// <summary>
        /// Gets or sets the security credentials for securing the traffic between replicators.
        /// </summary>
        /// <value>
        /// The default is <see langword="null"/>.
        /// </value>
        public SecurityCredentials SecurityCredentials { get; set; }

        /// <summary>
        /// Gets or sets the initial size of the copy operation queue inside the replicator.
        /// </summary>
        /// <value>
        /// The default is 64.
        /// </value>
        /// <remarks>
        /// The value is the number of operations and must be a power of 2.
        /// </remarks>
        public long? InitialCopyQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the copy operation queue inside the replicator.
        /// </summary>
        /// <value>
        /// The default is 16384.
        /// </value>
        /// <remarks>
        /// The value is the maximum number of operations and must be a power of 2.
        /// </remarks>
        public long? MaxCopyQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum replication message size.
        /// </summary>
        /// <value>
        /// The default is 52428800 (50 MB).
        /// </value>
        /// <remarks>
        /// The value is specified in bytes.
        /// </remarks>
        public long? MaxReplicationMessageSize { get; set; }

        /// <summary>
        /// Gets or sets the initial size of the primary replication queue.
        /// </summary>
        /// <value>
        /// The default is 64.
        /// </value>
        /// <remarks>
        /// The value is the number of operations and must be a power of 2.
        /// </remarks>
        public long? InitialPrimaryReplicationQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the primary replication queue.
        /// </summary>
        /// <value>
        /// The default is 8192.
        /// </value>
        /// <remarks>
        /// The value is the maximum number of operations and must be a power of 2 and greater than 64.
        /// </remarks>
        public long? MaxPrimaryReplicationQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum memory size of the primary replication queue.
        /// </summary>
        /// <value>
        /// The default is 0, which means there is no memory limit.
        /// </value>
        /// <remarks>
        /// The value is specified in bytes.
        /// </remarks>
        public long? MaxPrimaryReplicationQueueMemorySize { get; set; }

        /// <summary>
        /// Gets or sets the initial size of the secondary replication queue.
        /// </summary>
        /// <value>
        /// The default is 64.
        /// </value>
        /// <remarks>
        /// The value is the number of operations and must be a power of 2.
        /// </remarks>
        public long? InitialSecondaryReplicationQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the secondary replication queue.
        /// </summary>
        /// <value>
        /// The default is 16384.
        /// </value>
        /// <remarks>
        /// The value is the maximum number of operations and must be a power of 2 and greater than 64.
        /// </remarks>
        public long? MaxSecondaryReplicationQueueSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum memory size of the secondary replication queue.
        /// </summary>
        /// <value>
        /// The default is 0, which means there is no memory limit.
        /// </value>
        /// <remarks>
        /// The value is specified in bytes.
        /// </remarks>
        public long? MaxSecondaryReplicationQueueMemorySize { get; set; }

        /// <summary>
        /// Gets or sets the GUID identifier for the log container shared by a number of replicas on the node including this
        /// one.
        /// </summary>
        /// <value>
        /// The default is an empty string, which causes the replicator to use the global shared log for the node.
        /// </value>
        /// <remarks>
        /// <see cref="SharedLogId"/> and <see cref="SharedLogPath"/> must either both be specified or both be omitted.
        /// When specified, the value must be a valid GUID.
        /// </remarks>
        public string SharedLogId { get; set; }

        /// <summary>
        /// Gets or sets the full pathname to the log container shared by a number of replicas on the node including this
        /// one.
        /// </summary>
        /// <value>
        /// The default is an empty string, which causes the replicator to use the global shared log for the node.
        /// </value>
        /// <remarks>
        /// <see cref="SharedLogPath"/> and <see cref="SharedLogId"/> must either both be specified or both be omitted.
        /// When specified, the value must be an absolute path.
        /// </remarks>
        public string SharedLogPath { get; set; }

        /// <summary>
        /// Gets or sets the maximum stream size.
        /// </summary>
        /// <value>
        /// The default is 1024, which applies only when <see cref="OptimizeLogForLowerDiskUsage"/> is explicitly set to
        /// <see langword="false"/>. While <see cref="OptimizeLogForLowerDiskUsage"/> stays at its default of
        /// <see langword="true"/>, the replicator uses a sparse log and the effective maximum stream size is 204800 (200 GB).
        /// </value>
        /// <remarks>
        /// This property is deprecated.
        /// </remarks>
        public int? MaxStreamSizeInMB { get; set; }

        /// <summary>
        /// Gets or sets the amount of persistent storage space reserved for replication-log metadata on this replica.
        /// </summary>
        /// <value>
        /// The default is 4.
        /// </value>
        /// <remarks>
        /// The value is specified in KB and must be a non-negative multiple of 4.
        /// </remarks>
        public int? MaxMetadataSizeInKB { get; set; }

        /// <summary>
        /// Gets or sets the largest record size that the replicator may write for the log associated with this replica.
        /// </summary>
        /// <value>
        /// The default is 1024.
        /// </value>
        /// <remarks>
        /// The value is specified in KB and must be a multiple of 4 and at least 128.
        /// When <see cref="OptimizeLogForLowerDiskUsage"/> is explicitly <see langword="false"/> and
        /// <see cref="MaxStreamSizeInMB"/> is set, the maximum stream size must be at least 16 times this value
        /// (<c>MaxStreamSizeInMB * 1024 &gt;= 16 * MaxRecordSizeInKB</c>).
        /// </remarks>
        public int? MaxRecordSizeInKB { get; set; }

        /// <summary>
        /// Gets or sets the maximum write queue depth that the core logger can use for the log associated with this replica.
        /// </summary>
        /// <value>
        /// The default is 0.
        /// </value>
        /// <remarks>
        /// The value is the maximum amount of data that can be outstanding during core logger updates.
        /// It may be 0, in which case the core logger computes an appropriate value; otherwise it must be a positive multiple of
        /// 4.
        /// The value is specified in KB.
        /// </remarks>
        public int? MaxWriteQueueDepthInKB { get; set; }

        /// <summary>
        /// Gets or sets the log usage threshold above which a checkpoint is initiated.
        /// </summary>
        /// <value>
        /// The default is 50.
        /// </value>
        /// <remarks>
        /// The value is specified in MB and must be at least 1.
        /// When <see cref="OptimizeLogForLowerDiskUsage"/> is explicitly <see langword="false"/> and
        /// <see cref="MaxStreamSizeInMB"/> is set, this value must not exceed it.
        /// </remarks>
        public int? CheckpointThresholdInMB { get; set; }

        /// <summary>
        /// Gets or sets the maximum size for an accumulated backup log across backups.
        /// </summary>
        /// <value>
        /// The default is 800.
        /// </value>
        /// <remarks>
        /// An incremental backup request fails when the backup logs it generates would cause the total amount of logs accumulated
        /// since the last full backup to exceed this value. In that case, take a full backup.
        /// The value is specified in MB and must be at least 1.
        /// When <see cref="MaxStreamSizeInMB"/> is set, this value must also be smaller than it.
        /// </remarks>
        public int? MaxAccumulatedBackupLogSizeInMB { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the replicator is optimized for local SSD storage.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the replicator is optimized for local SSD storage; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This property is deprecated.
        /// </remarks>
        public bool? OptimizeForLocalSSD { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the log should be optimized to use less disk space at the cost of
        /// IO performance.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the log uses less disk space at the cost of IO performance; otherwise, <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// When set to <see langword="false"/>, the log uses more disk space but has better IO performance.
        /// </remarks>
        public bool? OptimizeLogForLowerDiskUsage { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the secondary replicator should clear the in-memory queue after acknowledging
        /// operations to the primary.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the secondary replicator clears the in-memory queue after acknowledging operations
        /// (after they are flushed to disk); otherwise, <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// Setting this to <see langword="true"/> can result in additional disk reads on the new primary when catching up
        /// replicas after a failover.
        /// </remarks>
        public bool? SecondaryClearAcknowledgedOperations { get; set; }

        /// <summary>
        /// Gets or sets the interval after which the replicator sends a warning health report indicating that the API is
        /// slow and taking longer than expected.
        /// </summary>
        /// <value>
        /// The default is 5 minutes.
        /// </value>
        /// <remarks>
        /// The value must not be negative or <see cref="TimeSpan.MaxValue"/>.
        /// Set this value to <see cref="TimeSpan.Zero"/> to disable slow API monitoring.
        /// </remarks>
        public TimeSpan? SlowApiMonitoringDuration { get; set; }

        /// <summary>
        /// Gets or sets the minimum log size.
        /// </summary>
        /// <value>
        /// The default is 0, which directs the replicator to derive the minimum log size from
        /// <see cref="CheckpointThresholdInMB"/>, using half of it but no less than 1.
        /// </value>
        /// <remarks>
        /// A truncation is not initiated if it would reduce the size of the log below the resulting value.
        /// Any explicitly specified nonzero value must be at least 1. It must also be smaller than the effective maximum
        /// stream size: the value of <see cref="MaxStreamSizeInMB"/> when <see cref="OptimizeLogForLowerDiskUsage"/> is
        /// explicitly <see langword="false"/>; otherwise 204800 (200 GB), the sparse-log value.
        /// </remarks>
        public int? MinLogSizeInMB { get; set; }

        /// <summary>
        /// Gets or sets the multiplier applied to <see cref="MinLogSizeInMB"/> to determine the log usage threshold
        /// above which truncation is initiated.
        /// </summary>
        /// <value>
        /// The default is 2.
        /// </value>
        /// <remarks>
        /// Must be greater than 1. In addition, the product of <see cref="MinLogSizeInMB"/> and this factor must be
        /// smaller than the effective maximum stream size: the value of <see cref="MaxStreamSizeInMB"/> when
        /// <see cref="OptimizeLogForLowerDiskUsage"/> is explicitly <see langword="false"/>; otherwise 204800 (200 GB),
        /// the sparse-log value.
        /// </remarks>
        public int? TruncationThresholdFactor { get; set; }

        /// <summary>
        /// Gets or sets the multiplier applied to <see cref="MinLogSizeInMB"/> and <see cref="CheckpointThresholdInMB"/>
        /// to determine the log usage threshold above which throttling is initiated; throttling starts at the larger
        /// of the two products.
        /// </summary>
        /// <value>
        /// The default is 4.
        /// </value>
        /// <remarks>
        /// Must be greater than <see cref="TruncationThresholdFactor"/> and at least 3.
        /// In addition, the throttling threshold, which is the larger of <see cref="MinLogSizeInMB"/> and
        /// <see cref="CheckpointThresholdInMB"/> multiplied by this factor, must be smaller than the effective maximum stream size:
        /// the value of <see cref="MaxStreamSizeInMB"/> when <see cref="OptimizeLogForLowerDiskUsage"/> is explicitly
        /// <see langword="false"/>; otherwise 204800 (200 GB), the sparse-log value.
        /// </remarks>
        public int? ThrottlingThresholdFactor { get; set; }

#if NETFRAMEWORK
        // 12529905 - Disable new configuration for LogTruncationIntervalSeconds in CoreCLR
        /// <summary>
        /// Gets or sets the time interval at which log truncation is initiated.
        /// </summary>
        /// <value>
        /// The default is 0.
        /// </value>
        /// <remarks>
        /// The value must not be negative.
        /// </remarks>
        public int? LogTruncationIntervalSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether incremental backups can be chained across primary replicas.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a primary replica can take an incremental backup whether or not it took the last backup
        /// with the same data-loss number; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// When <see langword="false"/>, a primary replica can take an incremental backup only if it took the last backup at
        /// the same epoch.
        /// </remarks>
        internal bool? EnableIncrementalBackupsAcrossReplicas { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the send window size for primary queues is measured in bytes rather
        /// than number of messages.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the send window size is measured in bytes; otherwise, <see langword="false"/>.
        /// The default is <see langword="false"/>.
        /// </value>
        internal bool? EnableSendWindowSizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes from the replication queue that can be put on the wire when
        /// <see cref="EnableSendWindowSizeInBytes"/> is set.
        /// </summary>
        /// <value>
        /// The default is 0.
        /// </value>
        internal uint? MaxReplicationQueueSendWindowSizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes from the copy queue that can be put on the wire when
        /// <see cref="EnableSendWindowSizeInBytes"/> is set.
        /// </summary>
        /// <value>
        /// The default is 0.
        /// </value>
        internal uint? MaxCopyQueueSendWindowSizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether multiple replicas within a process use their own individual heaps
        /// rather than a shared heap.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if each replica uses its own individual heap; otherwise, <see langword="false"/>.
        /// The default is <see langword="true"/>.
        /// </value>
        internal bool? UseIndividualHeapPerReplica { get; set; }

        /// <summary>
        /// Gets or sets the initial size, in kilobytes, of the heap owned by a replica in a process when
        /// <see cref="UseIndividualHeapPerReplica"/> is enabled.
        /// </summary>
        /// <value>
        /// The default is 0.
        /// </value>
        internal uint? InitialReplicaHeapSizeInKB { get; set; }
#endif

        /// <summary>
        /// Returns a value that indicates whether the specified object is of exactly the same type and each V2 setting set on
        /// that object matches the corresponding setting on the current instance.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var arg = (ReliableStateManagerReplicatorSettings)obj;
            return InternalEquals(this, arg);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a multi-line listing of the V2 settings, and .NET Framework-only settings when applicable, that have
        /// been set on this instance.
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(Environment.NewLine);
            if (this.SharedLogId != null)
            {
                builder.AppendFormat("SharedLogId = {0}" + Environment.NewLine, this.SharedLogId);
            }

            if (this.SharedLogPath != null)
            {
                builder.AppendFormat("SharedLogPath = {0}" + Environment.NewLine, this.SharedLogPath);
            }

            if (this.MaxStreamSizeInMB.HasValue)
            {
                builder.AppendFormat("MaxStreamSizeInMB = {0}" + Environment.NewLine, this.MaxStreamSizeInMB);
            }

            if (this.MaxRecordSizeInKB.HasValue)
            {
                builder.AppendFormat("MaxRecordSizeInKB = {0}" + Environment.NewLine, this.MaxRecordSizeInKB);
            }

            if (this.MaxMetadataSizeInKB.HasValue)
            {
                builder.AppendFormat("MaxMetadataSizeInKB = {0}" + Environment.NewLine, this.MaxMetadataSizeInKB);
            }

            if (this.OptimizeForLocalSSD.HasValue)
            {
                builder.AppendFormat("OptimizeForLocalSSD = {0}" + Environment.NewLine, this.OptimizeForLocalSSD);
            }

            if (this.OptimizeLogForLowerDiskUsage.HasValue)
            {
                builder.AppendFormat("OptimizeLogForLowerDiskUsage = {0}" + Environment.NewLine, this.OptimizeLogForLowerDiskUsage);
            }

            if (this.CheckpointThresholdInMB.HasValue)
            {
                builder.AppendFormat("CheckpointThresholdInMB = {0}" + Environment.NewLine, this.CheckpointThresholdInMB);
            }

            if (this.MaxAccumulatedBackupLogSizeInMB.HasValue)
            {
                builder.AppendFormat("MaxAccumulatedBackupLogSizeInMB = {0}" + Environment.NewLine, this.MaxAccumulatedBackupLogSizeInMB);
            }

            if (this.MinLogSizeInMB.HasValue)
            {
                builder.AppendFormat("MinLogSizeInMB = {0}" + Environment.NewLine, this.MinLogSizeInMB);
            }

            if (this.TruncationThresholdFactor.HasValue)
            {
                builder.AppendFormat("TruncationThresholdFactor = {0}" + Environment.NewLine, this.TruncationThresholdFactor);
            }

            if (this.ThrottlingThresholdFactor.HasValue)
            {
                builder.AppendFormat("ThrottlingThresholdFactor = {0}" + Environment.NewLine, this.ThrottlingThresholdFactor);
            }

            if (this.SlowApiMonitoringDuration.HasValue)
            {
                builder.AppendFormat("SlowApiMonitoringDuration = {0}" + Environment.NewLine, this.SlowApiMonitoringDuration);
            }

#if NETFRAMEWORK
            // 12529905 - Disable new configuration for LogTruncationIntervalSeconds in CoreCLR
            if (this.LogTruncationIntervalSeconds.HasValue)
            {
                builder.AppendFormat("LogTruncationIntervalSeconds = {0}" + Environment.NewLine, this.LogTruncationIntervalSeconds);
            }

            if (this.EnableIncrementalBackupsAcrossReplicas.HasValue)
            {
                builder.AppendFormat("EnableIncrementalBackupsAcrossReplicas = {0}" + Environment.NewLine, this.EnableIncrementalBackupsAcrossReplicas);
            }

            if (this.EnableSendWindowSizeInBytes.HasValue)
            {
                builder.AppendFormat("EnableSendWindowSizeInBytes = {0}" + Environment.NewLine, this.EnableSendWindowSizeInBytes);
            }

            if (this.MaxReplicationQueueSendWindowSizeInBytes.HasValue)
            {
                builder.AppendFormat("MaxReplicationQueueSendWindowSizeInBytes = {0}" + Environment.NewLine, this.MaxReplicationQueueSendWindowSizeInBytes);
            }

            if (this.MaxCopyQueueSendWindowSizeInBytes.HasValue)
            {
                builder.AppendFormat("MaxCopyQueueSendWindowSizeInBytes = {0}" + Environment.NewLine, this.MaxCopyQueueSendWindowSizeInBytes);
            }

            if (this.UseIndividualHeapPerReplica.HasValue)
            {
                builder.AppendFormat("UseIndividualHeapPerReplica = {0}" + Environment.NewLine, this.UseIndividualHeapPerReplica);
            }

            if (this.InitialReplicaHeapSizeInKB.HasValue)
            {
                builder.AppendFormat("InitialReplicaHeapSizeInKB = {0}" + Environment.NewLine, this.InitialReplicaHeapSizeInKB);
            }
#endif
            return builder.ToString();
        }

        /// <summary>
        /// Checks for equality of setting values.
        /// </summary>
        /// <param name="old">Old settings.</param>
        /// <param name="updated">Updated settings.</param>
        /// <returns>
        /// TRUE if the settings are equivalent.
        /// </returns>
        private static bool InternalEquals(ReliableStateManagerReplicatorSettings old, ReliableStateManagerReplicatorSettings updated)
        {
            // compare only the V2 settings.
            var areEqual = true;

            if (!string.IsNullOrEmpty(updated.SharedLogId))
            {
                areEqual = !string.IsNullOrEmpty(old.SharedLogId) && (old.SharedLogId == updated.SharedLogId);
            }

            if (areEqual && !string.IsNullOrEmpty(updated.SharedLogPath))
            {
                areEqual = !string.IsNullOrEmpty(old.SharedLogPath) && (old.SharedLogPath == updated.SharedLogPath);
            }

            if (areEqual && updated.MaxStreamSizeInMB.HasValue)
            {
                areEqual = old.MaxStreamSizeInMB.HasValue && (old.MaxStreamSizeInMB.Value == updated.MaxStreamSizeInMB.Value);
            }

            if (areEqual && updated.MaxRecordSizeInKB.HasValue)
            {
                areEqual = old.MaxRecordSizeInKB.HasValue && (old.MaxRecordSizeInKB.Value == updated.MaxRecordSizeInKB.Value);
            }

            if (areEqual && updated.MaxMetadataSizeInKB.HasValue)
            {
                areEqual = old.MaxMetadataSizeInKB.HasValue && (old.MaxMetadataSizeInKB.Value == updated.MaxMetadataSizeInKB.Value);
            }

            if (areEqual && updated.OptimizeForLocalSSD.HasValue)
            {
                areEqual = old.OptimizeForLocalSSD.HasValue && (old.OptimizeForLocalSSD.Value == updated.OptimizeForLocalSSD.Value);
            }

            if (areEqual && updated.OptimizeLogForLowerDiskUsage.HasValue)
            {
                areEqual = old.OptimizeLogForLowerDiskUsage.HasValue && (old.OptimizeLogForLowerDiskUsage.Value == updated.OptimizeLogForLowerDiskUsage.Value);
            }

            if (areEqual && updated.CheckpointThresholdInMB.HasValue)
            {
                areEqual = old.CheckpointThresholdInMB.HasValue && (old.CheckpointThresholdInMB.Value == updated.CheckpointThresholdInMB.Value);
            }

            if (areEqual && updated.MaxAccumulatedBackupLogSizeInMB.HasValue)
            {
                areEqual = old.MaxAccumulatedBackupLogSizeInMB.HasValue && (old.MaxAccumulatedBackupLogSizeInMB.Value == updated.MaxAccumulatedBackupLogSizeInMB.Value);
            }

            if (areEqual && updated.MinLogSizeInMB.HasValue)
            {
                areEqual = old.MinLogSizeInMB.HasValue && (old.MinLogSizeInMB.Value == updated.MinLogSizeInMB.Value);
            }

            if (areEqual && updated.TruncationThresholdFactor.HasValue)
            {
                areEqual = old.TruncationThresholdFactor.HasValue && (old.TruncationThresholdFactor.Value == updated.TruncationThresholdFactor.Value);
            }

            if (areEqual && updated.ThrottlingThresholdFactor.HasValue)
            {
                areEqual = old.ThrottlingThresholdFactor.HasValue && (old.ThrottlingThresholdFactor.Value == updated.ThrottlingThresholdFactor.Value);
            }

            if (areEqual && updated.SlowApiMonitoringDuration.HasValue)
            {
                areEqual = old.SlowApiMonitoringDuration.HasValue && (old.SlowApiMonitoringDuration == updated.SlowApiMonitoringDuration);
            }

#if NETFRAMEWORK
            // 12529905 - Disable new configuration for LogTruncationIntervalSeconds in CoreCLR
            if (areEqual && updated.LogTruncationIntervalSeconds.HasValue)
            {
                areEqual = old.LogTruncationIntervalSeconds.HasValue && (old.LogTruncationIntervalSeconds == updated.LogTruncationIntervalSeconds);
            }

            if (areEqual && updated.EnableIncrementalBackupsAcrossReplicas.HasValue)
            {
                areEqual = old.EnableIncrementalBackupsAcrossReplicas.HasValue && (old.EnableIncrementalBackupsAcrossReplicas == updated.EnableIncrementalBackupsAcrossReplicas);
            }

            if (areEqual && updated.EnableSendWindowSizeInBytes.HasValue)
            {
                areEqual = old.EnableSendWindowSizeInBytes.HasValue && (old.EnableSendWindowSizeInBytes == updated.EnableSendWindowSizeInBytes);
            }

            if (areEqual && updated.MaxReplicationQueueSendWindowSizeInBytes.HasValue)
            {
                areEqual = old.MaxReplicationQueueSendWindowSizeInBytes.HasValue && (old.MaxReplicationQueueSendWindowSizeInBytes == updated.MaxReplicationQueueSendWindowSizeInBytes);
            }

            if (areEqual && updated.MaxCopyQueueSendWindowSizeInBytes.HasValue)
            {
                areEqual = old.MaxCopyQueueSendWindowSizeInBytes.HasValue && (old.MaxCopyQueueSendWindowSizeInBytes == updated.MaxCopyQueueSendWindowSizeInBytes);
            }

            if (areEqual && updated.UseIndividualHeapPerReplica.HasValue)
            {
                areEqual = old.UseIndividualHeapPerReplica.HasValue && (old.UseIndividualHeapPerReplica == updated.UseIndividualHeapPerReplica);
            }

            if (areEqual && updated.InitialReplicaHeapSizeInKB.HasValue)
            {
                areEqual = old.InitialReplicaHeapSizeInKB.HasValue && (old.InitialReplicaHeapSizeInKB == updated.InitialReplicaHeapSizeInKB);
            }
#endif
            return areEqual;
        }
    }
}
