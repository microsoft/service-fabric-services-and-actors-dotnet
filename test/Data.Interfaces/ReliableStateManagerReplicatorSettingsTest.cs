// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Runtime.CompilerServices;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class ReliableStateManagerReplicatorSettingsTest
{
    readonly ReliableStateManagerReplicatorSettings sut = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class BatchAcknowledgementInterval : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            TimeSpan expected = fuzzy.TimeSpan();
            sut.BatchAcknowledgementInterval = expected;
            Assert.Equal(expected, sut.BatchAcknowledgementInterval);
        }
    }

    public sealed class CheckpointThresholdInMB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.CheckpointThresholdInMB = expected;
            Assert.Equal(expected, sut.CheckpointThresholdInMB);
        }
    }

#if NETFRAMEWORK
    public sealed class EnableIncrementalBackupsAcrossReplicas : ReliableStateManagerReplicatorSettingsTest
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.EnableIncrementalBackupsAcrossReplicas = value;
            Assert.Equal(value, sut.EnableIncrementalBackupsAcrossReplicas);
        }
    }
#endif

#if NETFRAMEWORK
    public sealed class EnableSendWindowSizeInBytes : ReliableStateManagerReplicatorSettingsTest
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.EnableSendWindowSizeInBytes = value;
            Assert.Equal(value, sut.EnableSendWindowSizeInBytes);
        }
    }
