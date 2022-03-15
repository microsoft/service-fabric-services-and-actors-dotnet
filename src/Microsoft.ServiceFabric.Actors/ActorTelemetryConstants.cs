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

        internal static readonly string ActorServiceKind = "ActorService";
    }
}
