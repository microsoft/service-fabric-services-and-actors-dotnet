// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;

    internal struct ActorStateDiagnosticData
    {
        internal ActorId ActorId;
        internal TimeSpan? OperationTime;
    }
}
