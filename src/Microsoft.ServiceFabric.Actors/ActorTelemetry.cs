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

    internal static class ActorTelemetry
    {
        internal static void ActorServiceInitializeEvent(StatefulServiceContext context, ActorTypeInformation actorTypeInformation)
        {
            ActorStateProviderUsageEvent(context, actorTypeInformation);
        }

        internal static void CheckCustomActorServiceUsageEvent(string actorType, string actorServiceType)
        {
            if (!actorServiceType.Equals(TelemetryConstants.ActorServiceType))
            {
                ActorEventSource.Instance.CustomActorServiceUsageEvent(
                    TelemetryConstants.CustomActorServiceUsageEventName,
                    TelemetryConstants.OsType,
                    TelemetryConstants.RuntimePlatform,
                    actorType,
                    actorServiceType);
            }
        }

        internal static void ActorReminderRegisterationEvent(StatefulServiceContext context, ActorReminder reminder)
        {
            ActorEventSource.Instance.ActorReminderRegisterationEvent(
                TelemetryConstants.ActorReminderRegisterationEventName,
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

        internal static void ActorStateProviderUsageEvent(StatefulServiceContext context, ActorTypeInformation actorTypeInformation)
        {
            var isPersistedStateProvider = actorTypeInformation.StatePersistence.Equals(StatePersistence.Persisted);
            var isVolatileStateProvider = actorTypeInformation.StatePersistence.Equals(StatePersistence.Volatile);
            var stateProviderName = TelemetryConstants.NullActorStateProvider;

            if (isPersistedStateProvider)
            {
#if DotNetCoreClr
                if (TelemetryConstants.OsType.Equals(TelemetryConstants.ClusterOSWindows))
                {
                    stateProviderName = TelemetryConstants.KvsActorStateProvider;
                }
                else
                {
                    stateProviderName = TelemetryConstants.ReliableCollectionsActorStateProvider;
                }
#else
                stateProviderName = TelemetryConstants.KvsActorStateProvider;
#endif
            }
            else if (isVolatileStateProvider)
            {
                stateProviderName = TelemetryConstants.VolatileActorStateProvider;
            }

            ActorEventSource.Instance.ActorStateProviderUsageEvent(
                TelemetryConstants.ActorReminderRegisterationEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                stateProviderName);
        }
    }
}
