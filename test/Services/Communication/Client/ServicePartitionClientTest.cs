// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Services.Client;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ServicePartitionClientTest
{
    readonly ServicePartitionClient<ICommunicationClient> sut;

    // Constructor parameters
    readonly Mock<ICommunicationClientFactory<ICommunicationClient>> communicationClientFactory = new(MockBehavior.Strict);
    readonly Uri serviceUri = fuzzy.Uri();
    readonly ServicePartitionKey partitionKey = new(fuzzy.Int64());
    readonly TargetReplicaSelector targetReplicaSelector = fuzzy.Enum<TargetReplicaSelector>();
    readonly string listenerName = fuzzy.String();
    readonly OperationRetrySettings retrySettings = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServicePartitionClientTest() =>
        sut = new ServicePartitionClient<ICommunicationClient>(
            communicationClientFactory.Object,
            serviceUri,
            partitionKey,
            targetReplicaSelector,
            listenerName,
            retrySettings);

    public sealed class Constructor : ServicePartitionClientTest
    {
        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for communicationClientFactory.
        public void ThrowsArgumentNullExceptionWhenCommunicationClientFactoryIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ServicePartitionClient<ICommunicationClient>(
                    null, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings));
            Assert.Equal(nameof(communicationClientFactory), actual.ParamName);
        }

        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(communicationClientFactory.Object, sut.Factory);
            Assert.Same(serviceUri, sut.ServiceUri);
            Assert.Same(partitionKey, sut.PartitionKey);
            Assert.Same(listenerName, sut.ListenerName);
        }

        [Theory]
        [InlineData(TargetReplicaSelector.Default)]
        [InlineData(TargetReplicaSelector.RandomReplica)]
        [InlineData(TargetReplicaSelector.RandomSecondaryReplica)]
        public void InitializesTargetReplicaSelector(TargetReplicaSelector selector)
        {
            var sut = new ServicePartitionClient<ICommunicationClient>(
                communicationClientFactory.Object, serviceUri, partitionKey, selector, listenerName, retrySettings);
            Assert.Equal(selector, sut.TargetReplicaSelector);
        }

        [Fact]
        public void DefaultsPartitionKeyToSingletonWhenPartitionKeyIsNull()
        {
            var sut = new ServicePartitionClient<ICommunicationClient>(
                communicationClientFactory.Object, serviceUri, null, targetReplicaSelector, listenerName, retrySettings);
            Assert.Same(ServicePartitionKey.Singleton, sut.PartitionKey);
        }

        [Fact]
        public async Task DefaultsRetrySettingsWhenArgumentIsNull()
        {
            var sut = new ServicePartitionClient<ICommunicationClient>(
                communicationClientFactory.Object, serviceUri, partitionKey, targetReplicaSelector, listenerName, null);

            // Capture the retrySettings forwarded to GetClientAsync and assert observable defaults.
            OperationRetrySettings forwarded = null;
            var communicationClient = new Mock<ICommunicationClient>();
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, It.IsAny<OperationRetrySettings>(), It.IsAny<CancellationToken>()))
                .Callback((Uri _, ServicePartitionKey _, TargetReplicaSelector _, string _, OperationRetrySettings settings, CancellationToken _) => forwarded = settings)
                .ReturnsAsync(communicationClient.Object);

            _ = await sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(new object()), TestContext.Current.CancellationToken);

            Assert.NotNull(forwarded);
            Assert.Equal(10, forwarded.DefaultMaxRetryCountForTransientErrors);
            Assert.Equal(Timeout.InfiniteTimeSpan, forwarded.ClientRetryTimeout);
        }

        [Fact]
        public void DefaultsTargetReplicaSelectorToDefault()
        {
            var sut = new ServicePartitionClient<ICommunicationClient>(communicationClientFactory.Object, serviceUri);
            Assert.Equal(TargetReplicaSelector.Default, sut.TargetReplicaSelector);
        }

        [Fact]
        public void DefaultsListenerNameToNull()
        {
            var sut = new ServicePartitionClient<ICommunicationClient>(communicationClientFactory.Object, serviceUri);
            Assert.Null(sut.ListenerName);
        }
    }

    public sealed class InvokeWithRetryAsync_FuncOfTCommunicationClientTaskOfTResult_CancellationToken_TypeArray : InvokeWithRetryAsyncBase
    {
        static readonly Method method = typeof(ServicePartitionClient<ICommunicationClient>)
            .Method<Func<Func<ICommunicationClient, Task<object>>, CancellationToken, Type[], Task<object>>>(
                nameof(ServicePartitionClient<ICommunicationClient>.InvokeWithRetryAsync));

        readonly CancellationToken cancellation = TestContext.Current.CancellationToken;

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for func.
        public async Task ThrowsArgumentNullExceptionWhenFuncIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync<object>((Func<ICommunicationClient, Task<object>>)null, cancellation));
            Assert.Equal(method.Parameter<Func<ICommunicationClient, Task<object>>>().Name, actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for doNotRetryExceptionTypes.
        public async Task ThrowsArgumentNullExceptionWhenDoNotRetryExceptionTypesIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync<object>(_ => throw clientException, cancellation, (Type[])null));
            Assert.Equal(method.Parameter<Type[]>().Name, actual.ParamName);
        }

        [Fact]
        public async Task ReturnsResultOfFuncWhenNoExceptionThrown()
        {
            var expected = new object();

            object actual = await sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(expected), cancellation);

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task PassesResolvedClientToFunc()
        {
            ICommunicationClient actual = null;

            _ = await sut.InvokeWithRetryAsync<object>(c => { actual = c; return Task.FromResult(new object()); }, cancellation);

            Assert.Same(client, actual);
        }

        [Fact]
        public async Task PassesCancellationTokenToGetClientAsync()
        {
            _ = await sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(new object()), cancellation);

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, cancellation),
                Times.Once);
        }

        [Fact]
        public async Task RetriesAfterTransientException()
        {
            SetupReportOperationException(TransientRetry());
            int calls = 0;

            object actual = await sut.InvokeWithRetryAsync<object>(
                _ =>
                {
                    calls++;
                    if (calls == 1) throw clientException;
                    return Task.FromResult<object>(calls);
                },
                cancellation);

            Assert.Equal(2, calls);
            Assert.Equal(2, actual);
        }

        [Fact]
        public async Task RethrowsExceptionWhenInDoNotRetryExceptionTypes()
        {
            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ => throw clientException,
                    cancellation,
                    clientException.GetType()));

            Assert.Same(clientException, actual);
        }

        [Fact]
        public async Task RetriesAfterAggregateExceptionWhenNoInnerExceptionIsInDoNotRetryExceptionTypes()
        {
            var aggregate = new AggregateException(clientException);
            SetupReportOperationException(TransientRetry(), aggregate);
            int calls = 0;

            object actual = await sut.InvokeWithRetryAsync<object>(
                _ =>
                {
                    calls++;
                    if (calls == 1) throw aggregate;
                    return Task.FromResult<object>(calls);
                },
                cancellation);

            Assert.Equal(2, calls);
            Assert.Equal(2, actual);
        }

        [Fact]
        public async Task RethrowsAggregateExceptionWhenInnerExceptionIsInDoNotRetryExceptionTypes()
        {
            var aggregate = new AggregateException(clientException);

            AggregateException actual = await Assert.ThrowsAsync<AggregateException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ => throw aggregate,
                    cancellation,
                    clientException.GetType()));

            Assert.Same(clientException, Assert.Single(actual.InnerExceptions));
        }

        [Fact]
        public async Task ThrowsExceptionFromReportResultWhenShouldRetryIsFalseAndReportExceptionIsSet()
        {
            var transformed = new ApplicationException();
            SetupReportOperationException(new OperationRetryControl { ShouldRetry = false, Exception = transformed });

            ApplicationException actual = await Assert.ThrowsAsync<ApplicationException>(
                () => sut.InvokeWithRetryAsync<object>(_ => throw clientException, cancellation));

            Assert.Same(transformed, actual);
        }

        [Fact]
        public async Task ThrowsOriginalExceptionWhenShouldRetryIsFalseAndReportExceptionIsNull()
        {
            SetupReportOperationException(new OperationRetryControl { ShouldRetry = false, Exception = null });

            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync<object>(_ => throw clientException, cancellation));

            Assert.Same(clientException, actual);
        }

        [Fact]
        public async Task ThrowsAfterMaxRetryCountReached()
        {
            SetupReportOperationException(TransientRetry(maxRetryCount: 2));
            int calls = 0;

            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ => { calls++; throw clientException; },
                    cancellation));

            Assert.Same(clientException, actual);
            Assert.Equal(3, calls);
        }

        [Fact]
        public async Task ThrowsAfterZeroMaxRetryCount()
        {
            SetupReportOperationException(new OperationRetryControl { ShouldRetry = true, IsTransient = true, MaxRetryCount = 0, ExceptionId = "ex", GetRetryDelay = _ => ShortRetryDelay });
            int calls = 0;

            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ => { calls++; throw clientException; },
                    cancellation));

            Assert.Same(clientException, actual);
            Assert.Equal(1, calls);
        }

        [Fact]
        public async Task ResetsRetryCountWhenExceptionIdChangesAcrossIterations()
        {
            // Utility.ShouldRetryOperation resets the retry counter whenever ExceptionId differs from the previous
            // iteration. Verify the SUT forwards ExceptionId so distinct ids cause more than MaxRetryCount retries.
            const int maxRetryCount = 2;
            var controls = new Queue<OperationRetryControl>(new[]
            {
                Retry("a"), // count: null→"a", 1
                Retry("a"), // count: 2
                Retry("b"), // reset: "a"→"b", 1
                Retry("b"), // count: 2
                Retry("b"), // 2 >= max → throws
            });
            _ = communicationClientFactory
                .Setup(_ => _.ReportOperationExceptionAsync(client, It.IsAny<ExceptionInformation>(), retrySettings, CancellationToken.None))
                .ReturnsAsync(controls.Dequeue);
            int calls = 0;

            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ => { calls++; throw clientException; },
                    cancellation));

            Assert.Same(clientException, actual);
            Assert.Equal(5, calls);

            static OperationRetryControl Retry(string id) => new()
            {
                ShouldRetry = true,
                IsTransient = true,
                MaxRetryCount = maxRetryCount,
                ExceptionId = id,
                GetRetryDelay = _ => ShortRetryDelay,
            };
        }

        [Fact]
        public async Task ResetsCommunicationClientWhenExceptionIsNotTransient()
        {
            // After a non-transient exception, the SUT clears the cached client. Because lastRsp has already been
            // captured from the first GetClientAsync call, the next iteration takes the rsp-based GetClientAsync
            // overload.
            SetupReportOperationException(new OperationRetryControl
            {
                ShouldRetry = true,
                IsTransient = false,
                MaxRetryCount = 5,
                ExceptionId = "ex",
                GetRetryDelay = _ => ShortRetryDelay,
            });
            int calls = 0;

            _ = await sut.InvokeWithRetryAsync<object>(
                _ =>
                {
                    calls++;
                    if (calls == 1) throw clientException;
                    return Task.FromResult<object>(calls);
                },
                cancellation);

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(rsp, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()),
                Times.Once);
            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdatesLastRspFromReResolvedClientAfterNonTransientException()
        {
            // After a non-transient exception the SUT re-resolves via the rsp-based GetClientAsync overload. The
            // second client carries a distinct ResolvedServicePartition, and the SUT must adopt it as the new lastRsp.
            var newRsp = Type<ResolvedServicePartition>.Uninitialized();
            var newCommunicationClient = new Mock<ICommunicationClient>();
            _ = newCommunicationClient.SetupGet(_ => _.ResolvedServicePartition).Returns(newRsp);
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(rsp, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newCommunicationClient.Object);
            SetupReportOperationException(new OperationRetryControl
            {
                ShouldRetry = true,
                IsTransient = false,
                MaxRetryCount = 5,
                ExceptionId = "ex",
                GetRetryDelay = _ => ShortRetryDelay,
            });
            int calls = 0;

            _ = await sut.InvokeWithRetryAsync<object>(
                _ =>
                {
                    calls++;
                    if (calls == 1) throw clientException;
                    return Task.FromResult<object>(calls);
                },
                cancellation);

            Assert.True(sut.TryGetLastResolvedServicePartition(out ResolvedServicePartition actual));
            Assert.Same(newRsp, actual);
        }

        [Fact]
        public async Task DoesNotResetCommunicationClientWhenExceptionIsTransient()
        {
            SetupReportOperationException(TransientRetry());
            int calls = 0;

            _ = await sut.InvokeWithRetryAsync<object>(
                _ =>
                {
                    calls++;
                    if (calls == 1) throw clientException;
                    return Task.FromResult<object>(calls);
                },
                cancellation);

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(rsp, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()),
                Times.Never);
            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ThrowsOperationCanceledExceptionWhenTokenIsCanceledBeforeFunc()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(new object()), cts.Token));

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(rsp, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()),
                Times.Never);
            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CancelsWhenTokenIsSignaledMidRetry()
        {
            SetupReportOperationException(TransientRetry());
            using var cts = new CancellationTokenSource();
            int calls = 0;
            int retryCount = 5;

            _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ =>
                    {
                        calls++;
                        if (calls == retryCount) cts.Cancel();
                        throw clientException;
                    },
                    cts.Token));

            Assert.Equal(retryCount, calls);
        }

        [Fact]
        public async Task CancelsWhenClientRetryTimeoutElapses()
        {
            var timeout = TimeSpan.FromMilliseconds(500);
            var policy = new Mock<IRetryPolicy>();
            _ = policy.SetupGet(_ => _.ClientRetryTimeout).Returns(timeout);
            var retrySettings = new OperationRetrySettings(policy.Object);
            var sut = new ServicePartitionClient<ICommunicationClient>(
                communicationClientFactory.Object,
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings);
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
            _ = communicationClientFactory
                .Setup(_ => _.ReportOperationExceptionAsync(
                    client,
                    It.Is<ExceptionInformation>(i => i.Exception == clientException && i.TargetReplica == targetReplicaSelector),
                    retrySettings,
                    CancellationToken.None))
                .ReturnsAsync(TransientRetry(maxRetryCount: 1_000_000, delay: ShortRetryDelay));
            using var cts = new CancellationTokenSource();
            int calls = 0;

            var stopwatch = Stopwatch.StartNew();
            _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => sut.InvokeWithRetryAsync<object>(
                    _ => { calls++; throw clientException; },
                    cts.Token));
            stopwatch.Stop();

            // Stopwatch precision varies; allow a small margin per Microsoft documentation.
            Assert.True(stopwatch.ElapsedMilliseconds >= timeout.TotalMilliseconds - 25, $"Elapsed {stopwatch.ElapsedMilliseconds}ms < {timeout.TotalMilliseconds}ms");
            Assert.False(cts.IsCancellationRequested);
        }

        [Fact]
        public async Task CancelsBeforeLargeRetryDelayElapses()
        {
            var timeout = TimeSpan.FromMilliseconds(100);
            var retryDelay = TimeSpan.FromSeconds(30);
            var policy = new Mock<IRetryPolicy>();
            _ = policy.SetupGet(_ => _.ClientRetryTimeout).Returns(timeout);
            var retrySettings = new OperationRetrySettings(policy.Object);
            var sut = new ServicePartitionClient<ICommunicationClient>(
                communicationClientFactory.Object,
                serviceUri,
                partitionKey,
                targetReplicaSelector,
                listenerName,
                retrySettings);
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
            _ = communicationClientFactory
                .Setup(_ => _.ReportOperationExceptionAsync(
                    client,
                    It.Is<ExceptionInformation>(i => i.Exception == clientException && i.TargetReplica == targetReplicaSelector),
                    retrySettings,
                    CancellationToken.None))
                .ReturnsAsync(TransientRetry(maxRetryCount: 1_000_000, delay: retryDelay));

            var stopwatch = Stopwatch.StartNew();
            _ = await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => sut.InvokeWithRetryAsync<object>(_ => throw clientException, TestContext.Current.CancellationToken));
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < retryDelay.TotalMilliseconds, $"Elapsed {stopwatch.ElapsedMilliseconds}ms >= retry delay {retryDelay.TotalMilliseconds}ms");
        }

        [Fact]
        public async Task SetsClientRequestTrackerToLogContextRequestIdWhenPresent()
        {
            LogContext.Clear();
            ClientRequestTracker.Set(null);
            try
            {
                var requestId = fuzzy.Guid();
                LogContext.Set(new LogContext { RequestId = requestId });
                string actual = null;

                _ = await sut.InvokeWithRetryAsync<object>(
                    _ =>
                    {
                        ClientRequestTracker.TryGet(out actual);
                        return Task.FromResult(new object());
                    },
                    cancellation);

                Assert.Equal(requestId.ToString(), actual);
            }
            finally
            {
                LogContext.Clear();
                ClientRequestTracker.Set(null);
            }
        }

        [Fact]
        public async Task SetsClientRequestTrackerToGeneratedGuidWhenLogContextIsAbsent()
        {
            LogContext.Clear();
            ClientRequestTracker.Set(null);
            try
            {
                string actual = null;

                _ = await sut.InvokeWithRetryAsync<object>(
                    _ =>
                    {
                        ClientRequestTracker.TryGet(out actual);
                        return Task.FromResult(new object());
                    },
                    cancellation);

                Assert.True(Guid.TryParse(actual, out Guid parsed));
                Assert.NotEqual(Guid.Empty, parsed);
            }
            finally
            {
                LogContext.Clear();
                ClientRequestTracker.Set(null);
            }
        }
    }

    public sealed class InvokeWithRetryAsync_FuncOfTCommunicationClientTaskOfTResult_TypeArray : InvokeWithRetryAsyncBase
    {
        static readonly Method method = typeof(ServicePartitionClient<ICommunicationClient>)
            .Method<Func<Func<ICommunicationClient, Task<object>>, Type[], Task<object>>>(
                nameof(ServicePartitionClient<ICommunicationClient>.InvokeWithRetryAsync));

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for func.
        public async Task ThrowsArgumentNullExceptionWhenFuncIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync<object>((Func<ICommunicationClient, Task<object>>)null));
            Assert.Equal(method.Parameter<Func<ICommunicationClient, Task<object>>>().Name, actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for doNotRetryExceptionTypes.
        public async Task ThrowsArgumentNullExceptionWhenDoNotRetryExceptionTypesIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync<object>(_ => throw clientException, (Type[])null));
            Assert.Equal(method.Parameter<Type[]>().Name, actual.ParamName);
        }

        [Fact]
        public async Task ReturnsResultOfFuncWhenNoExceptionThrown()
        {
            var expected = new object();

            object actual = await sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(expected));

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task DefaultsCancellationTokenToNoneWhenNotSpecified()
        {
            _ = await sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(new object()));

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task RethrowsExceptionWhenInDoNotRetryExceptionTypes()
        {
            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync<object>(_ => throw clientException, clientException.GetType()));

            Assert.Same(clientException, actual);
        }
    }

    public sealed class InvokeWithRetryAsync_FuncOfTCommunicationClientTask_CancellationToken_TypeArray : InvokeWithRetryAsyncBase
    {
        static readonly Method method = typeof(ServicePartitionClient<ICommunicationClient>)
            .Method<Func<Func<ICommunicationClient, Task>, CancellationToken, Type[], Task>>(
                nameof(ServicePartitionClient<ICommunicationClient>.InvokeWithRetryAsync));

        readonly CancellationToken cancellation = TestContext.Current.CancellationToken;

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for func.
        public async Task ThrowsArgumentNullExceptionWhenFuncIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync((Func<ICommunicationClient, Task>)null, cancellation));
            Assert.Equal(method.Parameter<Func<ICommunicationClient, Task>>().Name, actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for doNotRetryExceptionTypes.
        public async Task ThrowsArgumentNullExceptionWhenDoNotRetryExceptionTypesIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync(_ => throw clientException, cancellation, (Type[])null));
            Assert.Equal(method.Parameter<Type[]>().Name, actual.ParamName);
        }

        [Fact]
        public async Task PassesResolvedClientToFunc()
        {
            ICommunicationClient actual = null;

            await sut.InvokeWithRetryAsync(c => { actual = c; return Task.CompletedTask; }, cancellation);

            Assert.Same(client, actual);
        }

        [Fact]
        public async Task PassesCancellationTokenToGetClientAsync()
        {
            await sut.InvokeWithRetryAsync(_ => Task.CompletedTask, cancellation);

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, cancellation),
                Times.Once);
        }

        [Fact]
        public async Task RethrowsExceptionWhenInDoNotRetryExceptionTypes()
        {
            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync(
                    _ => throw clientException,
                    cancellation,
                    clientException.GetType()));

            Assert.Same(clientException, actual);
        }

        [Fact]
        public async Task AwaitsTaskReturnedByFunc()
        {
            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync(
                    _ => Task.FromException(clientException),
                    cancellation,
                    clientException.GetType()));

            Assert.Same(clientException, actual);
        }
    }

    public sealed class InvokeWithRetryAsync_FuncOfTCommunicationClientTask_TypeArray : InvokeWithRetryAsyncBase
    {
        static readonly Method method = typeof(ServicePartitionClient<ICommunicationClient>)
            .Method<Func<Func<ICommunicationClient, Task>, Type[], Task>>(
                nameof(ServicePartitionClient<ICommunicationClient>.InvokeWithRetryAsync));

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for func.
        public async Task ThrowsArgumentNullExceptionWhenFuncIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync((Func<ICommunicationClient, Task>)null));
            Assert.Equal(method.Parameter<Func<ICommunicationClient, Task>>().Name, actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for doNotRetryExceptionTypes.
        public async Task ThrowsArgumentNullExceptionWhenDoNotRetryExceptionTypesIsNull()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => sut.InvokeWithRetryAsync(_ => throw clientException, (Type[])null));
            Assert.Equal(method.Parameter<Type[]>().Name, actual.ParamName);
        }

        [Fact]
        public async Task PassesResolvedClientToFunc()
        {
            ICommunicationClient actual = null;

            await sut.InvokeWithRetryAsync(c => { actual = c; return Task.CompletedTask; });

            Assert.Same(client, actual);
        }

        [Fact]
        public async Task DefaultsCancellationTokenToNoneWhenNotSpecified()
        {
            await sut.InvokeWithRetryAsync(_ => Task.CompletedTask);

            communicationClientFactory.Verify(
                _ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task RethrowsExceptionWhenInDoNotRetryExceptionTypes()
        {
            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync(_ => throw clientException, clientException.GetType()));

            Assert.Same(clientException, actual);
        }

        [Fact]
        public async Task AwaitsTaskReturnedByFunc()
        {
            Exception actual = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.InvokeWithRetryAsync(_ => Task.FromException(clientException), clientException.GetType()));

            Assert.Same(clientException, actual);
        }
    }

    public sealed class TryGetLastResolvedServicePartition : ServicePartitionClientTest
    {
        [Fact]
        public void ReturnsFalseAndNullWhenNoPartitionResolvedYet()
        {
            bool actual = sut.TryGetLastResolvedServicePartition(out ResolvedServicePartition resolvedServicePartition);
            Assert.False(actual);
            Assert.Null(resolvedServicePartition);
        }

        [Fact]
        public async Task ReturnsTrueAndLastRspWhenSet()
        {
            // Drive InvokeWithRetryAsync so the SUT assigns lastRsp from communicationClient.ResolvedServicePartition.
            var rsp = Type<ResolvedServicePartition>.Uninitialized();
            var communicationClient = new Mock<ICommunicationClient>();
            _ = communicationClient.SetupGet(_ => _.ResolvedServicePartition).Returns(rsp);
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(communicationClient.Object);
            _ = await sut.InvokeWithRetryAsync<object>(_ => Task.FromResult(new object()), TestContext.Current.CancellationToken);

            bool actual = sut.TryGetLastResolvedServicePartition(out ResolvedServicePartition resolvedServicePartition);

            Assert.True(actual);
            Assert.Same(rsp, resolvedServicePartition);
        }
    }

    public abstract class InvokeWithRetryAsyncBase : ServicePartitionClientTest
    {
        protected readonly Mock<ICommunicationClient> communicationClient = new();
        protected readonly ResolvedServicePartition rsp = Type<ResolvedServicePartition>.Uninitialized();

        // Default: GetClientAsync returns the mock client; ReportOperationExceptionAsync handles `clientException`
        // with a transient retry that uses a short delay so tests don't slow down.
        protected static readonly TimeSpan ShortRetryDelay = TimeSpan.FromMilliseconds(1);

        protected readonly Exception clientException = new InvalidOperationException();

        protected ICommunicationClient client => communicationClient.Object;

        protected InvokeWithRetryAsyncBase()
        {
            _ = communicationClient.SetupGet(_ => _.ResolvedServicePartition).Returns(rsp);
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
            _ = communicationClientFactory
                .Setup(_ => _.GetClientAsync(rsp, targetReplicaSelector, listenerName, retrySettings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
        }

        protected void SetupReportOperationException(OperationRetryControl control, Exception expectedException = null) =>
            _ = communicationClientFactory
                .Setup(_ => _.ReportOperationExceptionAsync(
                    client,
                    It.Is<ExceptionInformation>(i => i.Exception == (expectedException ?? clientException) && i.TargetReplica == targetReplicaSelector),
                    retrySettings,
                    CancellationToken.None))
                .ReturnsAsync(control);

        protected static OperationRetryControl TransientRetry(int maxRetryCount = 5, TimeSpan? delay = null) =>
            new()
            {
                ShouldRetry = true,
                IsTransient = true,
                MaxRetryCount = maxRetryCount,
                ExceptionId = "exception",
                GetRetryDelay = _ => delay ?? ShortRetryDelay,
            };
    }
}
