// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyDictionaryClearEventArgsTest
{
    readonly NotifyDictionaryClearEventArgs<string, int> sut;

    // Constructor parameters
    readonly long commitSequenceNumber = fuzzy.Int64();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    NotifyDictionaryClearEventArgsTest() =>
        sut = new NotifyDictionaryClearEventArgs<string, int>(commitSequenceNumber);

    public sealed class Constructor : NotifyDictionaryClearEventArgsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Equal(commitSequenceNumber, sut.CommitSequenceNumber);
            Assert.Equal(NotifyDictionaryChangedAction.Clear, sut.Action);
        }
    }
}
