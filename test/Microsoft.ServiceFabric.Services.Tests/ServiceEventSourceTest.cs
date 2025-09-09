// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.InteropServices;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.ServiceFabric.Services
{
    public abstract class ServiceEventSourceTest: IDisposable
    {
        readonly ServiceEventSource sut;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected ServiceEventSourceTest()
        {
            // Dispose existing singleton instance to allow new test instance creation  
            ServiceEventSource.Instance.Dispose();

            // Disable Linux detection in sut to allow tests to run without UnstructuredTracePublisher which fails without FabricCommon
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(_ => false);

            sut = Type<ServiceEventSource>.New();
        }

        public virtual void Dispose()
        {
            sut.Dispose();

            // Restore original static state
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(new Func<OSPlatform, bool>(RuntimeInformation.IsOSPlatform));
            typeof(ServiceEventSource).Property<ServiceEventSource>().Set(Type<ServiceEventSource>.New());
        }

        public sealed class Guid : ServiceEventSourceTest
        {
            [Fact]
            public void RemainsUnchangedForBackwardCompatibilityWithCollectionTools()
            {
                Assert.Equal(new System.Guid("27b7a543-7280-5c2a-b053-f2f798e2cbb7"), sut.Guid);
            }
        }

        public sealed class Manifest : ServiceEventSourceTest
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

        public sealed class EventTest : ServiceEventSourceTest
        {
            // Method parameters
            readonly string id = fuzzy.String();
            readonly string type = fuzzy.String();
            readonly string message = fuzzy.String();

            const EventKeywords AllSessions = (EventKeywords)(0xFul << 44);
            readonly EventListener listener = new Mock<EventListener>() { CallBase = true }.Object;
            EventWrittenEventArgs actual;

            public EventTest()
            {
                listener.EventWritten += (object sender, EventWrittenEventArgs args) => actual = args;
                listener.EnableEvents(sut, EventLevel.Informational); // Verbose would also require changing TraceConfig
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
            public void ErrorTextPublishesExpectedEvent()
            {
                sut.ErrorText(id, type, message);

                Assert.NotNull(actual);
                Assert.Equal(3, actual.EventId);
                Assert.Equal(EventLevel.Error, actual.Level);
                Assert.Equal(AllSessions | ServiceEventSource.Keywords.Default, actual.Keywords);
                Assert.Equal("ErrorText", actual.EventName);
                AssertPayload(0, "id", id, actual);
                AssertPayload(1, "type", type, actual);
                AssertPayload(2, "message", message, actual);
            }

            [Fact]
            public void InfoTextPublishesExpectedEvent()
            {
                sut.InfoText(id, type, message);

                Assert.NotNull(actual);
                Assert.Equal(1, actual.EventId);
                Assert.Equal(EventLevel.Informational, actual.Level);
                Assert.Equal(AllSessions | ServiceEventSource.Keywords.Default, actual.Keywords);
                Assert.Equal("InfoText", actual.EventName);
                AssertPayload(0, "id", id, actual);
                AssertPayload(1, "type", type, actual);
                AssertPayload(2, "message", message, actual);
            }

            [Fact]
            public void WarningTextPublishesExpectedEvent()
            {
                sut.WarningText(id, type, message);

                Assert.NotNull(actual);
                Assert.Equal(2, actual.EventId);
                Assert.Equal(EventLevel.Warning, actual.Level);
                Assert.Equal(AllSessions | ServiceEventSource.Keywords.Default, actual.Keywords);
                Assert.Equal("WarningText", actual.EventName);
                AssertPayload(0, "id", id, actual);
                AssertPayload(1, "type", type, actual);
                AssertPayload(2, "message", message, actual);
            }
        }
    }
}
