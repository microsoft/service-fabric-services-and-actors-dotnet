// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
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
        internal Exception Exception;
        internal RemotingListenerVersion RemotingListener;
        internal TimeSpan? MethodExecutionTime;

        internal ActorMethodDiagnosticData(ActorId actorId, long interfaceMethodKey, Exception exception, RemotingListenerVersion remotingListener)
        {
            ActorId = actorId;
            InterfaceMethodKey = interfaceMethodKey;
            Exception = exception;
            RemotingListener = remotingListener;
        }
    }
}
