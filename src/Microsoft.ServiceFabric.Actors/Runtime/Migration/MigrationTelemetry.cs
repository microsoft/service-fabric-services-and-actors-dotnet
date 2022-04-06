// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Services;

    internal static class MigrationTelemetry
    {
        private static readonly ActorEventSource EventSource = ActorEventSource.Instance;

        internal static void MigrationStartEvent(StatefulServiceContext context, string settingsJson)
        {
            EventSource.MigrationStartEvent(
                MigrationTelemetryConstants.MigrationStartEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                settingsJson);
        }

        internal static void MigrationEndEvent(StatefulServiceContext context, string resultJson)
        {
            EventSource.MigrationEndEvent(
                MigrationTelemetryConstants.MigrationEndEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                resultJson);
        }

        internal static void MigrationPhaseStartEvent(StatefulServiceContext context, string inputJson)
        {
            EventSource.MigrationPhaseStartEvent(
                MigrationTelemetryConstants.MigrationPhaseStartEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                inputJson);
        }

        internal static void MigrationPhaseEndEvent(StatefulServiceContext context, string resultJson)
        {
            EventSource.MigrationPhaseEndEvent(
                MigrationTelemetryConstants.MigrationPhaseEndEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                resultJson);
        }

        internal static void MigrationFailureEvent(StatefulServiceContext context, string phase, string errorMsg)
        {
            EventSource.MigrationFailureEvent(
                MigrationTelemetryConstants.MigrationFailureEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                phase,
                errorMsg);
        }

        internal static void MigrationAbortEvent(StatefulServiceContext context, bool userTriggered)
        {
            EventSource.MigrationAbortEvent(
                MigrationTelemetryConstants.MigrationAbortEvent,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                userTriggered);
        }
    }
}
