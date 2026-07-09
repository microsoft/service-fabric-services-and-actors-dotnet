// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ExceptionHandlingRetryResultTest
{
    // Constructor parameters
    readonly TestException exception = new();
    readonly string exceptionId = fuzzy.String();
    readonly TimeSpan retryDelay = fuzzy.TimeSpan();
    readonly OperationRetrySettings retrySettings;
    readonly int maxRetryCount = fuzzy.Int32();

    // Fixture
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
    readonly Mock<IRetryPolicy> retryPolicy = new();

    ExceptionHandlingRetryResultTest() =>
        retrySettings = new OperationRetrySettings(retryPolicy.Object);

    public sealed class Constructor_Exception_Boolean_OperationRetrySettings_Int32 : ExceptionHandlingRetryResultTest
    {
        [Theory, InlineData(true), InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            TimeSpan expectedDelay = fuzzy.TimeSpan();
            _ = retryPolicy.Setup(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(p => p.RetryAttempt == 0 && p.IsTransient == isTransient))).Returns(expectedDelay);

            ExceptionHandlingRetryResult sut = new(exception, isTransient, retrySettings, maxRetryCount);

            Assert.Equal(exception.GetType().FullName, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(expectedDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
            retryPolicy.Verify(_ => _.GetNextRetryDelay(It.IsAny<RetryDelayParameters>()), Times.Once);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            ArgumentNullException actual = Assert.Throws<ArgumentNullException>(() => new ExceptionHandlingRetryResult((Exception)null, fuzzy.Boolean(), retrySettings, maxRetryCount));
            Assert.Equal(nameof(exception), actual.ParamName);
        }
    }

    public sealed class Constructor_Exception_Boolean_TimeSpan_Int32 : ExceptionHandlingRetryResultTest
    {
        [Theory, InlineData(true), InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            ExceptionHandlingRetryResult sut = new(exception, isTransient, retryDelay, maxRetryCount);

            Assert.Equal(exception.GetType().FullName, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(retryDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            ArgumentNullException actual = Assert.Throws<ArgumentNullException>(() => new ExceptionHandlingRetryResult((Exception)null, fuzzy.Boolean(), retryDelay, maxRetryCount));
            Assert.Equal(nameof(exception), actual.ParamName);
        }
    }

    public sealed class Constructor_String_Boolean_OperationRetrySettings_Int32 : ExceptionHandlingRetryResultTest
    {
        [Theory, InlineData(true), InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            TimeSpan expectedDelay = fuzzy.TimeSpan();
            _ = retryPolicy.Setup(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(p => p.RetryAttempt == 0 && p.IsTransient == isTransient))).Returns(expectedDelay);

            ExceptionHandlingRetryResult sut = new(exceptionId, isTransient, retrySettings, maxRetryCount);

            Assert.Same(exceptionId, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(expectedDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
            retryPolicy.Verify(_ => _.GetNextRetryDelay(It.IsAny<RetryDelayParameters>()), Times.Once);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenRetrySettingsIsNull()
        {
            ArgumentNullException actual = Assert.Throws<ArgumentNullException>(() => new ExceptionHandlingRetryResult(exceptionId, fuzzy.Boolean(), null, maxRetryCount));
            Assert.Equal(nameof(retrySettings), actual.ParamName);
        }
    }

    public sealed class Constructor_String_Boolean_TimeSpan_Int32 : ExceptionHandlingRetryResultTest
    {
        [Theory, InlineData(true), InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            ExceptionHandlingRetryResult sut = new(exceptionId, isTransient, retryDelay, maxRetryCount);

            Assert.Same(exceptionId, sut.ExceptionId);
            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(retryDelay, sut.RetryDelay);
            Assert.Equal(maxRetryCount, sut.MaxRetryCount);
        }
    }

    public sealed class GetRetryDelay : ExceptionHandlingRetryResultTest
    {
        // Method parameters
        readonly int retryAttempt = fuzzy.Int32().Minimum(1); // Must differ from the constructor's hard-coded attempt 0 to avoid verifying an extra call.

        [Fact]
        public void ReturnsRetryDelayWhenRetrySettingsWasNotProvided()
        {
            ExceptionHandlingRetryResult sut = new(exceptionId, fuzzy.Boolean(), retryDelay, maxRetryCount);
            Assert.Equal(retryDelay, sut.GetRetryDelay(retryAttempt));
        }

        [Theory, InlineData(true), InlineData(false)]
        public void ComputesRetryDelayFromRetryPolicyWhenRetrySettingsWasProvided(bool isTransient)
        {
            TimeSpan expected = fuzzy.TimeSpan();
            _ = retryPolicy.Setup(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(p => p.RetryAttempt == retryAttempt && p.IsTransient == isTransient))).Returns(expected);

            ExceptionHandlingRetryResult sut = new(exceptionId, isTransient, retrySettings, maxRetryCount);

            Assert.Equal(expected, sut.GetRetryDelay(retryAttempt));
            retryPolicy.Verify(_ => _.GetNextRetryDelay(It.Is<RetryDelayParameters>(p => p.RetryAttempt == retryAttempt && p.IsTransient == isTransient)), Times.Once);
            retryPolicy.Verify(_ => _.GetNextRetryDelay(It.IsAny<RetryDelayParameters>()), Times.Exactly(2)); // Constructor call plus this call; rules out unexpected extra calls.
        }
    }

    sealed class TestException : Exception;
}
