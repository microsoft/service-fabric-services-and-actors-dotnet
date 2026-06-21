// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using Fuzzy;
using Inspector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class HttpSysCommunicationListenerTest
{
    readonly HttpSysCommunicationListener sut;

    // Constructor parameters
    readonly ServiceContext serviceContext = fuzzy.ServiceContext();
    readonly string endpointName = fuzzy.String();
    readonly Func<string, AspNetCoreCommunicationListener, IWebHost> build = (_, _) => Mock.Of<IWebHost>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    HttpSysCommunicationListenerTest() =>
        sut = new HttpSysCommunicationListener(serviceContext, endpointName, build);

    public sealed class Constructor_ServiceContext_String_FuncOfStringOfAspNetCoreCommunicationListenerOfIHost : HttpSysCommunicationListenerTest
    {
        static readonly Constructor ctor = Type<HttpSysCommunicationListener>.Uninitialized()
            .Constructor<Action<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IHost>>>();

        new readonly Func<string, AspNetCoreCommunicationListener, IHost> build = (_, _) => Mock.Of<IHost>();

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new HttpSysCommunicationListener(null, endpointName, build));
            Assert.Equal(ctor.Parameter<ServiceContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HttpSysCommunicationListener(serviceContext, endpointName, (Func<string, AspNetCoreCommunicationListener, IHost>)null));
            Assert.Equal(ctor.Parameter<Func<string, AspNetCoreCommunicationListener, IHost>>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenEndpointNameIsNull()
        {
            var exception = Assert.Throws<ArgumentException>(() => new HttpSysCommunicationListener(serviceContext, null, build));
            Assert.StartsWith("endpointName cannot be null or empty string.", exception.Message);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing paramName argument to ArgumentException.
        public void SetsParamNameToEndpointNameOnArgumentException()
        {
            // The constructor throws `new ArgumentException(SR.EndpointNameNullOrEmptyExceptionMessage)` without
            // passing the paramName argument, so the resulting exception's ParamName is null and gives callers no
            // indication which argument was invalid. This test asserts the correct ParamName and will fail until the
            // SUT is fixed. Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentException>(() => new HttpSysCommunicationListener(serviceContext, null, build));
            Assert.Equal(ctor.Parameter<string>().Name, exception.ParamName);
        }
    }

    public sealed class Constructor_ServiceContext_String_FuncOfStringOfAspNetCoreCommunicationListenerOfIWebHost : HttpSysCommunicationListenerTest
    {
        static readonly Constructor ctor = Type<HttpSysCommunicationListener>.Uninitialized()
            .Constructor<Action<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>>>();

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new HttpSysCommunicationListener(null, endpointName, build));
            Assert.Equal(ctor.Parameter<ServiceContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new HttpSysCommunicationListener(serviceContext, endpointName, (Func<string, AspNetCoreCommunicationListener, IWebHost>)null));
            Assert.Equal(ctor.Parameter<Func<string, AspNetCoreCommunicationListener, IWebHost>>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenEndpointNameIsNull()
        {
            var exception = Assert.Throws<ArgumentException>(() => new HttpSysCommunicationListener(serviceContext, null, build));
            Assert.StartsWith("endpointName cannot be null or empty string.", exception.Message);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing paramName argument to ArgumentException.
        public void SetsParamNameToEndpointNameOnArgumentException()
        {
            // The constructor throws `new ArgumentException(SR.EndpointNameNullOrEmptyExceptionMessage)` without
            // passing the paramName argument, so the resulting exception's ParamName is null and gives callers no
            // indication which argument was invalid. This test asserts the correct ParamName and will fail until the
            // SUT is fixed. Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentException>(() => new HttpSysCommunicationListener(serviceContext, null, build));
            Assert.Equal(ctor.Parameter<string>().Name, exception.ParamName);
        }
    }

    public sealed class GetListenerUrl : HttpSysCommunicationListenerTest
    {
        [Theory]
        [InlineData(EndpointProtocol.Tcp, "tcp")]
        [InlineData(EndpointProtocol.Http, "http")]
        [InlineData(EndpointProtocol.Https, "https")]
        [InlineData(EndpointProtocol.Udp, "udp")]
        public void ReturnsUrlWithProtocolLowercaseAndPortFromEndpoint(EndpointProtocol protocol, string expectedScheme)
        {
            var endpoint = new EndpointResourceDescription
            {
                Name = endpointName,
                Protocol = protocol,
            };
            int port = fuzzy.UInt16();
            endpoint.Property<int>().Set(port);
            serviceContext.CodePackageActivationContext.GetEndpoints().Add(endpoint);

            string actual = sut.GetListenerUrl();

            string expected = $"{expectedScheme}://+:{port}";
            Assert.Equal(expected, actual);
        }

        [Theory]
        // Exercise both insertion orders so the test fails for any positional selection
        // (e.g. .First() or .Last()) and only passes for true name-based selection.
        [InlineData(true)]
        [InlineData(false)]
        public void ReturnsUrlForEndpointMatchingNameWhenMultipleEndpointsExist(bool matchingFirst)
        {
            var endpoint = new EndpointResourceDescription
            {
                Name = endpointName,
                Protocol = EndpointProtocol.Http,
            };
            int port = fuzzy.UInt16();
            endpoint.Property<int>().Set(port);

            var other = new EndpointResourceDescription
            {
                Name = endpointName + fuzzy.String(),
                Protocol = EndpointProtocol.Https,
            };
            other.Property<int>().Set(fuzzy.UInt16());

            KeyedCollection<string, EndpointResourceDescription> endpoints = serviceContext.CodePackageActivationContext.GetEndpoints();
            if (matchingFirst)
            {
                endpoints.Add(endpoint);
                endpoints.Add(other);
            }
            else
            {
                endpoints.Add(other);
                endpoints.Add(endpoint);
            }

            string actual = sut.GetListenerUrl();

            string expected = $"http://+:{port}";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionWhenEndpointIsNotInManifest()
        {
            // Add a non-matching endpoint so the SUT must iterate and fail due to name mismatch
            // rather than an empty collection. This proves name-based discrimination on the failure path.
            var other = new EndpointResourceDescription
            {
                Name = endpointName + fuzzy.String(),
                Protocol = EndpointProtocol.Http,
            };
            other.Property<int>().Set(fuzzy.UInt16());
            serviceContext.CodePackageActivationContext.GetEndpoints().Add(other);

            var exception = Assert.Throws<InvalidOperationException>(sut.GetListenerUrl);
            Assert.Equal($"{endpointName} not found in Service Manifest.", exception.Message);
        }

        // The IHost constructor overload independently re-implements the endpointName assignment
        // performed by the IWebHost overload. This test proves that branch is exercised end-to-end
        // by constructing the SUT via the IHost overload and observing GetListenerUrl resolves the
        // endpoint stored by that constructor.
        [Fact]
        public void ReturnsUrlForListenerBuiltWithIHostDelegate()
        {
            var sut = new HttpSysCommunicationListener(serviceContext, endpointName, (_, _) => Mock.Of<IHost>());
            var endpoint = new EndpointResourceDescription
            {
                Name = endpointName,
                Protocol = EndpointProtocol.Http,
            };
            int port = fuzzy.UInt16();
            endpoint.Property<int>().Set(port);
            serviceContext.CodePackageActivationContext.GetEndpoints().Add(endpoint);

            Assert.Equal($"http://+:{port}", sut.GetListenerUrl());
        }
    }
}
