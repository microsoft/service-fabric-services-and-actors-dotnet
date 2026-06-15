// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyDictionaryItemRemovedEventArgsTest
{
    readonly NotifyDictionaryItemRemovedEventArgs<string, int> sut;

    // Constructor parameters
    readonly ITransaction transaction = Mock.Of<ITransaction>();
    readonly string key = fuzzy.String();

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    NotifyDictionaryItemRemovedEventArgsTest() =>
        sut = new NotifyDictionaryItemRemovedEventArgs<string, int>(transaction, key);

    public sealed class Constructor : NotifyDictionaryItemRemovedEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(transaction, sut.Transaction);
            Assert.Same(key, sut.Key);
            Assert.Equal(NotifyDictionaryChangedAction.Remove, sut.Action);
        }
    }
}
