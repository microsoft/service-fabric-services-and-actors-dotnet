// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyTransactionChangedEventArgsTest
{
    readonly ITransaction transaction = Mock.Of<ITransaction>();

    public sealed class Constructor : NotifyTransactionChangedEventArgsTest
    {
        [Theory, InlineData(NotifyTransactionChangedAction.Commit)] // single-item enum
        public void InitializesProperties(NotifyTransactionChangedAction action)
        {
            NotifyTransactionChangedEventArgs sut = new(transaction, action);

            Assert.Same(transaction, sut.Transaction);
            Assert.Equal(action, sut.Action);
        }

        [Theory(Explicit = true), InlineData(NotifyTransactionChangedAction.Commit)] // TODO: SUT bug. Constructor doesn't validate; consumers dereferencing Transaction will NRE.
        public void ThrowsArgumentNullExceptionWhenTransactionIsNull(NotifyTransactionChangedAction action)
        {
            // The constructor stores the transaction argument verbatim without a null check, so passing null
            // succeeds here and the NullReferenceException only surfaces later when a consumer dereferences
            // Transaction. Validating the argument up front would fail fast at the call site that supplied null.
            var e = Assert.Throws<ArgumentNullException>(() => new NotifyTransactionChangedEventArgs(null, action));
            Assert.Equal(nameof(transaction), e.ParamName);
        }
    }
}
