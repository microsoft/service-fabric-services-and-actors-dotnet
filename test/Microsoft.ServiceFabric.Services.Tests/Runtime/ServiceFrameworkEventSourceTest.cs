using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.IO;
using System.Runtime.InteropServices;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class ServiceFrameworkEventSourceTest : IDisposable
    {
        readonly ServiceFrameworkEventSource sut;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public ServiceFrameworkEventSourceTest()
        {
            // Allow event enablement to work on instances created by the tests
            ServiceFrameworkEventSource.Writer.Dispose();

            // Disable Linux detection in sut to allow tests to run without UnstructuredTracePublisher which fails without FabricCommon
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(_ => false);

            sut = new ServiceFrameworkEventSource();
        }

        public virtual void Dispose()
        {
            sut.Dispose();

            // Restore original static state
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(RuntimeInformation.IsOSPlatform);
            typeof(ServiceFrameworkEventSource).Property<ServiceFrameworkEventSource>().Set(new ServiceFrameworkEventSource());
        }

        public sealed class Class : ServiceFrameworkEventSourceTest
        {
            [Fact]
            public void InheritsFromServiceFabricEventSourceToSupportTracingOnLinux()
            {
                Assert.IsAssignableFrom<ServiceFabricEventSource>(sut);
            }
        }

        public sealed class EventTest : ServiceFrameworkEventSourceTest
        {
            // Method parameters
            readonly StatefulServiceContext statefulService = fuzzy.StatefulServiceContext();
            readonly StatelessServiceContext statelessService = fuzzy.StatelessServiceContext();
            readonly bool wasCanceled = fuzzy.Boolean();
            readonly Exception exception = new Exception(fuzzy.String());
            readonly TimeSpan slowCancellationTime = fuzzy.TimeSpan();
            readonly TimeSpan actualCancellationTime = fuzzy.TimeSpan();

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
            public void StatefulRunAsyncInvocationPublishesExpectedEvent()
            {
                sut.StatefulRunAsyncInvocation(statefulService);

                Assert.NotNull(actual);
                Assert.Equal(1, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatefulRunAsyncInvocation", actual.EventName);
                AssertPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statefulService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statefulService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statefulService.PartitionId.ToString(), actual);
                AssertPayload(5, "replicaId", statefulService.ReplicaId, actual);
            }

            [Fact]
            public void StatefulRunAsyncCancellationPublishesExpectedEvent()
            {
                sut.StatefulRunAsyncCancellation(statefulService, slowCancellationTime);

                Assert.NotNull(actual);
                Assert.Equal(2, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatefulRunAsyncCancellation", actual.EventName);
                AssertPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statefulService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statefulService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statefulService.PartitionId.ToString(), actual);
                AssertPayload(5, "replicaId", statefulService.ReplicaId, actual);
                AssertPayload(6, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds, actual);
            }

            [Fact]
            public void StatefulRunAsyncCompletionPublishesExpectedEvent()
            {
                sut.StatefulRunAsyncCompletion(statefulService, wasCanceled);

                Assert.NotNull(actual);
                Assert.Equal(3, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatefulRunAsyncCompletion", actual.EventName);
                AssertPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statefulService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statefulService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statefulService.PartitionId.ToString(), actual);
                AssertPayload(5, "replicaId", statefulService.ReplicaId, actual);
                AssertPayload(6, "wasCanceled", wasCanceled, actual);
            }

            [Fact]
            public void StatefulRunAsyncSlowCancellationPublishesExpectedEvent()
            {
                sut.StatefulRunAsyncSlowCancellation(statefulService, actualCancellationTime, slowCancellationTime);

                Assert.NotNull(actual);
                Assert.Equal(4, actual.EventId);
                Assert.Equal(EventLevel.Warning, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatefulRunAsyncSlowCancellation", actual.EventName);
                AssertPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statefulService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statefulService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statefulService.PartitionId.ToString(), actual);
                AssertPayload(5, "replicaId", statefulService.ReplicaId, actual);
                AssertPayload(6, "actualCancellationTimeMillis", actualCancellationTime.TotalMilliseconds, actual);
                AssertPayload(7, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds, actual);
            }

            [Fact]
            public void StatefulRunAsyncFailurePublishesExpectedEvent()
            {
                sut.StatefulRunAsyncFailure(statefulService, wasCanceled, exception);

                Assert.NotNull(actual);
                Assert.Equal(5, actual.EventId);
                Assert.Equal(EventLevel.Error, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatefulRunAsyncFailure", actual.EventName);
                AssertPayload(0, "applicationTypeName", statefulService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statefulService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statefulService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statefulService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statefulService.PartitionId.ToString(), actual);
                AssertPayload(5, "replicaId", statefulService.ReplicaId, actual);
                AssertPayload(6, "wasCanceled", wasCanceled, actual);
                AssertPayload(7, "exception", exception.ToString(), actual);
            }

            [Fact]
            public void StatelessRunAsyncInvocationPublishesExpectedEvent()
            {
                sut.StatelessRunAsyncInvocation(statelessService);

                Assert.NotNull(actual);
                Assert.Equal(6, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatelessRunAsyncInvocation", actual.EventName);
                AssertPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statelessService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statelessService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statelessService.PartitionId.ToString(), actual);
                AssertPayload(5, "instanceId", statelessService.InstanceId, actual);
            }

            [Fact]
            public void StatelessRunAsyncCancellationPublishesExpectedEvent()
            {
                sut.StatelessRunAsyncCancellation(statelessService, slowCancellationTime);

                Assert.NotNull(actual);
                Assert.Equal(7, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatelessRunAsyncCancellation", actual.EventName);
                AssertPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statelessService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statelessService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statelessService.PartitionId.ToString(), actual);
                AssertPayload(5, "instanceId", statelessService.InstanceId, actual);
                AssertPayload(6, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds, actual);
            }

            [Fact]
            public void StatelessRunAsyncCompletionPublishesExpectedEvent()
            {
                sut.StatelessRunAsyncCompletion(statelessService, wasCanceled);

                Assert.NotNull(actual);
                Assert.Equal(8, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatelessRunAsyncCompletion", actual.EventName);
                AssertPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statelessService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statelessService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statelessService.PartitionId.ToString(), actual);
                AssertPayload(5, "instanceId", statelessService.InstanceId, actual);
                AssertPayload(6, "wasCanceled", wasCanceled, actual);
            }

            [Fact]
            public void StatelessRunAsyncSlowCancellationPublishesExpectedEvent()
            {
                sut.StatelessRunAsyncSlowCancellation(statelessService, actualCancellationTime, slowCancellationTime);

                Assert.NotNull(actual);
                Assert.Equal(9, actual.EventId);
                Assert.Equal(EventLevel.Warning, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatelessRunAsyncSlowCancellation", actual.EventName);
                AssertPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statelessService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statelessService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statelessService.PartitionId.ToString(), actual);
                AssertPayload(5, "instanceId", statelessService.InstanceId, actual);
                AssertPayload(6, "actualCancellationTimeMillis", actualCancellationTime.TotalMilliseconds, actual);
                AssertPayload(7, "slowCancellationTimeMillis", slowCancellationTime.TotalMilliseconds, actual);
            }

            [Fact]
            public void StatelessRunAsyncFailurePublishesExpectedEvent()
            {
                sut.StatelessRunAsyncFailure(statelessService, wasCanceled, exception);

                Assert.NotNull(actual);
                Assert.Equal(10, actual.EventId);
                Assert.Equal(EventLevel.Error, actual.Level);
                Assert.Equal(AllSessions, actual.Keywords);
                Assert.Equal("StatelessRunAsyncFailure", actual.EventName);
                AssertPayload(0, "applicationTypeName", statelessService.CodePackageActivationContext.ApplicationTypeName, actual);
                AssertPayload(1, "applicationName", statelessService.CodePackageActivationContext.ApplicationName, actual);
                AssertPayload(2, "serviceTypeName", statelessService.ServiceTypeName, actual);
                AssertPayload(3, "serviceName", statelessService.ServiceName.OriginalString, actual);
                AssertPayload(4, "partitionId", statelessService.PartitionId.ToString(), actual);
                AssertPayload(5, "instanceId", statelessService.InstanceId, actual);
                AssertPayload(6, "wasCanceled", wasCanceled, actual);
                AssertPayload(7, "exception", exception.ToString(), actual);
            }
        }

        public sealed class Guid : ServiceFrameworkEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("13c2a97d-71da-5ab5-47cb-1497aec602e1"), sut.Guid);
            }
        }

        public sealed class Manifest : ServiceFrameworkEventSourceTest
        {
            readonly ITestOutputHelper output;

            public Manifest(ITestOutputHelper output) => this.output = output;

            [Fact]
            public void CanBeSavedForRegistrationWithExternalTools()
            {
                using var sut = new ServiceFrameworkEventSource();

                string manifest = EventSource.GenerateManifest(sut.GetType(), sut.GetType().Assembly.Location);

                string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(sut.GetType().Assembly.Location), sut.Name), "man");
                File.WriteAllText(manifestFile, manifest);
                output.WriteLine("To register generated manifest for ETL tools, run");
                output.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
            }
        }
    }
}
