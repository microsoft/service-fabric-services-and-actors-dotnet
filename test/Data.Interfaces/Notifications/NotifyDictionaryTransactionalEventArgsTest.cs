// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyDictionaryTransactionalEventArgsTest
{
    readonly NotifyDictionaryTransactionalEventArgs<string, int> sut;

    // Constructor parameters
    readonly ITransaction transaction = Mock.Of<ITransaction>();
    readonly NotifyDictionaryChangedAction action = fuzzy.Enum<NotifyDictionaryChangedAction>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    NotifyDictionaryTransactionalEventArgsTest() =>
        sut = new TestArgs(transaction, action);

    public sealed class Constructor : NotifyDictionaryTransactionalEventArgsTest
    {
        [Fact]
        public void SetsTransaction() =>
            Assert.Same(transaction, sut.Transaction);

        [Theory]
        [InlineData(NotifyDictionaryChangedAction.Add)]
        [InlineData(NotifyDictionaryChangedAction.Update)]
        [InlineData(NotifyDictionaryChangedAction.Remove)]
        [InlineData(NotifyDictionaryChangedAction.Clear)]
        [InlineData(NotifyDictionaryChangedAction.Rebuild)]
        public void ForwardsActionToBaseClass(NotifyDictionaryChangedAction action) =>
            Assert.Equal(action, new TestArgs(transaction, action).Action);

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate; consumers dereferencing Transaction will NRE.
        public void ThrowsArgumentNullExceptionWhenTransactionIsNull()
        {
            // The constructor stores the transaction argument verbatim without a null check, so passing null
            // succeeds here and the NullReferenceException only surfaces later when a consumer dereferences
            // Transaction. Validating the argument up front would fail fast at the call site that supplied null.
            var e = Assert.Throws<ArgumentNullException>(() => new TestArgs(null, action));
            Assert.Equal(sut.Constructor().Parameter<ITransaction>().Name, e.ParamName);
        }
    }

    sealed class TestArgs(ITransaction transaction, NotifyDictionaryChangedAction action)
        : NotifyDictionaryTransactionalEventArgs<string, int>(transaction, action);
}
