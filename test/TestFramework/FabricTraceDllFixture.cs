// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#if NET

using System;
using System.Runtime.InteropServices;
using Inspector;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.TestFramework
{
    /// <summary>
    /// Disables <see cref="ServiceFabricEventSource"/> Linux detection to prevent
    /// <c>UnstructuredTracePublisher</c> from P/Invoking into <c>libFabricCommon</c>,
    /// which is unavailable outside of Service Fabric clusters.
    /// </summary>
    public class FabricTraceDllFixture : IDisposable
    {
        public FabricTraceDllFixture() =>
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(_ => false);

        public virtual void Dispose() =>
            typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(new Func<OSPlatform, bool>(RuntimeInformation.IsOSPlatform));
    }
}

#endif
