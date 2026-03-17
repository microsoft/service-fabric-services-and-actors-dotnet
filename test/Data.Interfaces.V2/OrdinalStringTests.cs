// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace System.Fabric.ReplicatorStack.Test
{
    using Microsoft.ServiceFabric.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    class OrdinalStringTests
    {
        [TestMethod]
        public void ToString_OrdinalString_ReturnsSameString()
        {
            string expected = "café";
            var sut = new OrdinalString(expected);
            string actual = sut.ToString();
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void ImplicitConversionToOrdinalString_String_ReturnsEqualOrdinalString()
        {
            var expected = new OrdinalString("café");
            string sut = "café";
            OrdinalString actual = sut;
            Assert.AreSame(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void ExplicitConversionToString_OrdinalString_ReturnsSameString()
        {
            string expected = "café";
            var sut = new OrdinalString(expected);
            var actual = (string)sut;

            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void StaticEquals_DifferentValues_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsFalse(OrdinalString.Equals(left, right));
        }

        [TestMethod]
        public void StaticEquals_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsTrue(OrdinalString.Equals(left, right));
        }

        [TestMethod]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsFalse(left.Equals(right));
        }

        [TestMethod]
        public void Equals_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsTrue(left.Equals(right));
        }

        [TestMethod]
        public void ObjectEquals_OneNonOrdinalStringType_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            object right = default;
            Assert.IsFalse(left.Equals(right));
        }

        [TestMethod]
        public void ObjectEquals_DifferentValuesObjectType_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            object right = new OrdinalString("cafe\u0301");
            Assert.IsFalse(left.Equals(right));
        }

        [TestMethod]
        public void ObjectEquals_EqualValueObjectType_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            object right = new OrdinalString("café");
            Assert.IsTrue(left.Equals(right));
        }

        [TestMethod]
        public void EqualsOperator_DifferentValues_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsFalse(left == right);
        }

        [TestMethod]
        public void EqualsOperator_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsTrue(left == right);
        }

        [TestMethod]
        public void NotEqualsOperator_DifferentValues_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsTrue(left != right);
        }

        [TestMethod]
        public void NotEqualsOperator_EqualValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsFalse(left != right);
        }

        [TestMethod]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
        {
            var left = new OrdinalString("café");
            string right = "cafe\u0301";
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_EqualValue_ReturnsEqualHashCode()
        {
            var left = new OrdinalString("café");
            string right = "café";
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }

        [TestMethod]
        public void CompareTo_LargerLeftValue_ReturnsPostive()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsTrue(left.CompareTo(right) > 0);
        }

        [TestMethod]
        public void CompareTo_LargerRightValue_ReturnsNegative()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.IsTrue(left.CompareTo(right) < 0);
        }

        [TestMethod]
        public void CompareTo_EqualValue_ReturnsZero()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsTrue(left.CompareTo(right) == 0);
        }

        [TestMethod]
        public void GreaterThanOperator_LargerLeftValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsTrue(left > right);
        }

        [TestMethod]
        public void GreaterThanOperator_LargerRightValue_ReturnsFalse()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.IsFalse(left > right);
        }

        [TestMethod]
        public void GreaterThanOperator_EqualValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsFalse(left > right);
        }

        [TestMethod]
        public void GreaterThanOrEqualOperator_LargerLeftValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsTrue(left >= right);
        }

        [TestMethod]
        public void GreaterThanOrEqualOperator_LargerRightValue_ReturnsFalse()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.IsFalse(left >= right);
        }

        [TestMethod]
        public void GreaterThanOrEqualOperator_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsTrue(left >= right);
        }

        [TestMethod]
        public void LessThanOperator_LargerLeftValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsFalse(left < right);
        }

        [TestMethod]
        public void LessThanOperator_LargerRightValue_ReturnsTrue()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.IsTrue(left < right);
        }

        [TestMethod]
        public void LessThanOperator_EqualValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsFalse(left < right);
        }

        [TestMethod]
        public void LessThanOrEqualOperator_LargerLeftValue_ReturnsFalse()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("cafe\u0301");
            Assert.IsFalse(left <= right);
        }

        [TestMethod]
        public void LessThanOrEqualOperator_LargerRightValue_ReturnsTrue()
        {
            var left = new OrdinalString("cafe\u0301");
            var right = new OrdinalString("café");
            Assert.IsTrue(left <= right);
        }

        [TestMethod]
        public void LessThanOrEqualOperator_EqualValue_ReturnsTrue()
        {
            var left = new OrdinalString("café");
            var right = new OrdinalString("café");
            Assert.IsTrue(left <= right);
        }
    }
}
