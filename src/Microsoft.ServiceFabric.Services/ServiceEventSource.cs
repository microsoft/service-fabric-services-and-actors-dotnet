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

        private const int ServiceLifecycleEventId = 5;
        private const int CommunicationListenerUsageEventId = 6;
        private const int CustomCommunicationClientUsageEventId = 7;
        private const int ServiceRemotingUsageEventId = 8;

        private const string ServiceLifecycleEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaOrInstanceId = {4}, " +
            "serviceName = {5}, serviceTypeName = {6}, applicationName = {7}, " +
            "applicationTypeName = {8}, lifecycleEvent = {9}, serviceKind = {10}";

        private const string CommunicationListenerUsageEventTraceFormat = "{0} : " +
            "clusterOsType = {1}, runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, " +
            "serviceName = {5}, serviceTypeName = {6}, applicationName = {7}, " +
            "applicationTypeName = {8}, communicationListenerType = {9}";

        private const string CustomCommunicationClientUsageEventTraceFormat = "{0} : " +
            "clusterOsType = {1}, runtimePlatform = {2}, serviceUri = {3}, " +
            "customCommunicationClientTypeName = {4}, partitionKey = {5}";

        private const string ServiceRemotingUsageEventTraceFormat = "{0} : clusterOsType = {1}, " +
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
            this.ServiceLifecycleEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaOrInstanceId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
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
            this.CommunicationListenerUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                communicationListenerType);
        }

        [NonEvent]
        internal void CustomCommunicationClientUsageEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string serviceUri,
            string customCommunicationClientTypeName,
            string partitionKey)
        {
            this.CustomCommunicationClientUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                serviceUri.GetHashCode().ToString(),
                customCommunicationClientTypeName.GetHashCode().ToString(),
                partitionKey);
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
            this.ServiceRemotingUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                isSecure,
                remotingVersion,
                communicationListenerType);
        }

        [Event(1, Message = "{2}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void InfoText(string id, string type, string message)
        {
            this.WriteEvent(1, id, type, message);
        }

        [Event(2, Message = "{2}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        private void WarningText(string id, string type, string message)
        {
            this.WriteEvent(2, id, type, message);
        }

        [Event(3, Message = "{2}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        private void ErrorText(string id, string type, string message)
        {
            this.WriteEvent(3, id, type, message);
        }

        [Event(4, Message = "{2}", Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        private void NoiseText(string id, string type, string message)
        {
            this.WriteEvent(4, id, type, message);
        }

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

        [Event(CustomCommunicationClientUsageEventId, Message = CustomCommunicationClientUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void CustomCommunicationClientUsageEvent(
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
