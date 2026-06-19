// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class OperationRetrySettingsTest
{
    readonly OperationRetrySettings sut;

    // Constructor parameters
    readonly TimeSpan maxRetryBackoffIntervalOnTransientErrors = fuzzy.TimeSpan();
    readonly TimeSpan maxRetryBackoffIntervalOnNonTransientErrors = fuzzy.TimeSpan();
    readonly int defaultMaxRetryCountForTransientErrors = fuzzy.Int32();
    readonly int defaultMaxRetryCountForNonTransientErrors = fuzzy.Int32();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    OperationRetrySettingsTest() =>
        sut = new OperationRetrySettings(
            maxRetryBackoffIntervalOnTransientErrors,
            maxRetryBackoffIntervalOnNonTransientErrors,
            defaultMaxRetryCountForTransientErrors,
            defaultMaxRetryCountForNonTransientErrors);

    public sealed class ClientRetryTimeout : OperationRetrySettingsTest
    {
        [Fact]
        public void ReturnsClientRetryTimeoutOfRetryPolicy()
        {
            TimeSpan clientRetryTimeout = fuzzy.TimeSpan();
            IRetryPolicy retryPolicy = Mock.Of<IRetryPolicy>(p => p.ClientRetryTimeout == clientRetryTimeout);
            var sut = new OperationRetrySettings(retryPolicy);
            Assert.Equal(clientRetryTimeout, sut.ClientRetryTimeout);
        }
    }

    public sealed class Constructor : OperationRetrySettingsTest
    {
        [Fact]
        public void InitializesRetryPolicyWithExponentialRetryPolicyDefaults()
        {
            var sut = new OperationRetrySettings();
            Assert.Equal(10, sut.DefaultMaxRetryCountForTransientErrors);
            Assert.Equal(Timeout.InfiniteTimeSpan, sut.ClientRetryTimeout);
        }
    }

    public sealed class Constructor_IRetryPolicy : OperationRetrySettingsTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for retryPolicy.
        public void ThrowsArgumentNullExceptionWhenRetryPolicyIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new OperationRetrySettings(null));
            Assert.Equal(typeof(OperationRetrySettings).Constructor().Parameter<IRetryPolicy>().Name, actual.ParamName);
        }
    }

    public sealed class Constructor_TimeSpan : OperationRetrySettingsTest
    {
        readonly TimeSpan clientRetryTimeout = fuzzy.TimeSpan();

        [Fact]
        public void InitializesRetryPolicyWithExponentialRetryPolicyAndGivenClientRetryTimeout()
        {
            var sut = new OperationRetrySettings(clientRetryTimeout);
            Assert.Equal(10, sut.DefaultMaxRetryCountForTransientErrors);
            Assert.Equal(clientRetryTimeout, sut.ClientRetryTimeout);
        }
    }

    public sealed class Constructor_TimeSpan_TimeSpan_Int32_Int32 : OperationRetrySettingsTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Equal(maxRetryBackoffIntervalOnTransientErrors, sut.MaxRetryBackoffIntervalOnTransientErrors);
            Assert.Equal(maxRetryBackoffIntervalOnNonTransientErrors, sut.MaxRetryBackoffIntervalOnNonTransientErrors);
            Assert.Equal(defaultMaxRetryCountForTransientErrors, sut.DefaultMaxRetryCountForTransientErrors);
            Assert.Equal(defaultMaxRetryCountForNonTransientErrors, sut.DefaultMaxRetryCountForNonTransientErrors);
            Assert.Equal(Timeout.InfiniteTimeSpan, sut.ClientRetryTimeout);
        }

        [Fact]
        public void DefaultsMaxRetryCountForNonTransientErrorsToInt32MaxValue()
        {
            var sut = new OperationRetrySettings(
                maxRetryBackoffIntervalOnTransientErrors,
                maxRetryBackoffIntervalOnNonTransientErrors,
                defaultMaxRetryCountForTransientErrors);
            Assert.Equal(int.MaxValue, sut.DefaultMaxRetryCountForNonTransientErrors);
        }
    }

    public sealed class DefaultMaxRetryCountForNonTransientErrors : OperationRetrySettingsTest
    {
        [Fact]
        public void ReturnsMaxRetryCountOnNonTransientErrorsOfConstantRetryPolicy() =>
            Assert.Equal(defaultMaxRetryCountForNonTransientErrors, sut.DefaultMaxRetryCountForNonTransientErrors);

        [Fact]
        public void ReturnsInt32MaxValueWhenRetryPolicyIsNotConstantRetryPolicy()
        {
            var sut = new OperationRetrySettings(Mock.Of<IRetryPolicy>());
            Assert.Equal(int.MaxValue, sut.DefaultMaxRetryCountForNonTransientErrors);
        }
    }

    public sealed class DefaultMaxRetryCountForTransientErrors : OperationRetrySettingsTest
    {
        [Fact]
        public void ReturnsTotalNumberOfRetriesOfRetryPolicy()
        {
            int totalNumberOfRetries = fuzzy.Int32();
            IRetryPolicy retryPolicy = Mock.Of<IRetryPolicy>(p => p.TotalNumberOfRetries == totalNumberOfRetries);
            var sut = new OperationRetrySettings(retryPolicy);
            Assert.Equal(totalNumberOfRetries, sut.DefaultMaxRetryCountForTransientErrors);
        }
    }

    public sealed class MaxRetryBackoffIntervalOnNonTransientErrors : OperationRetrySettingsTest
    {
        [Fact]
        public void ReturnsMaxRetryBackoffIntervalOnNonTransientErrorsOfConstantRetryPolicy() =>
            Assert.Equal(maxRetryBackoffIntervalOnNonTransientErrors, sut.MaxRetryBackoffIntervalOnNonTransientErrors);

        [Fact]
        public void ThrowsNotSupportedExceptionWhenRetryPolicyIsNotConstantRetryPolicy()
        {
            var sut = new OperationRetrySettings(Mock.Of<IRetryPolicy>());
            _ = Assert.Throws<NotSupportedException>(() => sut.MaxRetryBackoffIntervalOnNonTransientErrors);
        }
    }

    public sealed class MaxRetryBackoffIntervalOnTransientErrors : OperationRetrySettingsTest
    {
        [Fact]
        public void ReturnsMaxRetryBackoffIntervalOnTransientErrorsOfConstantRetryPolicy() =>
            Assert.Equal(maxRetryBackoffIntervalOnTransientErrors, sut.MaxRetryBackoffIntervalOnTransientErrors);

        [Fact]
        public void ThrowsNotSupportedExceptionWhenRetryPolicyIsNotConstantRetryPolicy()
        {
            var sut = new OperationRetrySettings(Mock.Of<IRetryPolicy>());
            _ = Assert.Throws<NotSupportedException>(() => sut.MaxRetryBackoffIntervalOnTransientErrors);
        }
    }

    public sealed class RetryPolicy : OperationRetrySettingsTest
    {
        [Fact]
        public void ReturnsRetryPolicyGivenToConstructor()
        {
            var retryPolicy = Mock.Of<IRetryPolicy>();
            var sut = new OperationRetrySettings(retryPolicy);
            Assert.Same(retryPolicy, sut.RetryPolicy);
        }
    }
}
