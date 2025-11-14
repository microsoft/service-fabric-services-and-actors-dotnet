// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Diagnostics;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal interface IDiagnosticsFactory
    {
        IDiagnostics CreateDiagnostics(IClock clock);
    }
}
