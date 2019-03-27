// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;

    /// <summary>
    /// Interface for DiagnosticsManager.
    /// </summary>
    internal interface IDiagnosticsManager : IDisposable
    {
        /// <summary>
        /// Gets DiagnosticsEventManager.
        /// </summary>
        DiagnosticsEventManager DiagnosticsEventManager { get; }
    }
}
