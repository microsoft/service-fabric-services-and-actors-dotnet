// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Fabric;
    using System.Runtime.InteropServices;
    using Microsoft.ServiceFabric.Services;

    // REMARKS:
    // When you apply EventAttribute attribute to an ETW event method defined on an EventSource-derived class,
    // you must call the WriteEvent method on the base class, passing the event ID, followed by the same
    // arguments as the defined method is passed. Details at:
    // https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventattribute(v=vs.110).aspx
    [EventSource(Name = "Microsoft-ServiceFabric-Actors", LocalizationResources = "Microsoft.ServiceFabric.Actors.SR", Guid = "0e1ec353-9f02-55d7-fbb8-f3857458acbd")]
    internal sealed class ActorFrameworkEventSource : EventSource
    {
#if !NETFRAMEWORK // Remove #if once on net472+ where IsOSPlatform is available
        private static Func<OSPlatform, bool> isOSPlatform = RuntimeInformation.IsOSPlatform;

        public ActorFrameworkEventSource()
        {
            if (isOSPlatform(OSPlatform.Linux))
            {
                var publisher = new UnstructuredTracePublisher();
                publisher.EnableEvents(this, EventLevel.LogAlways);
            }
        }
#endif

        internal static ActorFrameworkEventSource Writer { get; private set; } = new ActorFrameworkEventSource();

        [NonEvent]
        internal void ReplicaChangeRoleToPrimary(ServiceContext serviceContext)
        {
            this.ReplicaChangeRoleToPrimary(
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal void ReplicaChangeRoleFromPrimary(ServiceContext serviceContext)
        {
            this.ReplicaChangeRoleFromPrimary(
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal void ServiceInstanceOpen(ServiceContext serviceContext)
        {
            this.ServiceInstanceOpen(
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal void ServiceInstanceClose(ServiceContext serviceContext)
        {
            this.ServiceInstanceClose(
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal void ActorActivated(
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorActivated(
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal void ActorDeactivated(
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorDeactivated(
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal bool IsActorMethodStartEventEnabled()
        {
            return this.IsEnabled(EventLevel.Verbose, Keywords.ActorMethod);
        }

        [NonEvent]
        internal void ActorMethodStart(
            string methodName,
            string methodSignature,
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorMethodStart(
                methodName,
                methodSignature,
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal bool IsActorMethodStopEventEnabled()
        {
            return this.IsEnabled(EventLevel.Verbose, Keywords.ActorMethod);
        }

        [NonEvent]
        internal void ActorMethodStop(
           long methodExecutionTimeTicks,
           string methodName,
           string methodSignature,
           string actorType,
           ActorId actorId,
           ServiceContext serviceContext)
        {
            this.ActorMethodStop(
                methodExecutionTimeTicks,
                methodName,
                methodSignature,
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal void ActorMethodThrewException(
            string exception,
            long methodExecutionTimeTicks,
            string methodName,
            string methodSignature,
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorMethodThrewException(
                exception,
                methodExecutionTimeTicks,
                methodName,
                methodSignature,
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal bool IsActorSaveStateStartEventEnabled()
        {
            return this.IsEnabled(EventLevel.Verbose, Keywords.ActorState);
        }

        [NonEvent]
        internal void ActorSaveStateStart(
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorSaveStateStart(
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal bool IsActorSaveStateStopEventEnabled()
        {
            return this.IsEnabled(EventLevel.Verbose, Keywords.ActorState);
        }

        [NonEvent]
        internal void ActorSaveStateStop(
            long saveStateExecutionTimeTicks,
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorSaveStateStop(
                saveStateExecutionTimeTicks,
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [NonEvent]
        internal bool IsPendingMethodCallsEventEnabled()
        {
            return this.IsEnabled(EventLevel.Verbose, Keywords.MetricActorMethodCallsWaitingForLock);
        }

        [NonEvent]
        internal void ActorMethodCallsWaitingForLock(
            long countOfWaitingMethodCalls,
            string actorType,
            ActorId actorId,
            ServiceContext serviceContext)
        {
            this.ActorMethodCallsWaitingForLock(
                countOfWaitingMethodCalls,
                actorType,
                actorId.ToString(),
                actorId.Kind,
                serviceContext.ReplicaOrInstanceId,
                serviceContext.PartitionId,
                serviceContext.ServiceName.OriginalString,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName);
        }

        [Event(13, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        internal void ActorTypeRegistered(
            string actorType,
            string customeActorServiceType,
            string nodeName)
        {
            this.WriteEvent(
                13,
                actorType,
                customeActorServiceType,
                nodeName);
        }

        [Event(14, Level = EventLevel.Error, Keywords = Keywords.Default)]
        internal void ActorTypeRegistrationFailed(
            string exception,
            string actorType,
            string customeActorServiceType,
            string nodeName)
        {
            this.WriteEvent(
                14,
                exception,
                actorType,
                customeActorServiceType,
                nodeName);
        }

        [Event(1, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ReplicaChangeRoleToPrimary(
            long replicaId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(
                    1,
                    replicaId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(2, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ReplicaChangeRoleFromPrimary(
            long replicaId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(
                    2,
                    replicaId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(3, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ServiceInstanceOpen(
            long instanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(
                    3,
                    instanceId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(4, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ServiceInstanceClose(
            long instanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(
                    4,
                    instanceId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(5, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ActorActivated(
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(
                    5,
                    actorType,
                    actorId,
                    (int)actorIdKind,
                    replicaOrInstanceId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(6, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ActorDeactivated(
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(
                    6,
                    actorType,
                    actorId,
                    (int)actorIdKind,
                    replicaOrInstanceId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(7, Level = EventLevel.Verbose, Keywords = Keywords.ActorMethod)]
        private void ActorMethodStart(
            string methodName,
            string methodSignature,
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            this.WriteActorMethodEvent(
                7,
                null, // not applicable
                long.MaxValue, // not applicable
                methodName,
                methodSignature,
                actorType,
                actorId,
                (int)actorIdKind,
                replicaOrInstanceId,
                partitionId,
                serviceName,
                applicationName,
                serviceTypeName,
                applicationTypeName,
                nodeName);
        }

        [Event(8, Level = EventLevel.Verbose, Keywords = Keywords.ActorMethod)]
        private void ActorMethodStop(
            long methodExecutionTimeTicks,
            string methodName,
            string methodSignature,
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            this.WriteActorMethodEvent(
                8,
                null, // not applicable
                methodExecutionTimeTicks,
                methodName,
                methodSignature,
                actorType,
                actorId,
                (int)actorIdKind,
                replicaOrInstanceId,
                partitionId,
                serviceName,
                applicationName,
                serviceTypeName,
                applicationTypeName,
                nodeName);
        }

        [Event(9, Level = EventLevel.Warning, Keywords = (Keywords.Default | Keywords.ActorMethod))]
        private void ActorMethodThrewException(
            string exception,
            long methodExecutionTimeTicks,
            string methodName,
            string methodSignature,
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            if (this.IsEnabled())
            {
                this.WriteActorMethodEvent(
                    9,
                    exception,
                    methodExecutionTimeTicks,
                    methodName,
                    methodSignature,
                    actorType,
                    actorId,
                    (int)actorIdKind,
                    replicaOrInstanceId,
                    partitionId,
                    serviceName,
                    applicationName,
                    serviceTypeName,
                    applicationTypeName,
                    nodeName);
            }
        }

        [Event(10, Level = EventLevel.Verbose, Keywords = Keywords.ActorState)]
        private void ActorSaveStateStart(
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            this.WriteEvent(
                10,
                actorType,
                actorId,
                (int)actorIdKind,
                replicaOrInstanceId,
                partitionId,
                serviceName,
                applicationName,
                serviceTypeName,
                applicationTypeName,
                nodeName);
        }

        [Event(11, Level = EventLevel.Verbose, Keywords = Keywords.ActorState)]
        private void ActorSaveStateStop(
            long saveStateExecutionTimeTicks,
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            this.WriteEvent(
                11,
                saveStateExecutionTimeTicks,
                actorType,
                actorId,
                (int)actorIdKind,
                replicaOrInstanceId,
                partitionId,
                serviceName,
                applicationName,
                serviceTypeName,
                applicationTypeName,
                nodeName);
        }

        [Event(12, Level = EventLevel.Verbose, Keywords = Keywords.MetricActorMethodCallsWaitingForLock)]
        private void ActorMethodCallsWaitingForLock(
            long countOfWaitingMethodCalls,
            string actorType,
            string actorId,
            ActorIdKind actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            this.WriteEvent(
                12,
                countOfWaitingMethodCalls,
                actorType,
                actorId,
                (int)actorIdKind,
                replicaOrInstanceId,
                partitionId,
                serviceName,
                applicationName,
                serviceTypeName,
                applicationTypeName,
                nodeName);
        }

        /// <summary>
        /// Implement WriteEvent overload to match our reconfiguration events. If this overload is
        /// not implemented, the standard WriteEvent overload method in the base class that accepts
        /// an object array as argument gets used and it is not very efficient.
        /// </summary>
        [NonEvent]
        private unsafe void WriteEvent(
            int eventId,
            long arg0,
            Guid arg1,
            string arg2,
            string arg3,
            string arg4,
            string arg5,
            string arg6)
        {
            const int dataDescCount = 7;
            var dataDesc = stackalloc EventData[dataDescCount];

            this.SetLongData(&arg0, out dataDesc[0]);
            this.SetGuidData(&arg1, out dataDesc[1]);
            fixed (char* str2 = arg2, str3 = arg3, str4 = arg4, str5 = arg5, str6 = arg6)
            {
                this.SetStringData(arg2, str2, out dataDesc[2]);
                this.SetStringData(arg3, str3, out dataDesc[3]);
                this.SetStringData(arg4, str4, out dataDesc[4]);
                this.SetStringData(arg5, str5, out dataDesc[5]);
                this.SetStringData(arg6, str6, out dataDesc[6]);

                this.WriteEventCore(eventId, dataDescCount, dataDesc);
            }
        }

        /// <summary>
        /// Implement WriteEvent overload to match our activation/deactivation events. If this overload is
        /// not implemented, the standard WriteEvent overload method in the base class that accepts an
        /// object array as argument gets used and it is not very efficient.
        /// </summary>
        [NonEvent]
        private unsafe void WriteEvent(
            int eventId,
            string arg0,
            string arg1,
            int arg2,
            long arg3,
            Guid arg4,
            string arg5,
            string arg6,
            string arg7,
            string arg8,
            string arg9)
        {
            const int DataDescCount = 10;
            var dataDesc = stackalloc EventData[DataDescCount];

            this.SetIntData(&arg2, out dataDesc[2]);
            this.SetLongData(&arg3, out dataDesc[3]);
            this.SetGuidData(&arg4, out dataDesc[4]);

            fixed (char* str0 = arg0, str1 = arg1, str5 = arg5, str6 = arg6, str7 = arg7, str8 = arg8, str9 = arg9)
            {
                this.SetStringData(arg0, str0, out dataDesc[0]);
                this.SetStringData(arg1, str1, out dataDesc[1]);
                this.SetStringData(arg5, str5, out dataDesc[5]);
                this.SetStringData(arg6, str6, out dataDesc[6]);
                this.SetStringData(arg7, str7, out dataDesc[7]);
                this.SetStringData(arg8, str8, out dataDesc[8]);
                this.SetStringData(arg9, str9, out dataDesc[9]);

                this.WriteEventCore(eventId, DataDescCount, dataDesc);
            }
        }

        /// <summary>
        /// Implement WriteEvent overload to match our actor metric events. If this overload is not
        /// implemented, the standard WriteEvent overload method in the base class that accepts an
        /// object array as argument gets used and it is not very efficient.
        /// </summary>
        [NonEvent]
        private unsafe void WriteEvent(
            int eventId,
            long arg0,
            string arg1,
            string arg2,
            int arg3,
            long arg4,
            Guid arg5,
            string arg6,
            string arg7,
            string arg8,
            string arg9,
            string arg10)
        {
            const int DataDescCount = 11;
            var dataDesc = stackalloc EventData[DataDescCount];

            this.SetLongData(&arg0, out dataDesc[0]);
            this.SetIntData(&arg3, out dataDesc[3]);
            this.SetLongData(&arg4, out dataDesc[4]);
            this.SetGuidData(&arg5, out dataDesc[5]);

            fixed (char* str1 = arg1, str2 = arg2, str6 = arg6, str7 = arg7, str8 = arg8, str9 = arg9, str10 = arg10)
            {
                this.SetStringData(arg1, str1, out dataDesc[1]);
                this.SetStringData(arg2, str2, out dataDesc[2]);
                this.SetStringData(arg6, str6, out dataDesc[6]);
                this.SetStringData(arg7, str7, out dataDesc[7]);
                this.SetStringData(arg8, str8, out dataDesc[8]);
                this.SetStringData(arg9, str9, out dataDesc[9]);
                this.SetStringData(arg10, str10, out dataDesc[10]);

                this.WriteEventCore(eventId, DataDescCount, dataDesc);
            }
        }

        /// <summary>
        /// Implement our own method to write the event. If we called WriteEvent directly instead of
        /// implementing this method, then the standard WriteEvent overload method in the base class
        /// that accepts an object array as argument gets used and it is not very efficient.
        /// </summary>
        [NonEvent]
        private unsafe void WriteActorMethodEvent(
            int eventId,
            string exception,
            long methodExecutionTimeTicks,
            string methodName,
            string methodSignature,
            string actorType,
            string actorId,
            int actorIdKind,
            long replicaOrInstanceId,
            Guid partitionId,
            string serviceName,
            string applicationName,
            string serviceTypeName,
            string applicationTypeName,
            string nodeName)
        {
            const int minDataDescCount = 12;
            const int maxDataDescCount = 14;
            var dataDescCount = minDataDescCount;

            var dataDesc = stackalloc EventData[maxDataDescCount];

            if (methodExecutionTimeTicks != long.MaxValue)
            {
                this.SetLongData(&methodExecutionTimeTicks, out dataDesc[1]);
                dataDescCount++;
            }

            this.SetIntData(&actorIdKind, out dataDesc[6]);
            this.SetLongData(&replicaOrInstanceId, out dataDesc[7]);
            this.SetGuidData(&partitionId, out dataDesc[8]);

            fixed (char* str0 = exception, str1 = methodName, str2 = methodSignature, str3 = actorType, str4 = actorId, str5 = serviceName, str6 = applicationName, str7 = serviceTypeName, str8 = applicationTypeName, str9 = nodeName)
            {
                if (exception != null)
                {
                    this.SetStringData(exception, str0, out dataDesc[0]);
                    dataDescCount++;
                }

                this.SetStringData(methodName, str1, out dataDesc[2]);
                this.SetStringData(methodSignature, str2, out dataDesc[3]);
                this.SetStringData(actorType, str3, out dataDesc[4]);
                this.SetStringData(actorId, str4, out dataDesc[5]);
                this.SetStringData(serviceName, str5, out dataDesc[9]);
                this.SetStringData(applicationName, str6, out dataDesc[10]);
                this.SetStringData(serviceTypeName, str7, out dataDesc[11]);
                this.SetStringData(applicationTypeName, str8, out dataDesc[12]);
                this.SetStringData(nodeName, str9, out dataDesc[13]);

                var offset = maxDataDescCount - dataDescCount;
                this.WriteEventCore(eventId, dataDescCount, (dataDesc + offset));
            }
        }

        [NonEvent]
        private unsafe void SetStringData(string str, char* fixedStr, out EventData dataDesc)
        {
            dataDesc = new EventData() { DataPointer = (IntPtr)fixedStr, Size = ((str.Length + 1) * 2) };
        }

        [NonEvent]
        private unsafe void SetLongData(long* fixedDataPtr, out EventData dataDesc)
        {
            dataDesc = new EventData() { DataPointer = (IntPtr)fixedDataPtr, Size = sizeof(long) };
        }

        [NonEvent]
        private unsafe void SetIntData(int* fixedDataPtr, out EventData dataDesc)
        {
            dataDesc = new EventData() { DataPointer = (IntPtr)fixedDataPtr, Size = sizeof(int) };
        }

        [NonEvent]
        private unsafe void SetGuidData(Guid* fixedDataPtr, out EventData dataDesc)
        {
            dataDesc = new EventData() { DataPointer = (IntPtr)fixedDataPtr, Size = sizeof(Guid) };
        }

        public class Keywords
        {
            internal const EventKeywords Default = (EventKeywords)0x1;
            internal const EventKeywords ActorMethod = (EventKeywords)0x2;
            internal const EventKeywords ActorState = (EventKeywords)0x4;
            internal const EventKeywords MetricActorMethodCallsWaitingForLock = (EventKeywords)0x8;
        }
    }
}