#endif

    public new sealed class Equals : ReliableStateManagerReplicatorSettingsTest
    {
        new readonly object sut;
        readonly object obj;

        // Typed views of sut and obj used to mutate properties during setup and in tests; the SUT's Equals override accepts object.
        readonly ReliableStateManagerReplicatorSettings sutSettings;
        readonly ReliableStateManagerReplicatorSettings objSettings;

        readonly string sharedLogId = fuzzy.String();
        readonly string sharedLogPath = fuzzy.String();
        readonly int maxStreamSizeInMB = fuzzy.Int32();
        readonly int maxRecordSizeInKB = fuzzy.Int32();
        readonly int maxMetadataSizeInKB = fuzzy.Int32();
        readonly int checkpointThresholdInMB = fuzzy.Int32();
        readonly int maxAccumulatedBackupLogSizeInMB = fuzzy.Int32();
        readonly int minLogSizeInMB = fuzzy.Int32();
        readonly int truncationThresholdFactor = fuzzy.Int32();
        readonly int throttlingThresholdFactor = fuzzy.Int32();
        // Cap leaves headroom for + fuzzy.TimeSpan().Seconds() in Differs tests; SUT does not cap this.
        readonly TimeSpan slowApiMonitoringDuration = fuzzy.TimeSpan().Maximum(TimeSpan.MaxValue - TimeSpan.FromMinutes(1));
#if NETFRAMEWORK
        readonly int logTruncationIntervalSeconds = fuzzy.Int32();
        readonly uint maxReplicationQueueSendWindowSizeInBytes = fuzzy.UInt32();
        readonly uint maxCopyQueueSendWindowSizeInBytes = fuzzy.UInt32();
        readonly uint initialReplicaHeapSizeInKB = fuzzy.UInt32();
#endif

        public Equals()
        {
            sutSettings = Populate(new());
            sut = sutSettings;

            objSettings = Populate(new());
            obj = objSettings;

            ReliableStateManagerReplicatorSettings Populate(ReliableStateManagerReplicatorSettings s)
            {
                s.SharedLogId = sharedLogId;
                s.SharedLogPath = sharedLogPath;
                s.MaxStreamSizeInMB = maxStreamSizeInMB;
                s.MaxRecordSizeInKB = maxRecordSizeInKB;
                s.MaxMetadataSizeInKB = maxMetadataSizeInKB;
                s.OptimizeForLocalSSD = true;
                s.OptimizeLogForLowerDiskUsage = true;
                s.CheckpointThresholdInMB = checkpointThresholdInMB;
                s.MaxAccumulatedBackupLogSizeInMB = maxAccumulatedBackupLogSizeInMB;
                s.MinLogSizeInMB = minLogSizeInMB;
                s.TruncationThresholdFactor = truncationThresholdFactor;
                s.ThrottlingThresholdFactor = throttlingThresholdFactor;
                s.SlowApiMonitoringDuration = slowApiMonitoringDuration;
#if NETFRAMEWORK
                s.LogTruncationIntervalSeconds = logTruncationIntervalSeconds;
                s.EnableIncrementalBackupsAcrossReplicas = true;
                s.EnableSendWindowSizeInBytes = true;
                s.MaxReplicationQueueSendWindowSizeInBytes = maxReplicationQueueSendWindowSizeInBytes;
                s.MaxCopyQueueSendWindowSizeInBytes = maxCopyQueueSendWindowSizeInBytes;
                s.UseIndividualHeapPerReplica = true;
                s.InitialReplicaHeapSizeInKB = initialReplicaHeapSizeInKB;
#endif
                return s;
            }
        }

        [Fact]
        public void ReturnsTrueWhenAllPropertiesMatch() =>
            Assert.True(sut.Equals(obj));

        [Theory, InlineData(false), InlineData(null)]
        public void ReturnsTrueWhenNullableBoolPropertiesMatch(bool? value)
        {
            sutSettings.OptimizeForLocalSSD = value;
            sutSettings.OptimizeLogForLowerDiskUsage = value;
            objSettings.OptimizeForLocalSSD = value;
            objSettings.OptimizeLogForLowerDiskUsage = value;
#if NETFRAMEWORK
            sutSettings.EnableIncrementalBackupsAcrossReplicas = value;
            sutSettings.EnableSendWindowSizeInBytes = value;
            sutSettings.UseIndividualHeapPerReplica = value;
            objSettings.EnableIncrementalBackupsAcrossReplicas = value;
            objSettings.EnableSendWindowSizeInBytes = value;
            objSettings.UseIndividualHeapPerReplica = value;
#endif
            Assert.True(sut.Equals(obj));
        }

        [Fact]
        public void ReturnsFalseWhenObjIsNull() =>
            Assert.False(sut.Equals(null));

        [Fact]
        public void ReturnsFalseWhenObjIsDerivedType() =>
            Assert.False(sut.Equals(new DerivedSettings()));

        [Theory, MemberData(nameof(DifferingV2PropertyMutations))]
        public void ReturnsFalseWhenPropertyDiffers(string _, Action<ReliableStateManagerReplicatorSettings, ReliableStateManagerReplicatorSettings> differ)
        {
            differ(sutSettings, objSettings);
            Assert.False(sut.Equals(obj));
        }

        [Theory, MemberData(nameof(NullableV2Properties))]
        public void ReturnsFalseWhenSutPropertyIsNull(string _, Action<ReliableStateManagerReplicatorSettings> setNull)
        {
            setNull(sutSettings);
            Assert.False(sut.Equals(obj));
        }

        [Theory, MemberData(nameof(NullableV2Properties))]
        public void ReturnsTrueWhenObjPropertyIsNull(string _, Action<ReliableStateManagerReplicatorSettings> setNull)
        {
            setNull(objSettings);
            Assert.True(sut.Equals(obj));
        }

        [Theory, MemberData(nameof(EmptyV2StringProperties))]
        public void ReturnsTrueWhenObjPropertyIsEmpty(string _, Action<ReliableStateManagerReplicatorSettings> setEmpty)
        {
            setEmpty(objSettings);
            Assert.True(sut.Equals(obj));
        }

        [Theory, MemberData(nameof(NonV2Mutations))]
        public void IgnoresNonV2Property(string _, Action<ReliableStateManagerReplicatorSettings> mutate)
        {
            mutate(objSettings);
            Assert.True(sut.Equals(obj));
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Equals is asymmetric.
        public void IsSymmetric()
        {
            // InternalEquals only inspects properties set on the right-hand side, so sut.Equals(sparse) returns true
            // while sparse.Equals(sut) returns false, violating the Object.Equals symmetry contract.
            var sparse = new ReliableStateManagerReplicatorSettings();
            Assert.Equal(sut.Equals(sparse), sparse.Equals(sut));
        }

        public static TheoryData<string, Action<ReliableStateManagerReplicatorSettings>> EmptyV2StringProperties { get; } = new()
        {
            { nameof(ReliableStateManagerReplicatorSettings.SharedLogId), static o => o.SharedLogId = string.Empty },
            { nameof(ReliableStateManagerReplicatorSettings.SharedLogPath), static o => o.SharedLogPath = string.Empty },
        };

        public static TheoryData<string, Action<ReliableStateManagerReplicatorSettings, ReliableStateManagerReplicatorSettings>> DifferingV2PropertyMutations { get; } = new()
        {
            { nameof(ReliableStateManagerReplicatorSettings.SharedLogId), static (s, o) => o.SharedLogId = s.SharedLogId + fuzzy.String() },
            { nameof(ReliableStateManagerReplicatorSettings.SharedLogPath), static (s, o) => o.SharedLogPath = s.SharedLogPath + fuzzy.String() },
            { nameof(ReliableStateManagerReplicatorSettings.MaxStreamSizeInMB), static (s, o) => o.MaxStreamSizeInMB = s.MaxStreamSizeInMB + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.MaxRecordSizeInKB), static (s, o) => o.MaxRecordSizeInKB = s.MaxRecordSizeInKB + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.MaxMetadataSizeInKB), static (s, o) => o.MaxMetadataSizeInKB = s.MaxMetadataSizeInKB + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.OptimizeForLocalSSD), static (s, o) => o.OptimizeForLocalSSD = false },
            { nameof(ReliableStateManagerReplicatorSettings.OptimizeLogForLowerDiskUsage), static (s, o) => o.OptimizeLogForLowerDiskUsage = false },
            { nameof(ReliableStateManagerReplicatorSettings.CheckpointThresholdInMB), static (s, o) => o.CheckpointThresholdInMB = s.CheckpointThresholdInMB + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.MaxAccumulatedBackupLogSizeInMB), static (s, o) => o.MaxAccumulatedBackupLogSizeInMB = s.MaxAccumulatedBackupLogSizeInMB + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.MinLogSizeInMB), static (s, o) => o.MinLogSizeInMB = s.MinLogSizeInMB + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.TruncationThresholdFactor), static (s, o) => o.TruncationThresholdFactor = s.TruncationThresholdFactor + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.ThrottlingThresholdFactor), static (s, o) => o.ThrottlingThresholdFactor = s.ThrottlingThresholdFactor + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.SlowApiMonitoringDuration), static (s, o) => o.SlowApiMonitoringDuration = s.SlowApiMonitoringDuration + fuzzy.TimeSpan().Seconds() },
