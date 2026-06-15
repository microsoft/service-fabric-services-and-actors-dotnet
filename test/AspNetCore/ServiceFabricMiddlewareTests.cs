// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test class for ServiceFabricMiddleware.
    /// </summary>
    public class ServiceFabricMiddlewareTests
    {
        private readonly HttpContext httpContext;
        private AspNetCoreCommunicationListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricMiddlewareTests"/> class.
        /// </summary>
        public ServiceFabricMiddlewareTests()
        {
            // setup HttpRequest mock
            var mockHttpRequest = new Mock<HttpRequest>();
            mockHttpRequest.SetupAllProperties();

            // setup HttpResponse mock
            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpResponse.SetupAllProperties();

            // setup HttpContext mock
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(y => y.Response).Returns(mockHttpResponse.Object);
            mockHttpContext.Setup(y => y.Request).Returns(mockHttpRequest.Object);
            this.httpContext = mockHttpContext.Object;
        }

        /// <summary>
        /// Gets strings for the two Host Types - WebHost and GenericHost.
        /// </summary>
        public static IEnumerable<object[]> HostTypes
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { "WebHost" },
                    new object[] { "GenericHost" },
                };
            }
        }

        /// <summary>
        /// Verify ErrorCode 410 is returned from Middleware, when UrlSuffix in Middleware doesn't match with what listener used
        /// when constructing url before returning to Naming Service.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        /// <returns>A <see cref="Task"/> tracking asynchronous test completion.</returns>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public async Task VerifyReturnCode410(string hostType)
        {
            var nextCalled = false;

            this.listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), hostType);
            this.listener.ConfigureToUseUniqueServiceUrl();
            var middleware = new ServiceFabricMiddleware(
                (httpContext) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, this.listener.UrlSuffix);

            // send a request in which Path is different than urlSuffix
            this.httpContext.Request.Path = this.listener.UrlSuffix + "xyz";
            await middleware.Invoke(this.httpContext);

            Assert.Equal(StatusCodes.Status410Gone, this.httpContext.Response.StatusCode); // status code should be 410 when path base is different from url suffix.
            Assert.False(nextCalled, "next RequestDelegate is not called by middleware.");
        }

        /// <summary>
        /// Verify next RequestDelegate invocation when Path is valid.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyNextInvocationWithUrlSuffix(string hostType)
        {
            this.listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), hostType);

            // configure listener useUniqueServiceUrl
            this.listener.ConfigureToUseUniqueServiceUrl();
            this.VerifyNextInvocation();
        }

        /// <summary>
        /// Verify next RequestDelegate invocation when Path is valid.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyNextInvocatioWithoutUrlSuffix(string hostType)
        {
            this.listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), hostType);

            // do not configure listener useUniqueServiceUrl
            this.VerifyNextInvocation();
        }

        /// <summary>
        /// Verify Path and PathBase in next RequestDelegate invocation.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyPathsInNextInvocationWithUrlSuffix(string hostType)
        {
            this.listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), hostType);

            // configure listener useUniqueServiceUrl
            // In this case urlSuffix will be /PArtitionId/ReplicaId
            this.listener.ConfigureToUseUniqueServiceUrl();
            this.VerifyPathsInNextInvocation();
        }

        /// <summary>
        /// Verify Path and PathBase in next RequestDelegate invocation.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyPathsInNextInvocationWithoutUrlSuffix(string hostType)
        {
            this.listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), hostType);

            // do not configure listener useUniqueServiceUrl
            // In this case urlSuffix will be empty
            this.VerifyPathsInNextInvocation();
        }

        /// <summary>
        /// Verify next RequestDelegate invocation when Path is valid.
        /// </summary>
        private void VerifyNextInvocation()
        {
            var nextCalled = false;

            var middleware = new ServiceFabricMiddleware(
                (httpContext) =>
            {
                nextCalled = true;
                Console.WriteLine("In Next Request Delegate: HttpRequest.Path: " + httpContext.Request.Path);
                Console.WriteLine("In Next Request Delegate: HttpRequest.PathBase: " + httpContext.Request.PathBase);
                return Task.CompletedTask;
            }, this.listener.UrlSuffix);

            // send a request in which Path is same as urlSuffix
            Console.WriteLine("UrlSuffix is: " + this.listener.UrlSuffix);

            this.httpContext.Request.Path = this.listener.UrlSuffix;

            Console.WriteLine("Before Invoke: HttpRequest.Path: " + this.httpContext.Request.Path);
            Console.WriteLine("Before Invoke: HttpRequest.PathBase: " + this.httpContext.Request.PathBase);
            middleware.Invoke(this.httpContext).GetAwaiter().GetResult();

            Assert.True(nextCalled, "next RequestDelegate is called by middleware");
        }

        /// <summary>
        /// Verify Path and PathBase in next RequestDelegate invocation.
        /// </summary>
        private void VerifyPathsInNextInvocation()
        {
            PathString pathInNext = null;
            PathString pathBaseInNext = null;
            var nextCalled = false;

            var middleware = new ServiceFabricMiddleware(
                (httpContext) =>
            {
                pathInNext = httpContext.Request.Path;
                pathBaseInNext = httpContext.Request.PathBase;
                Console.WriteLine("In Next Request Delegate: HttpRequest.Path: " + httpContext.Request.Path);
                Console.WriteLine("In Next Request Delegate: HttpRequest.PathBase: " + httpContext.Request.PathBase);

                nextCalled = true;
                return Task.CompletedTask;
            }, this.listener.UrlSuffix);

            // send a request in which Path is different than urlSuffix, but has extra segment after it.
            // This extra segment should become Path for next delegate, and Path should become PathBase for next delegate.
            Console.WriteLine("UrlSuffix is: " + this.listener.UrlSuffix);

            var requestPathSuffix = "/abc";
            var requestPath = this.listener.UrlSuffix + requestPathSuffix;
            this.httpContext.Request.Path = requestPath;

            Console.WriteLine("Before Invoke: HttpRequest.Path: " + this.httpContext.Request.Path);
            Console.WriteLine("Before Invoke: HttpRequest.PathBase: " + this.httpContext.Request.PathBase);

            middleware.Invoke(this.httpContext).GetAwaiter().GetResult();

            Assert.True(nextCalled, "next RequestDelegate is called by middleware");
            Assert.Equal(this.listener.UrlSuffix, pathBaseInNext.ToString()); // pathBase for next RequestDelegate is changed by middleware.
            Assert.Equal(requestPathSuffix, pathInNext.ToString()); // Path for next RequestDelegate is changed by middleware.

            // Verify Path and PathBase again when returned from next delegate.
            Assert.Equal(requestPath, this.httpContext.Request.Path.ToString()); // Path after next RequestDelegate has been called should be the original requestPath
            Assert.Empty(this.httpContext.Request.PathBase.ToString()); // PathBase after next RequestDelegate has been called should be empty
        }

        private IWebHost IWebHostBuildFunc(string url, AspNetCoreCommunicationListener listener)
        {
            var mockServerAddressFeature = new Mock<IServerAddressesFeature>();
            mockServerAddressFeature.Setup(y => y.Addresses).Returns(new string[] { url });
            var featureCollection = new FeatureCollection();
            featureCollection.Set(mockServerAddressFeature.Object);

            // Create mock IWebHost and set required things used by this test.
            var mockWebHost = new Mock<IWebHost>();
            return mockWebHost.Object;
        }

        private IHost IHostBuildFunc(string url, AspNetCoreCommunicationListener listener)
        {
            var mockServerAddressFeature = new Mock<IServerAddressesFeature>();
            mockServerAddressFeature.Setup(y => y.Addresses).Returns(new string[] { url });
            var featureCollection = new FeatureCollection();
            featureCollection.Set(mockServerAddressFeature.Object);

            // Create mock IServer and set required things used by tests.
            var mockServer = new Mock<IServer>();
            mockServer.Setup(y => y.Features).Returns(featureCollection);

            // Create ServiceCollection and IServiceProvider and set required things used for tests.
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IServer>(mockServer.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Create mock IHost and set required things used by tests.
            var mockHost = new Mock<IHost>();
            mockHost.Setup(y => y.Services).Returns(serviceProvider);

            return mockHost.Object;
        }

        private KestrelCommunicationListener CreateListener(StatelessServiceContext context, string hostType)
        {
            if (hostType == "WebHost")
            {
                return new KestrelCommunicationListener(context, (uri, listen) => this.IWebHostBuildFunc(uri, listen));
            }
            else
            {
                return new KestrelCommunicationListener(context, (uri, listen) => this.IHostBuildFunc(uri, listen));
            }
        }
    }
}
