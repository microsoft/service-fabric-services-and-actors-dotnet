// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Collections.Beta;

public abstract class IReliableDictionaryExtensionsTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class RemoveAsync : IReliableDictionaryExtensionsTest
    {
        readonly Mock<IReliableDictionary4<string, int>> reliableDictionary4Interface = new(); // Hungarian suffix kept for consistency with SUT parameter name
        readonly ITransaction tx = Mock.Of<ITransaction>();
        readonly string key = fuzzy.String();

        [Fact]
        public void ForwardsTransactionAndKeyToReliableDictionaryWithDefaultTimeoutAndCancellation()
        {
            Task<bool> expected = new TaskCompletionSource<bool>().Task;
            _ = reliableDictionary4Interface
                .Setup(_ => _.RemoveAsync(tx, key, TimeSpan.FromSeconds(4), CancellationToken.None))
                .Returns(expected);

            Task<bool> actual = reliableDictionary4Interface.Object.RemoveAsync(tx, key);

            Assert.Same(expected, actual);
            reliableDictionary4Interface.Verify(
                _ => _.RemoveAsync(It.IsAny<ITransaction>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Extension method dereferences reliableDictionary4Interface unconditionally, producing NullReferenceException instead of ArgumentNullException.
        public void ThrowsArgumentNullExceptionWhenReliableDictionary4InterfaceIsNull()
        {
            // The SUT extension method synchronously dereferences its receiver to invoke RemoveAsync on it,
            // so calling it on a null reference throws NullReferenceException instead of the ArgumentNullException
            // expected from a guard clause. This test stays explicit until the SUT adds an explicit null check
            // on the reliableDictionary4Interface parameter.
            var e = Assert.Throws<ArgumentNullException>(() => { _ = ((IReliableDictionary4<string, int>)null).RemoveAsync(tx, key); });

            Assert.Equal(nameof(reliableDictionary4Interface), e.ParamName);
        }
    }
}
