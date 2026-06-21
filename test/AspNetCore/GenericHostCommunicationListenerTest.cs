// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class GenericHostCommunicationListenerTest
{
    readonly ICommunicationListener sut;

    // Constructor parameters
    readonly Mock<Func<string, AspNetCoreCommunicationListener, IHost>> build = new();
    readonly AspNetCoreCommunicationListener listener;

    readonly string listenerUrl = $"http://+:{fuzzy.UInt16()}";
    readonly StatelessServiceContext serviceContext = fuzzy.StatelessServiceContext();
    readonly Mock<IHost> host = new();
    readonly CancellationToken cancellation = TestContext.Current.CancellationToken;
    IHost buildHost;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    GenericHostCommunicationListenerTest()
    {
        buildHost = host.Object;
        _ = host.Setup(_ => _.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _ = host.Setup(_ => _.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _ = build.Setup(_ => _(It.IsAny<string>(), It.IsAny<AspNetCoreCommunicationListener>())).Returns(() => buildHost);

        listener = new TestListener(serviceContext, build.Object, listenerUrl);
        sut = new GenericHostCommunicationListener(build.Object, listener);
        SetupServer(listenerUrl);
    }

    public sealed class Abort : GenericHostCommunicationListenerTest
    {
        [Fact]
        public async Task DisposesHostAfterOpenAsync()
        {
            _ = await sut.OpenAsync(cancellation);
            sut.Abort();
            host.Verify(_ => _.Dispose(), Times.Once());
        }

        [Fact]
        public async Task DoesNotInvokeStopAsync()
        {
            _ = await sut.OpenAsync(cancellation);
            sut.Abort();
            host.Verify(_ => _.StopAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public void DoesNotInvokeHostBeforeOpenAsync()
        {
            sut.Abort();

            build.Verify(_ => _(It.IsAny<string>(), It.IsAny<AspNetCoreCommunicationListener>()), Times.Never());
        }
    }

    public sealed class CloseAsync : GenericHostCommunicationListenerTest
    {
        [Fact]
        public async Task PassesCancellationTokenToHostStopAsync()
        {
            _ = await sut.OpenAsync(CancellationToken.None);

            await sut.CloseAsync(cancellation);

            host.Verify(_ => _.StopAsync(cancellation), Times.Once());
            host.Verify(_ => _.StopAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DisposesHostAfterAwaitingStopAsync()
        {
            _ = await sut.OpenAsync(cancellation);
            var stop = new TaskCompletionSource<object>();
            _ = host.Setup(_ => _.StopAsync(cancellation)).Returns(stop.Task);

            Task close = sut.CloseAsync(cancellation);

            Assert.False(close.IsCompleted);
            host.Verify(_ => _.Dispose(), Times.Never());
            stop.SetResult(null);
            await close;
            host.Verify(_ => _.Dispose(), Times.Once());
        }

        [Fact]
        public async Task DoesNotInvokeHostBeforeOpenAsync()
        {
            await sut.CloseAsync(cancellation);

            build.Verify(_ => _(It.IsAny<string>(), It.IsAny<AspNetCoreCommunicationListener>()), Times.Never());
        }

        [Fact(Explicit = true)] // TODO: SUT bug. CloseAsync skips Dispose when StopAsync throws.
        public async Task DisposesHostWhenStopAsyncThrows()
        {
            // SUT awaits host.StopAsync then calls Dispose without try/finally,
            // so a faulted stop leaks the host. Expected behavior is to always Dispose.
            _ = await sut.OpenAsync(cancellation);
            _ = host.Setup(_ => _.StopAsync(cancellation)).ThrowsAsync(new InvalidOperationException());

            _ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CloseAsync(cancellation));

            host.Verify(_ => _.Dispose(), Times.Once());
        }
    }

    public sealed class Constructor : GenericHostCommunicationListenerTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Missing arg null validation.
        public void ThrowsArgumentNullExceptionWhenBuildIsNull()
        {
            // SUT currently stores `build` without validation and throws NullReferenceException
            // when OpenAsync dereferences it. Expected behavior is to fail fast in the constructor.
            var exception = Assert.Throws<ArgumentNullException>(() => new GenericHostCommunicationListener(null, listener));
            Assert.Equal(nameof(build), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing arg null validation.
        public void ThrowsArgumentNullExceptionWhenListenerIsNull()
        {
            // SUT currently dereferences `listener.ServiceContext` without validation,
            // throwing NullReferenceException instead of ArgumentNullException.
            var exception = Assert.Throws<ArgumentNullException>(() => new GenericHostCommunicationListener(build.Object, null));
            Assert.Equal(nameof(listener), exception.ParamName);
        }
    }

    public sealed class OpenAsync : GenericHostCommunicationListenerTest
    {
        [Fact]
        public async Task InvokesBuildWithListenerUrlAndListener()
        {
            _ = await sut.OpenAsync(cancellation);

            build.Verify(_ => _(listenerUrl, listener), Times.Once());
            build.Verify(_ => _(It.IsAny<string>(), It.IsAny<AspNetCoreCommunicationListener>()), Times.Once());
        }

        [Fact]
        public async Task ThrowsInvalidOperationExceptionWhenBuildReturnsNull()
        {
            buildHost = null;

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(cancellation));
            Assert.Equal(SR.HostNullExceptionMessage, exception.Message);
        }

        [Fact]
        public async Task PassesCancellationTokenToHostStartAsync()
        {
            _ = await sut.OpenAsync(cancellation);

            host.Verify(_ => _.StartAsync(cancellation), Times.Once());
            host.Verify(_ => _.StartAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AwaitsHostStartAsyncBeforeResolvingServer()
        {
            // Guards against a regression where IServer/IServerAddressesFeature is read before
            // StartAsync completes. In real hosting (e.g. Kestrel ":0") addresses are populated
            // during IServer.StartAsync, so an early read would return stale/unbound addresses.
            bool started = false;
            bool resolvedBeforeStart = false;
            var start = new TaskCompletionSource<object>();
            _ = host.Setup(_ => _.StartAsync(cancellation)).Returns(start.Task);

            var features = new FeatureCollection();
            features.Set(Mock.Of<IServerAddressesFeature>(_ => _.Addresses == new[] { $"http://+:{fuzzy.UInt16()}" }));
            var server = Mock.Of<IServer>(_ => _.Features == features);
            var services = new Mock<IServiceProvider>();
            _ = services.Setup(_ => _.GetService(typeof(IServer))).Returns(() =>
            {
                resolvedBeforeStart |= !started;
                return server;
            });
            _ = host.Setup(_ => _.Services).Returns(services.Object);

            Task<string> open = sut.OpenAsync(cancellation);

            Assert.False(open.IsCompleted);
            started = true;
            start.SetResult(null);
            _ = await open;

            Assert.False(resolvedBeforeStart, $"{nameof(IServer)} resolved before host.{nameof(IHost.StartAsync)} completed");
        }

        [Fact]
        public async Task ThrowsInvalidOperationExceptionWhenServerIsNotRegistered()
        {
            SetupServer((IServer)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(cancellation));
            Assert.Equal(SR.WebServerNotFound, exception.Message);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing null check on IServerAddressesFeature.
        public async Task ThrowsInvalidOperationExceptionWhenServerAddressesFeatureIsNotRegistered()
        {
            // SUT currently dereferences server.Features.Get<IServerAddressesFeature>() without a null check,
            // throwing NullReferenceException instead of InvalidOperationException with SR.ErrorNoUrlFromAspNetCore.
            SetupServer(Mock.Of<IServer>(_ => _.Features == new FeatureCollection()));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(cancellation));
            Assert.Equal(SR.ErrorNoUrlFromAspNetCore, exception.Message);
        }

        [Fact]
        public async Task ThrowsInvalidOperationExceptionWhenServerHasNoAddresses()
        {
            var features = new FeatureCollection();
            features.Set(Mock.Of<IServerAddressesFeature>(_ => _.Addresses == Array.Empty<string>()));
            SetupServer(Mock.Of<IServer>(_ => _.Features == features));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(cancellation));
            Assert.Equal(SR.ErrorNoUrlFromAspNetCore, exception.Message);
        }

        [Fact]
        public async Task UsesFirstServerAddressWhenMultipleAreConfigured()
        {
            ushort firstPort = fuzzy.UInt16().Maximum(ushort.MaxValue - 5);
            ushort secondPort = (ushort)(firstPort + fuzzy.SByte().Between(1, 5));
            var features = new FeatureCollection();
            features.Set(Mock.Of<IServerAddressesFeature>(_ => _.Addresses == new[] { $"http://127.0.0.1:{firstPort}", $"http://127.0.0.1:{secondPort}" }));
            SetupServer(Mock.Of<IServer>(_ => _.Features == features));

            string actual = await sut.OpenAsync(cancellation);

            Assert.Equal($"http://127.0.0.1:{firstPort}", actual);
        }

        [Fact]
        public async Task ReplacesPlusWildcardWithPublishAddress()
        {
            ushort port = fuzzy.UInt16();
            SetupServer($"http://+:{port}");

            string actual = await sut.OpenAsync(cancellation);

            Assert.Equal($"http://{serviceContext.PublishAddress}:{port}", actual);
        }

        [Fact]
        public async Task ReplacesIPv6WildcardWithPublishAddress()
        {
            ushort port = fuzzy.UInt16();
            SetupServer($"http://[::]:{port}");

            string actual = await sut.OpenAsync(cancellation);

            Assert.Equal($"http://{serviceContext.PublishAddress}:{port}", actual);
        }

        [Fact]
        public async Task ReturnsServerAddressUnchangedWhenNoWildcard()
        {
            ushort port = fuzzy.UInt16();
            SetupServer($"http://127.0.0.1:{port}");

            string actual = await sut.OpenAsync(cancellation);

            Assert.Equal($"http://127.0.0.1:{port}", actual);
        }

        [Fact]
        public async Task TrimsTrailingSlashFromServerAddress()
        {
            ushort port = fuzzy.UInt16();
            SetupServer($"http://+:{port}/");

            string actual = await sut.OpenAsync(cancellation);

            Assert.Equal($"http://{serviceContext.PublishAddress}:{port}", actual);
        }

        [Fact]
        public async Task AppendsUrlSuffixConfiguredDuringBuild()
        {
            // Covers both the common case (suffix set before OpenAsync) and the regression where UrlSuffix
            // is read before invoking build(...). UseServiceFabricIntegration configures the listener from
            // inside the host-builder delegate, so the suffix isn't known until after build returns.
            ushort port = fuzzy.UInt16();
            SetupServer($"http://+:{port}");
            _ = build.Setup(_ => _(It.IsAny<string>(), It.IsAny<AspNetCoreCommunicationListener>()))
                .Callback((string _, AspNetCoreCommunicationListener l) => l.ConfigureToUseUniqueServiceUrl())
                .Returns(() => buildHost);

            string actual = await sut.OpenAsync(cancellation);

            build.Verify(_ => _(listenerUrl, listener), Times.Once());
            build.Verify(_ => _(It.IsAny<string>(), It.IsAny<AspNetCoreCommunicationListener>()), Times.Once());
            Assert.Equal($"http://{serviceContext.PublishAddress}:{port}{listener.UrlSuffix}", actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. OpenAsync skips Dispose when StartAsync throws.
        public async Task DisposesHostWhenStartAsyncThrows()
        {
            // SUT assigns this.host = build(...) before awaiting host.StartAsync without try/finally,
            // so a faulted start leaks the host. Expected behavior is to Dispose the host on failure.
            _ = host.Setup(_ => _.StartAsync(cancellation)).ThrowsAsync(new InvalidOperationException());

            _ = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.OpenAsync(cancellation));

            host.Verify(_ => _.Dispose(), Times.Once());
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Second OpenAsync overwrites first host without disposing it.
        public async Task DisposesPreviousHostWhenInvokedTwice()
        {
            // SUT unconditionally assigns this.host = this.build(...) on every OpenAsync call,
            // so a second open overwrites the first reference, leaking the previous host.
            // Expected behavior is to dispose the previous host before replacing it.
            _ = await sut.OpenAsync(cancellation);
            var secondHost = new Mock<IHost>();
            _ = secondHost.Setup(_ => _.StartAsync(cancellation)).Returns(Task.CompletedTask);
            _ = secondHost.Setup(_ => _.Services).Returns(host.Object.Services);
            buildHost = secondHost.Object;

            _ = await sut.OpenAsync(cancellation);

            host.Verify(_ => _.Dispose(), Times.Once());
        }
    }

    void SetupServer(string address)
    {
        var features = new FeatureCollection();
        features.Set(Mock.Of<IServerAddressesFeature>(_ => _.Addresses == new[] { address }));
        SetupServer(Mock.Of<IServer>(_ => _.Features == features));
    }

    void SetupServer(IServer server)
    {
        var services = Mock.Of<IServiceProvider>(_ => _.GetService(typeof(IServer)) == server);
        _ = host.Setup(_ => _.Services).Returns(services);
    }

    sealed class TestListener(ServiceContext serviceContext, Func<string, AspNetCoreCommunicationListener, IHost> build, string listenerUrl)
        : AspNetCoreCommunicationListener(serviceContext, build)
    {
        protected internal override string GetListenerUrl() => listenerUrl;
    }
}