#if NETFRAMEWORK
            { nameof(ReliableStateManagerReplicatorSettings.LogTruncationIntervalSeconds), static (s, o) => o.LogTruncationIntervalSeconds = s.LogTruncationIntervalSeconds + fuzzy.SByte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.EnableIncrementalBackupsAcrossReplicas), static (s, o) => o.EnableIncrementalBackupsAcrossReplicas = false },
            { nameof(ReliableStateManagerReplicatorSettings.EnableSendWindowSizeInBytes), static (s, o) => o.EnableSendWindowSizeInBytes = false },
            { nameof(ReliableStateManagerReplicatorSettings.MaxReplicationQueueSendWindowSizeInBytes), static (s, o) => o.MaxReplicationQueueSendWindowSizeInBytes = s.MaxReplicationQueueSendWindowSizeInBytes + fuzzy.Byte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.MaxCopyQueueSendWindowSizeInBytes), static (s, o) => o.MaxCopyQueueSendWindowSizeInBytes = s.MaxCopyQueueSendWindowSizeInBytes + fuzzy.Byte().Between(1, 5) },
            { nameof(ReliableStateManagerReplicatorSettings.UseIndividualHeapPerReplica), static (s, o) => o.UseIndividualHeapPerReplica = false },
            { nameof(ReliableStateManagerReplicatorSettings.InitialReplicaHeapSizeInKB), static (s, o) => o.InitialReplicaHeapSizeInKB = s.InitialReplicaHeapSizeInKB + fuzzy.Byte().Between(1, 5) },
#endif
        };

        sealed class DerivedSettings : ReliableStateManagerReplicatorSettings { }
    }

    public new sealed class GetHashCode : ReliableStateManagerReplicatorSettingsTest
    {
        new readonly object sut;

        public GetHashCode() => sut = base.sut;

        [Fact]
        public void ReturnsBaseImplementation() =>
            Assert.Equal(RuntimeHelpers.GetHashCode(sut), sut.GetHashCode());

        [Fact(Explicit = true)] // TODO: SUT bug. Equals and GetHashCode are inconsistent.
        public void IsConsistentWithEquals()
        {
            // GetHashCode returns the identity hash, so two instances that Equals reports as equal
            // produce different hash codes, violating the Object.GetHashCode contract.
            base.sut.SharedLogId = fuzzy.String();
            object obj;
            // Guarantee the assertion fails for the right reason rather than a spurious identity-hash collision.
            do
                obj = new ReliableStateManagerReplicatorSettings { SharedLogId = base.sut.SharedLogId };
            while (RuntimeHelpers.GetHashCode(obj) == RuntimeHelpers.GetHashCode(sut));

            Assert.True(sut.Equals(obj));
            Assert.Equal(sut.GetHashCode(), obj.GetHashCode());
        }
    }

    public sealed class InitialCopyQueueSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.InitialCopyQueueSize = expected;
            Assert.Equal(expected, sut.InitialCopyQueueSize);
        }
    }

    public sealed class InitialPrimaryReplicationQueueSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.InitialPrimaryReplicationQueueSize = expected;
            Assert.Equal(expected, sut.InitialPrimaryReplicationQueueSize);
        }
    }

