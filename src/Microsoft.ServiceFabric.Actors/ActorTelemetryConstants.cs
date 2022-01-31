// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    internal static class ActorTelemetryConstants
    {
        // Names of all the actor telemetry events.
        internal static readonly string ActorStateProviderUsageEventName = "TelemetryEvents.ActorStateProviderUsageEvent";
        internal static readonly string CustomActorServiceUsageEventName = "TelemetryEvents.CustomActorServiceUsageEvent";
        internal static readonly string ActorReminderRegisterationEventName = "TelemetryEvents.ActorReminderRegisterationEvent";

        #region KVS to RC Migration Constants
        internal static readonly string KVSToRCMigrationStartEvent = "TelemetryEvents.KVSToRCMigrationStartEvent";
        internal static readonly string KVSToRCMigrationCopyPhaseEndEvent = "TelemetryEvents.KVSToRCMigrationCopyPhaseEndEvent";
        internal static readonly string KVSToRCMigrationCatchupPhaseEndEvent = "TelemetryEvents.KVSToRCMigrationCatchupPhaseEndEvent";
        internal static readonly string KVSToRCMigrationDowntimePhaseEndEvent = "TelemetryEvents.KVSToRCMigrationDowntimePhaseEndEvent";
        internal static readonly string KVSToRCMigrationDataValidationSuccessEvent = "TelemetryEvents.KVSToRCMigrationDataValidationSuccessEvent";
        internal static readonly string KVSToRCMigrationDataValidationFailureEvent = "TelemetryEvents.KVSToRCMigrationDataValidationFailureEvent";
        internal static readonly string KVSToRCMigrationCompletionWithSuccessEvent = "TelemetryEvents.KVSToRCMigrationCompletionWithSuccessEvent";
        internal static readonly string KVSToRCMigrationCompletionWithFailureEvent = "TelemetryEvents.KVSToRCMigrationCompletionWithFailureEvent";
        internal static readonly string KVSToRCMigrationResumeWritesEvent = "TelemetryEvents.KVSToRCMigrationResumeWritesEvent";
        #endregion KVS to RC Migration Constants

        internal static readonly string ActorServiceKind = "ActorService";
    }
}
