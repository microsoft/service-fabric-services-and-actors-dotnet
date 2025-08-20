using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.IO;
using System.Runtime.InteropServices;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Moq;
using Xunit;
using Xunit.Abstractions;

using ActorFrameworkKeywords = Microsoft.ServiceFabric.Actors.Diagnostics.ActorFrameworkEventSource.Keywords;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public abstract class ActorFrameworkEventSourceTest : IDisposable
    {
        readonly ActorFrameworkEventSource sut;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public ActorFrameworkEventSourceTest()
        {
            // Allow event enablement to work on instances created by the tests
            ActorFrameworkEventSource.Writer.Dispose();

            // Disable Linux detection in sut to allow tests to run without UnstructuredTracePublisher which fails without FabricCommon
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(_ => false);

            sut = new ActorFrameworkEventSource();
        }

        public virtual void Dispose()
        {
            sut.Dispose();

            // Restore original static state
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(RuntimeInformation.IsOSPlatform);
            typeof(ActorFrameworkEventSource).Property<ActorFrameworkEventSource>().Set(new ActorFrameworkEventSource());
        }

        public sealed class EventTest : ActorFrameworkEventSourceTest
        {
            // Method parameters
            readonly string exception = fuzzy.String();
            readonly long countOfWaitingMethodCalls = fuzzy.Int64();
            readonly TimeSpan executionTime = fuzzy.TimeSpan();
            readonly string methodName = fuzzy.String();
            readonly string methodSignature = fuzzy.String();
            readonly string actorType = fuzzy.String();
            readonly ActorId actorId = fuzzy.ActorId();
            readonly ServiceContext service = fuzzy.ServiceContext();

            const EventKeywords AllSessions = (EventKeywords)(0xFul << 44);
            EventWrittenEventArgs actual;

            readonly EventListener listener = new Mock<EventListener>() { CallBase = true }.Object;

            public EventTest()
            {
                listener.EventWritten += (object sender, EventWrittenEventArgs args) => actual = args;
                listener.EnableEvents(sut, EventLevel.LogAlways);
            }

            public override void Dispose()
            {
                listener.Dispose();
                base.Dispose();
            }

            static void AssertPayload<T>(int index, string name, T value, EventWrittenEventArgs actual)
            {
                Assert.Equal(name, actual.PayloadNames[index]);
                Assert.Equal(value, actual.Payload[index]);
            }

            [Fact]
            public void ActorActivatedPublishesExpectedEvent()
            {
                sut.ActorActivated(actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(5, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default, actual.Keywords);
                Assert.Equal("ActorActivated", actual.EventName);
                AssertPayload(0, "actorType", actorType, actual);
                AssertPayload(1, "actorId", actorId.ToString(), actual);
                AssertPayload(2, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(3, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(4, "partitionId", service.PartitionId, actual);
                AssertPayload(5, "serviceName", service.ServiceName, actual);
                AssertPayload(6, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(7, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(8, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(9, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorDeactivatedPublishesExpectedEvent()
            {
                sut.ActorDeactivated(actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(6, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default, actual.Keywords);
                Assert.Equal("ActorDeactivated", actual.EventName);
                AssertPayload(0, "actorType", actorType, actual);
                AssertPayload(1, "actorId", actorId.ToString(), actual);
                AssertPayload(2, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(3, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(4, "partitionId", service.PartitionId, actual);
                AssertPayload(5, "serviceName", service.ServiceName, actual);
                AssertPayload(6, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(7, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(8, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(9, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorMethodCallsWaitingForLockPublishesExpectedEvent()
            {
                sut.ActorMethodCallsWaitingForLock(countOfWaitingMethodCalls, actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(12, actual.EventId);
                Assert.Equal(EventLevel.Verbose, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.MetricActorMethodCallsWaitingForLock, actual.Keywords);
                Assert.Equal("ActorMethodCallsWaitingForLock", actual.EventName);
                AssertPayload(0, "countOfWaitingMethodCalls", countOfWaitingMethodCalls, actual);
                AssertPayload(1, "actorType", actorType, actual);
                AssertPayload(2, "actorId", actorId.ToString(), actual);
                AssertPayload(3, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(4, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(5, "partitionId", service.PartitionId, actual);
                AssertPayload(6, "serviceName", service.ServiceName, actual);
                AssertPayload(7, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(8, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(9, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(10, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorMethodStartPublishesExpectedEvent()
            {
                sut.ActorMethodStart(methodName, methodSignature, actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(7, actual.EventId);
                Assert.Equal(EventLevel.Verbose, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.ActorMethod, actual.Keywords);
                Assert.Equal("ActorMethodStart", actual.EventName);
                AssertPayload(0, "methodName", methodName, actual);
                AssertPayload(1, "methodSignature", methodSignature, actual);
                AssertPayload(2, "actorType", actorType, actual);
                AssertPayload(3, "actorId", actorId.ToString(), actual);
                AssertPayload(4, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(5, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(6, "partitionId", service.PartitionId, actual);
                AssertPayload(7, "serviceName", service.ServiceName, actual);
                AssertPayload(8, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(9, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(10, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(11, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorMethodStopPublishesExpectedEvent()
            {
                sut.ActorMethodStop(executionTime.Ticks, methodName, methodSignature, actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(8, actual.EventId);
                Assert.Equal(EventLevel.Verbose, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.ActorMethod, actual.Keywords);
                Assert.Equal("ActorMethodStop", actual.EventName);
                AssertPayload(0, "methodExecutionTimeTicks", executionTime.Ticks, actual);
                AssertPayload(1, "methodName", methodName, actual);
                AssertPayload(2, "methodSignature", methodSignature, actual);
                AssertPayload(3, "actorType", actorType, actual);
                AssertPayload(4, "actorId", actorId.ToString(), actual);
                AssertPayload(5, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(6, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(7, "partitionId", service.PartitionId, actual);
                AssertPayload(8, "serviceName", service.ServiceName, actual);
                AssertPayload(9, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(10, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(11, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(12, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorMethodThrewExceptionPublishesExpectedEvent()
            {
                sut.ActorMethodThrewException(exception, executionTime.Ticks, methodName, methodSignature, actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(9, actual.EventId);
                Assert.Equal(EventLevel.Warning, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default | ActorFrameworkKeywords.ActorMethod, actual.Keywords);
                Assert.Equal("ActorMethodThrewException", actual.EventName);
                AssertPayload(0, "exception", exception, actual);
                AssertPayload(1, "methodExecutionTimeTicks", executionTime.Ticks, actual);
                AssertPayload(2, "methodName", methodName, actual);
                AssertPayload(3, "methodSignature", methodSignature, actual);
                AssertPayload(4, "actorType", actorType, actual);
                AssertPayload(5, "actorId", actorId.ToString(), actual);
                AssertPayload(6, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(7, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(8, "partitionId", service.PartitionId, actual);
                AssertPayload(9, "serviceName", service.ServiceName, actual);
                AssertPayload(10, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(11, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(12, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(13, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorSaveStateStartPublishesExpectedEvent()
            {
                sut.ActorSaveStateStart(actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(10, actual.EventId);
                Assert.Equal(EventLevel.Verbose, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.ActorState, actual.Keywords);
                Assert.Equal("ActorSaveStateStart", actual.EventName);
                AssertPayload(0, "actorType", actorType, actual);
                AssertPayload(1, "actorId", actorId.ToString(), actual);
                AssertPayload(2, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(3, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(4, "partitionId", service.PartitionId, actual);
                AssertPayload(5, "serviceName", service.ServiceName, actual);
                AssertPayload(6, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(7, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(8, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(9, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ActorSaveStateStopPublishesExpectedEvent()
            {
                sut.ActorSaveStateStop(executionTime.Ticks, actorType, actorId, service);

                Assert.NotNull(actual);
                Assert.Equal(11, actual.EventId);
                Assert.Equal(EventLevel.Verbose, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.ActorState, actual.Keywords);
                Assert.Equal("ActorSaveStateStop", actual.EventName);
                AssertPayload(0, "saveStateExecutionTimeTicks", executionTime.Ticks, actual);
                AssertPayload(1, "actorType", actorType, actual);
                AssertPayload(2, "actorId", actorId.ToString(), actual);
                AssertPayload(3, "actorIdKind", (int)actorId.Kind, actual);
                AssertPayload(4, "replicaOrInstanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(5, "partitionId", service.PartitionId, actual);
                AssertPayload(6, "serviceName", service.ServiceName, actual);
                AssertPayload(7, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(8, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(9, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(10, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ReplicaChangeRoleFromPrimaryPublishesExpectedEvent()
            {
                sut.ReplicaChangeRoleFromPrimary(service);

                Assert.NotNull(actual);
                Assert.Equal(2, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default, actual.Keywords);
                Assert.Equal("ReplicaChangeRoleFromPrimary", actual.EventName);
                AssertPayload(0, "replicaId", service.ReplicaOrInstanceId, actual);
                AssertPayload(1, "partitionId", service.PartitionId, actual);
                AssertPayload(2, "serviceName", service.ServiceName, actual);
                AssertPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(4, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(6, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ReplicaChangeRoleToPrimaryPublishesExpectedEvent()
            {
                sut.ReplicaChangeRoleToPrimary(service);

                Assert.NotNull(actual);
                Assert.Equal(1, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default, actual.Keywords);
                Assert.Equal("ReplicaChangeRoleToPrimary", actual.EventName);
                AssertPayload(0, "replicaId", service.ReplicaOrInstanceId, actual);
                AssertPayload(1, "partitionId", service.PartitionId, actual);
                AssertPayload(2, "serviceName", service.ServiceName, actual);
                AssertPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(4, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(6, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ServiceInstanceClosePublishesExpectedEvent()
            {
                sut.ServiceInstanceClose(service);

                Assert.NotNull(actual);
                Assert.Equal(4, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default, actual.Keywords);
                Assert.Equal("ServiceInstanceClose", actual.EventName);
                AssertPayload(0, "instanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(1, "partitionId", service.PartitionId, actual);
                AssertPayload(2, "serviceName", service.ServiceName, actual);
                AssertPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(4, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(6, "nodeName", service.NodeContext.NodeName, actual);
            }

            [Fact]
            public void ServiceInstanceOpenPublishesExpectedEvent()
            {
                sut.ServiceInstanceOpen(service);

                Assert.NotNull(actual);
                Assert.Equal(3, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ActorFrameworkKeywords.Default, actual.Keywords);
                Assert.Equal("ServiceInstanceOpen", actual.EventName);
                AssertPayload(0, "instanceId", service.ReplicaOrInstanceId, actual);
                AssertPayload(1, "partitionId", service.PartitionId, actual);
                AssertPayload(2, "serviceName", service.ServiceName, actual);
                AssertPayload(3, "applicationName", service.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(4, "serviceTypeName", service.ServiceTypeName, actual);
                AssertPayload(5, "applicationTypeName", service.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(6, "nodeName", service.NodeContext.NodeName, actual);
            }
        }

        public sealed class EventEnabledTest : ActorFrameworkEventSourceTest
        {
            void AssertEventEnabled(bool expected, EventLevel level, EventKeywords keywords, Func<bool> actual)
            {
                using var listener = Mock.Of<EventListener>();
                listener.EnableEvents(sut, level, keywords);
                Assert.Equal(expected, actual());
            }

            [Theory]
            [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorMethod)]
            [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorMethod)]
            [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
            public void IsActorMethodStartEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords)
            {
                AssertEventEnabled(expected, level, keywords, sut.IsActorMethodStartEventEnabled);
            }

            [Theory]
            [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorMethod)]
            [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorMethod)]
            [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
            public void IsActorMethodStopEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords)
            {
                AssertEventEnabled(expected, level, keywords, sut.IsActorMethodStopEventEnabled);
            }

            [Theory]
            [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorState)]
            [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorState)]
            [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
            public void IsActorSaveStateStartEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords)
            {
                AssertEventEnabled(expected, level, keywords, sut.IsActorSaveStateStartEventEnabled);
            }

            [Theory]
            [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.ActorState)]
            [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.ActorState)]
            [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
            public void IsActorSaveStateStopEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords)
            {
                AssertEventEnabled(expected, level, keywords, sut.IsActorSaveStateStopEventEnabled);
            }

            [Theory]
            [InlineData(true, EventLevel.Verbose, ActorFrameworkKeywords.MetricActorMethodCallsWaitingForLock)]
            [InlineData(false, EventLevel.Informational, ActorFrameworkKeywords.MetricActorMethodCallsWaitingForLock)]
            [InlineData(false, EventLevel.Verbose, ActorFrameworkKeywords.Default)]
            public void IsPendingMethodCallsEventEnabledReturnsExpectedResult(bool expected, EventLevel level, EventKeywords keywords)
            {
                AssertEventEnabled(expected, level, keywords, sut.IsPendingMethodCallsEventEnabled);
            }
        }

        public sealed class Class : ActorFrameworkEventSourceTest
        {
            [Fact]
            public void InheritsFromServiceFabricEventSourceToSupportTracingOnLinux()
            {
                Assert.IsAssignableFrom<ServiceFabricEventSource>(sut);
            }
        }

        public sealed class Guid : ActorFrameworkEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("0e1ec353-9f02-55d7-fbb8-f3857458acbd"), sut.Guid);
            }
        }

        public sealed class Manifest : ActorFrameworkEventSourceTest
        {
            readonly ITestOutputHelper output;

            public Manifest(ITestOutputHelper output) => this.output = output;

            [Fact]
            public void CanBeSavedForRegistrationWithExternalTools()
            {
                string manifest = EventSource.GenerateManifest(sut.GetType(), sut.GetType().Assembly.Location);

                string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(sut.GetType().Assembly.Location), sut.Name), "man");
                File.WriteAllText(manifestFile, manifest);
                output.WriteLine("To register generated manifest for ETL tools, run");
                output.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
            }
        }
    }
}