#if NETFRAMEWORK
    public sealed class InitialReplicaHeapSizeInKB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            uint expected = fuzzy.UInt32();
            sut.InitialReplicaHeapSizeInKB = expected;
            Assert.Equal(expected, sut.InitialReplicaHeapSizeInKB);
        }
    }
#endif

    public sealed class InitialSecondaryReplicationQueueSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.InitialSecondaryReplicationQueueSize = expected;
            Assert.Equal(expected, sut.InitialSecondaryReplicationQueueSize);
        }
    }

#if NETFRAMEWORK
    public sealed class LogTruncationIntervalSeconds : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.LogTruncationIntervalSeconds = expected;
            Assert.Equal(expected, sut.LogTruncationIntervalSeconds);
        }
    }
#endif

    public sealed class MaxAccumulatedBackupLogSizeInMB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.MaxAccumulatedBackupLogSizeInMB = expected;
            Assert.Equal(expected, sut.MaxAccumulatedBackupLogSizeInMB);
        }
    }

#if NETFRAMEWORK
    public sealed class MaxCopyQueueSendWindowSizeInBytes : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            uint expected = fuzzy.UInt32();
            sut.MaxCopyQueueSendWindowSizeInBytes = expected;
            Assert.Equal(expected, sut.MaxCopyQueueSendWindowSizeInBytes);
        }
    }
#endif

    public sealed class MaxCopyQueueSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.MaxCopyQueueSize = expected;
            Assert.Equal(expected, sut.MaxCopyQueueSize);
        }
    }

    public sealed class MaxMetadataSizeInKB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.MaxMetadataSizeInKB = expected;
            Assert.Equal(expected, sut.MaxMetadataSizeInKB);
        }
    }

    public sealed class MaxPrimaryReplicationQueueMemorySize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.MaxPrimaryReplicationQueueMemorySize = expected;
            Assert.Equal(expected, sut.MaxPrimaryReplicationQueueMemorySize);
        }
    }

    public sealed class MaxPrimaryReplicationQueueSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.MaxPrimaryReplicationQueueSize = expected;
            Assert.Equal(expected, sut.MaxPrimaryReplicationQueueSize);
        }
    }

    public sealed class MaxRecordSizeInKB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.MaxRecordSizeInKB = expected;
            Assert.Equal(expected, sut.MaxRecordSizeInKB);
        }
    }

    public sealed class MaxReplicationMessageSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.MaxReplicationMessageSize = expected;
            Assert.Equal(expected, sut.MaxReplicationMessageSize);
        }
    }

#if NETFRAMEWORK
    public sealed class MaxReplicationQueueSendWindowSizeInBytes : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            uint expected = fuzzy.UInt32();
            sut.MaxReplicationQueueSendWindowSizeInBytes = expected;
            Assert.Equal(expected, sut.MaxReplicationQueueSendWindowSizeInBytes);
        }
    }
