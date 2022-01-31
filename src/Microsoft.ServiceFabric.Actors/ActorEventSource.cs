// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
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
        private const int KVSToRCMigrationStartEventId = 8;
        private const int KVSToRCMigrationCopyPhaseEventId = 9;
        private const int KVSToRCMigrationCatchupPhaseEventId = 10;
        private const int KVSToRCMigrationDowntimePhaseEventId = 11;
        private const int KVSToRCMigrationDataValidationSuccessEventId = 12;
        private const int KVSToRCMigrationDataValidationFailureEventId = 13;
        private const int KVSToRCMigrationSuccessEventId = 14;
        private const int KVSToRCMigrationFailureEventId = 15;
        private const int KVSToRCMigrationResumeWritesEventId = 16;

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

        #region KVS to RC Migration event formats

        private const string KVSToRCMigrationStartEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, starttimeUTC = {10}, noOfSNtoMigrate = {11}, copyPhaseParallelism = {12}, downtimeThreshold = {13}";

        private const string KVSToRCMigrationCopyPhaseEndEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, noOfKeysMigrated = {11}";

        private const string KVSToRCMigrationCatchupPhaseEndEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, noOfIterations = {11}, noOfKeysMigrated = {12}";

        private const string KVSToRCMigrationDowntimePhaseEndEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, noOfKeysMigrated = {11}";

        private const string KVSToRCMigrationDataValidationWithSuccessEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, noOfKeysMigrated{11}, noOfKeysValidated = {12}";

        private const string KVSToRCMigrationDataValidationWithFailureEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, isUserTestHook = {11}, errorMessage = {12}";

        private const string KVSToRCMigrationCompletedWithSuccessEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, noOfKeysMigrated = {11}";

        private const string KVSToRCMigrationCompletedWithFailureEventTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, " +
            "kvsServiceName = {9}, timeSpent = {10}, errorMessage = {13}";

        private const string KVSToRCMigrationResumeWritesTraceFormat = "{0} : clusterOsType = {1}, " +
            "runtimePlatform = {2}, partitionId = {3}, replicaId = {4}, serviceName = {5}, " +
            "serviceTypeName = {6}, applicationName = {7}, applicationTypeName = {8}, kvsServiceName = {9}";

        #endregion KVS to RC Migration event formats

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

        #region KVS to RC migration non events

        [NonEvent]
        internal void KVSToRCMigrationStartEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            DateTime startTimeUtc,
            long noOfSNtoMigrate,
            int copyPhaseParallelism,
            long downtimeThreshold)
        {
            Instance.KVSToRCMigrationStartEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                startTimeUtc,
                noOfSNtoMigrate,
                copyPhaseParallelism,
                downtimeThreshold);
        }

        [NonEvent]
        internal void KVSToRCMigrationCopyPhaseEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            Instance.KVSToRCMigrationCopyPhaseEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                noOfKeysMigrated);
        }

        [NonEvent]
        internal void KVSToRCMigrationCatchupPhaseEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            int noOfIterations,
            long noOfKeysMigrated)
        {
            Instance.KVSToRCMigrationCatchupPhaseEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                noOfIterations,
                noOfKeysMigrated);
        }

        [NonEvent]
        internal void KVSToRCMigrationDowntimePhaseEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            Instance.KVSToRCMigrationDowntimePhaseEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                noOfKeysMigrated);
        }

        [NonEvent]
        internal void KVSToRCMigrationDataValidationWithSuccessEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated,
            long noOfKeysValidated)
        {
            Instance.KVSToRCMigrationDataValidationWithSuccessEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                noOfKeysMigrated,
                noOfKeysValidated);
        }

        [NonEvent]
        internal void KVSToRCMigrationDataValidationWithFailureEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            bool isUserTestHook,
            string errorMessage)
        {
            Instance.KVSToRCMigrationDataValidationWithFailureEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                isUserTestHook,
                errorMessage);
        }

        [NonEvent]
        internal void KVSToRCMigrationCompletedWithSuccessEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            Instance.KVSToRCMigrationCompletedWithSuccessEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                noOfKeysMigrated);
        }

        [NonEvent]
        internal void KVSToRCMigrationCompletedWithFailureEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            string errorMessage)
        {
            Instance.KVSToRCMigrationCompletedWithFailureEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString(),
                timeSpent,
                errorMessage);
        }

        [NonEvent]
        internal void KVSToRCMigrationResumeWritesEvent(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName)
        {
            Instance.KVSToRCMigrationResumeWritesEventInternal(
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName.GetHashCode().ToString(),
                serviceTypeName.GetHashCode().ToString(),
                applicationName.GetHashCode().ToString(),
                applicationTypeName.GetHashCode().ToString(),
                kvsServiceName.GetHashCode().ToString());
        }

        #endregion KVS to RC migration non events

        #endregion

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

        #region KVS to RC Migration events

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationStartEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationStartEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            DateTime startTimeUtc,
            long noOfSNtoMigrate,
            int copyPhaseParallelism,
            long downtimeThreshold)
        {
            this.WriteEvent(
                KVSToRCMigrationStartEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                startTimeUtc,
                noOfSNtoMigrate,
                copyPhaseParallelism,
                downtimeThreshold);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationCopyPhaseEndEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationCopyPhaseEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            this.WriteEvent(
                KVSToRCMigrationCopyPhaseEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationCatchupPhaseEndEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationCatchupPhaseEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            int noOfIterations,
            long noOfKeysMigrated)
        {
            this.WriteEvent(
                KVSToRCMigrationCatchupPhaseEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfIterations,
                noOfKeysMigrated);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationDowntimePhaseEndEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationDowntimePhaseEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            this.WriteEvent(
                KVSToRCMigrationDowntimePhaseEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationDataValidationWithSuccessEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationDataValidationWithSuccessEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated,
            long noOfKeysValidated)
        {
            this.WriteEvent(
                KVSToRCMigrationDataValidationSuccessEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated,
                noOfKeysValidated);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationDataValidationWithFailureEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationDataValidationWithFailureEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            bool isUserTestHook,
            string errorMessage)
        {
            this.WriteEvent(
                KVSToRCMigrationDataValidationFailureEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                isUserTestHook,
                errorMessage);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationCompletedWithSuccessEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationCompletedWithSuccessEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            this.WriteEvent(
                KVSToRCMigrationSuccessEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationCompletedWithFailureEventTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationCompletedWithFailureEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName,
            TimeSpan timeSpent,
            string errorMessage)
        {
            this.WriteEvent(
                KVSToRCMigrationFailureEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName,
                timeSpent,
                errorMessage);
        }

        [Event(ActorStateProviderUsageEventId, Message = KVSToRCMigrationResumeWritesTraceFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        private void KVSToRCMigrationResumeWritesEventInternal(
            string type,
            string clusterOsType,
            string runtimePlatform,
            string partitionId,
            string replicaId,
            string serviceName,
            string serviceTypeName,
            string applicationName,
            string applicationTypeName,
            string kvsServiceName)
        {
            this.WriteEvent(
                KVSToRCMigrationFailureEventId,
                type,
                clusterOsType,
                runtimePlatform,
                partitionId,
                replicaId,
                serviceName,
                serviceTypeName,
                applicationName,
                applicationTypeName,
                kvsServiceName);
        }

        #endregion KVS to RC Migration events

        #endregion

        #region Keywords / Tasks / Opcodes

        public static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }

        #endregion
    }
}
