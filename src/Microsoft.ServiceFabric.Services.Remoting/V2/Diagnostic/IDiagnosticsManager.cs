// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal interface IDiagnosticsManager : IDisposable
    {
        void FabricTransportRequestBegin();
        void FabricTransportRequestEnd(Stopwatch stopwatch);
        void FabricTransportCreateTransportMessage(Stopwatch stopwatch);
        void FabricTransportCreateRemotingMessage(Stopwatch stopwatch);
    }
}
