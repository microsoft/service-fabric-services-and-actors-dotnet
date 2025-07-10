// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.Diagnostics
{
    internal interface IDiagnosticsManager : IDisposable
    {
        DiagnosticsEventManager DiagnosticsEventManager { get; }
    }
}
