// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Services
{
    /// <summary>
    /// Reliable Services event source collected by Service Fabric runtime diagnostics system.
    /// </summary>
    [EventSource(Guid = "27b7a543-7280-5c2a-b053-f2f798e2cbb7", Name = "ServiceFramework")]
    sealed class ServiceEventSource : ServiceFabricEventSource, ITextEventSource
    {
        /// <summary>
        /// Gets instance of <see cref="ServiceEventSource"/> class.
        /// </summary>
        internal static ServiceEventSource Instance { get; private set; } = new ServiceEventSource();

        private const int ServiceLifecycleEventId = 5;
        private const int CommunicationListenerUsageEventId = 6;
        private const int ServiceRemotingUsageEventId = 7;

        internal const string ServiceLifecycleEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaOrInstanceId = {4}, " +
            "serviceName = {5}, serviceTypeName = {6}, applicationName = {7}, " +
            "applicationTypeName = {8}, lifecycleEvent = {9}, serviceKind = {10}";

        internal const string CommunicationListenerUsageEventTraceFormat = "{0} : " +
            "clusterOsType = {1}, runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, " +
            "serviceName = {5}, serviceTypeName = {6}, applicationName = {7}, " +
            "applicationTypeName = {8}, communicationListenerType = {9}";

        internal const string ServiceRemotingUsageEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "isSecure = {9}, remotingVersion = {10}, communicationListenerType = {11}";

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceEventSource" /> class from being created.
        /// </summary>
        private ServiceEventSource()
        {
        }

        [NonEvent]
        internal void ServiceLifecycleEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaOrInstanceId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string lifecycleEvent,
            string serviceKind)
        {
            ServiceLifecycleEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaOrInstanceId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                lifecycleEvent,
                serviceKind);
        }

        [NonEvent]
        internal void CommunicationListenerUsageEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string communicationListenerType)
        {
            CommunicationListenerUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                communicationListenerType);
        }

        [NonEvent]
        internal void ServiceRemotingUsageEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            bool isSecure,
            string remotingVersion,
            string communicationListenerType)
        {
            ServiceRemotingUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                isSecure,
                remotingVersion,
                communicationListenerType);
        }

        [Event(InfoTextEventId, Message = TextEventFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void InfoText(string id, string type, string message) =>
            WriteEvent(InfoTextEventId, id, type, message);

        [Event(WarningTextEventId, Message = TextEventFormat, Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void WarningText(string id, string type, string message) =>
            WriteEvent(WarningTextEventId, id, type, message);

        [Event(ErrorTextEventId, Message = TextEventFormat, Level = EventLevel.Error, Keywords = Keywords.Default)]
        public void ErrorText(string id, string type, string message) =>
            WriteEvent(ErrorTextEventId, id, type, message);

        [Event(NoiseTextEventId, Message = TextEventFormat, Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        public void NoiseText(string id, string type, string message) =>
            WriteEvent(NoiseTextEventId, id, type, message);

        [Event(ServiceLifecycleEventId, Message = ServiceLifecycleEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ServiceLifecycleEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaOrInstanceId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string lifecycleEvent,
            string serviceKind)
        {
            this.WriteEvent(
                ServiceLifecycleEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaOrInstanceId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                lifecycleEvent,
                serviceKind);
        }

        [Event(CommunicationListenerUsageEventId, Message = CommunicationListenerUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void CommunicationListenerUsageEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string communicationListenerType)
        {
            this.WriteEvent(
                CommunicationListenerUsageEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                communicationListenerType);
        }

        [Event(ServiceRemotingUsageEventId, Message = ServiceRemotingUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ServiceRemotingUsageEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            bool isSecure,
            string remotingVersion,
            string communicationListenerType)
        {
            this.WriteEvent(
                ServiceRemotingUsageEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                isSecure,
                remotingVersion,
                communicationListenerType);
        }

        public static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }
    }
}
