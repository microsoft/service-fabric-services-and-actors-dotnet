// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    internal class MigrationTelemetryConstants
    {
        internal static readonly string MigrationStartEvent = "TelemetryEvents.MigrationStartOrResumeEvent";
        internal static readonly string MigrationEndEvent = "TelemetryEvents.MigrationEndEvent";
        internal static readonly string MigrationPhaseStartEvent = "TelemetryEvents.MigrationPhaseStartOrResumeEvent";
        internal static readonly string MigrationPhaseEndEvent = "TelemetryEvents.MigrationPhaseEndEvent";
        internal static readonly string MigrationFailureEvent = "TelemetryEvents.MigrationFailureEvent";
        internal static readonly string MigrationAbortEvent = "TelemetryEvents.MigrationAbortEvent";
    }
}
