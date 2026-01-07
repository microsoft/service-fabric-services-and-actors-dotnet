// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    interface IDiagnosticEvents
    {
        void OnRequestResponseBegin();
        void OnRequestResponseEnd(DateTime startTime);
        void OnCreateTransportMessageBegin();
        void OnCreateTransportMessageEnd(DateTime startTime);
        void OnRemotingRequestBegin();
        void OnRemotingRequestEnd(DateTime startTime);
    }
}
