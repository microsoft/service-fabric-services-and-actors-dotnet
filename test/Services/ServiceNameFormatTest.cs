// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.ServiceFabric.Services;

public abstract class ServiceNameFormatTest
{
    public sealed class GetEndpointName : ServiceNameFormatTest
    {
        [Fact]
        public void AppendsEndpointSuffixToNameDerivedFromTypeName() =>
            Assert.Equal("ObjectServiceEndpoint", ServiceNameFormat.GetEndpointName(typeof(object)));

        [Fact]
        public void StripsLeadingIFromInterfaceTypeNameAndAppendsEndpointSuffix() =>
            Assert.Equal("DisposableServiceEndpoint", ServiceNameFormat.GetEndpointName(typeof(IDisposable)));

        // Pins current behavior: SUT does not validate the argument and dereferences it. Bug fix is out of scope.
        [Fact]
        public void ThrowsNullReferenceExceptionWhenServiceInterfaceTypeIsNull() =>
            Assert.Throws<NullReferenceException>(() => ServiceNameFormat.GetEndpointName(null));
    }

    public sealed class GetName_String : ServiceNameFormatTest
    {
        [Fact]
        public void AppendsServiceSuffixWhenNameDoesNotEndWithService() =>
            Assert.Equal("FooService", ServiceNameFormat.GetName("Foo"));

        [Theory]
        [InlineData("MyService")]
        [InlineData("myservice")]
        [InlineData("MYSERVICE")]
        public void KeepsNameThatEndsWithServiceCaseInsensitively(string name) =>
            Assert.Equal(name, ServiceNameFormat.GetName(name));

        [Fact]
        public void StripsLeadingIWhenFollowedByUppercaseLetter() =>
            Assert.Equal("FooService", ServiceNameFormat.GetName("IFoo"));

        [Fact]
        public void KeepsLeadingIWhenFollowedByLowercaseLetter() =>
            Assert.Equal("IfooService", ServiceNameFormat.GetName("Ifoo"));

        [Fact]
        public void ReturnsServiceWhenNameIsEmpty() =>
            Assert.Equal("Service", ServiceNameFormat.GetName(string.Empty));

        [Fact]
        public void ReturnsServiceWhenNameIsSingleI() =>
            Assert.Equal("Service", ServiceNameFormat.GetName("I"));

        // Pins current behavior: SUT does not validate the argument and dereferences it. Bug fix is out of scope.
        [Fact]
        public void ThrowsNullReferenceExceptionWhenServiceInterfaceTypeNameIsNull() =>
            Assert.Throws<NullReferenceException>(() => ServiceNameFormat.GetName((string)null));
    }

    public sealed class GetName_Type : ServiceNameFormatTest
    {
        [Fact]
        public void ReturnsNameDerivedFromTypeName() =>
            Assert.Equal("ObjectService", ServiceNameFormat.GetName(typeof(object)));

        [Fact]
        public void StripsLeadingIFromInterfaceTypeName() =>
            Assert.Equal("DisposableService", ServiceNameFormat.GetName(typeof(IDisposable)));

        // Pins current behavior: SUT does not validate the argument and dereferences it. Bug fix is out of scope.
        [Fact]
        public void ThrowsNullReferenceExceptionWhenServiceInterfaceTypeIsNull() =>
            Assert.Throws<NullReferenceException>(() => ServiceNameFormat.GetName((Type)null));
    }
}
