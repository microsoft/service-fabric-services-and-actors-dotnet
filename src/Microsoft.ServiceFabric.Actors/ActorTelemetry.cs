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
