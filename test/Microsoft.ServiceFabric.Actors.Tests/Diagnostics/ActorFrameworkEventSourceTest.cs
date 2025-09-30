using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Tests;
using Xunit;

using ActorFrameworkKeywords = Microsoft.ServiceFabric.Actors.Diagnostics.ActorFrameworkEventSource.Keywords;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public sealed class ActorFrameworkEventSourceTest : IDisposable
    {
        readonly EventSourceTest<ActorFrameworkEventSource> test = new EventSourceTest<ActorFrameworkEventSource>();

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public void Dispose() =>
            test.Dispose();

        // Method parameters
        readonly string exception = fuzzy.String();
        readonly long countOfWaitingMethodCalls = fuzzy.Int64();
        readonly TimeSpan executionTime = fuzzy.TimeSpan();
        readonly string methodName = fuzzy.String();
        readonly string methodSignature = fuzzy.String();
        readonly string actorType = fuzzy.String();
        readonly ActorId actorId = fuzzy.ActorId();
        readonly ServiceContext service = fuzzy.ServiceContext();

        [Fact]
        public void ActorActivatedPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorActivated(actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(5, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default);
            Assert.Equal("ActorActivated", test.Event.EventName);
            test.EventPayload(0, "actorType", actorType);
            test.EventPayload(1, "actorId", actorId.ToString());
            test.EventPayload(2, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(3, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(4, "partitionId", service.PartitionId);
            test.EventPayload(5, "serviceName", service.ServiceName);
            test.EventPayload(6, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(7, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(8, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(9, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorDeactivatedPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorDeactivated(actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(6, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default);
            Assert.Equal("ActorDeactivated", test.Event.EventName);
            test.EventPayload(0, "actorType", actorType);
            test.EventPayload(1, "actorId", actorId.ToString());
            test.EventPayload(2, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(3, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(4, "partitionId", service.PartitionId);
            test.EventPayload(5, "serviceName", service.ServiceName);
            test.EventPayload(6, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(7, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(8, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(9, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorMethodCallsWaitingForLockPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorMethodCallsWaitingForLock(countOfWaitingMethodCalls, actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(12, test.Event.EventId);
            Assert.Equal(EventLevel.Verbose, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.MetricActorMethodCallsWaitingForLock);
            Assert.Equal("ActorMethodCallsWaitingForLock", test.Event.EventName);
            test.EventPayload(0, "countOfWaitingMethodCalls", countOfWaitingMethodCalls);
            test.EventPayload(1, "actorType", actorType);
            test.EventPayload(2, "actorId", actorId.ToString());
            test.EventPayload(3, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(4, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(5, "partitionId", service.PartitionId);
            test.EventPayload(6, "serviceName", service.ServiceName);
            test.EventPayload(7, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(8, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(9, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(10, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorMethodStartPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorMethodStart(methodName, methodSignature, actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(7, test.Event.EventId);
            Assert.Equal(EventLevel.Verbose, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.ActorMethod);
            Assert.Equal("ActorMethodStart", test.Event.EventName);
            test.EventPayload(0, "methodName", methodName);
            test.EventPayload(1, "methodSignature", methodSignature);
            test.EventPayload(2, "actorType", actorType);
            test.EventPayload(3, "actorId", actorId.ToString());
            test.EventPayload(4, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(5, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(6, "partitionId", service.PartitionId);
            test.EventPayload(7, "serviceName", service.ServiceName);
            test.EventPayload(8, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(9, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(10, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(11, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorMethodStopPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorMethodStop(executionTime.Ticks, methodName, methodSignature, actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(8, test.Event.EventId);
            Assert.Equal(EventLevel.Verbose, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.ActorMethod);
            Assert.Equal("ActorMethodStop", test.Event.EventName);
            test.EventPayload(0, "methodExecutionTimeTicks", executionTime.Ticks);
            test.EventPayload(1, "methodName", methodName);
            test.EventPayload(2, "methodSignature", methodSignature);
            test.EventPayload(3, "actorType", actorType);
            test.EventPayload(4, "actorId", actorId.ToString());
            test.EventPayload(5, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(6, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(7, "partitionId", service.PartitionId);
            test.EventPayload(8, "serviceName", service.ServiceName);
            test.EventPayload(9, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(10, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(11, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(12, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorMethodThrewExceptionPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorMethodThrewException(exception, executionTime.Ticks, methodName, methodSignature, actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(9, test.Event.EventId);
            Assert.Equal(EventLevel.Warning, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default | ActorFrameworkKeywords.ActorMethod);
            Assert.Equal("ActorMethodThrewException", test.Event.EventName);
            test.EventPayload(0, "exception", exception);
            test.EventPayload(1, "methodExecutionTimeTicks", executionTime.Ticks);
            test.EventPayload(2, "methodName", methodName);
            test.EventPayload(3, "methodSignature", methodSignature);
            test.EventPayload(4, "actorType", actorType);
            test.EventPayload(5, "actorId", actorId.ToString());
            test.EventPayload(6, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(7, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(8, "partitionId", service.PartitionId);
            test.EventPayload(9, "serviceName", service.ServiceName);
            test.EventPayload(10, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(11, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(12, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(13, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorSaveStateStartPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorSaveStateStart(actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(10, test.Event.EventId);
            Assert.Equal(EventLevel.Verbose, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.ActorState);
            Assert.Equal("ActorSaveStateStart", test.Event.EventName);
            test.EventPayload(0, "actorType", actorType);
            test.EventPayload(1, "actorId", actorId.ToString());
            test.EventPayload(2, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(3, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(4, "partitionId", service.PartitionId);
            test.EventPayload(5, "serviceName", service.ServiceName);
            test.EventPayload(6, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(7, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(8, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(9, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ActorSaveStateStopPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ActorSaveStateStop(executionTime.Ticks, actorType, actorId, service);

            Assert.NotNull(test.Event);
            Assert.Equal(11, test.Event.EventId);
            Assert.Equal(EventLevel.Verbose, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.ActorState);
            Assert.Equal("ActorSaveStateStop", test.Event.EventName);
            test.EventPayload(0, "saveStateExecutionTimeTicks", executionTime.Ticks);
            test.EventPayload(1, "actorType", actorType);
            test.EventPayload(2, "actorId", actorId.ToString());
            test.EventPayload(3, "actorIdKind", (int)actorId.Kind);
            test.EventPayload(4, "replicaOrInstanceId", service.ReplicaOrInstanceId);
            test.EventPayload(5, "partitionId", service.PartitionId);
            test.EventPayload(6, "serviceName", service.ServiceName);
            test.EventPayload(7, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(8, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(9, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(10, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ReplicaChangeRoleFromPrimaryPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ReplicaChangeRoleFromPrimary(service);

            Assert.NotNull(test.Event);
            Assert.Equal(2, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default);
            Assert.Equal("ReplicaChangeRoleFromPrimary", test.Event.EventName);
            test.EventPayload(0, "replicaId", service.ReplicaOrInstanceId);
            test.EventPayload(1, "partitionId", service.PartitionId);
            test.EventPayload(2, "serviceName", service.ServiceName);
            test.EventPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(4, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(6, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ReplicaChangeRoleToPrimaryPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ReplicaChangeRoleToPrimary(service);

            Assert.NotNull(test.Event);
            Assert.Equal(1, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default);
            Assert.Equal("ReplicaChangeRoleToPrimary", test.Event.EventName);
            test.EventPayload(0, "replicaId", service.ReplicaOrInstanceId);
            test.EventPayload(1, "partitionId", service.PartitionId);
            test.EventPayload(2, "serviceName", service.ServiceName);
            test.EventPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(4, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(6, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ServiceInstanceClosePublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ServiceInstanceClose(service);

            Assert.NotNull(test.Event);
            Assert.Equal(4, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default);
            Assert.Equal("ServiceInstanceClose", test.Event.EventName);
            test.EventPayload(0, "instanceId", service.ReplicaOrInstanceId);
            test.EventPayload(1, "partitionId", service.PartitionId);
            test.EventPayload(2, "serviceName", service.ServiceName);
            test.EventPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(4, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(6, "nodeName", service.NodeContext.NodeName);
        }

        [Fact]
        public void ServiceInstanceOpenPublishesExpectedEvent()
        {
            test.EnableEvents(EventLevel.LogAlways);

            test.Instance.ServiceInstanceOpen(service);

            Assert.NotNull(test.Event);
            Assert.Equal(3, test.Event.EventId);
            Assert.Equal(EventLevel.Informational, test.Event.Level);
            test.EventKeywords(ActorFrameworkKeywords.Default);
            Assert.Equal("ServiceInstanceOpen", test.Event.EventName);
            test.EventPayload(0, "instanceId", service.ReplicaOrInstanceId);
            test.EventPayload(1, "partitionId", service.PartitionId);
            test.EventPayload(2, "serviceName", service.ServiceName);
            test.EventPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName);
            test.EventPayload(4, "serviceTypeName", service.ServiceTypeName);
            test.EventPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName);
            test.EventPayload(6, "nodeName", service.NodeContext.NodeName);
        }

        [Theory]
        [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorMethod)]
        [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorMethod)]
        [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
        public void IsActorMethodStartEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords) =>
            test.EventEnabled(expected, level, keywords, test.Instance.IsActorMethodStartEventEnabled);

        [Theory]
        [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorMethod)]
        [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorMethod)]
        [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
        public void IsActorMethodStopEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords) =>
            test.EventEnabled(expected, level, keywords, test.Instance.IsActorMethodStopEventEnabled);

        [Theory]
        [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorState)]
        [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorState)]
        [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
        public void IsActorSaveStateStartEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords) =>
            test.EventEnabled(expected, level, keywords, test.Instance.IsActorSaveStateStartEventEnabled);

        [Theory]
        [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorState)]
        [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorState)]
        [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
        public void IsActorSaveStateStopEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords) =>
            test.EventEnabled(expected, level, keywords, test.Instance.IsActorSaveStateStopEventEnabled);

        [Theory]
        [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.MetricActorMethodCallsWaitingForLock)]
        [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.MetricActorMethodCallsWaitingForLock)]
        [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
        public void IsPendingMethodCallsEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords) =>
            test.EventEnabled(expected, level, keywords, test.Instance.IsPendingMethodCallsEventEnabled);

        [Fact]
        public void GuidRemainsUnchangedForBackwardCompatibilityWithCollectionTools() =>
            Assert.Equal(new Guid("0e1ec353-9f02-55d7-fbb8-f3857458acbd"), test.Instance.Guid);

        [Fact]
        public void ManifestCanBeSavedForRegistrationWithExternalTools() =>
            test.Manifest();
    }
}
