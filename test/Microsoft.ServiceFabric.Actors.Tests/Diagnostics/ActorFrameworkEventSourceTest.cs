using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.IO;
using FluentAssertions.Primitives;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Actors.Tests;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public abstract class ActorFrameworkEventSourceTest: IDisposable
    {
        readonly ActorFrameworkEventSource sut = new ActorFrameworkEventSource();

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz();

        public ActorFrameworkEventSourceTest()
        {
            // Allow event enablement to work on instances created by the tests
            ActorFrameworkEventSource.Writer.Dispose();
        }

        public void Dispose()
        {
            sut.Dispose();

            // Restore the singleton instance
            typeof(ActorFrameworkEventSource).Property<ActorFrameworkEventSource>().Set(new ActorFrameworkEventSource());
        }

        class TestEventListener : EventListener { }

        public sealed class ActorMethodStart : ActorFrameworkEventSourceTest
        {
            // Method parameters
            readonly string methodName = fuzzy.String();
            readonly string methodSignature = fuzzy.String();
            readonly string actorType = fuzzy.String();
            readonly ActorId actorId = fuzzy.ActorId();
            readonly ServiceContext serviceContext = fuzzy.ServiceContext();

            [Fact]
            public void PublishesExpectedEvent()
            {
                EventWrittenEventArgs actual = null;
                using var listener = new TestEventListener();
                listener.EventWritten += (object sender, EventWrittenEventArgs args) => actual = args;
                listener.EnableEvents(sut, EventLevel.LogAlways);

                sut.ActorMethodStart(methodName, methodSignature, actorType, actorId, serviceContext);

                Assert.NotNull(actual);
                Assert.Equal(7, actual.EventId);
                Assert.Equal("ActorMethodStart", actual.EventName);
                Assert.Equal("methodName", actual.PayloadNames[0]);
                Assert.Equal(methodName, actual.Payload[0]);
                Assert.Equal("methodSignature", actual.PayloadNames[1]);
                Assert.Equal(methodSignature, actual.Payload[1]);
                Assert.Equal("actorType", actual.PayloadNames[2]);
                Assert.Equal(actorType, actual.Payload[2]);
                Assert.Equal("actorId", actual.PayloadNames[3]);
                Assert.Equal(actorId.ToString(), actual.Payload[3]);
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