#endif

    public sealed class MaxSecondaryReplicationQueueMemorySize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.MaxSecondaryReplicationQueueMemorySize = expected;
            Assert.Equal(expected, sut.MaxSecondaryReplicationQueueMemorySize);
        }
    }

    public sealed class MaxSecondaryReplicationQueueSize : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            long expected = fuzzy.Int64();
            sut.MaxSecondaryReplicationQueueSize = expected;
            Assert.Equal(expected, sut.MaxSecondaryReplicationQueueSize);
        }
    }

    public sealed class MaxStreamSizeInMB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.MaxStreamSizeInMB = expected;
            Assert.Equal(expected, sut.MaxStreamSizeInMB);
        }
    }

    public sealed class MaxWriteQueueDepthInKB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.MaxWriteQueueDepthInKB = expected;
            Assert.Equal(expected, sut.MaxWriteQueueDepthInKB);
        }
    }

    public sealed class MinLogSizeInMB : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.MinLogSizeInMB = expected;
            Assert.Equal(expected, sut.MinLogSizeInMB);
        }
    }

    public sealed class OptimizeForLocalSSD : ReliableStateManagerReplicatorSettingsTest
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.OptimizeForLocalSSD = value;
            Assert.Equal(value, sut.OptimizeForLocalSSD);
        }
    }

    public sealed class OptimizeLogForLowerDiskUsage : ReliableStateManagerReplicatorSettingsTest
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.OptimizeLogForLowerDiskUsage = value;
            Assert.Equal(value, sut.OptimizeLogForLowerDiskUsage);
        }
    }

    public sealed class ReplicatorAddress : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.ReplicatorAddress = expected;
            Assert.Equal(expected, sut.ReplicatorAddress);
        }
    }

    public sealed class ReplicatorListenAddress : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.ReplicatorListenAddress = expected;
            Assert.Equal(expected, sut.ReplicatorListenAddress);
        }
    }

    public sealed class ReplicatorPublishAddress : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.ReplicatorPublishAddress = expected;
            Assert.Equal(expected, sut.ReplicatorPublishAddress);
        }
    }

    public sealed class RetryInterval : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            TimeSpan expected = fuzzy.TimeSpan();
            sut.RetryInterval = expected;
            Assert.Equal(expected, sut.RetryInterval);
        }
    }

    public sealed class SecondaryClearAcknowledgedOperations : ReliableStateManagerReplicatorSettingsTest
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.SecondaryClearAcknowledgedOperations = value;
            Assert.Equal(value, sut.SecondaryClearAcknowledgedOperations);
        }
    }

    public sealed class SecurityCredentials : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            var expected = new NoneSecurityCredentials();
            sut.SecurityCredentials = expected;
            Assert.Same(expected, sut.SecurityCredentials);
        }
    }

    public sealed class SharedLogId : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.SharedLogId = expected;
            Assert.Equal(expected, sut.SharedLogId);
        }
    }

    public sealed class SharedLogPath : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            string expected = fuzzy.String();
            sut.SharedLogPath = expected;
            Assert.Equal(expected, sut.SharedLogPath);
        }
    }

    public sealed class SlowApiMonitoringDuration : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            TimeSpan expected = fuzzy.TimeSpan();
            sut.SlowApiMonitoringDuration = expected;
            Assert.Equal(expected, sut.SlowApiMonitoringDuration);
        }
    }

    public sealed class ThrottlingThresholdFactor : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.ThrottlingThresholdFactor = expected;
            Assert.Equal(expected, sut.ThrottlingThresholdFactor);
        }
    }

    public new sealed class ToString : ReliableStateManagerReplicatorSettingsTest
    {
        new readonly object sut;
        readonly ReliableStateManagerReplicatorSettings sutSettings;

        readonly string sharedLogId = fuzzy.String();
        readonly string sharedLogPath = fuzzy.String();
        readonly int maxStreamSizeInMB = fuzzy.Int32();
        readonly int maxMetadataSizeInKB = fuzzy.Int32();
        readonly int maxRecordSizeInKB = fuzzy.Int32();
        readonly int checkpointThresholdInMB = fuzzy.Int32();
        readonly int maxAccumulatedBackupLogSizeInMB = fuzzy.Int32();
        readonly TimeSpan slowApiMonitoringDuration = fuzzy.TimeSpan();
        readonly int minLogSizeInMB = fuzzy.Int32();
        readonly int truncationThresholdFactor = fuzzy.Int32();
        readonly int throttlingThresholdFactor = fuzzy.Int32();
#if NETFRAMEWORK
        readonly int logTruncationIntervalSeconds = fuzzy.Int32();
        readonly uint maxReplicationQueueSendWindowSizeInBytes = fuzzy.UInt32();
        readonly uint maxCopyQueueSendWindowSizeInBytes = fuzzy.UInt32();
        readonly uint initialReplicaHeapSizeInKB = fuzzy.UInt32();
#endif

        public ToString()
        {
            sutSettings = new ReliableStateManagerReplicatorSettings
            {
                SharedLogId = sharedLogId,
                SharedLogPath = sharedLogPath,
                MaxStreamSizeInMB = maxStreamSizeInMB,
                MaxMetadataSizeInKB = maxMetadataSizeInKB,
                MaxRecordSizeInKB = maxRecordSizeInKB,
                CheckpointThresholdInMB = checkpointThresholdInMB,
                MaxAccumulatedBackupLogSizeInMB = maxAccumulatedBackupLogSizeInMB,
                OptimizeForLocalSSD = true,
                OptimizeLogForLowerDiskUsage = true,
                SlowApiMonitoringDuration = slowApiMonitoringDuration,
                MinLogSizeInMB = minLogSizeInMB,
                TruncationThresholdFactor = truncationThresholdFactor,
                ThrottlingThresholdFactor = throttlingThresholdFactor,
#if NETFRAMEWORK
                LogTruncationIntervalSeconds = logTruncationIntervalSeconds,
                EnableIncrementalBackupsAcrossReplicas = true,
                EnableSendWindowSizeInBytes = true,
                MaxReplicationQueueSendWindowSizeInBytes = maxReplicationQueueSendWindowSizeInBytes,
                MaxCopyQueueSendWindowSizeInBytes = maxCopyQueueSendWindowSizeInBytes,
                UseIndividualHeapPerReplica = true,
                InitialReplicaHeapSizeInKB = initialReplicaHeapSizeInKB,
#endif
            };
            sut = sutSettings;
        }

        [Fact]
        public void StartsWithNewLine() =>
            Assert.StartsWith(Environment.NewLine, sut.ToString());

        [Theory, MemberData(nameof(NonV2Mutations))]
        public void ExcludesNonV2Property(string property, Action<ReliableStateManagerReplicatorSettings> mutate)
        {
            mutate(sutSettings);
            Assert.DoesNotContain($"{property} = ", sut.ToString());
        }

        [Theory, MemberData(nameof(SetV2Properties))]
        public void IncludesPropertyWhenSet(string property, Action<ReliableStateManagerReplicatorSettings> setValue, Func<ReliableStateManagerReplicatorSettings, object> getValue)
        {
            setValue(sutSettings);
            Assert.Contains($"{property} = {getValue(sutSettings)}", sut.ToString());
        }

        [Theory, MemberData(nameof(NullableV2Properties))]
        public void OmitsPropertyWhenNull(string property, Action<ReliableStateManagerReplicatorSettings> setNull)
        {
            setNull(sutSettings);
            Assert.DoesNotContain($"{property} = ", sut.ToString());
        }

        public static TheoryData<string, Action<ReliableStateManagerReplicatorSettings>, Func<ReliableStateManagerReplicatorSettings, object>> SetV2Properties { get; } = new()
        {
            { nameof(ReliableStateManagerReplicatorSettings.SharedLogId), static _ => { }, static o => o.SharedLogId },
            { nameof(ReliableStateManagerReplicatorSettings.SharedLogPath), static _ => { }, static o => o.SharedLogPath },
            { nameof(ReliableStateManagerReplicatorSettings.MaxStreamSizeInMB), static _ => { }, static o => o.MaxStreamSizeInMB },
            { nameof(ReliableStateManagerReplicatorSettings.MaxRecordSizeInKB), static _ => { }, static o => o.MaxRecordSizeInKB },
            { nameof(ReliableStateManagerReplicatorSettings.MaxMetadataSizeInKB), static _ => { }, static o => o.MaxMetadataSizeInKB },
            { nameof(ReliableStateManagerReplicatorSettings.OptimizeForLocalSSD), static o => o.OptimizeForLocalSSD = true, static o => o.OptimizeForLocalSSD },
            { nameof(ReliableStateManagerReplicatorSettings.OptimizeForLocalSSD), static o => o.OptimizeForLocalSSD = false, static o => o.OptimizeForLocalSSD },
            { nameof(ReliableStateManagerReplicatorSettings.OptimizeLogForLowerDiskUsage), static o => o.OptimizeLogForLowerDiskUsage = true, static o => o.OptimizeLogForLowerDiskUsage },
            { nameof(ReliableStateManagerReplicatorSettings.OptimizeLogForLowerDiskUsage), static o => o.OptimizeLogForLowerDiskUsage = false, static o => o.OptimizeLogForLowerDiskUsage },
            { nameof(ReliableStateManagerReplicatorSettings.CheckpointThresholdInMB), static _ => { }, static o => o.CheckpointThresholdInMB },
            { nameof(ReliableStateManagerReplicatorSettings.MaxAccumulatedBackupLogSizeInMB), static _ => { }, static o => o.MaxAccumulatedBackupLogSizeInMB },
            { nameof(ReliableStateManagerReplicatorSettings.MinLogSizeInMB), static _ => { }, static o => o.MinLogSizeInMB },
            { nameof(ReliableStateManagerReplicatorSettings.TruncationThresholdFactor), static _ => { }, static o => o.TruncationThresholdFactor },
            { nameof(ReliableStateManagerReplicatorSettings.ThrottlingThresholdFactor), static _ => { }, static o => o.ThrottlingThresholdFactor },
            { nameof(ReliableStateManagerReplicatorSettings.SlowApiMonitoringDuration), static _ => { }, static o => o.SlowApiMonitoringDuration },
#if NETFRAMEWORK
            { nameof(ReliableStateManagerReplicatorSettings.LogTruncationIntervalSeconds), static _ => { }, static o => o.LogTruncationIntervalSeconds },
            { nameof(ReliableStateManagerReplicatorSettings.EnableIncrementalBackupsAcrossReplicas), static o => o.EnableIncrementalBackupsAcrossReplicas = true, static o => o.EnableIncrementalBackupsAcrossReplicas },
            { nameof(ReliableStateManagerReplicatorSettings.EnableIncrementalBackupsAcrossReplicas), static o => o.EnableIncrementalBackupsAcrossReplicas = false, static o => o.EnableIncrementalBackupsAcrossReplicas },
            { nameof(ReliableStateManagerReplicatorSettings.EnableSendWindowSizeInBytes), static o => o.EnableSendWindowSizeInBytes = true, static o => o.EnableSendWindowSizeInBytes },
            { nameof(ReliableStateManagerReplicatorSettings.EnableSendWindowSizeInBytes), static o => o.EnableSendWindowSizeInBytes = false, static o => o.EnableSendWindowSizeInBytes },
            { nameof(ReliableStateManagerReplicatorSettings.MaxReplicationQueueSendWindowSizeInBytes), static _ => { }, static o => o.MaxReplicationQueueSendWindowSizeInBytes },
            { nameof(ReliableStateManagerReplicatorSettings.MaxCopyQueueSendWindowSizeInBytes), static _ => { }, static o => o.MaxCopyQueueSendWindowSizeInBytes },
            { nameof(ReliableStateManagerReplicatorSettings.UseIndividualHeapPerReplica), static o => o.UseIndividualHeapPerReplica = true, static o => o.UseIndividualHeapPerReplica },
            { nameof(ReliableStateManagerReplicatorSettings.UseIndividualHeapPerReplica), static o => o.UseIndividualHeapPerReplica = false, static o => o.UseIndividualHeapPerReplica },
            { nameof(ReliableStateManagerReplicatorSettings.InitialReplicaHeapSizeInKB), static _ => { }, static o => o.InitialReplicaHeapSizeInKB },
#endif
        };
    }

    public sealed class TruncationThresholdFactor : ReliableStateManagerReplicatorSettingsTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            int expected = fuzzy.Int32();
            sut.TruncationThresholdFactor = expected;
            Assert.Equal(expected, sut.TruncationThresholdFactor);
        }
    }

