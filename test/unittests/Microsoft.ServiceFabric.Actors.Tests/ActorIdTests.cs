// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Globalization;
    using Microsoft.ServiceFabric.Actors;
    using Xunit;

#pragma warning disable xUnit1024

    /// <summary>
    /// ActorId tests.
    /// </summary>
    public class ActorIdTests
    {
        private enum ExpectedComparisionResult
        {
            Less,
            Equals,
            More,
        }

        /// <summary>
        /// Tests two actor ids for null equality.
        /// </summary>
        [Fact]
        public void TestBothNullEquality()
        {
            ActorId x = null;
            ActorId y = null;

            Assert.True(x == y, "Verify null == null is true");
            Assert.False(x != y, "Verify null != null is false");
        }

        /// <summary>
        /// Tests ActorId for null equality.
        /// </summary>
        [Fact]
        public void TestNullEquality()
        {
            ActorId x = null;
            var y = new ActorId(1);

            Assert.False(x == y, "Verify null == ActorId(1) is false");
            Assert.True(x != y, "Verify null != ActorId(1) is false");

            Assert.False(y.Equals(x), "Verify ActorId(1).Equals(null) is false");
            Assert.True(y.CompareTo(x) != 0, "Verify ActorId(1).CompareTo(null) is not zero");
        }

        /// <summary>
        /// Tests ActorIds for equality.
        /// </summary>
        [Fact]
        public void TestEquality()
        {
            TestEqualityLong(0, 0);
            TestEqualityLong(long.MaxValue, long.MaxValue);
            TestEqualityLong(long.MinValue, long.MinValue);

            var g = Guid.NewGuid();
            TestEqualityGuid(g, new Guid(g.ToByteArray()));

            TestEqualityString(string.Empty, string.Empty);
            try
            {
                TestEqualityString((string)null, (string)null);
            }
            catch (ArgumentNullException)
            {
            }

            TestEqualityString("Id1", "Id1");
            TestEqualityString("0", "0");
        }

        /// <summary>
        /// Tests aCtorIds for Inequality.
        /// </summary>
        [Fact]
        public void TestInEquality()
        {
            TestInEqualityLong(long.MaxValue, long.MinValue);
            TestInEqualityLong(0, long.MinValue);
            TestInEqualityLong(long.MaxValue, 0);
            TestInEqualityGuid(Guid.NewGuid(), Guid.NewGuid());
            TestInEqualityString(string.Empty, "null");
            TestInEqualityString("1", "2");
            TestInEquality(new ActorId(1), new ActorId("1"));
            TestInEquality(new ActorId(Guid.NewGuid()), new ActorId(string.Empty));
            TestInEquality(new ActorId(long.MaxValue), new ActorId(Guid.Empty));
        }

        /// <summary>
        /// Tests ActorId comparisons.
        /// </summary>
        [Fact]
        public void TestCompareTo()
        {
            TestCompareTo(new ActorId(long.MaxValue), new ActorId(long.MinValue), ExpectedComparisionResult.More);
            TestCompareTo(new ActorId(long.MinValue), new ActorId(0), ExpectedComparisionResult.Less);
            TestCompareTo(new ActorId(0), new ActorId(0), ExpectedComparisionResult.Equals);

            TestCompareTo(new ActorId(Guid.Empty), new ActorId(Guid.NewGuid()), ExpectedComparisionResult.Less);
            TestCompareTo(new ActorId(Guid.NewGuid()), new ActorId(Guid.Empty), ExpectedComparisionResult.More);
            TestCompareTo(new ActorId(Guid.Empty), new ActorId(Guid.Empty), ExpectedComparisionResult.Equals);

            TestCompareTo(new ActorId("A"), new ActorId("B2"), ExpectedComparisionResult.Less);
            TestCompareTo(new ActorId("0"), new ActorId("-234"), ExpectedComparisionResult.More);

            TestCompareTo(new ActorId(Guid.Empty), new ActorId(Guid.Empty.ToString()), ExpectedComparisionResult.Less);
        }

        private static void TestEqualityLong(long lx, long ly)
        {
            TestEquality(new ActorId(lx), new ActorId(ly));
        }

        private static void TestInEqualityLong(long lx, long ly)
        {
            TestInEquality(new ActorId(lx), new ActorId(ly));
        }

        private static void TestEqualityGuid(Guid lx, Guid ly)
        {
            TestEquality(new ActorId(lx), new ActorId(ly));
        }

        private static void TestInEqualityGuid(Guid lx, Guid ly)
        {
            TestInEquality(new ActorId(lx), new ActorId(ly));
        }

        private static void TestEqualityString(string lx, string ly)
        {
            TestEquality(new ActorId(lx), new ActorId(ly));
        }

        private static void TestInEqualityString(string lx, string ly)
        {
            TestInEquality(new ActorId(lx), new ActorId(ly));
        }

        private static void TestEquality(ActorId x, ActorId y)
        {
            Assert.True(
                x == y,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} == {1} is true",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));

            Assert.False(
                x != y,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} != {1} is false",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));

            Assert.True(
                x.Equals(y),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} Equals {1} is true",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));

            var z = y as object;
            Assert.True(
               x.Equals(z),
               string.Format(
                   CultureInfo.InvariantCulture,
                   "Verify {0} Equals {1} as object is true",
                   ToStringWithKind(x),
                   z.ToString()));

            Assert.True(
                x.CompareTo(y) == 0,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} CompareTo {1} is zero",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));
        }

        private static void TestInEquality(ActorId x, ActorId y)
        {
            Assert.False(
                x == y,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} == {1} is false",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));

            Assert.True(
                x != y,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} != {1} is true",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));

            Assert.False(
                x.Equals(y),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} Equals {1} is false",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));

            var z = y as object;
            Assert.False(
               x.Equals(z),
               string.Format(
                   CultureInfo.InvariantCulture,
                   "Verify {0} Equals {1} as object is false",
                   ToStringWithKind(x),
                   z.ToString()));

            Assert.False(
                x.CompareTo(y) == 0,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Verify {0} CompareTo {1} is not zero",
                    ToStringWithKind(x),
                    ToStringWithKind(y)));
        }

        private static void TestCompareTo(ActorId x, ActorId y, ExpectedComparisionResult expected)
        {
            if (expected == ExpectedComparisionResult.Equals)
            {
                Assert.True(
                    x == y,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Verify {0} == {1} is true",
                        ToStringWithKind(x),
                        ToStringWithKind(y)));

                Assert.True(
                    x.CompareTo(y) == 0,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Verify {0} CompareTo {1} is zero",
                        ToStringWithKind(x),
                        ToStringWithKind(y)));
            }

            if (expected == ExpectedComparisionResult.Less)
            {
                Assert.True(
                    x != y,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Verify {0} != {1} is true",
                        ToStringWithKind(x),
                        ToStringWithKind(y)));

                Assert.True(
                    x.CompareTo(y) < 0,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Verify {0} CompareTo {1} is less than zero",
                        ToStringWithKind(x),
                        ToStringWithKind(y)));
            }

            if (expected == ExpectedComparisionResult.More)
            {
                Assert.True(
                    x != y,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Verify {0} != {1} is true",
                        ToStringWithKind(x),
                        ToStringWithKind(y)));

                Assert.True(
                    x.CompareTo(y) > 0,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Verify {0} CompareTo {1} is more than zero",
                        ToStringWithKind(x),
                        ToStringWithKind(y)));
            }
        }

        private static string ToStringWithKind(ActorId id)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", id.Kind.ToString(), id.ToString());
        }
    }
}
