// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    public abstract class CommunicationListenerInfoTest
    {
        readonly CommunicationListenerInfo sut;

        // Constructor parameters
        readonly string name = fuzzy.String();
        readonly ICommunicationListener listener = Mock.Of<ICommunicationListener>();

        // Fixture
        static readonly IFuzz fuzzy = new RandomFuzz();

        protected CommunicationListenerInfoTest() =>
            sut = new CommunicationListenerInfo(name, listener);

        public sealed class Constructor : CommunicationListenerInfoTest
        {
            [Fact]
            public void InitializesPropertiesWithGivenArguments()
            {
                Assert.Same(name, sut.Name);
                Assert.Same(listener, sut.Listener);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenNameIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new CommunicationListenerInfo(null, listener));
                Assert.Equal(nameof(name), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenListenerIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new CommunicationListenerInfo(name, null));
                Assert.Equal(nameof(listener), exception.ParamName);
            }
        }

        public sealed class ToStringTest : CommunicationListenerInfoTest
        {
            [Fact]
            public void ReturnsStringWithDetailedInformationForTracing()
            {
                string expected = $"{listener.GetType().Name} '{name}' (#{listener.GetHashCode()})";
                Assert.Equal(expected, sut.ToString());
            }

            [Fact]
            public void ReturnsSameStringCachedForPerformance() =>
                Assert.Same(sut.ToString(), sut.ToString());
        }

        public sealed class EqualsTest : CommunicationListenerInfoTest
        {
            new readonly IEquatable<CommunicationListenerInfo> sut;

            public EqualsTest() =>
                sut = base.sut;

            [Fact]
            public void ReturnsTrueWhenNameAndListenerAreSame() =>
                Assert.True(sut.Equals(new CommunicationListenerInfo(name, listener)));

            [Fact]
            public void ReturnsTrueWhenNamesAreEqual() =>
                Assert.True(sut.Equals(new CommunicationListenerInfo(new string(name.ToCharArray()), listener)));

            [Fact]
            public void ReturnsFalseWhenNameIsDifferent() =>
                Assert.False(sut.Equals(new CommunicationListenerInfo(fuzzy.String(), listener)));

            [Fact]
            public void ReturnsFalseWhenListenerIsDifferent() =>
                Assert.False(sut.Equals(new CommunicationListenerInfo(name, Mock.Of<ICommunicationListener>())));

            [Fact]
            public void ReturnsFalseGivenNull() =>
                Assert.False(sut.Equals(null));
        }
    }
}
