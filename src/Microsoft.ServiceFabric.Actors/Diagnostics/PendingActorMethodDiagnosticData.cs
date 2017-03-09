// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal struct PendingActorMethodDiagnosticData
    {
        internal ActorId ActorId;
        internal long PendingActorMethodCalls;
        internal long PendingActorMethodCallsDelta;
    }
}