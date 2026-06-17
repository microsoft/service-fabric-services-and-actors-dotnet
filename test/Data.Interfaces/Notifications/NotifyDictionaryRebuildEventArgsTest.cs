// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Inspector;
using Moq;
using Xunit;
using IAsyncEnumerable = Microsoft.ServiceFabric.Data.IAsyncEnumerable<System.Collections.Generic.KeyValuePair<string, int>>;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyDictionaryRebuildEventArgsTest
{
    readonly NotifyDictionaryRebuildEventArgs<string, int> sut;
    readonly IAsyncEnumerable enumerableState = Mock.Of<IAsyncEnumerable>(); // Consistency with SUT parameter name

    NotifyDictionaryRebuildEventArgsTest() =>
        sut = new NotifyDictionaryRebuildEventArgs<string, int>(enumerableState);

    public sealed class Constructor : NotifyDictionaryRebuildEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(enumerableState, sut.State);
            Assert.Equal(NotifyDictionaryChangedAction.Rebuild, sut.Action);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate; consumers enumerating State will NRE.
        public void ThrowsArgumentNullExceptionWhenEnumerableStateIsNull()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new NotifyDictionaryRebuildEventArgs<string, int>(null));
            Assert.Equal(sut.Constructor().Parameter<IAsyncEnumerable>().Name, e.ParamName);
        }
    }
}
