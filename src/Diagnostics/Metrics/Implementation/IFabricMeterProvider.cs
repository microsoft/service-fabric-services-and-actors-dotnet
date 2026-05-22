// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

#if !NET
using GeneratedComInterfaceAttribute = System.Runtime.InteropServices.ComImportAttribute;
#endif

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

[GeneratedComInterface]
[Guid("89462876-f11e-41c6-bd99-c933b46c5e66")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
partial interface IFabricMeterProvider
{
    // Vtable slot 3: inherited from old IFabricMeterProvider (GUID 15AD37D2-F641-4188-824B-0D68CB4F6C17).
    // Declared for vtable alignment only; not called by managed code.
    [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricMeter>))]
    IFabricMeter CreateMeter(IntPtr metricNamespace, IntPtr name, uint count, IntPtr dimensionNames);

    // Vtable slot 4: new method on IFabricMeterProvider2.
    [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricMeter>))]
    IFabricMeter CreateMeter2(IntPtr meterDescription);
}
