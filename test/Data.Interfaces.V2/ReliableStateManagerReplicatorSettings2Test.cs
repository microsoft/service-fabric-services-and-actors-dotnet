// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Runtime.CompilerServices;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class ReliableStateManagerReplicatorSettings2Test
{
    readonly ReliableStateManagerReplicatorSettings2 sut = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class CopyBatchSizeInKB : ReliableStateManagerReplicatorSettings2Test
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.CopyBatchSizeInKB = expected;
            Assert.Equal(expected, sut.CopyBatchSizeInKB);
        }
    }

    public sealed class EnableStableReads : ReliableStateManagerReplicatorSettings2Test
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.EnableStableReads = value;
            Assert.Equal(value, sut.EnableStableReads);
        }
    }

    public new sealed class Equals : ReliableStateManagerReplicatorSettings2Test
    {
        readonly ReliableStateManagerReplicatorSettings2 obj;

        public Equals()
        {
            Populate();
            obj = CloneOf(sut);
        }

        [Fact]
        public void ReturnsTrueWhenObjPropertiesMatch() =>
            Assert.True(sut.Equals(obj));

        [Fact]
        public void ReturnsFalseWhenObjIsNull() =>
            Assert.False(sut.Equals(null));

        [Fact]
        public void ReturnsFalseWhenObjIsUnrelatedType() =>
            Assert.False(sut.Equals(fuzzy.String()));

        [Fact]
        public void ReturnsTrueWhenObjIsBaseSettingsAndItsSetPropertiesMatch()
        {
            var baseOther = new ReliableStateManagerReplicatorSettings
            {
                SharedLogId = sut.SharedLogId,
                SharedLogPath = sut.SharedLogPath,
            };
            Assert.True(sut.Equals(baseOther));
        }

        [Fact]
        public void ReturnsFalseWhenObjIsBaseSettingsAndItsSetPropertyDiffers()
        {
            var baseOther = new ReliableStateManagerReplicatorSettings
            {
                SharedLogId = sut.SharedLogId + fuzzy.String(),
            };
            Assert.False(sut.Equals(baseOther));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenCopyBatchSizeInKBDiffers()
        {
            obj.CopyBatchSizeInKB = sut.CopyBatchSizeInKB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjCopyBatchSizeInKBIsNull()
        {
            obj.CopyBatchSizeInKB = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenCopyBatchSizeInKBIsNull()
        {
            sut.CopyBatchSizeInKB = null;
            Assert.False(sut.Equals(obj));
        }

        [Theory(Explicit = true), InlineData(true), InlineData(false)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenEnableStableReadsDiffers(bool value)
        {
            sut.EnableStableReads = value;
            obj.EnableStableReads = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjEnableStableReadsIsNull()
        {
            obj.EnableStableReads = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenEnableStableReadsIsNull()
        {
            sut.EnableStableReads = null;
            Assert.False(sut.Equals(obj));
        }

        [Theory(Explicit = true), InlineData(true), InlineData(false)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenShouldAbortCopyForTruncationDiffers(bool value)
        {
            sut.ShouldAbortCopyForTruncation = value;
            obj.ShouldAbortCopyForTruncation = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjShouldAbortCopyForTruncationIsNull()
        {
            obj.ShouldAbortCopyForTruncation = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenShouldAbortCopyForTruncationIsNull()
        {
            sut.ShouldAbortCopyForTruncation = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenReplicationBatchSizeDiffers()
        {
            obj.ReplicationBatchSize = sut.ReplicationBatchSize + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjReplicationBatchSizeIsNull()
        {
            obj.ReplicationBatchSize = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. InternalEquals ignores previously-detected property inequality.
        public void ReturnsFalseWhenReplicationBatchSizeIsNull()
        {
            sut.ReplicationBatchSize = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenReplicationBatchSendIntervalDiffers()
        {
            obj.ReplicationBatchSendInterval = sut.ReplicationBatchSendInterval + fuzzy.TimeSpan().Seconds();
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjReplicationBatchSendIntervalIsNull()
        {
            obj.ReplicationBatchSendInterval = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenReplicationBatchSendIntervalIsNull()
        {
            sut.ReplicationBatchSendInterval = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenSharedLogIdDiffers()
        {
            obj.SharedLogId = sut.SharedLogId + fuzzy.String();
            Assert.False(sut.Equals(obj));
        }

        [Theory, InlineData(null), InlineData("")]
        public void ReturnsFalseWhenSharedLogIdIsNullOrEmpty(string value)
        {
            sut.SharedLogId = value;
            Assert.False(sut.Equals(obj));
        }

        [Theory, InlineData(null), InlineData("")]
        public void ReturnsTrueWhenObjSharedLogIdIsNullOrEmpty(string value)
        {
            obj.SharedLogId = value;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenSharedLogPathDiffers()
        {
            obj.SharedLogPath = sut.SharedLogPath + fuzzy.String();
            Assert.False(sut.Equals(obj));
        }

        [Theory, InlineData(null), InlineData("")]
        public void ReturnsFalseWhenSharedLogPathIsNullOrEmpty(string value)
        {
            sut.SharedLogPath = value;
            Assert.False(sut.Equals(obj));
        }

        [Theory, InlineData(null), InlineData("")]
        public void ReturnsTrueWhenObjSharedLogPathIsNullOrEmpty(string value)
        {
            obj.SharedLogPath = value;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxStreamSizeInMBDiffers()
        {
            obj.MaxStreamSizeInMB = sut.MaxStreamSizeInMB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxStreamSizeInMBIsNull()
        {
            sut.MaxStreamSizeInMB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMaxStreamSizeInMBIsNull()
        {
            obj.MaxStreamSizeInMB = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxRecordSizeInKBDiffers()
        {
            obj.MaxRecordSizeInKB = sut.MaxRecordSizeInKB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxRecordSizeInKBIsNull()
        {
            sut.MaxRecordSizeInKB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMaxRecordSizeInKBIsNull()
        {
            obj.MaxRecordSizeInKB = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxMetadataSizeInKBDiffers()
        {
            obj.MaxMetadataSizeInKB = sut.MaxMetadataSizeInKB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxMetadataSizeInKBIsNull()
        {
            sut.MaxMetadataSizeInKB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMaxMetadataSizeInKBIsNull()
        {
            obj.MaxMetadataSizeInKB = null;
            Assert.True(sut.Equals(obj));
        }

        [Theory, InlineData(true), InlineData(false)]
        public void ReturnsFalseWhenOptimizeForLocalSSDDiffers(bool value)
        {
            sut.OptimizeForLocalSSD = value;
            obj.OptimizeForLocalSSD = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenOptimizeForLocalSSDIsNull()
        {
            sut.OptimizeForLocalSSD = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjOptimizeForLocalSSDIsNull()
        {
            obj.OptimizeForLocalSSD = null;
            Assert.True(sut.Equals(obj));
        }

        [Theory, InlineData(true), InlineData(false)]
        public void ReturnsFalseWhenOptimizeLogForLowerDiskUsageDiffers(bool value)
        {
            sut.OptimizeLogForLowerDiskUsage = value;
            obj.OptimizeLogForLowerDiskUsage = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenOptimizeLogForLowerDiskUsageIsNull()
        {
            sut.OptimizeLogForLowerDiskUsage = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjOptimizeLogForLowerDiskUsageIsNull()
        {
            obj.OptimizeLogForLowerDiskUsage = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenCheckpointThresholdInMBDiffers()
        {
            obj.CheckpointThresholdInMB = sut.CheckpointThresholdInMB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenCheckpointThresholdInMBIsNull()
        {
            sut.CheckpointThresholdInMB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjCheckpointThresholdInMBIsNull()
        {
            obj.CheckpointThresholdInMB = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxAccumulatedBackupLogSizeInMBDiffers()
        {
            obj.MaxAccumulatedBackupLogSizeInMB = sut.MaxAccumulatedBackupLogSizeInMB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxAccumulatedBackupLogSizeInMBIsNull()
        {
            sut.MaxAccumulatedBackupLogSizeInMB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMaxAccumulatedBackupLogSizeInMBIsNull()
        {
            obj.MaxAccumulatedBackupLogSizeInMB = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMinLogSizeInMBDiffers()
        {
            obj.MinLogSizeInMB = sut.MinLogSizeInMB + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMinLogSizeInMBIsNull()
        {
            sut.MinLogSizeInMB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMinLogSizeInMBIsNull()
        {
            obj.MinLogSizeInMB = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenTruncationThresholdFactorDiffers()
        {
            obj.TruncationThresholdFactor = sut.TruncationThresholdFactor + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenTruncationThresholdFactorIsNull()
        {
            sut.TruncationThresholdFactor = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjTruncationThresholdFactorIsNull()
        {
            obj.TruncationThresholdFactor = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenThrottlingThresholdFactorDiffers()
        {
            obj.ThrottlingThresholdFactor = sut.ThrottlingThresholdFactor + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenThrottlingThresholdFactorIsNull()
        {
            sut.ThrottlingThresholdFactor = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjThrottlingThresholdFactorIsNull()
        {
            obj.ThrottlingThresholdFactor = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenSlowApiMonitoringDurationDiffers()
        {
            obj.SlowApiMonitoringDuration = sut.SlowApiMonitoringDuration + fuzzy.TimeSpan().Seconds();
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenSlowApiMonitoringDurationIsNull()
        {
            sut.SlowApiMonitoringDuration = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjSlowApiMonitoringDurationIsNull()
        {
            obj.SlowApiMonitoringDuration = null;
            Assert.True(sut.Equals(obj));
        }

#if NETFRAMEWORK
        [Fact]
        public void ReturnsFalseWhenLogTruncationIntervalSecondsDiffers()
        {
            obj.LogTruncationIntervalSeconds = sut.LogTruncationIntervalSeconds + fuzzy.SByte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenLogTruncationIntervalSecondsIsNull()
        {
            sut.LogTruncationIntervalSeconds = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjLogTruncationIntervalSecondsIsNull()
        {
            obj.LogTruncationIntervalSeconds = null;
            Assert.True(sut.Equals(obj));
        }

        [Theory, InlineData(true), InlineData(false)]
        public void ReturnsFalseWhenEnableIncrementalBackupsAcrossReplicasDiffers(bool value)
        {
            sut.EnableIncrementalBackupsAcrossReplicas = value;
            obj.EnableIncrementalBackupsAcrossReplicas = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenEnableIncrementalBackupsAcrossReplicasIsNull()
        {
            sut.EnableIncrementalBackupsAcrossReplicas = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjEnableIncrementalBackupsAcrossReplicasIsNull()
        {
            obj.EnableIncrementalBackupsAcrossReplicas = null;
            Assert.True(sut.Equals(obj));
        }

        [Theory, InlineData(true), InlineData(false)]
        public void ReturnsFalseWhenEnableSendWindowSizeInBytesDiffers(bool value)
        {
            sut.EnableSendWindowSizeInBytes = value;
            obj.EnableSendWindowSizeInBytes = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenEnableSendWindowSizeInBytesIsNull()
        {
            sut.EnableSendWindowSizeInBytes = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjEnableSendWindowSizeInBytesIsNull()
        {
            obj.EnableSendWindowSizeInBytes = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxReplicationQueueSendWindowSizeInBytesDiffers()
        {
            obj.MaxReplicationQueueSendWindowSizeInBytes = sut.MaxReplicationQueueSendWindowSizeInBytes + fuzzy.Byte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxReplicationQueueSendWindowSizeInBytesIsNull()
        {
            sut.MaxReplicationQueueSendWindowSizeInBytes = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMaxReplicationQueueSendWindowSizeInBytesIsNull()
        {
            obj.MaxReplicationQueueSendWindowSizeInBytes = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxCopyQueueSendWindowSizeInBytesDiffers()
        {
            obj.MaxCopyQueueSendWindowSizeInBytes = sut.MaxCopyQueueSendWindowSizeInBytes + fuzzy.Byte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenMaxCopyQueueSendWindowSizeInBytesIsNull()
        {
            sut.MaxCopyQueueSendWindowSizeInBytes = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjMaxCopyQueueSendWindowSizeInBytesIsNull()
        {
            obj.MaxCopyQueueSendWindowSizeInBytes = null;
            Assert.True(sut.Equals(obj));
        }

        [Theory, InlineData(true), InlineData(false)]
        public void ReturnsFalseWhenUseIndividualHeapPerReplicaDiffers(bool value)
        {
            sut.UseIndividualHeapPerReplica = value;
            obj.UseIndividualHeapPerReplica = !value;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenUseIndividualHeapPerReplicaIsNull()
        {
            sut.UseIndividualHeapPerReplica = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjUseIndividualHeapPerReplicaIsNull()
        {
            obj.UseIndividualHeapPerReplica = null;
            Assert.True(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT Bug. BaseInternalEquals doesn't compare InitialReplicaHeapSizeInKB
        public void ReturnsFalseWhenInitialReplicaHeapSizeInKBDiffers()
        {
            obj.InitialReplicaHeapSizeInKB = sut.InitialReplicaHeapSizeInKB + fuzzy.Byte().Between(1, 5);
            Assert.False(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT Bug. BaseInternalEquals doesn't compare InitialReplicaHeapSizeInKB
        public void ReturnsFalseWhenInitialReplicaHeapSizeInKBIsNull()
        {
            sut.InitialReplicaHeapSizeInKB = null;
            Assert.False(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsTrueWhenObjInitialReplicaHeapSizeInKBIsNull()
        {
            obj.InitialReplicaHeapSizeInKB = null;
            Assert.True(sut.Equals(obj));
        }
#endif

        [Theory, MemberData(nameof(UncomparedBaseMutations))]
        public void IgnoresUncomparedBaseProperty(string property, Action<ReliableStateManagerReplicatorSettings> mutate)
        {
            _ = property; // Shown in the test display name to identify the failing property.
            mutate(obj);
            Assert.True(sut.Equals(obj));
        }

        public static TheoryData<string, Action<ReliableStateManagerReplicatorSettings>> UncomparedBaseMutations => new()
        {
            { nameof(ReliableStateManagerReplicatorSettings.RetryInterval), o => o.RetryInterval = fuzzy.TimeSpan() },
            { nameof(ReliableStateManagerReplicatorSettings.BatchAcknowledgementInterval), o => o.BatchAcknowledgementInterval = fuzzy.TimeSpan() },
            { nameof(ReliableStateManagerReplicatorSettings.ReplicatorAddress), o => o.ReplicatorAddress = fuzzy.String() },
            { nameof(ReliableStateManagerReplicatorSettings.ReplicatorListenAddress), o => o.ReplicatorListenAddress = fuzzy.String() },
            { nameof(ReliableStateManagerReplicatorSettings.ReplicatorPublishAddress), o => o.ReplicatorPublishAddress = fuzzy.String() },
            { nameof(ReliableStateManagerReplicatorSettings.InitialCopyQueueSize), o => o.InitialCopyQueueSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxCopyQueueSize), o => o.MaxCopyQueueSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.InitialPrimaryReplicationQueueSize), o => o.InitialPrimaryReplicationQueueSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxPrimaryReplicationQueueSize), o => o.MaxPrimaryReplicationQueueSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxPrimaryReplicationQueueMemorySize), o => o.MaxPrimaryReplicationQueueMemorySize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.InitialSecondaryReplicationQueueSize), o => o.InitialSecondaryReplicationQueueSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxSecondaryReplicationQueueSize), o => o.MaxSecondaryReplicationQueueSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxSecondaryReplicationQueueMemorySize), o => o.MaxSecondaryReplicationQueueMemorySize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxReplicationMessageSize), o => o.MaxReplicationMessageSize = fuzzy.Int64() },
            { nameof(ReliableStateManagerReplicatorSettings.SecurityCredentials), o => o.SecurityCredentials = new NoneSecurityCredentials() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxWriteQueueDepthInKB), o => o.MaxWriteQueueDepthInKB = fuzzy.Int32() },
            { nameof(ReliableStateManagerReplicatorSettings.SecondaryClearAcknowledgedOperations), o => o.SecondaryClearAcknowledgedOperations = fuzzy.Boolean() },
        };

        [Fact(Explicit = true)] // TODO: SUT bug. Equals is asymmetric.
        public void IsSymmetric()
        {
            // InternalEquals only inspects properties set on the right-hand side, so sut.Equals(sparse) returns true
            // while sparse.Equals(sut) returns false, violating the Object.Equals symmetry contract.
            var sparse = new ReliableStateManagerReplicatorSettings2();
            bool sutEqualsSparse = sut.Equals(sparse);
            bool sparseEqualsSut = sparse.Equals(sut);
            Assert.Equal(sutEqualsSparse, sparseEqualsSut);
        }
    }

    public new sealed class GetHashCode : ReliableStateManagerReplicatorSettings2Test
    {
        [Fact]
        public void ReturnsIdentityHash() =>
            Assert.Equal(RuntimeHelpers.GetHashCode(sut), sut.GetHashCode());

        [Fact(Explicit = true)] // TODO: SUT bug. Equals and GetHashCode are inconsistent.
        public void IsConsistentWithEquals()
        {
            // GetHashCode returns the identity hash, so two instances that Equals reports as equal
            // produce different hash codes, violating the Object.GetHashCode contract.
            Populate();
            ReliableStateManagerReplicatorSettings2 other;
            // Guarantee the assertion fails for the right reason rather than a spurious identity-hash collision.
            do
                other = CloneOf(sut);
            while (RuntimeHelpers.GetHashCode(other) == RuntimeHelpers.GetHashCode(sut));

            Assert.True(sut.Equals(other));
            Assert.Equal(sut.GetHashCode(), other.GetHashCode());
        }
    }

    public sealed class ReplicationBatchSendInterval : ReliableStateManagerReplicatorSettings2Test
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            TimeSpan expected = fuzzy.TimeSpan();
            sut.ReplicationBatchSendInterval = expected;
            Assert.Equal(expected, sut.ReplicationBatchSendInterval);
        }
    }

    public sealed class ReplicationBatchSize : ReliableStateManagerReplicatorSettings2Test
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.ReplicationBatchSize = expected;
            Assert.Equal(expected, sut.ReplicationBatchSize);
        }
    }

    public sealed class ShouldAbortCopyForTruncation : ReliableStateManagerReplicatorSettings2Test
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.ShouldAbortCopyForTruncation = value;
            Assert.Equal(value, sut.ShouldAbortCopyForTruncation);
        }
    }

    public new sealed class ToString : ReliableStateManagerReplicatorSettings2Test
    {
        public ToString() => Populate();

        [Fact]
        public void StartsWithBaseToString() =>
            Assert.StartsWith(BaseCopyOf(sut).ToString(), sut.ToString());

        [Fact]
        public void IncludesCopyBatchSizeInKBWhenSet() =>
            Assert.Contains($"{nameof(sut.CopyBatchSizeInKB)} = {sut.CopyBatchSizeInKB}", sut.ToString());

        [Theory, InlineData(true), InlineData(false)]
        public void IncludesEnableStableReadsWhenSet(bool value)
        {
            sut.EnableStableReads = value;
            Assert.Contains($"{nameof(sut.EnableStableReads)} = {value}", sut.ToString());
        }

        [Theory, InlineData(true), InlineData(false)]
        public void IncludesShouldAbortCopyForTruncationWhenSet(bool value)
        {
            sut.ShouldAbortCopyForTruncation = value;
            Assert.Contains($"{nameof(sut.ShouldAbortCopyForTruncation)} = {value}", sut.ToString());
        }

        [Fact]
        public void IncludesReplicationBatchSizeWhenSet() =>
            Assert.Contains($"{nameof(sut.ReplicationBatchSize)} = {sut.ReplicationBatchSize}", sut.ToString());

        [Fact]
        public void IncludesReplicationBatchSendIntervalWhenSet() =>
            Assert.Contains($"{nameof(sut.ReplicationBatchSendInterval)} = {sut.ReplicationBatchSendInterval}", sut.ToString());

        [Fact]
        public void OmitsCopyBatchSizeInKBWhenNull()
        {
            sut.CopyBatchSizeInKB = null;
            Assert.DoesNotContain($"{nameof(sut.CopyBatchSizeInKB)} = ", sut.ToString());
        }

        [Fact]
        public void OmitsEnableStableReadsWhenNull()
        {
            sut.EnableStableReads = null;
            Assert.DoesNotContain($"{nameof(sut.EnableStableReads)} = ", sut.ToString());
        }

        [Fact]
        public void OmitsShouldAbortCopyForTruncationWhenNull()
        {
            sut.ShouldAbortCopyForTruncation = null;
            Assert.DoesNotContain($"{nameof(sut.ShouldAbortCopyForTruncation)} = ", sut.ToString());
        }

        [Fact]
        public void OmitsReplicationBatchSizeWhenNull()
        {
            sut.ReplicationBatchSize = null;
            Assert.DoesNotContain($"{nameof(sut.ReplicationBatchSize)} = ", sut.ToString());
        }

        [Fact]
        public void OmitsReplicationBatchSendIntervalWhenNull()
        {
            sut.ReplicationBatchSendInterval = null;
            Assert.DoesNotContain($"{nameof(sut.ReplicationBatchSendInterval)} = ", sut.ToString());
        }
    }

    void Populate()
    {
        sut.CopyBatchSizeInKB = fuzzy.Int64();
        sut.EnableStableReads = fuzzy.Boolean();
        sut.ShouldAbortCopyForTruncation = fuzzy.Boolean();
        sut.ReplicationBatchSize = fuzzy.Int64();
        // Cap leaves headroom for + fuzzy.TimeSpan().Seconds() in Differs tests; SUT does not cap this.
        sut.ReplicationBatchSendInterval = fuzzy.TimeSpan().Maximum(TimeSpan.MaxValue - TimeSpan.FromMinutes(1));
        sut.SharedLogId = fuzzy.String();
        sut.SharedLogPath = fuzzy.String();
        sut.MaxStreamSizeInMB = fuzzy.Int32();
        sut.MaxRecordSizeInKB = fuzzy.Int32();
        sut.MaxMetadataSizeInKB = fuzzy.Int32();
        sut.OptimizeForLocalSSD = fuzzy.Boolean();
        sut.OptimizeLogForLowerDiskUsage = fuzzy.Boolean();
        sut.CheckpointThresholdInMB = fuzzy.Int32();
        sut.MaxAccumulatedBackupLogSizeInMB = fuzzy.Int32();
        sut.MinLogSizeInMB = fuzzy.Int32();
        sut.TruncationThresholdFactor = fuzzy.Int32();
        sut.ThrottlingThresholdFactor = fuzzy.Int32();
        // Cap leaves headroom for + fuzzy.TimeSpan().Seconds() in Differs tests; SUT does not cap this.
        sut.SlowApiMonitoringDuration = fuzzy.TimeSpan().Maximum(TimeSpan.MaxValue - TimeSpan.FromMinutes(1));
#if NETFRAMEWORK
        sut.LogTruncationIntervalSeconds = fuzzy.Int32();
        sut.EnableIncrementalBackupsAcrossReplicas = fuzzy.Boolean();
        sut.EnableSendWindowSizeInBytes = fuzzy.Boolean();
        sut.MaxReplicationQueueSendWindowSizeInBytes = fuzzy.UInt32();
        sut.MaxCopyQueueSendWindowSizeInBytes = fuzzy.UInt32();
        sut.UseIndividualHeapPerReplica = fuzzy.Boolean();
        sut.InitialReplicaHeapSizeInKB = fuzzy.UInt32();
#endif
    }

    static ReliableStateManagerReplicatorSettings2 CloneOf(ReliableStateManagerReplicatorSettings2 source)
    {
        var clone = CopyBaseProperties(new ReliableStateManagerReplicatorSettings2(), source);
        clone.CopyBatchSizeInKB = source.CopyBatchSizeInKB;
        clone.EnableStableReads = source.EnableStableReads;
        clone.ShouldAbortCopyForTruncation = source.ShouldAbortCopyForTruncation;
        clone.ReplicationBatchSize = source.ReplicationBatchSize;
        clone.ReplicationBatchSendInterval = source.ReplicationBatchSendInterval;
        return clone;
    }

    static ReliableStateManagerReplicatorSettings BaseCopyOf(ReliableStateManagerReplicatorSettings2 source) =>
        CopyBaseProperties(new ReliableStateManagerReplicatorSettings(), source);

    static T CopyBaseProperties<T>(T target, ReliableStateManagerReplicatorSettings2 source)
        where T : ReliableStateManagerReplicatorSettings
    {
        target.SharedLogId = source.SharedLogId;
        target.SharedLogPath = source.SharedLogPath;
        target.MaxStreamSizeInMB = source.MaxStreamSizeInMB;
        target.MaxRecordSizeInKB = source.MaxRecordSizeInKB;
        target.MaxMetadataSizeInKB = source.MaxMetadataSizeInKB;
        target.OptimizeForLocalSSD = source.OptimizeForLocalSSD;
        target.OptimizeLogForLowerDiskUsage = source.OptimizeLogForLowerDiskUsage;
        target.CheckpointThresholdInMB = source.CheckpointThresholdInMB;
        target.MaxAccumulatedBackupLogSizeInMB = source.MaxAccumulatedBackupLogSizeInMB;
        target.MinLogSizeInMB = source.MinLogSizeInMB;
        target.TruncationThresholdFactor = source.TruncationThresholdFactor;
        target.ThrottlingThresholdFactor = source.ThrottlingThresholdFactor;
        target.SlowApiMonitoringDuration = source.SlowApiMonitoringDuration;
#if NETFRAMEWORK
        target.LogTruncationIntervalSeconds = source.LogTruncationIntervalSeconds;
        target.EnableIncrementalBackupsAcrossReplicas = source.EnableIncrementalBackupsAcrossReplicas;
        target.EnableSendWindowSizeInBytes = source.EnableSendWindowSizeInBytes;
        target.MaxReplicationQueueSendWindowSizeInBytes = source.MaxReplicationQueueSendWindowSizeInBytes;
        target.MaxCopyQueueSendWindowSizeInBytes = source.MaxCopyQueueSendWindowSizeInBytes;
        target.UseIndividualHeapPerReplica = source.UseIndividualHeapPerReplica;
        target.InitialReplicaHeapSizeInKB = source.InitialReplicaHeapSizeInKB;
#endif
        return target;
    }
}
