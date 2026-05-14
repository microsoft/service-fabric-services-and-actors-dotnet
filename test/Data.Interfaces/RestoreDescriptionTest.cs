// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class RestoreDescriptionTest
{
    // Constructor parameters
    readonly string backupFolderPath = fuzzy.String();
    readonly RestorePolicy restorePolicy = fuzzy.Enum<RestorePolicy>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor_String : RestoreDescriptionTest
    {
        [Fact]
        public void InitializesPropertiesWithSafePolicyByDefault()
        {
            var sut = new RestoreDescription(backupFolderPath);
            Assert.Same(backupFolderPath, sut.BackupFolderPath);
            Assert.Equal(RestorePolicy.Safe, sut.Policy);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Docs say backupFolderPath cannot be null, but constructor doesn't validate.
        public void ThrowsArgumentNullExceptionWhenBackupFolderPathIsNull()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new RestoreDescription(null));
            Assert.Equal(nameof(backupFolderPath), e.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Docs say backupFolderPath cannot be empty, but constructor doesn't validate.
        public void ThrowsArgumentExceptionWhenBackupFolderPathIsEmpty()
        {
            var e = Assert.Throws<ArgumentException>(() => new RestoreDescription(string.Empty));
            Assert.Equal(nameof(backupFolderPath), e.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Docs say backupFolderPath cannot be whitespace, but constructor doesn't validate.
        public void ThrowsArgumentExceptionWhenBackupFolderPathIsWhitespace()
        {
            var e = Assert.Throws<ArgumentException>(() => new RestoreDescription(" "));
            Assert.Equal(nameof(backupFolderPath), e.ParamName);
        }
    }

    public sealed class Constructor_String_RestorePolicy : RestoreDescriptionTest
    {
        [Theory, InlineData(RestorePolicy.Safe), InlineData(RestorePolicy.Force)]
        public void InitializesProperties(RestorePolicy policy)
        {
            var sut = new RestoreDescription(backupFolderPath, policy);
            Assert.Same(backupFolderPath, sut.BackupFolderPath);
            Assert.Equal(policy, sut.Policy);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Docs say backupFolderPath cannot be null, but constructor doesn't validate.
        public void ThrowsArgumentNullExceptionWhenBackupFolderPathIsNull()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new RestoreDescription(null, restorePolicy));
            Assert.Equal(nameof(backupFolderPath), e.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Docs say backupFolderPath cannot be empty, but constructor doesn't validate.
        public void ThrowsArgumentExceptionWhenBackupFolderPathIsEmpty()
        {
            var e = Assert.Throws<ArgumentException>(() => new RestoreDescription(string.Empty, restorePolicy));
            Assert.Equal(nameof(backupFolderPath), e.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Docs say backupFolderPath cannot be whitespace, but constructor doesn't validate.
        public void ThrowsArgumentExceptionWhenBackupFolderPathIsWhitespace()
        {
            var e = Assert.Throws<ArgumentException>(() => new RestoreDescription(" ", restorePolicy));
            Assert.Equal(nameof(backupFolderPath), e.ParamName);
        }
    }
}
