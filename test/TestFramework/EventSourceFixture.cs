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
#if NET
    readonly Func<OSPlatform, bool> previous;
#endif

    public EventSourceFixture()
    {
#if NET
        var field = typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>();
        previous = field.Value;
        field.Set(_ => false);
#endif
    }

    public virtual void Dispose()
    {
#if NET
        typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(previous);
#endif
    }
}
