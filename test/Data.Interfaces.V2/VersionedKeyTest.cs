// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class VersionedKeyTest
{
    readonly VersionedKey<string> sut;

    // Constructor parameters
    readonly string key = fuzzy.String();
    readonly long sequenceNumber = fuzzy.Int64();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    VersionedKeyTest() => sut = new(key, sequenceNumber);

    public sealed class Constructor : VersionedKeyTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(key, sut.Key);
            Assert.Equal(sequenceNumber, sut.SequenceNumber);
        }
    }
}
