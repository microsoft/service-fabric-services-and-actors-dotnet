// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Config;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Writer;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    /// <summary>
    /// ServiceFabricEventSource class is a wrapper on top of EventSource with following benefits
    /// 1) It contains stack-allocated WriteEvent overloads which are faster than ones available under EventSource
    /// 2) It has custom filtering for events through Settings.xml
    /// </summary>
    internal abstract class ServiceFabricEventSource : EventSource
    {
        private const string DefaultPackageName = "Config";
        private readonly ReadOnlyDictionary<int, TraceEvent> eventDescriptors;
        private readonly TraceConfig configMgr;
        private readonly string eventSourceName;

#if !NETFRAMEWORK
        protected static Func<OSPlatform, bool> IsOSPlatform = RuntimeInformation.IsOSPlatform;
#endif
        
        /// <summary>
        /// Constructor which populates the events descriptor data for all events defined
        /// </summary>
        protected ServiceFabricEventSource() : this(DefaultPackageName)
        {
            if(IsLinuxPlatform())
            {
                var publisher = new UnstructuredTracePublisher();
                publisher.EnableEvents(this, EventLevel.Informational);
            }
        }

        /// <summary>
        /// Constructor which populates the events descriptor data for all events defined
        /// </summary>
        protected ServiceFabricEventSource(string configPackageName)
        {
            Type eventSourceType = this.GetType();

            EventSourceAttribute eventSourceAttribute = (EventSourceAttribute)eventSourceType.GetCustomAttributes(typeof(EventSourceAttribute), true).SingleOrDefault();

            this.eventSourceName = eventSourceType.Name;
            if (eventSourceAttribute != null && !string.IsNullOrEmpty(eventSourceAttribute.Name))
            {
                this.eventSourceName = eventSourceAttribute.Name;
            }

            this.configMgr = new TraceConfig(this.eventSourceName, configPackageName);
            this.eventDescriptors = this.GenerateEventDescriptors(eventSourceType);
        }

        [NonEvent]
        protected new void WriteEvent(int eventId)
        {
            this.VariantWrite(eventId, 0);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0)
        {
            this.VariantWrite(eventId, 1, param0);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1)
        {
            this.VariantWrite(eventId, 2, param0, param1);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2)
        {
            this.VariantWrite(eventId, 3, param0, param1, param2);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2, Variant param3)
        {
            this.VariantWrite(eventId, 4, param0, param1, param2, param3);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2, Variant param3, Variant param4)
        {
            this.VariantWrite(eventId, 5, param0, param1, param2, param3, param4);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2, Variant param3, Variant param4, Variant param5)
        {
            this.VariantWrite(eventId, 6, param0, param1, param2, param3, param4, param5);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2, Variant param3, Variant param4, Variant param5, Variant param6)
        {
            this.VariantWrite(eventId, 7, param0, param1, param2, param3, param4, param5, param6);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2, Variant param3, Variant param4, Variant param5, Variant param6, Variant param7)
        {
            this.VariantWrite(eventId, 8, param0, param1, param2, param3, param4, param5, param6, param7);
        }

        [NonEvent]
        protected void WriteEvent(int eventId, Variant param0, Variant param1, Variant param2, Variant param3, Variant param4, Variant param5, Variant param6, Variant param7, Variant param8)
        {
            this.VariantWrite(eventId, 9, param0, param1, param2, param3, param4, param5, param6, param7, param8);
        }

        /// <summary>
        /// Generates descriptors for all the events inside a specified class type
        /// </summary>
        /// <param name="eventSourceType">type of the class deriving from ServiceFabricEventSource</param>
        /// <returns>Dictionary of events</returns>
        [NonEvent]
        private ReadOnlyDictionary<int, TraceEvent> GenerateEventDescriptors(Type eventSourceType)
        {
            var eventDescriptors = new Dictionary<int, TraceEvent>();
            MethodInfo[] methods = eventSourceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var eventMethod in methods)
            {
                EventAttribute eventAttribute = (EventAttribute)eventMethod.GetCustomAttributes(typeof(EventAttribute), false).SingleOrDefault();
                if (eventAttribute == null)
                {
                    continue;
                }

                var eventId = eventAttribute.EventId;

                eventDescriptors[eventId] = new TraceEvent(
                    eventMethod.Name,
                    eventAttribute.Level,
                    eventAttribute.Keywords,
                    eventAttribute.Message,
                    this.configMgr);
            }

            return new ReadOnlyDictionary<int, TraceEvent>(eventDescriptors);
        }

        [NonEvent]
        unsafe void VariantWrite(int eventId, int argCount, Variant v0 = default, Variant v1 = default, Variant v2 = default, Variant v3 = default, Variant v4 = default, Variant v5 = default, Variant v6 = default, Variant v7 = default, Variant v8 = default)
        {
            if (this.eventDescriptors[eventId].IsEventSinkEnabled())
            {
                if (argCount == 0)
                {
                    this.WriteEventCore(eventId, argCount, null);
                }
                else
                {
                    EventData* eventSourceData = stackalloc EventData[argCount]; // allocation for the data descriptors
                    byte* dataBuffer = stackalloc byte[EventDataArrayBuilder.BasicTypeAllocationBufferSize * argCount];

                    // 16 byte for non-string argument
                    var edb = new EventDataArrayBuilder((Writer.EventData*)eventSourceData, dataBuffer);

                    // The block below goes through all the arguments and fills in the data
                    // descriptors.
                    edb.AddEventData(v0);
                    if (argCount > 1)
                    {
                        edb.AddEventData(v1);
                        if (argCount > 2)
                        {
                            edb.AddEventData(v2);
                            if (argCount > 3)
                            {
                                edb.AddEventData(v3);
                                if (argCount > 4)
                                {
                                    edb.AddEventData(v4);
                                    if (argCount > 5)
                                    {
                                        edb.AddEventData(v5);
                                        if (argCount > 6)
                                        {
                                            edb.AddEventData(v6);
                                            if (argCount > 7)
                                            {
                                                edb.AddEventData(v7);
                                                if (argCount > 8)
                                                {
                                                    edb.AddEventData(v8);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!edb.Validate(eventId))
                        return;

                    fixed (
                        char* s0 = v0.ConvertToString(),
                        s1 = v1.ConvertToString(),
                        s2 = v2.ConvertToString(),
                        s3 = v3.ConvertToString(),
                        s4 = v4.ConvertToString(),
                        s5 = v5.ConvertToString(),
                        s6 = v6.ConvertToString(),
                        s7 = v7.ConvertToString(),
                        s8 = v8.ConvertToString())
                    {
                        var eventDataPtr = edb.ToEventDataArray(s0, s1, s2, s3, s4, s5, s6, s7, s8);
                        this.WriteEventCore(eventId, argCount, (EventData*)eventDataPtr);
                    }
                }
            }
        }

        private bool IsLinuxPlatform()
        {
#if !NETFRAMEWORK
            return IsOSPlatform(OSPlatform.Linux);
#else
            return false;  
#endif
        }
    }
}
