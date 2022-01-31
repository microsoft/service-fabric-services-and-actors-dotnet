// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services;

    /// <summary>
    /// ActorTelemetry contains the telemetry methods for ActorFramework.
    /// </summary>
    internal static class ActorTelemetry
    {
        /// <summary>
        /// ActorServiceInitializeEvent captures the telemetry event of the initialization of an Actor Service.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/></param>
        /// <param name="actorStateProviderReplicaType">The StateProviderReplicaType to capture the state provider used.</param>
        internal static void ActorServiceInitializeEvent(StatefulServiceContext context, string actorStateProviderReplicaType)
        {
            ActorServiceReplicaInstantiateEvent(context);
            ActorStateProviderUsageEvent(context, actorStateProviderReplicaType);
        }

        /// <summary>
        /// CheckCustomActorServiceUsageEvent captures the telemetry event of the usage of a custom ActorService.
        /// </summary>
        /// <param name="actorType">Type of the actor.</param>
        /// <param name="actorServiceType">Type of the actor service.</param>
        internal static void CheckCustomActorServiceUsageEvent(Type actorType, Type actorServiceType)
        {
            if (!actorServiceType.Equals(typeof(ActorService)))
            {
                ActorEventSource.Instance.CustomActorServiceUsageEventWrapper(
                    ActorTelemetryConstants.CustomActorServiceUsageEventName,
                    TelemetryConstants.OsType,
                    TelemetryConstants.RuntimePlatform,
                    actorType.ToString(),
                    actorServiceType.ToString());
            }
        }

        /// <summary>
        /// ActorReminderRegisterationEvent captures the telemetry event of the registeration of an Actor Reminder
        /// in a given service partition.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/> which contains the context of the service.</param>
        /// <param name="reminder"><see cref="ActorReminder"/> which tracks the actor reminder.</param>
        internal static void ActorReminderRegisterationEvent(StatefulServiceContext context, ActorReminder reminder)
        {
            ActorEventSource.Instance.ActorReminderRegisterationEventWrapper(
                ActorTelemetryConstants.ActorReminderRegisterationEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                reminder.OwnerActorId.ToString(),
                reminder.Period.ToString(),
                reminder.Name);
        }

        /// <summary>
        /// ActorServiceReplicaInstantiateEvent captures the telemetry event of the ActorService replica
        /// initializing.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/> is the service context.</param>
        internal static void ActorServiceReplicaInstantiateEvent(StatefulServiceContext context)
        {
            ActorServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventOpened);
        }

        /// <summary>
        /// ActorServiceReplicaCloseEvent captures the telemetry event for the closing of an ActorService
        /// replica.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/> is the service context.</param>
        internal static void ActorServiceReplicaCloseEvent(StatefulServiceContext context)
        {
            ActorServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventClosed);
        }

        /// <summary>
        /// ActorStateProviderUsageEvent captures the telemetry event for the usage of a state provider.
        /// It is also used for the telemetry of usage of custom state providers.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/> is the service context.</param>
        /// <param name="actorStateProviderReplicaType">The type of the actor state provider.</param>
        internal static void ActorStateProviderUsageEvent(StatefulServiceContext context, string actorStateProviderReplicaType)
        {
            ActorEventSource.Instance.ActorStateProviderUsageEventWrapper(
                ActorTelemetryConstants.ActorStateProviderUsageEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                actorStateProviderReplicaType);
        }

        internal static void KVSToRCMigrationStartEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            DateTime startTimeUtc,
            long noOfSNtoMigrate,
            int copyPhaseParallelism,
            long downtimeThreshold)
        {
            ActorEventSource.Instance.KVSToRCMigrationStartEvent(
                ActorTelemetryConstants.KVSToRCMigrationStartEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                startTimeUtc,
                noOfSNtoMigrate,
                copyPhaseParallelism,
                downtimeThreshold);
        }

        internal static void KVSToRCMigrationCopyPhaseEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            ActorEventSource.Instance.KVSToRCMigrationCopyPhaseEvent(
                ActorTelemetryConstants.KVSToRCMigrationCopyPhaseEndEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated);
        }

        internal static void KVSToRCMigrationCatchupPhaseEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            int noOfIterations,
            long noOfKeysMigrated)
        {
            ActorEventSource.Instance.KVSToRCMigrationCatchupPhaseEvent(
                ActorTelemetryConstants.KVSToRCMigrationCatchupPhaseEndEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfIterations,
                noOfKeysMigrated);
        }

        internal static void KVSToRCMigrationDowntimePhaseEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            ActorEventSource.Instance.KVSToRCMigrationDowntimePhaseEvent(
                ActorTelemetryConstants.KVSToRCMigrationDowntimePhaseEndEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated);
        }

        internal static void KVSToRCMigrationDataValidationWithSuccessEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated,
            long noOfKeysValidated)
        {
            ActorEventSource.Instance.KVSToRCMigrationDataValidationWithSuccessEvent(
                ActorTelemetryConstants.KVSToRCMigrationDataValidationSuccessEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated,
                noOfKeysValidated);
        }

        internal static void KVSToRCMigrationDataValidationWithFailureEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            bool isUserTestHook,
            string errorMessage)
        {
            ActorEventSource.Instance.KVSToRCMigrationDataValidationWithFailureEvent(
                ActorTelemetryConstants.KVSToRCMigrationDataValidationFailureEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                isUserTestHook,
                errorMessage);
        }

        internal static void KVSToRCMigrationCompletionWithSuccessEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            long noOfKeysMigrated)
        {
            ActorEventSource.Instance.KVSToRCMigrationCompletedWithSuccessEvent(
                ActorTelemetryConstants.KVSToRCMigrationCompletionWithSuccessEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                noOfKeysMigrated);
        }

        internal static void KVSToRCMigrationCompletionWithFailureEvent(
            StatefulServiceContext context,
            string kvsServiceName,
            TimeSpan timeSpent,
            string errorMessage)
        {
            ActorEventSource.Instance.KVSToRCMigrationCompletedWithFailureEvent(
                ActorTelemetryConstants.KVSToRCMigrationCompletionWithFailureEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName,
                timeSpent,
                errorMessage);
        }

        internal static void KVSToRCMigrationResumeWritesEvent(
            StatefulServiceContext context,
            string kvsServiceName)
        {
            ActorEventSource.Instance.KVSToRCMigrationResumeWritesEvent(
                ActorTelemetryConstants.KVSToRCMigrationResumeWritesEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                kvsServiceName);
        }

        private static void ActorServiceLifecycleEvent(StatefulServiceContext context, string lifecycleEvent)
        {
            ServiceEventSource.Instance.ServiceLifecycleEventWrapper(
                TelemetryConstants.ServiceLifecycleEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName.ToString(),
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                lifecycleEvent,
                ActorTelemetryConstants.ActorServiceKind);
        }
    }
}
