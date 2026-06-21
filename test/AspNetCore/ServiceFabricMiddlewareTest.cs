// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Threading.Tasks;
using Fuzzy;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore;

public abstract class ServiceFabricMiddlewareTest
{
    readonly ServiceFabricMiddleware sut;

    // Constructor parameters
    readonly Mock<RequestDelegate> next = new();
    readonly string urlSuffix = "/" + fuzzy.String().LettersOrDigits();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceFabricMiddlewareTest() =>
        sut = new ServiceFabricMiddleware(next.Object, urlSuffix);

    public sealed class Constructor : ServiceFabricMiddlewareTest
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenNextIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ServiceFabricMiddleware(null, urlSuffix));
            Assert.Equal(nameof(next), exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenUrlSuffixIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new ServiceFabricMiddleware(next.Object, null));
            Assert.Equal(nameof(urlSuffix), exception.ParamName);
        }
    }

    public sealed class Invoke : ServiceFabricMiddlewareTest
    {
        // Method parameters
        readonly HttpContext context = new DefaultHttpContext();

        readonly PathString originalPathBase = "/" + fuzzy.String().LettersOrDigits();

        public Invoke() => context.Request.PathBase = originalPathBase;

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenContextIsNull()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Invoke(null));
            Assert.Equal(nameof(context), exception.ParamName);
        }

        [Fact]
        public async Task CallsNextWhenUrlSuffixIsEmpty()
        {
            // Arrange
            PathString originalPath = "/" + fuzzy.String().LettersOrDigits();
            context.Request.Path = originalPath;

            var middleware = new ServiceFabricMiddleware(next.Object, string.Empty);
            CapturedRequest captured = SetupNextToCaptureRequest();

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(originalPath, captured.Path);
            Assert.Equal(originalPathBase, captured.PathBase);
            Assert.Equal(originalPath, context.Request.Path);
            Assert.Equal(originalPathBase, context.Request.PathBase);
            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task SetsStatusCodeGoneWhenPathDoesNotStartWithUrlSuffixSegment()
        {
            var originalPath = new PathString(urlSuffix + fuzzy.String().LettersOrDigits()); // appended without '/' so not a segment match
            context.Request.Path = originalPath;

            await sut.Invoke(context);

            Assert.Equal(StatusCodes.Status410Gone, context.Response.StatusCode);
            Assert.Equal(originalPath, context.Request.Path);
            Assert.Equal(originalPathBase, context.Request.PathBase);
            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task CallsNextWithRemainingPathAndExtendedPathBaseWhenPathStartsWithUrlSuffix()
        {
            // Arrange
            string remainingPath = "/" + fuzzy.String().LettersOrDigits();
            PathString originalPath = urlSuffix + remainingPath;
            context.Request.Path = originalPath;

            CapturedRequest captured = SetupNextToCaptureRequest();

            // Act
            await sut.Invoke(context);

            // Assert
            Assert.Equal(remainingPath, captured.Path.Value);
            Assert.Equal(originalPathBase + urlSuffix, captured.PathBase.Value);
            Assert.Equal(originalPath, context.Request.Path);
            Assert.Equal(originalPathBase, context.Request.PathBase);
            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task CallsNextWithEmptyPathAndExtendedPathBaseWhenPathEqualsUrlSuffix()
        {
            // Arrange
            PathString originalPath = urlSuffix;
            context.Request.Path = originalPath;

            CapturedRequest captured = SetupNextToCaptureRequest();

            // Act
            await sut.Invoke(context);

            // Assert
            Assert.Equal(PathString.Empty, captured.Path);
            Assert.Equal(originalPathBase + urlSuffix, captured.PathBase.Value);
            Assert.Equal(originalPath, context.Request.Path);
            Assert.Equal(originalPathBase, context.Request.PathBase);
            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task RestoresPathAndPathBaseWhenNextThrows()
        {
            // Arrange
            string remainingPath = "/" + fuzzy.String().LettersOrDigits();
            PathString originalPath = urlSuffix + remainingPath;
            context.Request.Path = originalPath;

            var expected = new InvalidOperationException();
            _ = next.Setup(_ => _(context)).ThrowsAsync(expected);

            // Act
            var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Invoke(context));

            // Assert
            Assert.Same(expected, actual);
            Assert.Equal(originalPath, context.Request.Path);
            Assert.Equal(originalPathBase, context.Request.PathBase);
            next.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        CapturedRequest SetupNextToCaptureRequest()
        {
            var captured = new CapturedRequest();
            _ = next.Setup(_ => _(context))
                .Callback<HttpContext>(c => { captured.Path = c.Request.Path; captured.PathBase = c.Request.PathBase; })
                .Returns(Task.FromResult(fuzzy.Int32()));
            return captured;
        }

        sealed class CapturedRequest
        {
            internal PathString Path;
            internal PathString PathBase;
        }
    }
}
