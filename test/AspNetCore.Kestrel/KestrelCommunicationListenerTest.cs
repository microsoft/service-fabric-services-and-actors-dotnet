// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using Fuzzy;
using Inspector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class KestrelCommunicationListenerTest
{
    readonly AspNetCoreCommunicationListener sut;

    // Constructor parameters
    readonly ServiceContext serviceContext = fuzzy.ServiceContext();
    readonly string endpointName = fuzzy.String();
    readonly Func<string, AspNetCoreCommunicationListener, IWebHost> build = (_, _) => Mock.Of<IWebHost>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    KestrelCommunicationListenerTest() =>
        sut = new KestrelCommunicationListener(serviceContext, endpointName, build);

    public sealed class Constructor_ServiceContext_FuncOfStringOfAspNetCoreCommunicationListenerOfIHost : KestrelCommunicationListenerTest
    {
        // TODO: Inspector 0.3.12 `Constructor<TSignature>()` ignores the delegate parameter's return type, so both
        // 2-arg ctor overloads (which differ only by `Func<..., IHost>` vs `Func<..., IWebHost>`) match and `.Single()`
        // throws. Wrap the `ConstructorInfo` directly until a fixed Inspector version is available.
        // File an issue at https://github.com/olegsych/inspector/issues and remove this workaround once resolved.
        static readonly Constructor ctor = new(
            typeof(KestrelCommunicationListener).GetConstructor([typeof(ServiceContext), typeof(Func<string, AspNetCoreCommunicationListener, IHost>)]),
            Type<KestrelCommunicationListener>.Uninitialized());

        new readonly Func<string, AspNetCoreCommunicationListener, IHost> build = (_, _) => Mock.Of<IHost>();

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(null, build));
            Assert.Equal(ctor.Parameter<ServiceContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(serviceContext, (Func<string, AspNetCoreCommunicationListener, IHost>)null));
            Assert.Equal(ctor.Parameter<Func<string, AspNetCoreCommunicationListener, IHost>>().Name, exception.ParamName);
        }

        [Fact]
        public void GetListenerUrlReturnsDefaultHttpUrl()
        {
            // The 2-arg overload chains to the 3-arg ctor with endpointName: null. Testing the default-URL
            // path through the 2-arg entry point pins the IHost-specific 3-arg ctor, so a regression in
            // just one of the two duplicated 3-arg ctors is caught here.
            var sut = (AspNetCoreCommunicationListener)new KestrelCommunicationListener(serviceContext, build);
            Assert.Equal("http://+:0", sut.GetListenerUrl());
        }
    }

    public sealed class Constructor_ServiceContext_FuncOfStringOfAspNetCoreCommunicationListenerOfIWebHost : KestrelCommunicationListenerTest
    {
        // TODO: Inspector 0.3.12 `Constructor<TSignature>()` ignores the delegate parameter's return type, so both
        // 2-arg ctor overloads (which differ only by `Func<..., IHost>` vs `Func<..., IWebHost>`) match and `.Single()`
        // throws. Wrap the `ConstructorInfo` directly until a fixed Inspector version is available.
        // File an issue at https://github.com/olegsych/inspector/issues and remove this workaround once resolved.
        static readonly Constructor ctor = new(
            typeof(KestrelCommunicationListener).GetConstructor([typeof(ServiceContext), typeof(Func<string, AspNetCoreCommunicationListener, IWebHost>)]),
            Type<KestrelCommunicationListener>.Uninitialized());

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(null, build));
            Assert.Equal(ctor.Parameter<ServiceContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(serviceContext, (Func<string, AspNetCoreCommunicationListener, IWebHost>)null));
            Assert.Equal(ctor.Parameter<Func<string, AspNetCoreCommunicationListener, IWebHost>>().Name, exception.ParamName);
        }

        [Fact]
        public void GetListenerUrlReturnsDefaultHttpUrl()
        {
            // The 2-arg overload chains to the 3-arg ctor with endpointName: null. Testing the default-URL
            // path through the 2-arg entry point pins the IWebHost-specific 3-arg ctor, so a regression in
            // just one of the two duplicated 3-arg ctors is caught here.
            var sut = (AspNetCoreCommunicationListener)new KestrelCommunicationListener(serviceContext, build);
            Assert.Equal("http://+:0", sut.GetListenerUrl());
        }
    }

    public sealed class Constructor_ServiceContext_String_FuncOfStringOfAspNetCoreCommunicationListenerOfIHost : KestrelCommunicationListenerTest
    {
        static readonly Constructor ctor = Type<KestrelCommunicationListener>.Uninitialized()
            .Constructor<Action<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IHost>>>();

        new readonly Func<string, AspNetCoreCommunicationListener, IHost> build = (_, _) => Mock.Of<IHost>();

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(null, endpointName, build));
            Assert.Equal(ctor.Parameter<ServiceContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(serviceContext, endpointName, (Func<string, AspNetCoreCommunicationListener, IHost>)null));
            Assert.Equal(ctor.Parameter<Func<string, AspNetCoreCommunicationListener, IHost>>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenEndpointNameIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() => new KestrelCommunicationListener(serviceContext, string.Empty, build));
            Assert.Equal("endpointResourceName cannot be empty string.", exception.Message);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing paramName argument to ArgumentException.
        public void SetsParamNameToEndpointNameOnArgumentException()
        {
            // The constructor throws `new ArgumentException(SR.EndpointNameEmptyExceptionMessage)` without
            // passing the paramName argument, so the resulting exception's ParamName is null and gives callers no
            // indication which argument was invalid. This test asserts the correct ParamName and will fail until the
            // SUT is fixed. Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentException>(() => new KestrelCommunicationListener(serviceContext, string.Empty, build));
            Assert.Equal(ctor.Parameter<string>().Name, exception.ParamName);
        }

        [Fact]
        public void GetListenerUrlReturnsDefaultHttpUrl()
        {
            // Pins the null-endpoint default-URL path to this overload's copy of the
            // `endpointName?.Length == 0 / this.endpointName = endpointName` block so a regression
            // affecting only this ctor is caught here.
            AspNetCoreCommunicationListener sut = new KestrelCommunicationListener(serviceContext, null, build);
            Assert.Equal("http://+:0", sut.GetListenerUrl());
        }
    }

    public sealed class Constructor_ServiceContext_String_FuncOfStringOfAspNetCoreCommunicationListenerOfIWebHost : KestrelCommunicationListenerTest
    {
        static readonly Constructor ctor = Type<KestrelCommunicationListener>.Uninitialized()
            .Constructor<Action<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>>>();

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(null, endpointName, build));
            Assert.Equal(ctor.Parameter<ServiceContext>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new KestrelCommunicationListener(serviceContext, endpointName, (Func<string, AspNetCoreCommunicationListener, IWebHost>)null));
            Assert.Equal(ctor.Parameter<Func<string, AspNetCoreCommunicationListener, IWebHost>>().Name, exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenEndpointNameIsEmpty()
        {
            var exception = Assert.Throws<ArgumentException>(() => new KestrelCommunicationListener(serviceContext, string.Empty, build));
            Assert.Equal("endpointResourceName cannot be empty string.", exception.Message);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing paramName argument to ArgumentException.
        public void SetsParamNameToEndpointNameOnArgumentException()
        {
            // The constructor throws `new ArgumentException(SR.EndpointNameEmptyExceptionMessage)` without
            // passing the paramName argument, so the resulting exception's ParamName is null and gives callers no
            // indication which argument was invalid. This test asserts the correct ParamName and will fail until the
            // SUT is fixed. Fixing the SUT is out of scope for the current change.
            var exception = Assert.Throws<ArgumentException>(() => new KestrelCommunicationListener(serviceContext, string.Empty, build));
            Assert.Equal(ctor.Parameter<string>().Name, exception.ParamName);
        }

        [Fact]
        public void GetListenerUrlReturnsDefaultHttpUrl()
        {
            // Pins the null-endpoint default-URL path to this overload's copy of the
            // `endpointName?.Length == 0 / this.endpointName = endpointName` block so a regression
            // affecting only this ctor is caught here.
            AspNetCoreCommunicationListener sut = new KestrelCommunicationListener(serviceContext, null, build);
            Assert.Equal("http://+:0", sut.GetListenerUrl());
        }
    }

    public sealed class GetListenerUrl : KestrelCommunicationListenerTest
    {
        // The null-endpoint default-URL path is covered per-overload in the four `Constructor_*` classes,
        // co-located with the duplicated SUT code that stores `this.endpointName`.

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
            // Minimum(1) avoids port 0: for the Http row, port 0 would make `expected` byte-identical
            // to the literal returned by KestrelCommunicationListener.GetListenerUrl's no-endpoint
            // default path ("http://+:0"), defeating this test's discrimination of the endpoint branch.
            int port = fuzzy.UInt16().Minimum(1);
            endpoint.Property<int>().Set(port);
            serviceContext.CodePackageActivationContext.GetEndpoints().Add(endpoint);

            string actual = sut.GetListenerUrl();

            string expected = FormattableString.Invariant($"{expectedScheme}://+:{port}");
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
            // Minimum(1) avoids port 0: for the Http row, port 0 would make `expected` byte-identical
            // to the literal returned by KestrelCommunicationListener.GetListenerUrl's no-endpoint
            // default path ("http://+:0"), defeating this test's discrimination of the endpoint branch.
            int port = fuzzy.UInt16().Minimum(1);
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

            string expected = FormattableString.Invariant($"http://+:{port}");
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
            Assert.Equal(string.Format(CultureInfo.CurrentCulture, SR.EndpointNameNotFoundExceptionMessage, endpointName), exception.Message);
        }
    }
}
