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

        private const int ActorStateProviderUsageEventId = 5;
        private const int CustomActorServiceUsageEventId = 6;
        private const int ActorReminderRegisterationEventId = 7;

        private const string ActorStateProviderUsageEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "stateProviderName = {9}";

        private const string CustomActorServiceUsageEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, actorType = {3}, actorServiceType = {4}";

        private const string ActorReminderRegisterationEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "ownerActorId = {9}, reminderPeriod = {10}, reminderName = {11}";

        #region Migration
        private const string MigrationBaseFormat = "{0} : " +
            "clusterOsType = {1}, " +
            "runtimePlatform = {2}, " +
            "partitionId = {3}, " +
            "replicaId = {4}, " +
            "serviceName = {5}, " +
            "serviceTypeName = {6}, " +
            "applicationName = {7}, " +
            "applicationTypeName = {8}";

        private const int MigrationStartEventId = 8;
        private const string MigrationStartEventFormat = MigrationBaseFormat + ", settings = {9}";

        private const int MigrationEndEventId = 9;
        private const string MigrationEndEventFormat = MigrationBaseFormat + ", result = {9}";

        private const int MigrationPhaseStartEventId = 10;
        private const string MigrationPhaseStartEventFormat = MigrationBaseFormat + ", input = {9}";

        private const int MigrationPhaseEndEventId = 11;
        private const string MigrationPhaseEndEventFormat = MigrationBaseFormat + ", result = {9}";

        private const int MigrationFailureEventId = 12;
        private const string MigrationFailureEventFormat = MigrationBaseFormat + ", phase = {9}, errorMessage = {10}";

        private const int MigrationAbortEventId = 13;
        private const string MigrationAbortEventFormat = MigrationBaseFormat + ", userTriggered = {9}";

        #endregion Migration

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
            Instance.ActorStateProviderUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
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
            Instance.CustomActorServiceUsageEvent(
                type,
                clusterOsType,
                runtimePlatform,
                actorType.GetHashCode().ToString(),
                actorServiceType.GetHashCode().ToString());
        }

        [NonEvent]
        internal void ActorReminderRegisterationEventWrapper(
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
            Instance.ActorReminderRegisterationEvent(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                ownerActorId.GetHashCode().ToString(),
                reminderPeriod,
                reminderName.GetHashCode().ToString());
        }

        #endregion

        #region MigrationEvents
        [Event(
            MigrationStartEventId,
            Message = MigrationStartEventFormat,
            Level = EventLevel.Informational,
            Keywords = Keywords.Default)]
        internal void MigrationStartEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string settingsJson)
        {
            this.WriteEvent(
                MigrationStartEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                settingsJson);
        }

        [Event(
            MigrationEndEventId,
            Message = MigrationEndEventFormat,
            Level = EventLevel.Informational,
            Keywords = Keywords.Default)]
        internal void MigrationEndEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string resultJson)
        {
            this.WriteEvent(
                MigrationEndEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                resultJson);
        }

        [Event(
            MigrationPhaseStartEventId,
            Message = MigrationPhaseStartEventFormat,
            Level = EventLevel.Informational,
            Keywords = Keywords.Default)]
        internal void MigrationPhaseStartEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string inputJson)
        {
            this.WriteEvent(
                MigrationPhaseStartEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                inputJson);
        }

        [Event(
            MigrationPhaseEndEventId,
            Message = MigrationPhaseEndEventFormat,
            Level = EventLevel.Informational,
            Keywords = Keywords.Default)]
        internal void MigrationPhaseEndEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string resultJson)
        {
            this.WriteEvent(
                MigrationPhaseEndEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                resultJson);
        }

        [Event(
            MigrationFailureEventId,
            Message = MigrationFailureEventFormat,
            Level = EventLevel.Error,
            Keywords = Keywords.Default)]
        internal void MigrationFailureEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string phase,
            string errorMsg)
        {
            this.WriteEvent(
                MigrationFailureEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                phase,
                errorMsg);
        }

        [Event(
            MigrationAbortEventId,
            Message = MigrationAbortEventFormat,
            Level = EventLevel.Informational,
            Keywords = Keywords.Default)]
        internal void MigrationAbortEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            bool userTriggered)
        {
            this.WriteEvent(
                MigrationAbortEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                userTriggered);
        }

        #endregion MigrationEvents

        #region Events
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

        [Event(ActorReminderRegisterationEventId, Message = ActorReminderRegisterationEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void ActorReminderRegisterationEvent(
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

        #endregion

        #region Keywords / Tasks / Opcodes

        public static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }

        #endregion
    }
}
