// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using Fuzzy;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public sealed class ServiceFrameworkEventSourceTest : IDisposable
    {
        readonly EventSourceTest<ServiceFrameworkEventSource> test;

        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public ServiceFrameworkEventSourceTest(ITestOutputHelper output) =>
            test = new EventSourceTest<ServiceFrameworkEventSource>(output);

        public void Dispose() =>
            test.Dispose();

        // Method parameters
        readonly StatefulServiceContext statefulService = fuzzy.StatefulServiceContext();
        readonly StatelessServiceContext statelessService = fuzzy.StatelessServiceContext();
        readonly bool wasCanceled = fuzzy.Boolean();
        readonly Exception exception = new Exception(fuzzy.String());
        readonly TimeSpan slowCancellationTime = fuzzy.TimeSpan();
        readonly TimeSpan actualCancellationTime = fuzzy.TimeSpan();

        [Fact]
        public void StatefulRunAsyncInvocationPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatefulRunAsyncInvocation(statefulService);

            Assert.NotNull(test.Event);
            Assert.Equal(1, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatefulRunAsyncInvocation", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statefulService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statefulService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statefulService.PartitionId.ToString());
            test.EventPayload(5, "replicaId", statefulService.ReplicaId);
        }

        [Fact]
        public void StatefulRunAsyncCancellationPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatefulRunAsyncCancellation(statefulService, slowCancellationTime);

            Assert.NotNull(test.Event);
            Assert.Equal(2, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatefulRunAsyncCancellation", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statefulService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statefulService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statefulService.PartitionId.ToString());
            test.EventPayload(5, "replicaId", statefulService.ReplicaId);
            test.EventPayload(6, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds);
        }

        [Fact]
        public void StatefulRunAsyncCompletionPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatefulRunAsyncCompletion(statefulService, wasCanceled);

            Assert.NotNull(test.Event);
            Assert.Equal(3, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatefulRunAsyncCompletion", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statefulService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statefulService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statefulService.PartitionId.ToString());
            test.EventPayload(5, "replicaId", statefulService.ReplicaId);
            test.EventPayload(6, "wasCanceled", wasCanceled);
        }

        [Fact]
        public void StatefulRunAsyncSlowCancellationPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatefulRunAsyncSlowCancellation(statefulService, actualCancellationTime, slowCancellationTime);

            Assert.NotNull(test.Event);
            Assert.Equal(4, test.Event.EventId);
            Assert.Equal(EventLevel.Warning, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatefulRunAsyncSlowCancellation", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statefulService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statefulService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statefulService.PartitionId.ToString());
            test.EventPayload(5, "replicaId", statefulService.ReplicaId);
            test.EventPayload(6, "actualCancellationTimeMillis", actualCancellationTime.TotalMilliseconds);
            test.EventPayload(7, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds);
        }

        [Fact]
        public void StatefulRunAsyncFailurePublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatefulRunAsyncFailure(statefulService, wasCanceled, exception);

            Assert.NotNull(test.Event);
            Assert.Equal(5, test.Event.EventId);
            Assert.Equal(EventLevel.Error, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatefulRunAsyncFailure", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statefulService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statefulService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statefulService.PartitionId.ToString());
            test.EventPayload(5, "replicaId", statefulService.ReplicaId);
            test.EventPayload(6, "wasCanceled", wasCanceled);
            test.EventPayload(7, "exception", exception.ToString());
        }

        [Fact]
        public void StatelessRunAsyncInvocationPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatelessRunAsyncInvocation(statelessService);

            Assert.NotNull(test.Event);
            Assert.Equal(6, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatelessRunAsyncInvocation", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statelessService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statelessService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statelessService.PartitionId.ToString());
            test.EventPayload(5, "instanceId", statelessService.InstanceId);
        }

        [Fact]
        public void StatelessRunAsyncCancellationPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatelessRunAsyncCancellation(statelessService, slowCancellationTime);

            Assert.NotNull(test.Event);
            Assert.Equal(7, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatelessRunAsyncCancellation", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statelessService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statelessService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statelessService.PartitionId.ToString());
            test.EventPayload(5, "instanceId", statelessService.InstanceId);
            test.EventPayload(6, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds);
        }

        [Fact]
        public void StatelessRunAsyncCompletionPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatelessRunAsyncCompletion(statelessService, wasCanceled);

            Assert.NotNull(test.Event);
            Assert.Equal(8, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatelessRunAsyncCompletion", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statelessService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statelessService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statelessService.PartitionId.ToString());
            test.EventPayload(5, "instanceId", statelessService.InstanceId);
            test.EventPayload(6, "wasCanceled", wasCanceled);
        }

        [Fact]
        public void StatelessRunAsyncSlowCancellationPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatelessRunAsyncSlowCancellation(statelessService, actualCancellationTime, slowCancellationTime);

            Assert.NotNull(test.Event);
            Assert.Equal(9, test.Event.EventId);
            Assert.Equal(EventLevel.Warning, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatelessRunAsyncSlowCancellation", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statelessService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statelessService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statelessService.PartitionId.ToString());
            test.EventPayload(5, "instanceId", statelessService.InstanceId);
            test.EventPayload(6, "actualCancellationTimeMillis", actualCancellationTime.TotalMilliseconds);
            test.EventPayload(7, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds);
        }

        [Fact]
        public void StatelessRunAsyncFailurePublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.StatelessRunAsyncFailure(statelessService, wasCanceled, exception);

            Assert.NotNull(test.Event);
            Assert.Equal(10, test.Event.EventId);
            Assert.Equal(EventLevel.Error, test.Event.Level);
            test.EventKeywords(EventKeywords.None);
            Assert.Equal("StatelessRunAsyncFailure", test.Event.EventName);
            test.EventPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName);
            test.EventPayload(2, "serviceTypeName", statelessService.ServiceTypeName);
            test.EventPayload(3, "serviceName", statelessService.ServiceName.OriginalString);
            test.EventPayload(4, "partitionId", statelessService.PartitionId.ToString());
            test.EventPayload(5, "instanceId", statelessService.InstanceId);
            test.EventPayload(6, "wasCanceled", wasCanceled);
            test.EventPayload(7, "exception", exception.ToString());
        }

        [Fact]
        public void GuidRemainsUnchangedForBackwardCompatibilityWithCollectionTools() =>
            Assert.Equal(new Guid("13c2a97d-71da-5ab5-47cb-1497aec602e1"), test.Instance.Guid);

        [Fact]
        public void ManifestCanBeSavedForRegistrationWithExternalTools() =>
            test.Manifest();
    }
}
