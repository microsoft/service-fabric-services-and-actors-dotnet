// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using Xunit;

    /// <summary>
    /// Tests for Exception retyr.
    /// </summary>
    public class TestExceptionRetryLogic
    {
        /// <summary>
        /// Test with MaxRetryCount = 0.
        /// </summary>
        [Fact]
        public void TestIfMaxRetryCountIsZero()
        {
            var currentExceptionId = "1234";
            var maxRetryCount = 0;
            var lastSeenExceptionId = "1";
            var retryCount = 0;
            var shouldRetry = Communication.Client.Utility.ShouldRetryOperation(
                currentExceptionId,
                maxRetryCount,
                ref lastSeenExceptionId,
                ref retryCount);
            Assert.False(shouldRetry);
            Assert.Equal(0, retryCount);
        }

        /// <summary>
        /// Test with MaxRetryCount > 0.
        /// </summary>
        [Fact]
        public void TestIfMaxRetryCountIsMorethanZero()
        {
            var currentExceptionId = "1234";
            var maxRetryCount = 1;
            var lastSeenExceptionId = "1";
            var retryCount = 0;

            // First Time it should retry
            var shouldRetry = Communication.Client.Utility.ShouldRetryOperation(
                currentExceptionId,
                maxRetryCount,
                ref lastSeenExceptionId,
                ref retryCount);
            Assert.True(shouldRetry);
            Assert.Equal(1, retryCount);
            Assert.Equal(currentExceptionId, lastSeenExceptionId);

            // second time it should not
            var shouldRetrySecondTime = Communication.Client.Utility.ShouldRetryOperation(
                lastSeenExceptionId,
                maxRetryCount,
                ref lastSeenExceptionId,
                ref retryCount);
            Assert.False(shouldRetrySecondTime);
            Assert.Equal(1, retryCount);
        }
    }
}
