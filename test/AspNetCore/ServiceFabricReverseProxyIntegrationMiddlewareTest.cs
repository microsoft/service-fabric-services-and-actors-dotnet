// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class ServiceFabricReverseProxyIntegrationMiddlewareTest
{
    readonly ServiceFabricReverseProxyIntegrationMiddleware sut;

    // Constructor parameters
    readonly Mock<RequestDelegate> next = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceFabricReverseProxyIntegrationMiddlewareTest() =>
        sut = new ServiceFabricReverseProxyIntegrationMiddleware(next.Object);

    public sealed class Constructor : ServiceFabricReverseProxyIntegrationMiddlewareTest
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenNextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ServiceFabricReverseProxyIntegrationMiddleware(null));
            Assert.Equal(nameof(next), exception.ParamName);
        }
    }

    public sealed class Invoke : ServiceFabricReverseProxyIntegrationMiddlewareTest
    {
        // Method parameters
        readonly Mock<HttpContext> context = new();

        readonly Mock<HttpResponse> response = new();
        Func<object, Task> actualCallback;
        object actualState;

        public Invoke()
        {
            _ = context.SetupGet(_ => _.Response).Returns(response.Object);
            _ = response.Setup(_ => _.OnStarting(It.IsAny<Func<object, Task>>(), response.Object))
                .Callback<Func<object, Task>, object>((callback, state) =>
                {
                    actualCallback = callback;
                    actualState = state;
                });
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenContextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => { _ = sut.Invoke(null); });
            Assert.Equal(nameof(context), exception.ParamName);
        }

        [Fact]
        public void ReturnsTaskReturnedByNext()
        {
            Task<int> expected = Task.FromResult(fuzzy.Int32());
            _ = next.Setup(_ => _(context.Object)).Returns(expected);

            Task actual = sut.Invoke(context.Object);

            Assert.Same(expected, actual);
            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public void RegistersOnStartingCallbackWithResponseAsState()
        {
            _ = sut.Invoke(context.Object);

            Assert.Same(response.Object, actualState);
            response.Verify(_ => _.OnStarting(It.IsAny<Func<object, Task>>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void RegistersOnStartingCallbackBeforeInvokingNext()
        {
            _ = next.Setup(_ => _(context.Object))
                .Callback(() => Assert.NotNull(actualCallback))
                .Returns(Task.FromResult(fuzzy.Int32()));

            _ = sut.Invoke(context.Object);

            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task CallbackSetsXServiceFabricHeaderToResourceNotFoundWhenResponseStatusCodeIs404()
        {
            var headers = new HeaderDictionary();
            _ = response.SetupGet(_ => _.Headers).Returns(headers);
            _ = response.SetupGet(_ => _.StatusCode).Returns(StatusCodes.Status404NotFound);
            _ = sut.Invoke(context.Object);

            await actualCallback(actualState);

            Assert.Equal("ResourceNotFound", headers["X-ServiceFabric"]);
        }

        [Theory]
        [InlineData(StatusCodes.Status200OK)]
        [InlineData(StatusCodes.Status301MovedPermanently)]
        [InlineData(StatusCodes.Status400BadRequest)]
        [InlineData(StatusCodes.Status403Forbidden)]
        [InlineData(StatusCodes.Status500InternalServerError)]
        public async Task CallbackDoesNotSetXServiceFabricHeaderWhenResponseStatusCodeIsNot404(int statusCode)
        {
            var headers = new HeaderDictionary();
            _ = response.SetupGet(_ => _.Headers).Returns(headers);
            _ = response.SetupGet(_ => _.StatusCode).Returns(statusCode);
            _ = sut.Invoke(context.Object);

            await actualCallback(actualState);

            Assert.False(headers.ContainsKey("X-ServiceFabric"));
        }
    }
}
