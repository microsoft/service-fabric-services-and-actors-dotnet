// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class OrdinalStringTest
{
    readonly OrdinalString sut;

    // Constructor parameters
    readonly string value = fuzzy.String();

    // Test fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    static readonly string precomposed = "caf\u00e9";
    static readonly string decomposed = "cafe\u0301";

    OrdinalStringTest() =>
        sut = new OrdinalString(value);

    public new sealed class ToString : OrdinalStringTest
    {
        [Fact]
        public void ReturnsValueGivenToConstructor() =>
            Assert.Same(value, sut.ToString());
    }

    public sealed class ImplicitConversion : OrdinalStringTest
    {
        [Fact]
        public void CreatesOrdinalStringWithSameValue()
        {
            OrdinalString result = value;
            Assert.Same(value, result.ToString());
        }
    }

    public sealed class ExplicitConversion : OrdinalStringTest
    {
        [Fact]
        public void ReturnsValueGivenToConstructor() =>
            Assert.Same(value, (string)sut);
    }

    public new sealed class Equals : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueForEqualOrdinalString() =>
            Assert.True(sut.Equals(new OrdinalString(value)));

        [Fact]
        public void ReturnsFalseForOrdinallyDifferentOrdinalString() =>
            Assert.False(new OrdinalString(precomposed).Equals(new OrdinalString(decomposed)));

        [Fact]
        public void ReturnsTrueForEqualBoxedOrdinalString() =>
            Assert.True(sut.Equals((object)new OrdinalString(value)));

        [Fact]
        public void ReturnsFalseForOrdinallyDifferentBoxedOrdinalString() =>
            Assert.False(new OrdinalString(precomposed).Equals((object)new OrdinalString(decomposed)));

        [Fact]
        public void ReturnsFalseForNull() =>
            Assert.False(sut.Equals(default(object)));
    }

    public sealed class EqualityOperator : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueForEqualValues() =>
            Assert.True(new OrdinalString(value) == sut);

        [Fact]
        public void ReturnsFalseForOrdinallyDifferentValues() =>
            Assert.False(new OrdinalString(precomposed) == new OrdinalString(decomposed));
    }

    public sealed class InequalityOperator : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueForOrdinallyDifferentValues() =>
            Assert.True(new OrdinalString(precomposed) != new OrdinalString(decomposed));

        [Fact]
        public void ReturnsFalseForEqualValues() =>
            Assert.False(new OrdinalString(value) != sut);
    }

    public new sealed class GetHashCode : OrdinalStringTest
    {
        [Fact]
        public void ReturnsSameHashCodeForEqualValues() =>
            Assert.Equal(sut.GetHashCode(), new OrdinalString(value).GetHashCode());

        [Fact]
        public void ReturnsDifferentHashCodeForOrdinallyDifferentValues() =>
            Assert.NotEqual(new OrdinalString(precomposed).GetHashCode(), new OrdinalString(decomposed).GetHashCode());
    }

    public sealed class CompareTo : OrdinalStringTest
    {
        [Fact]
        public void ReturnsZeroForEqualValues() =>
            Assert.Equal(0, sut.CompareTo(new OrdinalString(value)));

        [Fact]
        public void ReturnsPositiveWhenLeftIsGreater() =>
            Assert.True(new OrdinalString(precomposed).CompareTo(new OrdinalString(decomposed)) > 0);

        [Fact]
        public void ReturnsNegativeWhenLeftIsSmaller() =>
            Assert.True(new OrdinalString(decomposed).CompareTo(new OrdinalString(precomposed)) < 0);
    }

    public sealed class GreaterThanOperator : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueWhenLeftIsGreater() =>
            Assert.True(new OrdinalString(precomposed) > new OrdinalString(decomposed));

        [Fact]
        public void ReturnsFalseWhenLeftIsSmaller() =>
            Assert.False(new OrdinalString(decomposed) > new OrdinalString(precomposed));

        [Fact]
        public void ReturnsFalseForEqualValues() =>
            Assert.False(new OrdinalString(value) > sut);
    }

    public sealed class LessThanOperator : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueWhenLeftIsSmaller() =>
            Assert.True(new OrdinalString(decomposed) < new OrdinalString(precomposed));

        [Fact]
        public void ReturnsFalseWhenLeftIsGreater() =>
            Assert.False(new OrdinalString(precomposed) < new OrdinalString(decomposed));

        [Fact]
        public void ReturnsFalseForEqualValues() =>
            Assert.False(new OrdinalString(value) < sut);
    }

    public sealed class GreaterThanOrEqualOperator : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueWhenLeftIsGreater() =>
            Assert.True(new OrdinalString(precomposed) >= new OrdinalString(decomposed));

        [Fact]
        public void ReturnsFalseWhenLeftIsSmaller() =>
            Assert.False(new OrdinalString(decomposed) >= new OrdinalString(precomposed));

        [Fact]
        public void ReturnsTrueForEqualValues() =>
            Assert.True(new OrdinalString(value) >= sut);
    }

    public sealed class LessThanOrEqualOperator : OrdinalStringTest
    {
        [Fact]
        public void ReturnsTrueWhenLeftIsSmaller() =>
            Assert.True(new OrdinalString(decomposed) <= new OrdinalString(precomposed));

        [Fact]
        public void ReturnsFalseWhenLeftIsGreater() =>
            Assert.False(new OrdinalString(precomposed) <= new OrdinalString(decomposed));

        [Fact]
        public void ReturnsTrueForEqualValues() =>
            Assert.True(new OrdinalString(value) <= sut);
    }
}
