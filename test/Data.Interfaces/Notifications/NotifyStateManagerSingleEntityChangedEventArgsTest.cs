// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyStateManagerSingleEntityChangedEventArgsTest
{
    readonly NotifyStateManagerSingleEntityChangedEventArgs sut;

    // Constructor parameters
    readonly ITransaction transaction = Mock.Of<ITransaction>();
    readonly IReliableState reliableState = Mock.Of<IReliableState>();
    readonly NotifyStateManagerChangedAction action = fuzzy.Enum<NotifyStateManagerChangedAction>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    NotifyStateManagerSingleEntityChangedEventArgsTest() =>
        sut = new NotifyStateManagerSingleEntityChangedEventArgs(transaction, reliableState, action);

    public sealed class Constructor : NotifyStateManagerSingleEntityChangedEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(transaction, sut.Transaction);
            Assert.Same(reliableState, sut.ReliableState);
        }

        [Theory]
        [InlineData(NotifyStateManagerChangedAction.Add)]
        [InlineData(NotifyStateManagerChangedAction.Remove)]
        [InlineData(NotifyStateManagerChangedAction.Rebuild)]
        public void ForwardsActionToBase(NotifyStateManagerChangedAction action) =>
            Assert.Equal(action, new NotifyStateManagerSingleEntityChangedEventArgs(transaction, reliableState, action).Action);

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate; consumers dereferencing Transaction will NRE.
        public void ThrowsArgumentNullExceptionWhenTransactionIsNull()
        {
            // The constructor stores the transaction argument verbatim without a null check, so passing null
            // succeeds here and the NullReferenceException only surfaces later when a consumer dereferences
            // Transaction. Validating the argument up front would fail fast at the call site that supplied null.
            var e = Assert.Throws<ArgumentNullException>(() => new NotifyStateManagerSingleEntityChangedEventArgs(null, reliableState, action));
            Assert.Equal(nameof(transaction), e.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate; consumers dereferencing ReliableState will NRE.
        public void ThrowsArgumentNullExceptionWhenReliableStateIsNull()
        {
            // The constructor stores the reliableState argument verbatim without a null check, so passing null
            // succeeds here and the NullReferenceException only surfaces later when a consumer dereferences
            // ReliableState. Validating the argument up front would fail fast at the call site that supplied null.
            var e = Assert.Throws<ArgumentNullException>(() => new NotifyStateManagerSingleEntityChangedEventArgs(transaction, null, action));
            Assert.Equal(nameof(reliableState), e.ParamName);
        }
    }
}
