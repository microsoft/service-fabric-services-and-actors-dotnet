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
    private readonly Func<OSPlatform, bool> previousIsOSPlatform;
#endif

    public EventSourceFixture()
    {
#if NET
        var isOSPlatformField = typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>();
        this.previousIsOSPlatform = isOSPlatformField.Get();
        isOSPlatformField.Set(_ => false);
#endif
    }

    public virtual void Dispose()
    {
#if NET
        typeof(ServiceFabricEventSource).Field<Func<OSPlatform, bool>>().Set(this.previousIsOSPlatform);
#endif
    }
}
