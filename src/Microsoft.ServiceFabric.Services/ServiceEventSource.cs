// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System.Diagnostics.Tracing;
    using System.Globalization;
    using Microsoft.ServiceFabric.Diagnostics.Tracing;

    /// <summary>
    /// Reliable Services event source collected by Service Fabric runtime diagnostics system.
    /// </summary>
    [EventSource(Guid = "27b7a543-7280-5c2a-b053-f2f798e2cbb7", Name = "ServiceFramework")]
    internal sealed class ServiceEventSource : ServiceFabricEventSource
    {
        /// <summary>
        /// Gets instance of <see cref="ServiceEventSource"/> class.
        /// </summary>
        internal static readonly ServiceEventSource Instance = new ServiceEventSource();

        private const int ServiceLifecycleEventId = 1;
        private const int CommunicationListenerUsageEventId = 2;
        private const int CustomCommunicationClientUsageEventId = 3;
        private const int ServiceRemotingUsageEventId = 4;

        private const string ServiceLifecycleEventTraceFormat = "{1} : clusterOsType = {2}, " +
            "runtimePlatform = {3}, partitionId = {4}, replicaOrInstanceId = {5}, " +
            "serviceName = {6}, serviceTypeName = {7}, applicationName = {8}, " +
            "applicationTypeName = {9}, lifecycleEvent = {10}, serviceKind = {11}";

        private const string CommunicationListenerUsageEventTraceFormat = "{1} : " +
            "clusterOsType = {2}, runtimePlatform = {3}, partitionId = {4}, replicaId = {5}, " +
            "serviceName = {6}, serviceTypeName = {7}, applicationName = {8}, " +
            "applicationTypeName = {9}, communicationListenerType = {10}";

        private const string CustomCommunicationClientUsageEventTraceFormat = "{1} : " +
            "clusterOsType = {2}, runtimePlatform = {3}, serviceUri = {4}, " +
            "customCommunicationClientTypeName = {5}, partitionKey = {6}";

        private const string ServiceRemotingUsageEventTraceFormat = "{1} : clusterOsType = {2}, " +
            "runtimePlatform = {3}, partitionId = {4}, replicaId = {5}, serviceName = {6}, " +
            "serviceTypeName = {7}, applicationName = {8}, applicationTypeName = {9}, " +
            "isSecure = {10}, remotingVersion = {11}, communicationListenerType = {12}";

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceEventSource" /> class from being created.
        /// </summary>
        private ServiceEventSource()
        {
        }

        [NonEvent]
        internal void WriteErrorWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Instance.ErrorText(id, type, format);
            }
            else
            {
                Instance.ErrorText(id, type, string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [NonEvent]
        internal void WriteWarning(string type, string format, params object[] args)
        {
            this.WriteWarningWithId(type, string.Empty, format, args);
        }

        [NonEvent]
        internal void WriteWarningWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Instance.WarningText(id, type, format);
            }
            else
            {
                Instance.WarningText(id, type, string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [NonEvent]
        internal void WriteInfo(string type, string format, params object[] args)
        {
            this.WriteInfoWithId(type, string.Empty, format, args);
        }

        [NonEvent]
        internal void WriteInfoWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Instance.InfoText(id, type, format);
            }
            else
            {
                Instance.InfoText(id, type, string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [NonEvent]
        internal void WriteNoise(string type, string format, params object[] args)
        {
            this.WriteNoiseWithId(type, string.Empty, format, args);
        }

        [NonEvent]
        internal void WriteNoiseWithId(string type, string id, string format, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Instance.NoiseText(id, type, format);
            }
            else
            {
                Instance.NoiseText(id, type, string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [Event(ServiceLifecycleEventId, Message = ServiceLifecycleEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        internal void ServiceLifecycleEvent(
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
        internal void CommunicationListenerUsageEvent(
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

        [Event(CustomCommunicationClientUsageEventId, Message = CustomCommunicationClientUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        internal void CustomCommunicationClientUsageEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string serviceUri,
            string customCommunicationClientTypeName,
            string partitionKey)
        {
            this.WriteEvent(
                CustomCommunicationClientUsageEventId,
                type,
                clusterOsType,
                runtimePlatform,
                serviceUri,
                customCommunicationClientTypeName,
                partitionKey);
        }

        [Event(ServiceRemotingUsageEventId, Message = ServiceRemotingUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        internal void ServiceRemotingUsageEvent(
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

        [Event(5, Message = "{2}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void InfoText(string id, string type, string message)
        {
            this.WriteEvent(5, id, type, message);
        }

        [Event(6, Message = "{2}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        private void WarningText(string id, string type, string message)
        {
            this.WriteEvent(6, id, type, message);
        }

        [Event(7, Message = "{2}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        private void ErrorText(string id, string type, string message)
        {
            this.WriteEvent(7, id, type, message);
        }

        [Event(8, Message = "{2}", Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        private void NoiseText(string id, string type, string message)
        {
            this.WriteEvent(8, id, type, message);
        }

        public static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }
    }
}
