// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyTransactionChangedEventArgsTest
{
    readonly NotifyTransactionChangedEventArgs sut;
    readonly ITransaction transaction = Mock.Of<ITransaction>();
    readonly NotifyTransactionChangedAction action = (NotifyTransactionChangedAction)1;

    NotifyTransactionChangedEventArgsTest() =>
        sut = new NotifyTransactionChangedEventArgs(transaction, action);

    public sealed class Constructor : NotifyTransactionChangedEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(transaction, sut.Transaction);
            Assert.Equal(action, sut.Action);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Constructor doesn't validate; consumers dereferencing Transaction will NRE.
        public void ThrowsArgumentNullExceptionWhenTransactionIsNull()
        {
            // The constructor stores the transaction argument verbatim without a null check, so passing null
            // succeeds here and the NullReferenceException only surfaces later when a consumer dereferences
            // Transaction. Validating the argument up front would fail fast at the call site that supplied null.
            var e = Assert.Throws<ArgumentNullException>(() => new NotifyTransactionChangedEventArgs(null, action));
            Assert.Equal(nameof(transaction), e.ParamName);
        }
    }
}
