#if NET

using Fuzzy;
using Inspector;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    public abstract class UnstructuredTracePublisherTest
    {
        readonly UnstructuredTracePublisher sut = new UnstructuredTracePublisher();

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

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
            // Test fixture
            readonly Mock<WriteUnstructured> writeUnstructured = new Mock<WriteUnstructured>();
            readonly IList<object> payload = fuzzy.List(() => (object)fuzzy.DateTime()); // culture-sensitive
            readonly IList<string> payloadNames;

            public OnEventWritten()
            {
                sut.Field<WriteUnstructured>().Set(writeUnstructured.Object);
                payloadNames = fuzzy.List(fuzzy.String, Count.Exactly(payload.Count));
            }

            [Fact]
            public void ConvertsEventToStringAndWritesItAsUnstructuredTrace()
            {
                var eventSource = new EventSource(fuzzy.String());
                EventWrittenEventArgs eventWritten = CreateEvent(eventSource);

                sut.Declared().Method<Action<EventWrittenEventArgs>>().Invoke(eventWritten);

                string expectedTask = eventSource.Name;
                string expectedName = eventWritten.EventName;
                string expectedId = string.Empty;
                var expectedLevel = (ushort)eventWritten.Level;
                var expectedText = string.Format(CultureInfo.InvariantCulture, eventWritten.Message, eventWritten.Payload.ToArray());
                writeUnstructured.Verify(_ => _.Invoke(expectedTask, expectedName, expectedId, expectedLevel, expectedText));
            }

            [Theory]
            [InlineData(ServiceFabricEventSource.ErrorTextEventId, nameof(ITextEventSource.ErrorText), EventLevel.Error)]
            [InlineData(ServiceFabricEventSource.InfoTextEventId, nameof(ITextEventSource.InfoText), EventLevel.Informational)]
            [InlineData(ServiceFabricEventSource.NoiseTextEventId, nameof(ITextEventSource.NoiseText), EventLevel.Verbose)]
            [InlineData(ServiceFabricEventSource.WarningTextEventId, nameof(ITextEventSource.WarningText), EventLevel.Warning)]
            public void UsesIdAndTypeWhenBothAreIncludedInEventPayload(int eventId, string eventName, EventLevel eventLevel)
            {
                EventSource eventSource = CreateTextEventSource();
                string expectedId = "Id" + fuzzy.String();
                InsertPayload(0, "id", expectedId);
                string expectedName = "Type" + fuzzy.String();
                InsertPayload(1, "type", expectedName);
                EventWrittenEventArgs eventWritten = CreateEvent(eventSource, eventId, eventName, eventLevel);

                sut.Declared().Method<Action<EventWrittenEventArgs>>().Invoke(eventWritten);

                string expectedTask = eventSource.Name;
                var expectedLevel = (ushort)eventWritten.Level;
                var expectedText = string.Format(CultureInfo.InvariantCulture, eventWritten.Message, eventWritten.Payload.ToArray());
                writeUnstructured.Verify(_ => _.Invoke(expectedTask, expectedName, expectedId, expectedLevel, expectedText));
            }

            EventWrittenEventArgs CreateEvent(EventSource eventSource)
            {
                int id = fuzzy.Int32().Maximum(0); // To prevent EventWrittenEventArgs from fetching EventLevel from uninitialized EventMetadata
                string name = "Event" + fuzzy.String();
                var level = fuzzy.Enum<EventLevel>();
                return CreateEvent(eventSource, id, name, level);
            }

            EventWrittenEventArgs CreateEvent(EventSource eventSource, int id, string name, EventLevel level)
            {
                var @event = Type<EventWrittenEventArgs>.New(eventSource, id);
                @event.Property<string>(nameof(EventWrittenEventArgs.EventName)).Set(name);
                @event.Property<EventLevel>().Set(level);
                @event.Property<ReadOnlyCollection<object>>().Set(new ReadOnlyCollection<object>(payload));
                @event.Property<ReadOnlyCollection<string>>().Set(new ReadOnlyCollection<string>(payloadNames));
                @event.Property<string>(nameof(EventWrittenEventArgs.Message)).Set(CreateEventMessage());
                return @event;
            }

            string CreateEventMessage()
            {
                var message = new StringBuilder("Message " + fuzzy.String().LettersOrDigits());
                for (int i = 0; i < payload.Count; i++)
                    message.Append(" {" + i + "}");
                return message.ToString();
            }

            EventSource CreateTextEventSource()
            {
                var mock = new Mock<EventSource>(fuzzy.String());
                mock.As<ITextEventSource>();

                EventSource eventSource = mock.Object;
                InitializeMetadata(eventSource);

                return eventSource;
            }

            void InitializeMetadata(EventSource eventSource)
            {
                // EventSource dynamically builds and caches event metadata from the methods decorated with the EventAttribute.
                // The metadata types are internal, so we "borrow" them from the ServiceFabricStringEventSource.

                const string eventSourceMetadataField = "m_eventData";

                using var stringEventSource = new EventSourceTest<ServiceFabricStringEventSource>();
                stringEventSource.EnableEvents(EventLevel.LogAlways);
                object metadata = stringEventSource.Instance.Field(eventSourceMetadataField).Value;
                Assert.NotNull(metadata);

                eventSource.Field(eventSourceMetadataField).Set(metadata);
            }

            void InsertPayload(int index, string name, object value)
            {
                payloadNames.Insert(index, name);
                payload.Insert(index, value);
            }
        }
    }
}

#endif
