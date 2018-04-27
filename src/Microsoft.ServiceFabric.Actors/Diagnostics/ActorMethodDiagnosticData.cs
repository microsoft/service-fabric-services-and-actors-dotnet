// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using Microsoft.ServiceFabric.Services.Remoting;

    internal struct ActorMethodDiagnosticData
    {
        internal ActorId ActorId;
        internal long InterfaceMethodKey;
        internal TimeSpan? MethodExecutionTime;
        internal Exception Exception;
        internal RemotingListenerVersion RemotingListenerVersion;
    }
}
