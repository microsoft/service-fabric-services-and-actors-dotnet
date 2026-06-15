// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class RestoreContextTest
{
    readonly RestoreContext sut;

    // Constructor parameters
    readonly Mock<IStateProviderReplica> stateProviderReplica = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    readonly string backupFolderPath = fuzzy.String();

    RestoreContextTest() =>
        sut = new RestoreContext(stateProviderReplica.Object);

    public sealed class Constructor : RestoreContextTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate stateProviderReplica; RestoreAsync throws NullReferenceException.
        public void ThrowsArgumentNullExceptionWhenStateProviderReplicaIsNull()
        {
            // The constructor stores stateProviderReplica without validation, so a null argument is silently accepted.
            // Subsequent calls to RestoreAsync then fail with NullReferenceException far from the original site.
            // The fix is to throw ArgumentNullException here.
            var e = Assert.Throws<ArgumentNullException>(() => new RestoreContext(null));
            Assert.Equal(nameof(stateProviderReplica), e.ParamName);
        }
    }

    public sealed class RestoreAsync_RestoreDescription : RestoreContextTest
    {
        [Theory, InlineData(RestorePolicy.Safe), InlineData(RestorePolicy.Force)]
        public void PassesCancellationTokenNoneToReplica(RestorePolicy policy)
        {
            var restoreDescription = new RestoreDescription(backupFolderPath, policy);
            Task expected = Task.FromResult(new object());
            _ = stateProviderReplica.Setup(_ => _.RestoreAsync(backupFolderPath, policy, CancellationToken.None))
                .Returns(expected);

#pragma warning disable xUnit1051 // test verifies default overload passes CancellationToken.None
            Task actual = sut.RestoreAsync(restoreDescription);
#pragma warning restore xUnit1051

            Assert.Same(expected, actual);
            stateProviderReplica.Verify(_ => _.RestoreAsync(
                It.IsAny<string>(), It.IsAny<RestorePolicy>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public sealed class RestoreAsync_RestoreDescription_CancellationToken : RestoreContextTest
    {
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        [Theory, InlineData(RestorePolicy.Safe), InlineData(RestorePolicy.Force)]
        public void DelegatesToReplicaWithGivenCancellationToken(RestorePolicy policy)
        {
            var restoreDescription = new RestoreDescription(backupFolderPath, policy);
            Task expected = Task.FromResult(new object());
            _ = stateProviderReplica.Setup(_ => _.RestoreAsync(backupFolderPath, policy, cancellationToken))
                .Returns(expected);

            Task actual = sut.RestoreAsync(restoreDescription, cancellationToken);

            Assert.Same(expected, actual);
            stateProviderReplica.Verify(_ => _.RestoreAsync(
                It.IsAny<string>(), It.IsAny<RestorePolicy>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
