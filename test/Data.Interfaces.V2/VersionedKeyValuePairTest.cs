// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class VersionedKeyValuePairTest
{
    readonly VersionedKeyValuePair<string, Uri> sut;

    // Constructor parameters
    readonly string key = fuzzy.String();
    readonly Uri value = fuzzy.Uri();
    readonly long sequenceNumber = fuzzy.Int64();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    VersionedKeyValuePairTest() => sut = new(key, value, sequenceNumber);

    public sealed class Constructor : VersionedKeyValuePairTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(value, sut.Value);
            Assert.Same(key, sut.VersionedKey.Key);
            Assert.Equal(sequenceNumber, sut.VersionedKey.SequenceNumber);
        }
    }

    public sealed class Key : VersionedKeyValuePairTest
    {
        [Fact]
        public void ReturnsKeyPassedToConstructor() => Assert.Same(key, sut.Key);
    }

    public sealed class KeyValuePair : VersionedKeyValuePairTest
    {
        [Fact]
        public void ReturnsPairOfKeyAndValuePassedToConstructor()
        {
            KeyValuePair<string, Uri> actual = sut.KeyValuePair;
            Assert.Same(key, actual.Key);
            Assert.Same(value, actual.Value);
        }
    }

    public sealed class SequenceNumber : VersionedKeyValuePairTest
    {
        [Fact]
        public void ReturnsSequenceNumberPassedToConstructor() => Assert.Equal(sequenceNumber, sut.SequenceNumber);
    }
}
