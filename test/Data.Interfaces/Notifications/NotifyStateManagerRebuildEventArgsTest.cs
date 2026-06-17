// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyStateManagerRebuildEventArgsTest
{
    readonly NotifyStateManagerRebuildEventArgs sut;
    readonly IAsyncEnumerable<IReliableState> reliableStates = Mock.Of<IAsyncEnumerable<IReliableState>>();

    NotifyStateManagerRebuildEventArgsTest() =>
        sut = new NotifyStateManagerRebuildEventArgs(reliableStates);

    public sealed class Constructor : NotifyStateManagerRebuildEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(reliableStates, sut.ReliableStates);
            Assert.Equal(NotifyStateManagerChangedAction.Rebuild, sut.Action);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate; consumers enumerating ReliableStates will NRE.
        public void ThrowsArgumentNullExceptionWhenReliableStatesIsNull()
        {
            // The constructor stores the reliableStates argument verbatim without a null check, so passing null
            // succeeds here and the NullReferenceException only surfaces later when a consumer enumerates
            // ReliableStates. Validating the argument up front would fail fast at the call site that supplied null.
            var e = Assert.Throws<ArgumentNullException>(() => new NotifyStateManagerRebuildEventArgs(null));
            Assert.Equal(sut.Constructor().Parameter<IAsyncEnumerable<IReliableState>>().Name, e.ParamName);
        }
    }
}
