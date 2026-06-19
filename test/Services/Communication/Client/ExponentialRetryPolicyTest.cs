// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ExponentialRetryPolicyTest
{
    readonly ExponentialRetryPolicy sut;

    // Constructor parameters
    readonly int defaultMaxRetryCount = fuzzy.Int32();
    readonly TimeSpan maxRetryJitter = fuzzy.TimeSpan()
        .Minimum(TimeSpan.FromMilliseconds(100))
        .Maximum(TimeSpan.FromMilliseconds(1000));
    readonly TimeSpan clientRetryTimeout = fuzzy.TimeSpan();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ExponentialRetryPolicyTest() =>
        sut = new ExponentialRetryPolicy(defaultMaxRetryCount, maxRetryJitter, clientRetryTimeout);

    public sealed class BaseRetryDelay : ExponentialRetryPolicyTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            TimeSpan expected = fuzzy.TimeSpan();
            sut.BaseRetryDelay = expected;
            Assert.Equal(expected, sut.BaseRetryDelay);
        }
    }

    public sealed class Constructor_Int32_TimeSpan : ExponentialRetryPolicyTest
    {
        new readonly ExponentialRetryPolicy sut;

        public Constructor_Int32_TimeSpan() =>
            sut = new ExponentialRetryPolicy(defaultMaxRetryCount, clientRetryTimeout);

        [Fact]
        public void InitializesProperties()
        {
            Assert.Equal(defaultMaxRetryCount, sut.TotalNumberOfRetries);
            Assert.Equal(clientRetryTimeout, sut.ClientRetryTimeout);
            Assert.Equal(TimeSpan.FromSeconds(1), sut.BaseRetryDelay);
        }

        [Fact]
        public void UsesTwoSecondsAsMaxRetryJitter()
        {
            // Eliminate the base delay term so the observed delay is the jitter component alone.
            sut.BaseRetryDelay = TimeSpan.Zero;
            var parameters = new RetryDelayParameters(0, fuzzy.Boolean());
            var expectedMax = TimeSpan.FromSeconds(2);

            var observedMax = TimeSpan.Zero;
            for (int i = 0; i < Samples; i++)
            {
                TimeSpan delay = sut.GetNextRetryDelay(parameters);
                Assert.InRange(delay, TimeSpan.Zero, expectedMax);
                if (delay > observedMax)
                    observedMax = delay;
            }

            // Probability of every sample falling below half of the configured max with a uniform [0,1) scale is 2^-Samples.
            Assert.True(observedMax.Ticks > expectedMax.Ticks / 2);
        }
    }

    public sealed class Constructor_Int32_TimeSpan_TimeSpan : ExponentialRetryPolicyTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Equal(defaultMaxRetryCount, sut.TotalNumberOfRetries);
            Assert.Equal(clientRetryTimeout, sut.ClientRetryTimeout);
            Assert.Equal(TimeSpan.FromSeconds(1), sut.BaseRetryDelay);
        }

        [Fact]
        public void UsesGivenMaxRetryJitter()
        {
            // Eliminate the base delay term so the observed delay is the jitter component alone.
            sut.BaseRetryDelay = TimeSpan.Zero;
            var parameters = new RetryDelayParameters(0, fuzzy.Boolean());

            var observedMax = TimeSpan.Zero;
            for (int i = 0; i < Samples; i++)
            {
                TimeSpan delay = sut.GetNextRetryDelay(parameters);
                Assert.InRange(delay, TimeSpan.Zero, maxRetryJitter);
                if (delay > observedMax)
                    observedMax = delay;
            }

            Assert.True(observedMax.Ticks > maxRetryJitter.Ticks / 2);
        }
    }

    public sealed class GetNextRetryDelay : ExponentialRetryPolicyTest
    {
        new readonly IRetryPolicy sut;

        readonly TimeSpan baseRetryDelay = fuzzy.TimeSpan()
            .Minimum(TimeSpan.FromMilliseconds(100))
            .Maximum(TimeSpan.FromMilliseconds(1000));

        public GetNextRetryDelay()
        {
            base.sut.BaseRetryDelay = baseRetryDelay;
            sut = base.sut;
        }

        [Fact]
        public void ReturnsBaseDelayPlusJitterWhenRetryAttemptIsBelowSameDelayRequestCounter()
        {
            int retryAttempt = fuzzy.Int32().Between(0, ExponentialRetryPolicy.SameDelayRequestCounter - 1);
            AssertDelayWithinExpectedRange(retryAttempt, delayMultiplier: 0);
        }

        [Fact]
        public void ShiftsBaseDelayByDelayMultiplierWhenRetryAttemptIsMultipleOfSameDelayRequestCounter()
        {
            int delayMultiplier = fuzzy.Int32().Between(1, ExponentialRetryPolicy.MaxDelayMultiplier - 1);
            int retryAttempt = delayMultiplier * ExponentialRetryPolicy.SameDelayRequestCounter;
            AssertDelayWithinExpectedRange(retryAttempt, delayMultiplier);
        }

        [Fact]
        public void UsesSameBaseDelayForSameDelayRequestCounterConsecutiveAttempts()
        {
            int delayMultiplier = fuzzy.Int32().Between(0, ExponentialRetryPolicy.MaxDelayMultiplier - 1);
            int firstAttempt = delayMultiplier * ExponentialRetryPolicy.SameDelayRequestCounter;
            int lastAttempt = firstAttempt + ExponentialRetryPolicy.SameDelayRequestCounter - 1;
            AssertDelayWithinExpectedRange(firstAttempt, delayMultiplier);
            AssertDelayWithinExpectedRange(lastAttempt, delayMultiplier);
        }

        [Fact]
        public void CapsDelayMultiplierAtMaxDelayMultiplier()
        {
            int extraAttempts = fuzzy.Int32().Between(1, 100);
            int retryAttempt = (ExponentialRetryPolicy.MaxDelayMultiplier + extraAttempts)
                * ExponentialRetryPolicy.SameDelayRequestCounter;
            AssertDelayWithinExpectedRange(retryAttempt, ExponentialRetryPolicy.MaxDelayMultiplier);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for retryDelayParameters.
        public void ThrowsArgumentNullExceptionWhenRetryDelayParametersIsNull() =>
            _ = Assert.Throws<ArgumentNullException>(() => sut.GetNextRetryDelay(null));

        void AssertDelayWithinExpectedRange(int retryAttempt, int delayMultiplier)
        {
            var parameters = new RetryDelayParameters(retryAttempt, fuzzy.Boolean());

            var expectedBaseMs = (long)((int)baseRetryDelay.TotalMilliseconds << delayMultiplier);
            var expectedMin = TimeSpan.FromMilliseconds(expectedBaseMs);
            var expectedMax = TimeSpan.FromMilliseconds(expectedBaseMs + maxRetryJitter.TotalMilliseconds);

            var observedMin = TimeSpan.MaxValue;
            var observedMax = TimeSpan.Zero;
            for (int i = 0; i < Samples; i++)
            {
                TimeSpan delay = sut.GetNextRetryDelay(parameters);
                Assert.InRange(delay, expectedMin, expectedMax);
                if (delay < observedMin)
                    observedMin = delay;
                if (delay > observedMax)
                    observedMax = delay;
            }

            // The random jitter term must contribute to the observed delay.
            Assert.True((observedMax - observedMin).Ticks > maxRetryJitter.Ticks / 2);
        }
    }

    public sealed class MaxDelayMultiplier : ExponentialRetryPolicyTest
    {
        [Fact]
        public void DefaultIsNine() =>
            Assert.Equal(9, ExponentialRetryPolicy.MaxDelayMultiplier);

        [Fact]
        public void CapsDelayMultiplierUsedByGetNextRetryDelay()
        {
            int original = ExponentialRetryPolicy.MaxDelayMultiplier;
            try
            {
                int newMax = fuzzy.Int32().Between(1, original - 1);
                ExponentialRetryPolicy.MaxDelayMultiplier = newMax;
                // Construct a policy with zero jitter so the cap can be asserted exactly: a no-op setter would leave
                // the original (larger) cap in effect, producing a strictly larger shifted base delay.
                var policy = new ExponentialRetryPolicy(defaultMaxRetryCount, TimeSpan.Zero, clientRetryTimeout);
                int retryAttempt = (newMax + fuzzy.Int32().Between(1, 10))
                    * ExponentialRetryPolicy.SameDelayRequestCounter;
                var expected = TimeSpan.FromMilliseconds((int)policy.BaseRetryDelay.TotalMilliseconds << newMax);
                TimeSpan delay = policy.GetNextRetryDelay(new RetryDelayParameters(retryAttempt, fuzzy.Boolean()));
                Assert.Equal(expected, delay);
            }
            finally
            {
                ExponentialRetryPolicy.MaxDelayMultiplier = original;
            }
        }
    }

    public sealed class SameDelayRequestCounter : ExponentialRetryPolicyTest
    {
        [Fact]
        public void DefaultIsThree() =>
            Assert.Equal(3, ExponentialRetryPolicy.SameDelayRequestCounter);

        [Fact]
        public void DeterminesGroupingUsedByGetNextRetryDelay()
        {
            int original = ExponentialRetryPolicy.SameDelayRequestCounter;
            try
            {
                // Doubling the counter ensures a no-op setter (keeping `original`) would produce a different multiplier
                // (`2 * delayMultiplier`) than the expected one (`delayMultiplier`), making the test distinguish setter
                // behavior. Restrict delayMultiplier so the no-op multiplier stays below MaxDelayMultiplier (avoiding
                // the cap masking the difference).
                int newCounter = original * 2;
                ExponentialRetryPolicy.SameDelayRequestCounter = newCounter;
                // Construct a policy with zero jitter so the multiplier-driven shift can be asserted exactly.
                var policy = new ExponentialRetryPolicy(defaultMaxRetryCount, TimeSpan.Zero, clientRetryTimeout);
                int delayMultiplier = fuzzy.Int32().Between(1, (ExponentialRetryPolicy.MaxDelayMultiplier - 1) / 2);
                int retryAttempt = delayMultiplier * newCounter;
                var expected = TimeSpan.FromMilliseconds((int)policy.BaseRetryDelay.TotalMilliseconds << delayMultiplier);
                TimeSpan delay = policy.GetNextRetryDelay(new RetryDelayParameters(retryAttempt, fuzzy.Boolean()));
                Assert.Equal(expected, delay);
            }
            finally
            {
                ExponentialRetryPolicy.SameDelayRequestCounter = original;
            }
        }
    }

    const int Samples = 100;
}
