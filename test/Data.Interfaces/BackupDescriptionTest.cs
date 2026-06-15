// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class BackupDescriptionTest
{
    // Constructor parameters
    readonly BackupOption option = fuzzy.Enum<BackupOption>();
    readonly Func<BackupInfo, CancellationToken, Task<bool>> backupCallback = static (_, _) => Task.FromResult(true);

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor_BackupOption_FuncOfBackupInfoCancellationTokenTaskOfBoolean : BackupDescriptionTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate backupCallback; consumers invoking it throw NullReferenceException.
        public void ThrowsArgumentNullExceptionWhenBackupCallbackIsNull()
        {
            // The constructor stores backupCallback without validation, so a null argument is silently accepted
            // and surfaced via the BackupCallback property. Consumers that later invoke the callback then
            // fail with NullReferenceException far from the original site. The fix is to throw ArgumentNullException here.
            var e = Assert.Throws<ArgumentNullException>(() => new BackupDescription(option, null));
            Assert.Equal(nameof(backupCallback), e.ParamName);
        }
    }

    public sealed class Constructor_FuncOfBackupInfoCancellationTokenTaskOfBoolean : BackupDescriptionTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate backupCallback; consumers invoking it throw NullReferenceException.
        public void ThrowsArgumentNullExceptionWhenBackupCallbackIsNull()
        {
            // The constructor stores backupCallback without validation, so a null argument is silently accepted
            // and surfaced via the BackupCallback property. Consumers that later invoke the callback then
            // fail with NullReferenceException far from the original site. The fix is to throw ArgumentNullException here.
            var e = Assert.Throws<ArgumentNullException>(() => new BackupDescription(null));
            Assert.Equal(nameof(backupCallback), e.ParamName);
        }
    }

    public sealed class BackupCallback : BackupDescriptionTest
    {
        [Fact]
        public void ReturnsValueSuppliedToConstructorWithBackupOption() =>
            Assert.Same(backupCallback, new BackupDescription(option, backupCallback).BackupCallback);

        [Fact]
        public void ReturnsValueSuppliedToConstructorWithoutBackupOption() =>
            Assert.Same(backupCallback, new BackupDescription(backupCallback).BackupCallback);
    }

    public sealed class Option : BackupDescriptionTest
    {
        [Theory, InlineData(BackupOption.Full), InlineData(BackupOption.Incremental)]
        public void ReturnsValueSuppliedToConstructor(BackupOption option) =>
            Assert.Equal(option, new BackupDescription(option, backupCallback).Option);

        [Fact]
        public void IsFullByDefault() =>
            Assert.Equal(BackupOption.Full, new BackupDescription(backupCallback).Option);
    }
}
