// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Services.Client;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class CommunicationClientFactoryBaseTest : IDisposable
{
    readonly TestFactory sut;

    // Constructor parameters
    readonly bool fireConnectEvents = fuzzy.Boolean();
    readonly IServicePartitionResolver servicePartitionResolver = Mock.Of<IServicePartitionResolver>();
    readonly IEnumerable<IExceptionHandler> exceptionHandlers = new[] { Mock.Of<IExceptionHandler>(), Mock.Of<IExceptionHandler>() };
    readonly string traceId = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    CommunicationClientFactoryBaseTest() =>
        sut = new TestFactory(fireConnectEvents, servicePartitionResolver, exceptionHandlers, traceId);

    void IDisposable.Dispose() =>
        sut.Dispose();

    public sealed class Constructor_Boolean_IServicePartitionResolver_IEnumerableOfIExceptionHandler_String : CommunicationClientFactoryBaseTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitializesProperties(bool fireConnectEvents)
        {
            using var other = new TestFactory(fireConnectEvents, servicePartitionResolver, exceptionHandlers, traceId);
            Assert.Equal(fireConnectEvents, other.Field<bool>().Value);
            Assert.Same(servicePartitionResolver, other.ServiceResolver);
            Assert.Equal(exceptionHandlers, other.ExceptionHandlers);
            Assert.Same(traceId, other.TraceIdValue);
        }

        [Fact]
        public void UsesDefaultServicePartitionResolverWhenArgumentIsNull()
        {
            using var other = new TestFactory(fireConnectEvents, null, exceptionHandlers, traceId);
            Assert.Same(ServicePartitionResolver.GetDefault(), other.ServiceResolver);
        }

        [Fact]
        public void ExposesEmptyExceptionHandlersWhenArgumentIsNull()
        {
            using var other = new TestFactory(fireConnectEvents, servicePartitionResolver, null, traceId);
            Assert.Empty(other.ExceptionHandlers);
        }

        [Fact]
        public void GeneratesGuidTraceIdWhenArgumentIsNull()
        {
            using var other = new TestFactory(fireConnectEvents, servicePartitionResolver, exceptionHandlers, null);
            Assert.True(Guid.TryParse(other.TraceIdValue, out _));
        }

        [Fact]
        public void CopiesExceptionHandlersToAvoidExternalMutation()
        {
            var handlers = new List<IExceptionHandler> { Mock.Of<IExceptionHandler>() };
            using var other = new TestFactory(fireConnectEvents, servicePartitionResolver, handlers, traceId);
            handlers.Add(Mock.Of<IExceptionHandler>());
            Assert.Single(other.ExceptionHandlers);
        }
    }

    public sealed class Constructor_IServicePartitionResolver_IEnumerableOfIExceptionHandler_String : CommunicationClientFactoryBaseTest
    {
        [Fact]
        public void InitializesProperties()
        {
            using var other = new TestFactory(servicePartitionResolver, exceptionHandlers, traceId);
            Assert.Same(servicePartitionResolver, other.ServiceResolver);
            Assert.Equal(exceptionHandlers, other.ExceptionHandlers);
            Assert.Same(traceId, other.TraceIdValue);
        }

        [Fact]
        public void DefaultsFireConnectEventsToFalse()
        {
            using var other = new TestFactory(servicePartitionResolver, exceptionHandlers, traceId);
            Assert.False(other.Field<bool>().Value);
        }
    }

    public sealed class Dispose : CommunicationClientFactoryBaseTest
    {
        [Fact]
        public void DisposesClientCache()
        {
            var cache = sut.Field<CommunicationClientCache<ICommunicationClient>>().Value;
            Guid partitionId = fuzzy.Guid();
            ResolvedServiceEndpoint endpoint = MakeEndpoint(fuzzy.String());
            string listenerName = fuzzy.String();
            _ = cache.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, MakeRsp());

            sut.Dispose();

            Assert.False(cache.TryGetClientCacheEntry(partitionId, endpoint, listenerName, out _));
        }
    }

    public sealed class GetClientAsync_ResolvedServicePartition_TargetReplicaSelector_String_OperationRetrySettings_CancellationToken : CommunicationClientFactoryBaseTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Endpoint selection depends on sealed System.Fabric types that cannot be populated.
        public void CreatesClientFromPreviousResolvedServicePartition()
        {
            // The overload delegates to CreateClientWithRetriesAsync, which walks ResolvedServicePartition.Endpoints /
            // GetEndpoint(). ResolvedServicePartition and ResolvedServiceEndpoint are sealed types from System.Fabric
            // whose Endpoints collection cannot be populated without modifying the SUT to take a seam for endpoint
            // selection. Documented here so the uncovered public API surface is discoverable from the test class.
            throw new NotImplementedException();
        }
    }

    public sealed class GetClientAsync_Uri_ServicePartitionKey_TargetReplicaSelector_String_OperationRetrySettings_CancellationToken : CommunicationClientFactoryBaseTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Endpoint selection depends on sealed System.Fabric types that cannot be populated.
        public void ResolvesPartitionAndCreatesClient()
        {
            // The overload delegates to CreateClientWithRetriesAsync, which calls IServicePartitionResolver.ResolveAsync
            // and then walks ResolvedServicePartition.Endpoints / GetEndpoint(). ResolvedServicePartition and
            // ResolvedServiceEndpoint are sealed types from System.Fabric whose Endpoints collection cannot be populated
            // without modifying the SUT to take a seam for endpoint selection. Documented here so the uncovered public
            // API surface is discoverable from the test class.
            throw new NotImplementedException();
        }
    }

    public sealed class OnClientConnected : CommunicationClientFactoryBaseTest
    {
        // Method parameters
        readonly ICommunicationClient newClient = Mock.Of<ICommunicationClient>();

        [Fact]
        public void FiresClientConnectedEventWithNewClient()
        {
            object actualSender = null;
            CommunicationClientEventArgs<ICommunicationClient> actualArgs = null;
            sut.ClientConnected += (s, e) => { actualSender = s; actualArgs = e; };

            sut.OnClientConnected(newClient);

            Assert.Same(sut, actualSender);
            Assert.NotNull(actualArgs);
            Assert.Same(newClient, actualArgs.Client);
        }

        [Fact]
        public void DoesNotThrowWhenClientConnectedHasNoSubscribers() =>
            sut.OnClientConnected(newClient);
    }

    public sealed class OnClientDisconnected : CommunicationClientFactoryBaseTest
    {
        // Method parameters
        readonly ICommunicationClient faultedClient = Mock.Of<ICommunicationClient>();

        [Fact]
        public void FiresClientDisconnectedEventWithFaultedClient()
        {
            object actualSender = null;
            CommunicationClientEventArgs<ICommunicationClient> actualArgs = null;
            sut.ClientDisconnected += (s, e) => { actualSender = s; actualArgs = e; };

            sut.OnClientDisconnected(faultedClient);

            Assert.Same(sut, actualSender);
            Assert.NotNull(actualArgs);
            Assert.Same(faultedClient, actualArgs.Client);
        }

        [Fact]
        public void DoesNotThrowWhenClientDisconnectedHasNoSubscribers() =>
            sut.OnClientDisconnected(faultedClient);
    }

    public sealed class OpenClient : CommunicationClientFactoryBaseTest
    {
        // Method parameters
        readonly ICommunicationClient client = Mock.Of<ICommunicationClient>();
        readonly CancellationToken cancellationToken = default;

        [Fact]
        public async Task ReturnsCompletedTaskWithoutThrowing()
        {
            Task actual = sut.TestOpenClient(client, cancellationToken);

            Assert.Equal(TaskStatus.RanToCompletion, actual.Status);
            await actual;
        }
    }

    public sealed class ReportOperationExceptionAsync : CommunicationClientFactoryBaseTest
    {
        // Method parameters
        readonly ICommunicationClient client;
        readonly ExceptionInformation exceptionInformation;
        readonly OperationRetrySettings retrySettings = new();
        readonly CancellationToken cancellationToken = default;

        readonly Mock<IExceptionHandler> handler;
        readonly Exception reportedException = new InvalidOperationException(fuzzy.String());
        readonly ResolvedServicePartition rsp = MakeRsp();
        readonly ResolvedServiceEndpoint endpoint = MakeEndpoint(fuzzy.String());
        readonly string listenerName = fuzzy.String();

        public ReportOperationExceptionAsync()
        {
            client = Mock.Of<ICommunicationClient>(c =>
                c.ResolvedServicePartition == rsp &&
                c.Endpoint == endpoint &&
                c.ListenerName == listenerName);
            exceptionInformation = new ExceptionInformation(reportedException);
            handler = Mock.Get(exceptionHandlers.First());
        }

        [Fact]
        public async Task ReturnsShouldRetryFalseAndOriginalExceptionWhenNoHandlerMatches()
        {
            Mock<IExceptionHandler> handler2 = Mock.Get(exceptionHandlers.Last());

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.False(actual.ShouldRetry);
            Assert.Same(reportedException, actual.Exception);
            Assert.Equal(Timeout.InfiniteTimeSpan, actual.RetryDelay);
            handler.Verify(
                _ => _.TryHandleException(exceptionInformation, retrySettings, out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
            handler2.Verify(
                _ => _.TryHandleException(exceptionInformation, retrySettings, out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
        }

        [Fact]
        public async Task ReturnsThrowResultExceptionToThrowWhenHandlerReturnsThrowResultWithReplacement()
        {
            var replacement = new ApplicationException();
            ExceptionHandlingResult result = new ExceptionHandlingThrowResult { ExceptionToThrow = replacement };
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.False(actual.ShouldRetry);
            Assert.Same(replacement, actual.Exception);
            Assert.Equal(Timeout.InfiniteTimeSpan, actual.RetryDelay);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task ReturnsOriginalExceptionWhenHandlerReturnsThrowResultWithoutReplacement()
        {
            ExceptionHandlingResult result = new ExceptionHandlingThrowResult();
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.False(actual.ShouldRetry);
            Assert.Same(reportedException, actual.Exception);
            Assert.Equal(Timeout.InfiniteTimeSpan, actual.RetryDelay);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task ReturnsRetryControlPopulatedFromTransientRetryResult()
        {
            var retry = new ExceptionHandlingRetryResult(
                reportedException, true, fuzzy.TimeSpan(), fuzzy.Int32());
            ExceptionHandlingResult result = retry;
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.True(actual.ShouldRetry);
            Assert.True(actual.IsTransient);
            Assert.Equal(retry.RetryDelay, actual.RetryDelay);
            Assert.Equal(retry.ExceptionId, actual.ExceptionId);
            Assert.Equal(retry.MaxRetryCount, actual.MaxRetryCount);
            Func<int, TimeSpan> expectedGetRetryDelay = retry.GetRetryDelay;
            // Assert.Equal, not Assert.Same: ExceptionHandlingRetryResult.GetRetryDelay is a method,
            // so each access creates a new delegate via method-group conversion.
            Assert.Equal(expectedGetRetryDelay, actual.GetRetryDelay);
            Assert.Null(actual.Exception);
            Assert.Null(sut.AbortedClient);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task DoesNotAbortClientWhenNonTransientRetryAndCacheEntryHasNoClient()
        {
            var retry = new ExceptionHandlingRetryResult(
                reportedException, false, fuzzy.TimeSpan(), fuzzy.Int32());
            ExceptionHandlingResult result = retry;
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.True(actual.ShouldRetry);
            Assert.False(actual.IsTransient);
            Assert.Null(sut.AbortedClient);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task DoesNotAbortClientWhenNonTransientRetryAndCacheEntryHasDifferentClient()
        {
            var cache = sut.Field<CommunicationClientCache<ICommunicationClient>>().Value;
            CommunicationClientCacheEntry<ICommunicationClient> entry =
                cache.GetOrAddClientCacheEntry(rsp.Info.Id, endpoint, listenerName, rsp);
            var cachedClient = Mock.Of<ICommunicationClient>();
            entry.Client = cachedClient;

            var retry = new ExceptionHandlingRetryResult(
                reportedException, false, fuzzy.TimeSpan(), fuzzy.Int32());
            ExceptionHandlingResult result = retry;
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.True(actual.ShouldRetry);
            Assert.False(actual.IsTransient);
            Assert.Null(sut.AbortedClient);
            Assert.Same(cachedClient, entry.Client);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AbortsClientAndClearsCacheEntryWhenNonTransientRetryAndClientMatchesCacheEntry(bool fireConnectEvents)
        {
            using var other = new TestFactory(fireConnectEvents, servicePartitionResolver, exceptionHandlers, traceId);
            var cache = other.Field<CommunicationClientCache<ICommunicationClient>>().Value;
            CommunicationClientCacheEntry<ICommunicationClient> entry =
                cache.GetOrAddClientCacheEntry(rsp.Info.Id, endpoint, listenerName, rsp);
            entry.Client = client;

            var disconnected = new List<ICommunicationClient>();
            other.ClientDisconnected += (_, e) => disconnected.Add(e.Client);

            var retry = new ExceptionHandlingRetryResult(
                reportedException, false, fuzzy.TimeSpan(), fuzzy.Int32());
            ExceptionHandlingResult result = retry;
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            _ = await other.ReportOperationExceptionAsync(client, exceptionInformation, retrySettings, cancellationToken);

            Assert.Same(client, other.AbortedClient);
            Assert.Null(entry.Client);
            Assert.Null(entry.Rsp);
            Assert.Equal(fireConnectEvents ? new[] { client } : Array.Empty<ICommunicationClient>(), disconnected);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task UnwrapsAggregateExceptionAndForwardsInnerExceptionsToHandler()
        {
            var inner = new InvalidOperationException(fuzzy.String());
            var info = new ExceptionInformation(new AggregateException(inner), TargetReplicaSelector.RandomReplica);

            ExceptionHandlingResult result = new ExceptionHandlingRetryResult(inner, true, fuzzy.TimeSpan(), 1);
            _ = handler
                .Setup(_ => _.TryHandleException(
                    It.Is<ExceptionInformation>(ei => ei.Exception == inner && ei.TargetReplica == info.TargetReplica),
                    retrySettings,
                    out result))
                .Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(client, info, retrySettings, cancellationToken);

            Assert.True(actual.ShouldRetry);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task ReturnsShouldRetryFalseAndOriginalAggregateWhenNoInnerExceptionIsHandled()
        {
            var aggregate = new AggregateException(
                new InvalidOperationException(fuzzy.String()),
                new ApplicationException(fuzzy.String()));
            var info = new ExceptionInformation(aggregate);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(client, info, retrySettings, cancellationToken);

            Assert.False(actual.ShouldRetry);
            Assert.Same(aggregate, actual.Exception);
            Mock<IExceptionHandler> handler2 = Mock.Get(exceptionHandlers.Last());
            foreach (Mock<IExceptionHandler> h in new[] { handler, handler2 })
            {
                h.Verify(
                    _ => _.TryHandleException(
                        It.Is<ExceptionInformation>(ei => ei.Exception == aggregate.InnerExceptions[0]),
                        retrySettings,
                        out It.Ref<ExceptionHandlingResult>.IsAny),
                    Times.Once);
                h.Verify(
                    _ => _.TryHandleException(
                        It.Is<ExceptionInformation>(ei => ei.Exception == aggregate.InnerExceptions[1]),
                        retrySettings,
                        out It.Ref<ExceptionHandlingResult>.IsAny),
                    Times.Once);
                h.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Exactly(2));
            }
        }

        [Fact]
        public async Task InvokesLaterInnerExceptionWhenEarlierInnerExceptionIsNotHandled()
        {
            var unhandled = new InvalidOperationException(fuzzy.String());
            var handled = new ApplicationException(fuzzy.String());
            var info = new ExceptionInformation(new AggregateException(unhandled, handled));

            ExceptionHandlingResult result = new ExceptionHandlingRetryResult(handled, true, fuzzy.TimeSpan(), 1);
            _ = handler
                .Setup(_ => _.TryHandleException(
                    It.Is<ExceptionInformation>(ei => ei.Exception == handled),
                    retrySettings,
                    out result))
                .Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(client, info, retrySettings, cancellationToken);

            Assert.True(actual.ShouldRetry);
            handler.Verify(
                _ => _.TryHandleException(
                    It.Is<ExceptionInformation>(ei => ei.Exception == unhandled),
                    retrySettings,
                    out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
            Mock<IExceptionHandler> handler2 = Mock.Get(exceptionHandlers.Last());
            handler2.Verify(
                _ => _.TryHandleException(
                    It.Is<ExceptionInformation>(ei => ei.Exception == unhandled),
                    retrySettings,
                    out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
            handler2.Verify(
                _ => _.TryHandleException(
                    It.Is<ExceptionInformation>(ei => ei.Exception == handled),
                    retrySettings,
                    out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Never);
            handler.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Exactly(2));
            handler2.Verify(_ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny), Times.Once);
        }

        [Fact]
        public async Task InvokesSubsequentHandlerWhenPreviousHandlerDoesNotMatch()
        {
            Mock<IExceptionHandler> handler2 = Mock.Get(exceptionHandlers.Last());
            var retry = new ExceptionHandlingRetryResult(
                reportedException, true, fuzzy.TimeSpan(), fuzzy.Int32());
            ExceptionHandlingResult result = retry;
            _ = handler2.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);

            OperationRetryControl actual = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            Assert.True(actual.ShouldRetry);
            handler.Verify(
                _ => _.TryHandleException(exceptionInformation, retrySettings, out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
            handler.Verify(
                _ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
            handler2.Verify(
                _ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
        }

        [Fact]
        public async Task DoesNotInvokeSubsequentHandlerWhenPreviousHandlerMatches()
        {
            ExceptionHandlingResult result = new ExceptionHandlingThrowResult();
            _ = handler.Setup(_ => _.TryHandleException(exceptionInformation, retrySettings, out result)).Returns(true);
            Mock<IExceptionHandler> handler2 = Mock.Get(exceptionHandlers.Last());

            _ = await sut.ReportOperationExceptionAsync(
                client, exceptionInformation, retrySettings, cancellationToken);

            handler.Verify(
                _ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Once);
            handler2.Verify(
                _ => _.TryHandleException(It.IsAny<ExceptionInformation>(), It.IsAny<OperationRetrySettings>(), out It.Ref<ExceptionHandlingResult>.IsAny),
                Times.Never);
        }
    }

    static ResolvedServiceEndpoint MakeEndpoint(string address)
    {
        var endpoint = new ResolvedServiceEndpoint();
        endpoint.Property<string>().Set(address);
        return endpoint;
    }

    static ResolvedServicePartition MakeRsp()
    {
        var rsp = Type<ResolvedServicePartition>.Uninitialized();
        rsp.Property<ServicePartitionInformation>().Set(Type<SingletonPartitionInformation>.Uninitialized());
        return rsp;
    }

    sealed class TestFactory : CommunicationClientFactoryBase<ICommunicationClient>, IDisposable
    {
        internal ICommunicationClient AbortedClient;

        internal TestFactory(
            bool fireConnectEvents,
            IServicePartitionResolver servicePartitionResolver,
            IEnumerable<IExceptionHandler> exceptionHandlers,
            string traceId)
            : base(fireConnectEvents, servicePartitionResolver, exceptionHandlers, traceId)
        {
        }

        internal TestFactory(
            IServicePartitionResolver servicePartitionResolver,
            IEnumerable<IExceptionHandler> exceptionHandlers,
            string traceId)
            : base(servicePartitionResolver, exceptionHandlers, traceId)
        {
        }

        internal string TraceIdValue => TraceId;

        protected override bool ValidateClient(ICommunicationClient client) => true;

        protected override bool ValidateClient(string endpoint, ICommunicationClient client) => true;

        protected override Task<ICommunicationClient> CreateClientAsync(string endpoint, CancellationToken cancellationToken) =>
            Task.FromResult(Mock.Of<ICommunicationClient>());

        protected override void AbortClient(ICommunicationClient client) =>
            AbortedClient = client;

        internal Task TestOpenClient(ICommunicationClient client, CancellationToken cancellationToken) =>
            base.OpenClient(client, cancellationToken);
    }
}
