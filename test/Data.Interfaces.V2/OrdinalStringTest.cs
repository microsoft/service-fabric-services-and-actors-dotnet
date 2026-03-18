// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

namespace Microsoft.ServiceFabric.Data
{
    public class OrdinalStringTest
    {
        [Fact]
        public void ToString_OrdinalString_ReturnsSameString()
        {
            string expected = "café";
            var sut = new OrdinalString(expected);
            string actual = sut.ToString();
            Assert.Same(expected, actual);
        }

        [Fact]
        public void ImplicitConversionToOrdinalString_String_ReturnsEqualOrdinalString()
        {
            var expected = new OrdinalString("café");
            string sut = "café";
            OrdinalString actual = sut;
            Assert.Same(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void ExplicitConversionToString_OrdinalString_ReturnsSameString()
        {
            string expected = "café";
            var sut = new OrdinalString(expected);
            var actual = (string)sut;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void StaticEquals_DifferentValues_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.False(OrdinalString.Equals(left, right));
        }

        [Fact]
        public void StaticEquals_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.True(OrdinalString.Equals(left, right));
        }

        [Fact]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.False(left.Equals(right));
        }

        [Fact]
        public void Equals_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.True(left.Equals(right));
        }

        [Fact]
        public void ObjectEquals_OneNonOrdinalStringType_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            object right = default;
            Assert.False(left.Equals(right));
        }

        [Fact]
        public void ObjectEquals_DifferentValuesObjectType_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            object right = new OrdinalString("cafe\u0301");
            Assert.False(left.Equals(right));
        }

        [Fact]
        public void ObjectEquals_EqualValueObjectType_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            object right = new OrdinalString("café");
            Assert.True(left.Equals(right));
        }

        [Fact]
        public void EqualsOperator_DifferentValues_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.False(left == right);
        }

        [Fact]
        public void EqualsOperator_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.True(left == right);
        }

        [Fact]
        public void NotEqualsOperator_DifferentValues_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.True(left != right);
        }

        [Fact]
        public void NotEqualsOperator_EqualValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.False(left != right);
        }

        [Fact]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
        {
            var left = new OrdinalString("café");
            string right = "cafe\u0301";
            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void GetHashCode_EqualValue_ReturnsEqualHashCode()
        {
            var left = new OrdinalString("café");
            string right = "café";
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void CompareTo_LargerLeftValue_ReturnsPostive()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.True(left.CompareTo(right) > 0);
        }

        [Fact]
        public void CompareTo_LargerRightValue_ReturnsNegative()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.True(left.CompareTo(right) < 0);
        }

        [Fact]
        public void CompareTo_EqualValue_ReturnsZero()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.True(left.CompareTo(right) == 0);
        }

        [Fact]
        public void GreaterThanOperator_LargerLeftValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.True(left > right);
        }

        [Fact]
        public void GreaterThanOperator_LargerRightValue_ReturnsFalse()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.False(left > right);
        }

        [Fact]
        public void GreaterThanOperator_EqualValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.False(left > right);
        }

        [Fact]
        public void GreaterThanOrEqualOperator_LargerLeftValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.True(left >= right);
        }

        [Fact]
        public void GreaterThanOrEqualOperator_LargerRightValue_ReturnsFalse()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.False(left >= right);
        }

        [Fact]
        public void GreaterThanOrEqualOperator_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.True(left >= right);
        }

        [Fact]
        public void LessThanOperator_LargerLeftValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.False(left < right);
        }

        [Fact]
        public void LessThanOperator_LargerRightValue_ReturnsTrue()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.True(left < right);
        }

        [Fact]
        public void LessThanOperator_EqualValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.False(left < right);
        }

        [Fact]
        public void LessThanOrEqualOperator_LargerLeftValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.False(left <= right);
        }

        [Fact]
        public void LessThanOrEqualOperator_LargerRightValue_ReturnsTrue()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.True(left <= right);
        }

        [Fact]
        public void LessThanOrEqualOperator_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.True(left <= right);
        }
    }
}
