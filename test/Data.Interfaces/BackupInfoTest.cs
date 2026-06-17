// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using Fuzzy;
using Xunit;
using BackupVersion = Microsoft.ServiceFabric.Data.BackupInfo.BackupVersion;

namespace Microsoft.ServiceFabric.Data;

public abstract class BackupInfoTest
{
    // Constructor parameters
    readonly string directory = fuzzy.String();
    readonly BackupOption option = fuzzy.Enum<BackupOption>();
    readonly BackupVersion version = fuzzy.BackupVersion();
    readonly BackupVersion startBackupVersion = fuzzy.BackupVersion();
    readonly Guid backupId = Guid.NewGuid();
    readonly Guid parentBackupId = Guid.NewGuid();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor_String_BackupOption_BackupVersion : BackupInfoTest
    {
        [Theory, InlineData(BackupOption.Full), InlineData(BackupOption.Incremental)]
        public void InitializesPropertiesAndDefaults(BackupOption option)
        {
            var sut = new BackupInfo(directory, option, version);
            Assert.Same(directory, sut.Directory);
            Assert.Equal(option, sut.Option);
            Assert.Equal(version, sut.Version);
            Assert.Equal(BackupVersion.InvalidBackupVersion, sut.StartBackupVersion);
            Assert.Equal(Guid.Empty, sut.BackupId);
            Assert.Equal(Guid.Empty, sut.ParentBackupId);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate directory; null propagates to consumers.
        public void ThrowsArgumentNullExceptionWhenDirectoryIsNull()
        {
            // The constructor stores directory without validation, so a null argument is silently accepted
            // and surfaced via the Directory auto-property. Downstream callers that dereference the returned
            // value then fail far from the original site. The fix is to throw ArgumentNullException here.
            var e = Assert.Throws<ArgumentNullException>(() => new BackupInfo(null, option, version));
            Assert.Equal(nameof(directory), e.ParamName);
        }
    }

    public sealed class Constructor_String_BackupOption_BackupVersion_BackupVersion_Guid_Guid : BackupInfoTest
    {
        [Theory, InlineData(BackupOption.Full), InlineData(BackupOption.Incremental)]
        public void InitializesProperties(BackupOption option)
        {
            var sut = new BackupInfo(directory, option, version, startBackupVersion, backupId, parentBackupId);
            Assert.Same(directory, sut.Directory);
            Assert.Equal(option, sut.Option);
            Assert.Equal(version, sut.Version);
            Assert.Equal(startBackupVersion, sut.StartBackupVersion);
            Assert.Equal(backupId, sut.BackupId);
            Assert.Equal(parentBackupId, sut.ParentBackupId);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate directory; null propagates to consumers.
        public void ThrowsArgumentNullExceptionWhenDirectoryIsNull()
        {
            // The constructor stores directory without validation, so a null argument is silently accepted
            // and surfaced via the Directory auto-property. Downstream callers that dereference the returned
            // value then fail far from the original site. The fix is to throw ArgumentNullException here.
            var e = Assert.Throws<ArgumentNullException>(() => new BackupInfo(null, option, version, startBackupVersion, backupId, parentBackupId));
            Assert.Equal(nameof(directory), e.ParamName);
        }
    }

    public abstract class BackupVersionTest
    {
        readonly BackupVersion sut;

        readonly Epoch epoch = new(SafeInt64(), SafeInt64());
        readonly long lsn = SafeInt64(); // logical sequence number

        BackupVersionTest() =>
            sut = new BackupVersion(epoch, lsn);

        public sealed class Constructor : BackupVersionTest
        {
            [Fact]
            public void InitializesProperties()
            {
                Assert.Equal(epoch, sut.Epoch);
                Assert.Equal(lsn, sut.Lsn);
            }
        }

        public sealed class CompareTo : BackupVersionTest
        {
            [Fact]
            public void ReturnsZeroWhenOtherEqualsThis() =>
                Assert.Equal(0, sut.CompareTo(new BackupVersion(epoch, lsn)));

            [Fact]
            public void ReturnsNegativeWhenOtherEpochIsHigher() =>
                Assert.True(sut.CompareTo(new BackupVersion(new Epoch(epoch.DataLossNumber + 1, epoch.ConfigurationNumber), lsn)) < 0);

            [Fact]
            public void ReturnsPositiveWhenOtherEpochIsLower() =>
                Assert.True(sut.CompareTo(new BackupVersion(new Epoch(epoch.DataLossNumber - 1, epoch.ConfigurationNumber), lsn)) > 0);

            [Fact]
            public void ReturnsNegativeWhenOtherLsnIsHigher() =>
                Assert.True(sut.CompareTo(new BackupVersion(epoch, lsn + 1)) < 0);

            [Fact]
            public void ReturnsPositiveWhenOtherLsnIsLower() =>
                Assert.True(sut.CompareTo(new BackupVersion(epoch, lsn - 1)) > 0);

            [Fact]
            public void PrioritizesEpochOverLsn() =>
                Assert.True(sut.CompareTo(new BackupVersion(new Epoch(epoch.DataLossNumber + 1, epoch.ConfigurationNumber), lsn - 1)) < 0);
        }

        public sealed class Equals_BackupVersion : BackupVersionTest
        {
            [Fact]
            public void ReturnsTrueWhenOtherEqualsThis() =>
                Assert.True(sut.Equals(new BackupVersion(epoch, lsn)));

            [Fact]
            public void ReturnsFalseWhenOtherDiffers() =>
                Assert.False(sut.Equals(new BackupVersion(epoch, lsn + 1)));
        }

        public sealed class Equals_Object : BackupVersionTest
        {
            [Fact]
            public void ReturnsTrueWhenObjEqualsThis() =>
                Assert.True(sut.Equals((object)new BackupVersion(epoch, lsn)));

            [Fact]
            public void ReturnsFalseWhenObjIsNull() =>
                Assert.False(sut.Equals(null));

            [Fact]
            public void ReturnsFalseWhenObjIsNotBackupVersion() =>
                Assert.False(sut.Equals(new object()));
        }

        public new sealed class GetHashCode : BackupVersionTest
        {
            [Fact]
            public void ReturnsSameValueForEqualVersions() =>
                Assert.Equal(sut.GetHashCode(), new BackupVersion(epoch, lsn).GetHashCode());

            [Fact]
            public void VariesWithDataLossNumber() =>
                Assert.NotEqual(
                    sut.GetHashCode(),
                    new BackupVersion(new Epoch(epoch.DataLossNumber + 1, epoch.ConfigurationNumber), lsn).GetHashCode());

            [Fact]
            public void VariesWithConfigurationNumber() =>
                Assert.NotEqual(
                    sut.GetHashCode(),
                    new BackupVersion(new Epoch(epoch.DataLossNumber, epoch.ConfigurationNumber + 1), lsn).GetHashCode());

            [Fact]
            public void VariesWithLsn() =>
                Assert.NotEqual(
                    sut.GetHashCode(),
                    new BackupVersion(epoch, lsn + 1).GetHashCode());
        }

        public sealed class InvalidBackupVersion : BackupVersionTest
        {
            [Fact]
            public void RepresentsInvalidEpochAndLsn()
            {
                BackupVersion invalid = BackupVersion.InvalidBackupVersion;
                Assert.Equal(-1, invalid.Epoch.DataLossNumber);
                Assert.Equal(-1, invalid.Epoch.ConfigurationNumber);
                Assert.Equal(-1, invalid.Lsn);
            }
        }

        // Constrain to leave headroom for +1 arithmetic and avoid -1, whose Int64.GetHashCode() collides with 0.
        static long SafeInt64() => fuzzy.Int64().Minimum(0).Maximum(long.MaxValue - 1);
    }

    public new sealed class ToString : BackupInfoTest
    {
        [Theory, InlineData(BackupOption.Full, "Backup folder: {0}, backup option: Full")]
        [InlineData(BackupOption.Incremental, "Backup folder: {0}, backup option: Incremental")]
        public void ReturnsFormattedString(BackupOption option, string format)
        {
            var sut = new BackupInfo(directory, option, version);
            Assert.Equal(string.Format(format, directory), sut.ToString());
        }
    }
}
