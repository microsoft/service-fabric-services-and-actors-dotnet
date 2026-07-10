// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// Actor Framework event source collected by Service Fabric runtime diagnostics system.
    /// </summary>
    [EventSource(Guid = "e2f2656b-985e-5c5b-5ba3-bbe8a851e1d7", Name = "ActorFramework")]
    sealed class ActorEventSource : ServiceFabricEventSource, ITextEventSource
    {
        /// <summary>
        /// Gets instance of <see cref="ActorEventSource"/> class.
        /// </summary>
        internal static ActorEventSource Instance { get; private set; } = new ActorEventSource();

        private const int ActorStateProviderUsageEventId = 5;
        private const int CustomActorServiceUsageEventId = 6;
        private const int ActorReminderRegistrationEventId = 7;

        internal const string ActorStateProviderUsageEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "stateProviderName = {9}";

        internal const string CustomActorServiceUsageEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, actorType = {3}, actorServiceType = {4}";

        internal const string ActorReminderRegistrationEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "ownerActorId = {9}, reminderPeriod = {10}, reminderName = {11}";

        /// <summary>
        /// Prevents a default instance of the <see cref="ActorEventSource" /> class from being created.
        /// </summary>
        private ActorEventSource()
        {
        }

        #region NonEvents

        [NonEvent]
        internal void ActorStateProviderUsageEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string stateProviderName)
        {
            ActorStateProviderUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                stateProviderName);
        }

        [NonEvent]
        internal void CustomActorServiceUsageEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string actorType,
            string actorServiceType)
        {
            CustomActorServiceUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                actorType,
                actorServiceType);
        }

        [NonEvent]
        internal void ActorReminderRegistrationEventWrapper(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string ownerActorId,
            string reminderPeriod,
            string reminderName)
        {
            ActorReminderRegistrationEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                ownerActorId,
                reminderPeriod,
                reminderName);
        }

        #endregion

        #region Events
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

        [Event(ActorStateProviderUsageEventId, Message = ActorStateProviderUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ActorStateProviderUsageEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string stateProviderName)
        {
            this.WriteEvent(
                ActorStateProviderUsageEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                stateProviderName);
        }

        [Event(CustomActorServiceUsageEventId, Message = CustomActorServiceUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void CustomActorServiceUsageEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string actorType,
            string actorServiceType)
        {
            this.WriteEvent(
                CustomActorServiceUsageEventId,
                type,
                clusterOsType,
                runtimePlatform,
                actorType,
                actorServiceType);
        }

        [Event(ActorReminderRegistrationEventId, Message = ActorReminderRegistrationEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ActorReminderRegistrationEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string ownerActorId,
            string reminderPeriod,
            string reminderName)
        {
            this.WriteEvent(
                ActorReminderRegistrationEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                ownerActorId,
                reminderPeriod,
                reminderName);
        }

        #endregion

        #region Keywords / Tasks / Opcodes

        public static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }

        #endregion
    }
}
