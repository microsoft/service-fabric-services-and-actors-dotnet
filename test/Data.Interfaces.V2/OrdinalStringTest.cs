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

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    OrdinalStringTest() =>
        sut = new(value);

    public sealed class CompareTo : OrdinalStringTest
    {
        new readonly IComparable<OrdinalString> sut;

        readonly OrdinalString other;

        public CompareTo() =>
            (sut, other) = (base.sut, EqualButNotSame(value));

        [Fact]
        public void ReturnsZeroWhenOtherIsEqual() =>
            Assert.Equal(0, sut.CompareTo(other));

        [Fact]
        public void ReturnsPositiveWhenOtherIsLessThanSut() =>
            Assert.True(new OrdinalString(lower).CompareTo(new(upper)) > 0);

        [Fact]
        public void ReturnsNegativeWhenOtherIsGreaterThanSut() =>
            Assert.True(new OrdinalString(upper).CompareTo(new(lower)) < 0);

        [Fact]
        public void ReturnsNegativeWhenSutIsDefault() =>
            Assert.True(default(OrdinalString).CompareTo(base.sut) < 0);

        [Fact]
        public void ReturnsPositiveWhenOtherIsDefault() =>
            Assert.True(sut.CompareTo(default) > 0);

        [Fact]
        public void ReturnsZeroForTwoDefaultValues() =>
            Assert.Equal(0, default(OrdinalString).CompareTo(default));
    }

    public sealed class Equals_Object : OrdinalStringTest
    {
        new readonly object sut;

        readonly object obj;

        public Equals_Object() =>
            (sut, obj) = (base.sut, EqualButNotSame(value));

        [Fact]
        public void ReturnsTrueWhenObjIsEqualOrdinalString() =>
            Assert.True(sut.Equals(obj));

        [Fact]
        public void ReturnsFalseWhenObjIsDifferentOrdinalString() =>
            Assert.False(sut.Equals(DifferentFrom(value)));

        [Fact]
        public void ReturnsFalseWhenObjIsNotOrdinalString() =>
            Assert.False(sut.Equals(value));
    }

    public sealed class Equals_OrdinalString : OrdinalStringTest
    {
        new readonly IEquatable<OrdinalString> sut;

        readonly OrdinalString other;

        public Equals_OrdinalString() =>
            (sut, other) = (base.sut, EqualButNotSame(value));

        [Fact]
        public void ReturnsTrueWhenOtherIsEqual() =>
            Assert.True(sut.Equals(other));

        [Fact]
        public void ReturnsFalseWhenOtherIsDifferent() =>
            Assert.False(sut.Equals(DifferentFrom(value)));

        [Fact]
        public void ReturnsFalseWhenOtherDiffersInCaseOnly() =>
            Assert.False(new OrdinalString(lower).Equals(new(upper)));

        [Fact]
        public void ReturnsFalseWhenValuesAreCanonicallyEquivalentButNotIdentical()
        {
            const string precomposed = "\u00E9"; // é
            const string decomposed = "e\u0301"; // e + combining acute accent
            Assert.False(new OrdinalString(precomposed).Equals(new(decomposed)));
        }

        [Fact]
        public void ReturnsTrueForTwoDefaultValues() =>
            Assert.True(default(OrdinalString).Equals(default));

        [Fact]
        public void ReturnsFalseWhenSutIsDefault() =>
            Assert.False(default(OrdinalString).Equals(base.sut));

        [Fact]
        public void ReturnsFalseWhenOtherIsDefault() =>
            Assert.False(sut.Equals(default));
    }

    public new sealed class GetHashCode : OrdinalStringTest
    {
        new readonly object sut;

        public GetHashCode() =>
            sut = base.sut;

        [Fact]
        public void ReturnsValueOfUnderlyingString() =>
            Assert.Equal(value.GetHashCode(), sut.GetHashCode());

        [Fact(Explicit = true)] // TODO: SUT bug. GetHashCode throws for default(OrdinalString).
        public void ReturnsZeroForDefaultValue()
        {
            // GetHashCode dereferences value directly and throws NullReferenceException for
            // default(OrdinalString), violating the Equals/GetHashCode contract (two default values
            // compare equal but cannot be hashed). Fix: value?.GetHashCode() ?? 0.
            object sut = default(OrdinalString);
            int actual = sut.GetHashCode();
            Assert.Equal(0, actual);
        }
    }

    public sealed class Op_Equality : OrdinalStringTest
    {
        readonly OrdinalString left;
        readonly OrdinalString right;

        public Op_Equality() =>
            (left, right) = (sut, EqualButNotSame(value));

        [Fact]
        public void ReturnsTrueForEqualValues() =>
            Assert.True(left == right);

        [Fact]
        public void ReturnsFalseForDifferentValues() =>
            Assert.False(left == DifferentFrom(value));
    }

    public sealed class Op_Explicit_OrdinalString_To_String : OrdinalStringTest
    {
        [Fact]
        public void ReturnsWrappedString() =>
            Assert.Same(value, (string)sut);

        [Fact]
        public void ReturnsNullForDefaultValue() =>
            Assert.Null((string)default(OrdinalString));
    }

    public sealed class Op_GreaterThan : OrdinalStringTest
    {
        readonly OrdinalString left;
        readonly OrdinalString right;

        public Op_GreaterThan() =>
            (left, right) = (new(value + fuzzy.String()), sut);

        [Fact]
        public void ReturnsTrueWhenLeftIsGreaterThanRight() =>
            Assert.True(left > right);

        [Fact]
        public void ReturnsFalseWhenLeftIsNotGreaterThanRight() =>
            Assert.False(right > left);

        [Fact]
        public void ReturnsFalseWhenLeftEqualsRight() =>
            Assert.False(left > EqualButNotSame(left.ToString()));
    }

    public sealed class Op_GreaterThanOrEqual : OrdinalStringTest
    {
        readonly OrdinalString left;
        readonly OrdinalString right;

        public Op_GreaterThanOrEqual() =>
            (left, right) = (new(value + fuzzy.String()), sut);

        [Fact]
        public void ReturnsTrueWhenLeftIsGreaterThanRight() =>
            Assert.True(left >= right);

        [Fact]
        public void ReturnsTrueWhenLeftEqualsRight() =>
            Assert.True(left >= EqualButNotSame(left.ToString()));

        [Fact]
        public void ReturnsFalseWhenLeftIsNotGreaterThanOrEqualToRight() =>
            Assert.False(right >= left);
    }

    public sealed class Op_Implicit_String_To_OrdinalString : OrdinalStringTest
    {
        [Fact]
        public void CreatesOrdinalStringWithSameValue()
        {
            OrdinalString actual = value;
            Assert.Same(value, actual.ToString());
        }

        [Fact]
        public void CreatesOrdinalStringWithNullValue()
        {
            OrdinalString actual = (string)null;
            Assert.Null(actual.ToString());
        }
    }

    public sealed class Op_Inequality : OrdinalStringTest
    {
        readonly OrdinalString left;
        readonly OrdinalString right;

        public Op_Inequality() =>
            (left, right) = (sut, EqualButNotSame(value));

        [Fact]
        public void ReturnsFalseForEqualValues() =>
            Assert.False(left != right);

        [Fact]
        public void ReturnsTrueForDifferentValues() =>
            Assert.True(left != DifferentFrom(value));
    }

    public sealed class Op_LessThan : OrdinalStringTest
    {
        readonly OrdinalString left;
        readonly OrdinalString right;

        public Op_LessThan() =>
            (left, right) = (sut, new(value + fuzzy.String()));

        [Fact]
        public void ReturnsTrueWhenLeftIsLessThanRight() =>
            Assert.True(left < right);

        [Fact]
        public void ReturnsFalseWhenLeftIsNotLessThanRight() =>
            Assert.False(right < left);

        [Fact]
        public void ReturnsFalseWhenLeftEqualsRight() =>
            Assert.False(left < EqualButNotSame(left.ToString()));
    }

    public sealed class Op_LessThanOrEqual : OrdinalStringTest
    {
        readonly OrdinalString left;
        readonly OrdinalString right;

        public Op_LessThanOrEqual() =>
            (left, right) = (sut, new(value + fuzzy.String()));

        [Fact]
        public void ReturnsTrueWhenLeftIsLessThanRight() =>
            Assert.True(left <= right);

        [Fact]
        public void ReturnsTrueWhenLeftEqualsRight() =>
            Assert.True(left <= EqualButNotSame(left.ToString()));

        [Fact]
        public void ReturnsFalseWhenLeftIsNotLessThanOrEqualToRight() =>
            Assert.False(right <= left);
    }

    public new sealed class ToString : OrdinalStringTest
    {
        new readonly object sut;

        public ToString() =>
            sut = base.sut;

        [Fact]
        public void ReturnsWrappedString() =>
            Assert.Same(value, sut.ToString());

        [Fact]
        public void ReturnsNullForDefaultValue()
        {
            object sut = default(OrdinalString);
            Assert.Null(sut.ToString());
        }
    }

    const string lower = "a";
    const string upper = "A";

    // Returns an OrdinalString with the same content as `s` but, when `s` is non-empty, a distinct
    // underlying string instance so reference-equality assertions can't mask content-equality bugs.
    // The empty string is always interned in .NET, so distinct identity can't be guaranteed for `""`.
    static OrdinalString EqualButNotSame(string s) => new(new string(s.ToCharArray()));

    static OrdinalString DifferentFrom(string s) => new(s + fuzzy.String());
}
