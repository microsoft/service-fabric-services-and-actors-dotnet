// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ConstantRetryPolicyTest
{
    readonly ConstantRetryPolicy sut;

    // Constructor parameters
    readonly TimeSpan maxRetryBackoffIntervalOnTransientErrors = fuzzy.TimeSpan()
        .Minimum(TimeSpan.FromTicks(1000))
        .Maximum(TimeSpan.FromTicks(TimeSpan.MaxValue.Ticks / 3));
    readonly TimeSpan maxRetryBackoffIntervalOnNonTransientErrors;
    readonly int maxRetryCount = fuzzy.Int32();
    readonly int maxRetryCountOnNonTransientErrors = fuzzy.Int32();
    readonly TimeSpan clientRetryTimeout = fuzzy.TimeSpan();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ConstantRetryPolicyTest()
    {
        // Derive non-transient maximum from transient so the two are separated by more than 2x.
        // This makes the branch selection in GetNextRetryDelay testable: swapping the two fields
        // would land the observed delay outside (expectedMax/2, expectedMax] and fail the assertions.
        maxRetryBackoffIntervalOnNonTransientErrors = maxRetryBackoffIntervalOnTransientErrors
            + fuzzy.TimeSpan()
                .Minimum(maxRetryBackoffIntervalOnTransientErrors)
                .Maximum(TimeSpan.MaxValue - maxRetryBackoffIntervalOnTransientErrors);
        sut = new ConstantRetryPolicy(
            maxRetryBackoffIntervalOnTransientErrors,
            maxRetryBackoffIntervalOnNonTransientErrors,
            maxRetryCount,
            maxRetryCountOnNonTransientErrors,
            clientRetryTimeout);
    }

    public sealed class Constructor : ConstantRetryPolicyTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Equal(maxRetryBackoffIntervalOnTransientErrors, sut.MaxRetryBackoffIntervalOnTransientErrors);
            Assert.Equal(maxRetryBackoffIntervalOnNonTransientErrors, sut.MaxRetryBackoffIntervalOnNonTransientErrors);
            Assert.Equal(maxRetryCount, sut.TotalNumberOfRetries);
            Assert.Equal(maxRetryCountOnNonTransientErrors, sut.MaxRetryCountOnNonTransientErrors);
            Assert.Equal(clientRetryTimeout, sut.ClientRetryTimeout);
        }
    }

    public sealed class GetNextRetryDelay : ConstantRetryPolicyTest
    {
        new readonly IRetryPolicy sut;

        // Method parameters
        RetryDelayParameters retryDelayParameters;

        readonly int retryAttempt = fuzzy.Int32();

        const int Samples = 100;

        public GetNextRetryDelay() => sut = base.sut;

        [Fact]
        public void ReturnsRandomDelayScaledByMaxRetryBackoffIntervalOnTransientErrorsWhenIsTransient() =>
            AssertDelayScaledBy(isTransient: true, maxRetryBackoffIntervalOnTransientErrors);

        [Fact]
        public void ReturnsRandomDelayScaledByMaxRetryBackoffIntervalOnNonTransientErrorsWhenIsNotTransient() =>
            AssertDelayScaledBy(isTransient: false, maxRetryBackoffIntervalOnNonTransientErrors);

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for retryDelayParameters.
        public void ThrowsArgumentNullExceptionWhenRetryDelayParametersIsNull() =>
            _ = Assert.Throws<ArgumentNullException>(() => sut.GetNextRetryDelay(null));

        void AssertDelayScaledBy(bool isTransient, TimeSpan expectedMax)
        {
            retryDelayParameters = new RetryDelayParameters(retryAttempt, isTransient);

            TimeSpan observedMax = TimeSpan.Zero;
            for (int i = 0; i < Samples; i++)
            {
                TimeSpan delay = sut.GetNextRetryDelay(retryDelayParameters);
                Assert.InRange(delay, TimeSpan.Zero, expectedMax);
                if (delay > observedMax)
                    observedMax = delay;
            }

            // Probability of every sample falling below half of the configured max with a uniform [0,1) scale is 2^-Samples.
            Assert.True(observedMax.Ticks > expectedMax.Ticks / 2);
        }
    }
}
