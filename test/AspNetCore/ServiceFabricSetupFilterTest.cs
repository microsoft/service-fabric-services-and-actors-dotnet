// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class ServiceFabricSetupFilterTest
{
    readonly IStartupFilter sut;

    // Constructor parameters
    readonly string urlSuffix = "/" + fuzzy.String().LettersOrDigits();
    readonly ServiceFabricIntegrationOptions options = fuzzy.Enum<ServiceFabricIntegrationOptions>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceFabricSetupFilterTest() =>
        sut = new ServiceFabricSetupFilter(urlSuffix, options);

    public sealed class Configure : ServiceFabricSetupFilterTest
    {
        // Method parameters
        readonly Action<IApplicationBuilder> next = Mock.Of<Action<IApplicationBuilder>>();

        readonly Mock<IApplicationBuilder> app = new();
        readonly List<Func<RequestDelegate, RequestDelegate>> factories = [];

        public Configure()
        {
            _ = app.Setup(_ => _.ApplicationServices).Returns(Mock.Of<IServiceProvider>());
            _ = app.Setup(_ => _.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Callback<Func<RequestDelegate, RequestDelegate>>(factories.Add)
                .Returns(app.Object);
        }

        [Fact]
        public void ReturnsActionThatCallsNextWithApp()
        {
            Action<IApplicationBuilder> configured = sut.Configure(next);
            configured(app.Object);

            Mock.Get(next).Verify(_ => _(app.Object), Times.Once);
            Mock.Get(next).Verify(_ => _(It.IsAny<IApplicationBuilder>()), Times.Once);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing arg null validation.
        public void ThrowsArgumentNullExceptionWhenNextIsNull()
        {
            // SUT returns an action without validation; it throws NullReferenceException when the action
            // dereferences `next`. Expected behavior is to fail fast in Configure.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.Configure(null));
            Assert.Equal(nameof(next), exception.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing arg null validation.
        public void ReturnsActionThatThrowsArgumentNullExceptionWhenAppIsNull()
        {
            // SUT's returned action does not validate `app`. Depending on branch it dereferences `app`
            // (NullReferenceException) or forwards null to `next` silently. Expected behavior is to fail fast.
            var sut = new ServiceFabricSetupFilter(null, ServiceFabricIntegrationOptions.None);
            var exception = Assert.Throws<ArgumentNullException>(() => sut.Configure(next)(null));
            Assert.Equal(nameof(app), exception.ParamName);
        }

        [Theory, InlineData(ServiceFabricIntegrationOptions.None)]
        [InlineData(ServiceFabricIntegrationOptions.UseUniqueServiceUrl)]
        [InlineData(ServiceFabricIntegrationOptions.UseReverseProxyIntegration)]
        [InlineData(ServiceFabricIntegrationOptions.UseReverseProxyIntegration | ServiceFabricIntegrationOptions.UseUniqueServiceUrl)]
        public async Task ReturnsActionThatRegistersServiceFabricMiddlewareWithUrlSuffixWhenUrlSuffixIsNotEmpty(ServiceFabricIntegrationOptions options)
        {
            var sut = new ServiceFabricSetupFilter(urlSuffix, options);
            sut.Configure(next)(app.Object);

            // Verify SUT forwarded urlSuffix by exercising the registered middleware's observable behavior:
            // for a request whose Path starts with urlSuffix as a segment, the middleware strips urlSuffix
            // into PathBase before invoking the inner delegate. A wrong suffix would not rewrite Path/PathBase.
            Func<RequestDelegate, RequestDelegate> factory = factories
                .Single(f => f(_ => Task.CompletedTask).Target is ServiceFabricMiddleware);
            PathString observedPathBase = default, observedPath = default;
            var nextCalled = false;
            RequestDelegate pipeline = factory(ctx =>
            {
                nextCalled = true;
                observedPathBase = ctx.Request.PathBase;
                observedPath = ctx.Request.Path;
                return Task.CompletedTask;
            });
            var remainingPath = fuzzy.String().LettersOrDigits();
            var context = new DefaultHttpContext();
            context.Request.Path = urlSuffix + "/" + remainingPath;
            await pipeline(context);
            Assert.True(nextCalled);
            Assert.Equal(urlSuffix, observedPathBase);
            Assert.Equal("/" + remainingPath, observedPath);
        }

        [Theory, InlineData(null), InlineData("")]
        public void ReturnsActionThatDoesNotRegisterServiceFabricMiddlewareWhenUrlSuffixIsNullOrEmpty(string urlSuffix)
        {
            var sut = new ServiceFabricSetupFilter(urlSuffix, options);
            sut.Configure(next)(app.Object);
            Assert.Empty(RegisteredMiddlewares().OfType<ServiceFabricMiddleware>());
        }

        [Theory, InlineData(ServiceFabricIntegrationOptions.UseReverseProxyIntegration)]
        [InlineData(ServiceFabricIntegrationOptions.UseReverseProxyIntegration | ServiceFabricIntegrationOptions.UseUniqueServiceUrl)]
        public void ReturnsActionThatRegistersReverseProxyIntegrationMiddlewareWhenOptionsHasUseReverseProxyIntegration(ServiceFabricIntegrationOptions options)
        {
            var sut = new ServiceFabricSetupFilter(urlSuffix, options);
            sut.Configure(next)(app.Object);
            _ = Assert.Single(RegisteredMiddlewares().OfType<ServiceFabricReverseProxyIntegrationMiddleware>());
        }

        [Theory, InlineData(ServiceFabricIntegrationOptions.None), InlineData(ServiceFabricIntegrationOptions.UseUniqueServiceUrl)]
        public void ReturnsActionThatDoesNotRegisterReverseProxyIntegrationMiddlewareWhenOptionsDoesNotHaveUseReverseProxyIntegration(ServiceFabricIntegrationOptions options)
        {
            var sut = new ServiceFabricSetupFilter(urlSuffix, options);
            sut.Configure(next)(app.Object);
            Assert.Empty(RegisteredMiddlewares().OfType<ServiceFabricReverseProxyIntegrationMiddleware>());
        }

        [Fact]
        public void ReturnsActionThatRegistersServiceFabricMiddlewareBeforeReverseProxyIntegrationMiddleware()
        {
            var sut = new ServiceFabricSetupFilter(urlSuffix, ServiceFabricIntegrationOptions.UseReverseProxyIntegration);
            sut.Configure(next)(app.Object);

            Type[] middlewareTypes = [.. RegisteredMiddlewares().Select(_ => _.GetType())];
            Assert.Equal([typeof(ServiceFabricMiddleware), typeof(ServiceFabricReverseProxyIntegrationMiddleware)], middlewareTypes);
        }

        [Fact]
        public void ReturnsActionThatRegistersMiddlewaresBeforeCallingNext()
        {
            var sut = new ServiceFabricSetupFilter(urlSuffix, ServiceFabricIntegrationOptions.UseReverseProxyIntegration);
            List<int> middlewareCountsWhenNextCalled = [];
            _ = Mock.Get(next).Setup(_ => _(app.Object)).Callback(() => middlewareCountsWhenNextCalled.Add(factories.Count));

            sut.Configure(next)(app.Object);

            Assert.Equal([factories.Count], middlewareCountsWhenNextCalled);
            Mock.Get(next).Verify(_ => _(It.IsAny<IApplicationBuilder>()), Times.Once);
        }

        IReadOnlyList<object> RegisteredMiddlewares() =>
            [.. factories.Select(f => f(_ => Task.CompletedTask).Target)];
    }
}
