#if NET

using Fuzzy;
using Inspector;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Diagnostics.Tests
{
    public abstract class UnstructuredTracePublisherTest
    {
        readonly UnstructuredTracePublisher sut = new UnstructuredTracePublisher();

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz();

        public sealed class Class : UnstructuredTracePublisherTest
        {
            [Fact]
            public void InheritsFromEventListenerToInterceptEventSourceEvents()
            {
                Assert.IsAssignableFrom<EventListener>(sut);
            }

            [Fact]
            public void UsesTraceViaNativeWriteUnstructuredMethod()
            {
                var traceViaNative = Type.GetType("System.Fabric.Common.Tracing.TraceViaNative, System.Fabric");
                WriteUnstructured expected = traceViaNative.Method<WriteUnstructured>();
                WriteUnstructured actual = sut.Field<WriteUnstructured>();
                Assert.Equal(expected, actual);
            }
        }

        public class OnEventWritten : UnstructuredTracePublisherTest
        {
            // Method arguments
            readonly EventWrittenEventArgs eventWritten;

            // Test fixture
            readonly EventSource eventSource = new EventSource(fuzzy.String());

            public OnEventWritten()
            {
                int eventId = fuzzy.Int32().Maximum(0); // To prevent EventLevel from being fetched from uninitialized EventMetadata

                eventWritten = Type<EventWrittenEventArgs>.New(eventSource, eventId);

                eventWritten.Property<string>(nameof(EventWrittenEventArgs.EventName)).Set("EventName " + fuzzy.String());

                eventWritten.Property<EventLevel>().Set(fuzzy.Enum<EventLevel>());

                var payload = new ReadOnlyCollection<object>(fuzzy.List(() => (object)fuzzy.DateTime())); // culture-sensitive
                eventWritten.Field<ReadOnlyCollection<object>>().Set(payload);

                var message = new StringBuilder("Message " + fuzzy.String());
                for (int i = 0; i < payload.Count; i++)
                    message.Append(" {" + i + "}");
                eventWritten.Property<string>(nameof(EventWrittenEventArgs.Message)).Set(message.ToString());
            }

            [Fact]
            public void ConvertsEventToStringAndWritesItAsUnstructuredTrace()
            {
                var writeUnstructured = Mock.Of<WriteUnstructured>();
                sut.Field<WriteUnstructured>().Set(writeUnstructured);

                sut.Declared().Method<Action<EventWrittenEventArgs>>().Invoke(eventWritten);

                string expectedTask = eventSource.Name;
                string expectedName = eventWritten.EventName;
                string expectedId = eventWritten.EventId.ToString();
                var expectedLevel = (ushort)eventWritten.Level;
                var expectedText = string.Format(CultureInfo.InvariantCulture, eventWritten.Message, eventWritten.Payload.ToArray());
                Mock.Get(writeUnstructured).Verify(_ => _.Invoke(expectedTask, expectedName, expectedId, expectedLevel, expectedText));
            }
        }
    }
}

#endif
