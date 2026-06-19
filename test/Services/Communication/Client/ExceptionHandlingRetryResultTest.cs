// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ExceptionHandlingRetryResultTest
{
    readonly ExceptionHandlingRetryResult sut;

    // Constructor parameters
    readonly Exception exception = new InvalidOperationException();
    readonly bool isTransient = fuzzy.Boolean();
    readonly TimeSpan retryDelay = fuzzy.TimeSpan();
    readonly int maxRetryCount = fuzzy.Int32();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ExceptionHandlingRetryResultTest() =>
        sut = new ExceptionHandlingRetryResult(exception, isTransient, retryDelay, maxRetryCount);

    public sealed class Constructor_Exception_Boolean_OperationRetrySettings : ExceptionHandlingRetryResultTest
    {
        readonly Mock<IRetryPolicy> retryPolicy = new();
        readonly OperationRetrySettings retrySettings;
        readonly int totalNumberOfRetries = fuzzy.Int32();
        readonly TimeSpan initialRetryDelay = fuzzy.TimeSpan();

        public Constructor_Exception_Boolean_OperationRetrySettings() =>
            retrySettings = new OperationRetrySettings(retryPolicy.Object);

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            if (isTransient)
                _ = retryPolicy.SetupGet(_ => _.TotalNumberOfRetries).Returns(totalNumberOfRetries);
            _ = retryPolicy
                .Setup(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(
                    p => p.RetryAttempt == 0 && p.IsTransient == isTransient)))
                .Returns(initialRetryDelay);

            var sut = new ExceptionHandlingRetryResult(exception, isTransient, retrySettings);

            Assert.Equal(exception.GetType().FullName, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(initialRetryDelay, sut.RetryDelay);
            // OperationRetrySettings.DefaultMaxRetryCountForNonTransientErrors returns int.MaxValue
            // when the underlying retry policy is not a ConstantRetryPolicy.
            Assert.Equal(isTransient ? totalNumberOfRetries : int.MaxValue, sut.MaxRetryCount);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for exception.
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionHandlingRetryResult(null, isTransient, retrySettings));
            Assert.Equal(nameof(exception), actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for retrySettings.
        public void ThrowsArgumentNullExceptionWhenRetrySettingsIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionHandlingRetryResult(exception, isTransient, (OperationRetrySettings)null));
            Assert.Equal(nameof(retrySettings), actual.ParamName);
        }
    }

    public sealed class Constructor_Exception_Boolean_OperationRetrySettings_Int32 : ExceptionHandlingRetryResultTest
    {
        readonly Mock<IRetryPolicy> retryPolicy = new();
        readonly OperationRetrySettings retrySettings;
        readonly TimeSpan initialRetryDelay = fuzzy.TimeSpan();

        public Constructor_Exception_Boolean_OperationRetrySettings_Int32() =>
            retrySettings = new OperationRetrySettings(retryPolicy.Object);

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            _ = retryPolicy
                .Setup(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(
                    p => p.RetryAttempt == 0 && p.IsTransient == isTransient)))
                .Returns(initialRetryDelay);

            var sut = new ExceptionHandlingRetryResult(exception, isTransient, retrySettings, maxRetryCount);

            Assert.Equal(exception.GetType().FullName, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(initialRetryDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for exception.
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionHandlingRetryResult(null, isTransient, retrySettings, maxRetryCount));
            Assert.Equal(nameof(exception), actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for retrySettings.
        public void ThrowsArgumentNullExceptionWhenRetrySettingsIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionHandlingRetryResult(exception, isTransient, (OperationRetrySettings)null, maxRetryCount));
            Assert.Equal(nameof(retrySettings), actual.ParamName);
        }
    }

    public sealed class Constructor_Exception_Boolean_TimeSpan_Int32 : ExceptionHandlingRetryResultTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            var sut = new ExceptionHandlingRetryResult(exception, isTransient, retryDelay, maxRetryCount);

            Assert.Equal(exception.GetType().FullName, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(retryDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for exception.
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionHandlingRetryResult((Exception)null, isTransient, retryDelay, maxRetryCount));
            Assert.Equal(nameof(exception), actual.ParamName);
        }
    }

    public sealed class Constructor_String_Boolean_TimeSpan_Int32 : ExceptionHandlingRetryResultTest
    {
        readonly string exceptionId = fuzzy.String();

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            var sut = new ExceptionHandlingRetryResult(exceptionId, isTransient, retryDelay, maxRetryCount);

            Assert.Same(exceptionId, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(retryDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
        }

        [Fact]
        public void InitializesExceptionIdToNullWhenExceptionIdIsNull()
        {
            var sut = new ExceptionHandlingRetryResult((string)null, isTransient, retryDelay, maxRetryCount);
            Assert.Null(sut.ExceptionId);
        }
    }

    public sealed class GetRetryDelay : ExceptionHandlingRetryResultTest
    {
        // Method parameters
        readonly int retryAttempt = fuzzy.Int32();

        [Fact]
        public void ReturnsRetryDelayPassedToConstructorWhenRetrySettingsIsNull() =>
            Assert.Equal(retryDelay, sut.GetRetryDelay(retryAttempt));

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReturnsDelayFromRetryPolicyWhenRetrySettingsIsNotNull(bool isTransient)
        {
            var retryPolicy = new Mock<IRetryPolicy>();
            var expected = fuzzy.TimeSpan();
            _ = retryPolicy
                .Setup(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(
                    p => p.RetryAttempt == retryAttempt && p.IsTransient == isTransient)))
                .Returns(expected);
            var settings = new OperationRetrySettings(retryPolicy.Object);
            var sut = new ExceptionHandlingRetryResult(exception, isTransient, settings, maxRetryCount);

            Assert.Equal(expected, sut.GetRetryDelay(retryAttempt));
        }
    }
}
