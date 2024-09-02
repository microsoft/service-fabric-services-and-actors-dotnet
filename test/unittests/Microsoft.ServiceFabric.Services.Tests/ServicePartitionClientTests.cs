// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Castle.Core.Logging;
    using FluentAssertions;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Moq;
    using Xunit;

    /// <summary>
    /// Tests logic related to ServicePartitionClient class.
    /// </summary>
    public class ServicePartitionClientTests
    {
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);
        private static readonly Uri ExampleUri = new Uri("fabric:/fake/service");
        private static readonly Client.ServicePartitionKey ExampleServicePartitionKey = new Client.ServicePartitionKey(1);
        private static readonly TargetReplicaSelector ExampleTargetReplicaSelector = TargetReplicaSelector.PrimaryReplica;
        private static readonly string ExampleListenerName = "DefaultListener";

        private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromMilliseconds(50);

        /// <summary>
        /// Tests handling of cancellation by the passed cancellation token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CancelOnToken()
        {
            var retryCount = 5;
            var clientRetryTimeout = TimeSpan.FromMinutes(1);
            var retryDelay = DefaultRetryDelay;

            var result = await this.SetupCancelTestAsync(
                clientRetryTimeout,
                retryCount,
                retryDelay);

            result.ExceptionFromInvoke.Should().BeAssignableTo(typeof(OperationCanceledException), "Should indicate a canceled operation.");
            result.CallCount.Should().Be(retryCount, "Should cancel when token is signaled.");
            result.CancellationTokenSource.Token.IsCancellationRequested.Should().Be(true, "Cancellation should have occured due to the token.");
        }

        /// <summary>
        /// Tests handling of cancellation by the operation timer.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CancelOnTimer()
        {
            var clientRetryTimeout = TimeSpan.FromSeconds(1);
            var retryCount = (int)(2 * (clientRetryTimeout.Ticks / DefaultRetryDelay.Ticks));
            var retryDelay = DefaultRetryDelay;

            var sw = new Stopwatch();
            sw.Start();
            var result = await this.SetupCancelTestAsync(
                clientRetryTimeout,
                retryCount,
                retryDelay);

            sw.ElapsedMilliseconds.Should().BeGreaterThan((long)clientRetryTimeout.TotalMilliseconds, "Should be longer than the ClientRetryTimeout.");
            result.ExceptionFromInvoke.Should().BeAssignableTo(typeof(OperationCanceledException), $"Should indicate a canceled operation. {result.ExceptionFromInvoke}");
            result.CallCount.Should().BeLessThan(retryCount, "Should cancel before token is signaled.");
            result.CancellationTokenSource.Token.IsCancellationRequested.Should().Be(false, "Cancellation should have occured due to the timer.");
        }

        /// <summary>
        /// Tests handling of cancellation after the operation is complete. This is meant to expose bugs where callbacks are registered on cancellation taken.
        /// /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CancelAfterCall()
        {
            var clientRetryTimeout = TimeSpan.FromSeconds(1);
            var retryCount = (int)(2 * (clientRetryTimeout.Ticks / DefaultRetryDelay.Ticks));
            var retryDelay = DefaultRetryDelay;

            var sw = new Stopwatch();
            sw.Start();
            var result = await this.SetupCancelTestAsync(
                clientRetryTimeout,
                retryCount,
                retryDelay);
            result.CancellationTokenSource.Cancel();

            sw.ElapsedMilliseconds.Should().BeGreaterThan((long)clientRetryTimeout.TotalMilliseconds - 10, "Should be longer than the ClientRetryTimeout.");
            result.ExceptionFromInvoke.Should().BeAssignableTo(typeof(OperationCanceledException), $"Should indicate a canceled operation. {result.ExceptionFromInvoke}");
            result.CallCount.Should().BeLessThan(retryCount, "Should cancel before token is signaled.");
        }

        /// <summary>
        /// Tests handling of cancellation by the operation timer when retry delay is large.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CancelOnTimerWithLargeRetryDelay()
        {
            var logger = new ConsoleLogger("[gor]");

            var clientRetryTimeout = TimeSpan.FromSeconds(1);
            var retryCount = (int)(2 * (clientRetryTimeout.Ticks / DefaultRetryDelay.Ticks));
            var retryDelay = TimeSpan.FromTicks(clientRetryTimeout.Ticks * 2);

            long millisecondsStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var sw = new Stopwatch();
            sw.Start();
            var result = await this.SetupCancelTestAsync(
                clientRetryTimeout,
                retryCount,
                retryDelay);

            long millisecondsEnd = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            logger.Info("sw.ElapsedMilliseconds = " + sw.ElapsedMilliseconds);
            logger.Info("wall elapsed milliseconds " + (millisecondsEnd - millisecondsStart));

            sw.ElapsedMilliseconds.Should().BeGreaterThan((long)clientRetryTimeout.TotalMilliseconds, "Should be longer than the ClientRetryTimeout.");
            sw.ElapsedMilliseconds.Should().BeLessThan((long)retryDelay.TotalMilliseconds, "Should return before the retry delay.");

            Console.WriteLine("[gor] CancellationTokenSource cancellation requested: " + result.CancellationTokenSource.Token.IsCancellationRequested);
            Console.WriteLine("[gor] exception from invoke " + result.ExceptionFromInvoke.GetType());

            sw.ElapsedMilliseconds.Should().BeLessThan(0, "tst");
            result.ExceptionFromInvoke.Should().BeAssignableTo(typeof(OperationCanceledException), "Should indicate a canceled operation.");
            result.CallCount.Should().BeLessThan(retryCount, "Should cancel before token is signaled.");
            result.CancellationTokenSource.Token.IsCancellationRequested.Should().Be(false, "Cancellation should have occured due to the timer.");
        }

        private async Task<SetupCancelTestResult> SetupCancelTestAsync(
            TimeSpan clientRetryTimeout,
            int retryCount,
            TimeSpan retryDelay)
        {
            retryCount = 23; // Just for diagnostics.
            var mockClient = Repository.Create<ICommunicationClient>();
            mockClient.SetupAllProperties();

            var mockFactory = Repository.Create<ICommunicationClientFactory<ICommunicationClient>>();
            var mockRetryPolicy = Repository.Create<IRetryPolicy>();
            mockRetryPolicy.Setup(m => m.ClientRetryTimeout).Returns(clientRetryTimeout);

            var operationRetrySettings = new OperationRetrySettings(mockRetryPolicy.Object);
            mockFactory.Setup(f => f.GetClientAsync(
                It.Is<Uri>(u => u == ExampleUri),
                It.Is<Client.ServicePartitionKey>(s => s == ExampleServicePartitionKey),
                It.Is<TargetReplicaSelector>(trs => trs == ExampleTargetReplicaSelector),
                It.Is<string>(l => l == ExampleListenerName),
                It.Is<OperationRetrySettings>(o => o == operationRetrySettings),
                It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(mockClient.Object));

            var clientException = new InvalidOperationException();
            mockFactory.Setup(f => f.ReportOperationExceptionAsync(
                It.Is<ICommunicationClient>(c => c == mockClient.Object),
                It.Is<ExceptionInformation>(ei => ei.Exception == clientException),
                It.IsAny<OperationRetrySettings>(),
                It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new OperationRetryControl() { IsTransient = true, ShouldRetry = true, MaxRetryCount = retryCount * 2, GetRetryDelay = c => retryDelay }));

            var servicePartitonClient = new ServicePartitionClient<ICommunicationClient>(
                mockFactory.Object,
                ExampleUri,
                ExampleServicePartitionKey,
                ExampleTargetReplicaSelector,
                ExampleListenerName,
                operationRetrySettings);

            var cts = new CancellationTokenSource();
            var callCount = 0;
            Func<ICommunicationClient, Task> clientCall = (client) =>
            {
                callCount++;
                Console.WriteLine("[gor] callCount: " + callCount);
                if (callCount == retryCount)
                {
                    Console.WriteLine("[gor] cancelling due to retry count " + retryCount);
                    cts.Cancel();
                }

                throw clientException;
            };

            Exception e = null;
            try
            {
                await servicePartitonClient.InvokeWithRetryAsync(clientCall, cts.Token);
            }
            catch (Exception ex)
            {
                e = ex;
            }

            return new SetupCancelTestResult { CallCount = callCount, CancellationTokenSource = cts, ExceptionFromInvoke = e };
        }

        private class SetupCancelTestResult
        {
            public int CallCount { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }

            public Exception ExceptionFromInvoke { get; set; }
        }
    }
}
