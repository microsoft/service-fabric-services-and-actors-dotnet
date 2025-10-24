// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class DiagnosticsManagerActorContext
    {
#pragma warning disable SA1401 // Fields should be private. Used in Interlocked increment and decrement.
        internal long PendingActorMethodCalls;
#pragma warning restore SA1401 // Fields should be private

        internal long LastReportedPendingActorMethodCalls { get; set; }
    }
}
