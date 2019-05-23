// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System.Diagnostics.Tracing;
    using System.Globalization;
    using Microsoft.ServiceFabric.Diagnostics.Tracing;

    /// <summary>
    /// Actor Framework event source collected by Service Fabric runtime diagnostics system.
    /// </summary>
    [EventSource(Guid = "e2f2656b-985e-5c5b-5ba3-bbe8a851e1d7", Name = "ActorFramework")]
    internal sealed class ActorEventSource : ServiceFabricEventSource
    {
        /// <summary>
        /// Gets instance of <see cref="ActorEventSource"/> class.
        /// </summary>
        internal static readonly ActorEventSource Instance = new ActorEventSource();

        private const int ActorStateProviderUsageEventId = 1;
        private const int CustomActorServiceUsageEventId = 2;
        private const int ActorReminderRegisterationEventId = 3;

        private const string ActorStateProviderUsageEventTraceFormat = "{1} : clusterOsType = {2}, " +
            "runtimePlatform = {3}, partitionId = {4}, replicaId = {5}, serviceName = {6}, " +
            "serviceTypeName = {7}, applicationName = {8}, applicationTypeName = {9}, " +
            "stateProviderName = {10}";

        private const string CustomActorServiceUsageEventTraceFormat = "{1} : clusterOsType = {2}, " +
            "runtimePlatform = {3}, actorType = {4}, actorServiceType = {5}";

        private const string ActorReminderRegisterationEventTraceFormat = "{1} : clusterOsType = {2}, " +
            "runtimePlatform = {3}, partitionId = {4}, replicaId = {5}, serviceName = {6}, " +
            "serviceTypeName = {7}, applicationName = {8}, applicationTypeName = {9}, " +
            "ownerActorId = {10}, reminderPeriod = {11}, reminderName = {12}";

        /// <summary>
        /// Prevents a default instance of the <see cref="ActorEventSource" /> class from being created.
        /// </summary>
        private ActorEventSource()
        {
        }

        #region NonEvents

        [NonEvent]
        internal void WriteError(string type, string format, params object[] args)
        {
            this.WriteErrorWithId(type, string.Empty, format, args);
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

        #endregion

        #region Events

        [Event(ActorStateProviderUsageEventId, Message = ActorStateProviderUsageEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        internal void ActorStateProviderUsageEvent(
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
        internal void CustomActorServiceUsageEvent(
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

        [Event(CustomActorServiceUsageEventId, Message = ActorReminderRegisterationEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        internal void ActorReminderRegisterationEvent(
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
                ActorReminderRegisterationEventId,
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

        [Event(4, Message = "{2}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void InfoText(string id, string type, string message)
        {
            this.WriteEvent(4, id, type, message);
        }

        [Event(5, Message = "{2}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        private void WarningText(string id, string type, string message)
        {
            this.WriteEvent(5, id, type, message);
        }

        [Event(6, Message = "{2}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        private void ErrorText(string id, string type, string message)
        {
            this.WriteEvent(6, id, type, message);
        }

        [Event(7, Message = "{2}", Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        private void NoiseText(string id, string type, string message)
        {
            this.WriteEvent(7, id, type, message);
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
