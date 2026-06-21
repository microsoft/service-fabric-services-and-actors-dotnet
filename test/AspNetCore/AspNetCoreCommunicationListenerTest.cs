// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class AspNetCoreCommunicationListenerTest
{
    readonly AspNetCoreCommunicationListener sut;

    // Constructor parameters
    readonly ServiceContext serviceContext = fuzzy.ServiceContext();
    readonly Func<string, AspNetCoreCommunicationListener, IWebHost> build = (_, _) => Mock.Of<IWebHost>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    AspNetCoreCommunicationListenerTest() =>
        sut = new TestListener(serviceContext, build);

    public sealed class Abort : AspNetCoreCommunicationListenerTest
    {
        [Fact]
        public async Task DisposesHostAfterOpenAsyncOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);
            _ = await fixture.Sut.OpenAsync(CancellationToken.None);

            fixture.Sut.Abort();

            fixture.Host.Verify(_ => _.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DoesNotInvokeStopAsyncOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);
            _ = await fixture.Sut.OpenAsync(CancellationToken.None);

            fixture.Sut.Abort();

            fixture.Host.Verify(_ => _.StopAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void DoesNotBuildHostWhenNotOpenedOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);

            fixture.Sut.Abort();

            Assert.Equal(0, fixture.BuildCallCount);
        }
    }

    public sealed class CloseAsync : AspNetCoreCommunicationListenerTest
    {
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken; // Matches SUT parameter name

        [Fact]
        public async Task PassesCancellationTokenToHostStopAsyncOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);
            _ = await fixture.Sut.OpenAsync(CancellationToken.None);

            await fixture.Sut.CloseAsync(cancellationToken);

            fixture.Host.Verify(_ => _.StopAsync(cancellationToken), Times.Once);
            fixture.Host.Verify(_ => _.StopAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DisposesHostAfterStopAsyncOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);
            _ = await fixture.Sut.OpenAsync(CancellationToken.None);
            bool stopped = false;
            bool stoppedBeforeDispose = false;
            _ = fixture.Host.Setup(_ => _.StopAsync(cancellationToken)).Callback(() => stopped = true).Returns(Task.CompletedTask);
            _ = fixture.Host.Setup(_ => _.Dispose()).Callback(() => stoppedBeforeDispose = stopped);

            await fixture.Sut.CloseAsync(cancellationToken);

            Assert.True(stoppedBeforeDispose, "Dispose called before StopAsync");
            fixture.Host.Verify(_ => _.Dispose(), Times.Once);
        }

        [Fact]
        public async Task AwaitsHostStopAsyncBeforeReturningOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);
            _ = await fixture.Sut.OpenAsync(CancellationToken.None);
            var tcs = new TaskCompletionSource<object>();
            _ = fixture.Host.Setup(_ => _.StopAsync(cancellationToken)).Returns(tcs.Task);

            Task closeTask = fixture.Sut.CloseAsync(cancellationToken);

            Assert.False(closeTask.IsCompleted);
            fixture.Host.Verify(_ => _.Dispose(), Times.Never);
            tcs.SetResult(null);
            await closeTask;
            fixture.Host.Verify(_ => _.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DoesNotBuildHostWhenNotOpenedOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);

            await fixture.Sut.CloseAsync(cancellationToken);

            Assert.Equal(0, fixture.BuildCallCount);
        }
    }

    public sealed class ConfigureToUseUniqueServiceUrl : AspNetCoreCommunicationListenerTest
    {
        [Fact]
        public void AppendsPartitionAndInstanceToUrlSuffixForStatelessContext()
        {
            StatelessServiceContext context = fuzzy.StatelessServiceContext();
            var listener = new TestListener(context, build);

            listener.ConfigureToUseUniqueServiceUrl();

            string expected = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", context.PartitionId, context.ReplicaOrInstanceId);
            Assert.Equal(expected, listener.UrlSuffix);
        }

        [Fact]
        public void AppendsPartitionReplicaAndNonEmptyGuidToUrlSuffixForStatefulContext()
        {
            StatefulServiceContext context = fuzzy.StatefulServiceContext();
            var listener = new TestListener(context, build);

            listener.ConfigureToUseUniqueServiceUrl();

            string prefix = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}/", context.PartitionId, context.ReplicaOrInstanceId);
            Assert.StartsWith(prefix, listener.UrlSuffix);
            var trailing = Guid.Parse(listener.UrlSuffix[prefix.Length..]);
            Assert.NotEqual(Guid.Empty, trailing);
        }

        [Fact]
        public void DoesNotChangeUrlSuffixOnSecondCall()
        {
            StatefulServiceContext context = fuzzy.StatefulServiceContext();
            var listener = new TestListener(context, build);
            listener.ConfigureToUseUniqueServiceUrl();
            string first = listener.UrlSuffix;

            listener.ConfigureToUseUniqueServiceUrl();

            Assert.Equal(first, listener.UrlSuffix);
        }
    }

    public sealed class Constructor_ServiceContext_FuncOfStringOfAspNetCoreCommunicationListenerOfIHost : AspNetCoreCommunicationListenerTest
    {
        new readonly AspNetCoreCommunicationListener sut;

        new readonly Func<string, AspNetCoreCommunicationListener, IHost> build = (_, _) => Mock.Of<IHost>();

        public Constructor_ServiceContext_FuncOfStringOfAspNetCoreCommunicationListenerOfIHost() =>
            sut = new TestListener(serviceContext, build);

        [Fact]
        public void InitializesProperties()
        {
            Assert.Empty(sut.UrlSuffix);
            Assert.Same(serviceContext, sut.ServiceContext);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new TestListener(null, build));
            Assert.Equal(nameof(serviceContext), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TestListener(serviceContext, (Func<string, AspNetCoreCommunicationListener, IHost>)null));
            Assert.Equal(nameof(build), exception.ParamName);
        }
    }

    public sealed class Constructor_ServiceContext_FuncOfStringOfAspNetCoreCommunicationListenerOfIWebHost : AspNetCoreCommunicationListenerTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Empty(sut.UrlSuffix);
            Assert.Same(serviceContext, sut.ServiceContext);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenServiceContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new TestListener(null, build));
            Assert.Equal(nameof(serviceContext), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TestListener(serviceContext, (Func<string, AspNetCoreCommunicationListener, IWebHost>)null));
            Assert.Equal(nameof(build), exception.ParamName);
        }
    }

    public sealed class GetEndpointResourceDescription : AspNetCoreCommunicationListenerTest
    {
        readonly StatelessServiceContext context = fuzzy.StatelessServiceContext();
        readonly TestListener listener;

        // Method parameters
        readonly string endpointName = fuzzy.String();

        public GetEndpointResourceDescription() =>
            listener = new TestListener(context, build);

        [Fact]
        public void ReturnsEndpointResourceDescriptionFromManifest()
        {
            var endpoint = new EndpointResourceDescription { Name = endpointName };
            context.CodePackageActivationContext.GetEndpoints().Add(endpoint);

            EndpointResourceDescription actual = listener.GetEndpointResourceDescription(endpoint.Name);

            Assert.Same(endpoint, actual);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenEndpointNameIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => listener.GetEndpointResourceDescription(null));
            Assert.Equal(nameof(endpointName), exception.ParamName);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionWhenEndpointIsNotInManifest()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => listener.GetEndpointResourceDescription(endpointName));
            Assert.Equal(string.Format(CultureInfo.CurrentCulture, SR.EndpointNameNotFoundExceptionMessage, endpointName), exception.Message);
        }
    }

    public sealed class OpenAsync : AspNetCoreCommunicationListenerTest
    {
        readonly CancellationToken cancellationToken = TestContext.Current.CancellationToken; // Matches SUT parameter name

        [Fact]
        public async Task InvokesBuildWithGetListenerUrlAndSelfOnGenericHost()
        {
            // Minimum(1) avoids port 0: it would make listenerUrl byte-identical to the fixture's
            // "http://+:0" default, letting a buggy SUT that ignored GetListenerUrl() still pass.
            string listenerUrl = "http://+:" + fuzzy.UInt16().Minimum(1);
            var fixture = new GenericHostFixture(serviceContext, listenerUrl);

            _ = await fixture.Sut.OpenAsync(cancellationToken);

            Assert.Equal(1, fixture.BuildCallCount);
            Assert.Equal(listenerUrl, fixture.BuildUrl);
            Assert.Same(fixture.Sut, fixture.BuildListener);
        }

        [Fact]
        public async Task InvokesBuildWithGetListenerUrlAndSelfOnWebHost()
        {
            // Minimum(1) avoids port 0: it would make listenerUrl byte-identical to the fixture's
            // "http://+:0" default, letting a buggy SUT that ignored GetListenerUrl() still pass.
            string listenerUrl = "http://+:" + fuzzy.UInt16().Minimum(1);
            var fixture = new WebHostFixture(serviceContext, listenerUrl);

            _ = await fixture.Sut.OpenAsync(cancellationToken);

            Assert.Equal(1, fixture.BuildCallCount);
            Assert.Equal(listenerUrl, fixture.BuildUrl);
            Assert.Same(fixture.Sut, fixture.BuildListener);
        }

        [Fact]
        public async Task PassesCancellationTokenToHostStartAsyncOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);

            _ = await fixture.Sut.OpenAsync(cancellationToken);

            fixture.Host.Verify(_ => _.StartAsync(cancellationToken), Times.Once);
            fixture.Host.Verify(_ => _.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReturnsUrlFromServerAddressFeatureOnWebHost()
        {
            string serverAddress = "http://127.0.0.1:" + fuzzy.UInt16();
            var fixture = new WebHostFixture(serviceContext, "http://+:" + fuzzy.UInt16(), serverAddress);

            string actual = await fixture.Sut.OpenAsync(cancellationToken);

            Assert.Equal(serverAddress, actual);
        }

        [Fact]
        public async Task AppendsUrlSuffixToReturnedUrlOnWebHost()
        {
            string serverAddress = "http://127.0.0.1:" + fuzzy.UInt16();
            var fixture = new WebHostFixture(serviceContext, serverAddress: serverAddress);
            fixture.Sut.ConfigureToUseUniqueServiceUrl();

            string actual = await fixture.Sut.OpenAsync(cancellationToken);

            Assert.Equal(serverAddress + fixture.Sut.UrlSuffix, actual);
        }

        [Fact]
        public async Task AwaitsHostStartAsyncBeforeReturningOnWebHost()
        {
            var fixture = new WebHostFixture(serviceContext);
            var tcs = new TaskCompletionSource<object>();
            _ = fixture.Host.Setup(_ => _.StartAsync(cancellationToken)).Returns(tcs.Task);

            Task<string> openTask = fixture.Sut.OpenAsync(cancellationToken);

            Assert.False(openTask.IsCompleted);
            tcs.SetResult(null);
            _ = await openTask;
        }
    }

    sealed class WebHostFixture
    {
        internal readonly Mock<IWebHost> Host = new();
        internal readonly TestListener Sut;
        internal string BuildUrl;
        internal AspNetCoreCommunicationListener BuildListener;
        internal int BuildCallCount;

        internal WebHostFixture(ServiceContext context, string listenerUrl = "http://+:0", string serverAddress = null)
        {
            _ = Host.Setup(_ => _.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _ = Host.Setup(_ => _.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            Sut = new TestListener(context, (url, listener) =>
            {
                BuildCallCount++;
                BuildUrl = url;
                BuildListener = listener;
                var features = new FeatureCollection();
                features.Set(Mock.Of<IServerAddressesFeature>(_ => _.Addresses == new[] { serverAddress ?? url }));
                _ = Host.Setup(_ => _.ServerFeatures).Returns(features);
                return Host.Object;
            })
            { ListenerUrl = listenerUrl };
        }
    }

    sealed class GenericHostFixture
    {
        internal readonly Mock<IHost> Host = new();
        internal readonly TestListener Sut;
        internal string BuildUrl;
        internal AspNetCoreCommunicationListener BuildListener;
        internal int BuildCallCount;

        internal GenericHostFixture(ServiceContext context, string listenerUrl = "http://+:0", string serverAddress = null)
        {
            _ = Host.Setup(_ => _.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _ = Host.Setup(_ => _.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            Sut = new TestListener(context, (url, listener) =>
            {
                BuildCallCount++;
                BuildUrl = url;
                BuildListener = listener;
                var features = new FeatureCollection();
                features.Set(Mock.Of<IServerAddressesFeature>(_ => _.Addresses == new[] { serverAddress ?? url }));
                var server = Mock.Of<IServer>(_ => _.Features == features);
                var services = Mock.Of<IServiceProvider>(_ => _.GetService(typeof(IServer)) == server);
                _ = Host.Setup(_ => _.Services).Returns(services);
                return Host.Object;
            })
            { ListenerUrl = listenerUrl };
        }
    }

    sealed class TestListener : AspNetCoreCommunicationListener
    {
        internal TestListener(ServiceContext serviceContext, Func<string, AspNetCoreCommunicationListener, IWebHost> build)
            : base(serviceContext, build)
        {
        }

        internal TestListener(ServiceContext serviceContext, Func<string, AspNetCoreCommunicationListener, IHost> build)
            : base(serviceContext, build)
        {
        }

        internal string ListenerUrl = "http://+:0";

        protected internal override string GetListenerUrl() =>
            ListenerUrl;

        internal new EndpointResourceDescription GetEndpointResourceDescription(string endpointName) =>
            base.GetEndpointResourceDescription(endpointName);
    }
}