#if NETFRAMEWORK
    public sealed class UseIndividualHeapPerReplica : ReliableStateManagerReplicatorSettingsTest
    {
        [Theory, InlineData(true), InlineData(false), InlineData(null)]
        public void IsSetToGivenValue(bool? value)
        {
            sut.UseIndividualHeapPerReplica = value;
            Assert.Equal(value, sut.UseIndividualHeapPerReplica);
        }
    }
#endif

    public static TheoryData<string, Action<ReliableStateManagerReplicatorSettings>> NullableV2Properties { get; } = new()
    {
        { nameof(ReliableStateManagerReplicatorSettings.SharedLogId), static o => o.SharedLogId = null },
        { nameof(ReliableStateManagerReplicatorSettings.SharedLogPath), static o => o.SharedLogPath = null },
        { nameof(ReliableStateManagerReplicatorSettings.MaxStreamSizeInMB), static o => o.MaxStreamSizeInMB = null },
        { nameof(ReliableStateManagerReplicatorSettings.MaxRecordSizeInKB), static o => o.MaxRecordSizeInKB = null },
        { nameof(ReliableStateManagerReplicatorSettings.MaxMetadataSizeInKB), static o => o.MaxMetadataSizeInKB = null },
        { nameof(ReliableStateManagerReplicatorSettings.OptimizeForLocalSSD), static o => o.OptimizeForLocalSSD = null },
        { nameof(ReliableStateManagerReplicatorSettings.OptimizeLogForLowerDiskUsage), static o => o.OptimizeLogForLowerDiskUsage = null },
        { nameof(ReliableStateManagerReplicatorSettings.CheckpointThresholdInMB), static o => o.CheckpointThresholdInMB = null },
        { nameof(ReliableStateManagerReplicatorSettings.MaxAccumulatedBackupLogSizeInMB), static o => o.MaxAccumulatedBackupLogSizeInMB = null },
        { nameof(ReliableStateManagerReplicatorSettings.MinLogSizeInMB), static o => o.MinLogSizeInMB = null },
        { nameof(ReliableStateManagerReplicatorSettings.TruncationThresholdFactor), static o => o.TruncationThresholdFactor = null },
        { nameof(ReliableStateManagerReplicatorSettings.ThrottlingThresholdFactor), static o => o.ThrottlingThresholdFactor = null },
        { nameof(ReliableStateManagerReplicatorSettings.SlowApiMonitoringDuration), static o => o.SlowApiMonitoringDuration = null },
#if NETFRAMEWORK
        { nameof(ReliableStateManagerReplicatorSettings.LogTruncationIntervalSeconds), static o => o.LogTruncationIntervalSeconds = null },
        { nameof(ReliableStateManagerReplicatorSettings.EnableIncrementalBackupsAcrossReplicas), static o => o.EnableIncrementalBackupsAcrossReplicas = null },
        { nameof(ReliableStateManagerReplicatorSettings.EnableSendWindowSizeInBytes), static o => o.EnableSendWindowSizeInBytes = null },
        { nameof(ReliableStateManagerReplicatorSettings.MaxReplicationQueueSendWindowSizeInBytes), static o => o.MaxReplicationQueueSendWindowSizeInBytes = null },
        { nameof(ReliableStateManagerReplicatorSettings.MaxCopyQueueSendWindowSizeInBytes), static o => o.MaxCopyQueueSendWindowSizeInBytes = null },
        { nameof(ReliableStateManagerReplicatorSettings.UseIndividualHeapPerReplica), static o => o.UseIndividualHeapPerReplica = null },
        { nameof(ReliableStateManagerReplicatorSettings.InitialReplicaHeapSizeInKB), static o => o.InitialReplicaHeapSizeInKB = null },
#endif
    };

    public static TheoryData<string, Action<ReliableStateManagerReplicatorSettings>> NonV2Mutations { get; } = new()
    {
        { nameof(ReliableStateManagerReplicatorSettings.RetryInterval), static o => o.RetryInterval = fuzzy.TimeSpan() },
        { nameof(ReliableStateManagerReplicatorSettings.BatchAcknowledgementInterval), static o => o.BatchAcknowledgementInterval = fuzzy.TimeSpan() },
        { nameof(ReliableStateManagerReplicatorSettings.ReplicatorAddress), static o => o.ReplicatorAddress = fuzzy.String() },
        { nameof(ReliableStateManagerReplicatorSettings.ReplicatorListenAddress), static o => o.ReplicatorListenAddress = fuzzy.String() },
        { nameof(ReliableStateManagerReplicatorSettings.ReplicatorPublishAddress), static o => o.ReplicatorPublishAddress = fuzzy.String() },
        { nameof(ReliableStateManagerReplicatorSettings.InitialCopyQueueSize), static o => o.InitialCopyQueueSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxCopyQueueSize), static o => o.MaxCopyQueueSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.InitialPrimaryReplicationQueueSize), static o => o.InitialPrimaryReplicationQueueSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxPrimaryReplicationQueueSize), static o => o.MaxPrimaryReplicationQueueSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxPrimaryReplicationQueueMemorySize), static o => o.MaxPrimaryReplicationQueueMemorySize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.InitialSecondaryReplicationQueueSize), static o => o.InitialSecondaryReplicationQueueSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxSecondaryReplicationQueueSize), static o => o.MaxSecondaryReplicationQueueSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxSecondaryReplicationQueueMemorySize), static o => o.MaxSecondaryReplicationQueueMemorySize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxReplicationMessageSize), static o => o.MaxReplicationMessageSize = fuzzy.Int64() },
        { nameof(ReliableStateManagerReplicatorSettings.SecurityCredentials), static o => o.SecurityCredentials = new NoneSecurityCredentials() },
        { nameof(ReliableStateManagerReplicatorSettings.MaxWriteQueueDepthInKB), static o => o.MaxWriteQueueDepthInKB = fuzzy.Int32() },
        { nameof(ReliableStateManagerReplicatorSettings.SecondaryClearAcknowledgedOperations), static o => o.SecondaryClearAcknowledgedOperations = fuzzy.Boolean() },
    };
}
