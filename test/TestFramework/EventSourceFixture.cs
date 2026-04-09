// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

#if NET
using System.Runtime.InteropServices;
using Inspector;
#endif

namespace Microsoft.ServiceFabric;

/// <summary>
/// Disables <see cref="ServiceFabricEventSource"/> Linux detection to prevent <see cref="UnstructuredTracePublisher"/> 
/// from loading <c>libFabricCommon.so</c>, which is unavailable outside of Service Fabric clusters.
/// </summary>
public class EventSourceFixture : IDisposable
{
    public EventSourceFixture()
    {
#if NET
        typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(_ => false);
#endif
    }

    public virtual void Dispose()
    {
#if NET
        typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(new Func<OSPlatform, bool>(RuntimeInformation.IsOSPlatform));
#endif
    }
}
