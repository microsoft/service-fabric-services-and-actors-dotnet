// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.IO;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric
{
    /// <summary>
    /// Reusable test fixture for testing descendants of the <see cref="ServiceFabricEventSource"/>.
    /// </summary>
    /// <remarks>
    /// <para>This class uses terse names that assume that its instances will be stored in a variable called <c>test</c>.</para>
    /// <para>Disposing instances of this class at the end of each test is required for events to work in subsequent tests.</para>
    /// </remarks>
    sealed class EventSourceTest<TEventSource> : EventSourceFixture where TEventSource : ServiceFabricEventSource
    {
        /// <summary>
        /// Returns the instance under test.
        /// </summary>
        internal TEventSource Instance { get; }

        /// <summary>
        /// Returns the last event written by the <see cref="Instance"/>.
        /// </summary>
        internal EventWrittenEventArgs Event { get; private set; }

        internal EventSourceTest()
        {
            // Dispose existing singleton instance to allow the test instance emit events
            singleton.Value.Dispose();

            Instance = Type<TEventSource>.New();

            listener.EventWritten += (object sender, EventWrittenEventArgs args) => Event = args;
        }

        /// <summary>
        /// Must be called at the end of each test for events to work in subsequent tests.
        /// </summary>
        public override void Dispose()
        {
            listener.Dispose();

            Instance.Dispose();

            singleton.Set(Type<TEventSource>.New());

            base.Dispose();
        }

        /// <summary>
        /// Enables events of given <see cref="EventLevel"/> and <see cref="EventKeywords"/> for the
        /// event source <see cref="Instance"/>. Once events are enabled, the <see cref="Event"/> property
        /// will capture the last event emitted by the event source.
        /// </summary>
        internal void EnableEvents(EventLevel level, EventKeywords keywords = default)
        {
            listener.EnableEvents(Instance, level, keywords);
            EnableEventsInServiceFabricConfiguration(level, keywords);
        }

        /// <summary>
        /// Verifies that a function checking whether an event is enabled returns expected result when events
        /// are enabled with the the given <see cref="EventLevel"/> and <see cref="EventKeywords"/>.
        /// </summary>
        internal void EventEnabled(bool expected, EventLevel level, EventKeywords keywords, Func<bool> isEventEnabled)
        {
            EnableEvents(level, keywords);
            Assert.Equal(expected, isEventEnabled());
        }

        /// <summary>
        /// Verifies that the last <see cref="Event"/> contains expected keywords.
        /// </summary>
        internal void EventKeywords(EventKeywords expected) =>
            Assert.Equal(AllSessions | expected, Event.Keywords);

        /// <summary>
        /// Verifies that the last <see cref="Event"/> contains expected payload.
        /// </summary>
        internal void EventPayload<TPayload>(int index, string name, TPayload value)
        {
            Assert.Equal(name, Event.PayloadNames[index]);
            Assert.Equal(value, Event.Payload[index]);
        }

        /// <summary>
        /// Saves generated event manifest to a file for review and use with ETL tools.
        /// </summary>
        internal void Manifest()
        {
            Type type = typeof(TEventSource);
            string manifest = EventSource.GenerateManifest(type, type.Assembly.Location);
            string manifestFile = Path.ChangeExtension(Path.Combine(Path.GetDirectoryName(type.Assembly.Location), Instance.Name), "man");
            File.WriteAllText(manifestFile, manifest);
            Console.WriteLine("To register generated manifest for ETL tools, run");
            Console.WriteLine($"sudo wevtutil install-manifest {manifestFile}");
        }

        internal ITextEventSourceTest ITextEventSource =>
            new ITextEventSourceTest(this);

        /// <summary>
        /// Provides test methods for <typeparamref name="TEventSource"/> implementation of the <see cref="ITextEventSource"/> interface.
        /// </summary>
        internal sealed class ITextEventSourceTest
        {
            readonly EventSourceTest<TEventSource> test;

            // Method parameters
            readonly string id = fuzzy.String();
            readonly string type = fuzzy.String();
            readonly string message = fuzzy.String();

            static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);
            const EventKeywords Default = (EventKeywords)0x0001;

            internal ITextEventSourceTest(EventSourceTest<TEventSource> test) =>
                this.test = test ?? throw new ArgumentNullException(nameof(test));

            /// <summary>
            /// Tests the <typeparamref name="TEventSource"/> implementation of the <see cref="ITextEventSource.ErrorText"/> method.
            /// </summary>
            internal void ErrorText() =>
                VerifyTextEvent(_ => _.ErrorText, ServiceFabricEventSource.ErrorTextEventId, EventLevel.Error);


            /// <summary>
            /// Tests the <typeparamref name="TEventSource"/> implementation of the <see cref="ITextEventSource.InfoText"/> method.
            /// </summary>
            internal void InfoText() =>
                VerifyTextEvent(_ => _.InfoText, ServiceFabricEventSource.InfoTextEventId, EventLevel.Informational);

            /// <summary>
            /// Tests the <typeparamref name="TEventSource"/> implementation of the <see cref="ITextEventSource.NoiseText"/> method.
            /// </summary>
            internal void NoiseText() =>
                VerifyTextEvent(_ => _.NoiseText, ServiceFabricEventSource.NoiseTextEventId, EventLevel.Verbose);

            /// <summary>
            /// Tests the <typeparamref name="TEventSource"/> implementation of the <see cref="ITextEventSource.WarningText"/> method.
            /// </summary>
            internal void WarningText() =>
                VerifyTextEvent(_ => _.WarningText, ServiceFabricEventSource.WarningTextEventId, EventLevel.Warning);

            void VerifyTextEvent(Func<ITextEventSource, Action<string, string, string>> getEventMethod, int eventId, EventLevel level)
            {
                test.EnableEvents(EventLevel.LogAlways);

                var instance = Assert.IsAssignableFrom<ITextEventSource>(test.Instance);
                Action<string, string, string> act = getEventMethod(instance);
                act(id, type, message);

                Assert.NotNull(test.Event);
                Assert.Equal(eventId, test.Event.EventId);
                Assert.Equal(level, test.Event.Level);
                Assert.Equal(AllSessions | Default, test.Event.Keywords);
                Assert.Equal(act.Method.Name, test.Event.EventName);
                Assert.Equal(ServiceFabricEventSource.TextEventFormat, test.Event.Message);
                test.EventPayload(0, "id", id);
                test.EventPayload(1, "type", type);
                test.EventPayload(2, "message", message);
            }
        }

        #region Implementation

        const EventKeywords AllSessions = (EventKeywords)(0xFul << 44);

        // The EventSource class is expected to have a static, get-only property returning the singleton instance.
        readonly Property<TEventSource> singleton = typeof(TEventSource).Property<TEventSource>();

        readonly EventListener listener = new Mock<EventListener>() { CallBase = true }.Object;

        void EnableEventsInServiceFabricConfiguration(EventLevel level, EventKeywords keywords)
        {
            var metadata = Instance.Field<ReadOnlyDictionary<int, TraceEvent>>().Value;
            foreach (KeyValuePair<int, TraceEvent> item in metadata)
            {
                TraceEvent @event = item.Value;

                bool enable = true;
                enable &= level == EventLevel.LogAlways || level > @event.Level;
                enable &= keywords == System.Diagnostics.Tracing.EventKeywords.None || keywords == (@event.Keywords & keywords);

                @event.Field<bool>().Set(enable);
            }
        }

        #endregion
    }
}
