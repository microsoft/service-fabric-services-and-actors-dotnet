// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyDictionaryItemAddedEventArgsTest
{
    readonly NotifyDictionaryItemAddedEventArgs<string, int> sut;

    // Constructor parameters
    readonly ITransaction transaction = Mock.Of<ITransaction>();
    readonly string key = fuzzy.String();
    readonly int value = fuzzy.Int32();

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    NotifyDictionaryItemAddedEventArgsTest() =>
        sut = new NotifyDictionaryItemAddedEventArgs<string, int>(transaction, key, value);

    public sealed class Constructor : NotifyDictionaryItemAddedEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(transaction, sut.Transaction);
            Assert.Same(key, sut.Key);
            Assert.Equal(value, sut.Value);
            Assert.Equal(NotifyDictionaryChangedAction.Add, sut.Action);
        }
    }
}
