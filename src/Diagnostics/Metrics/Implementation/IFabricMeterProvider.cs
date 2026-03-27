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

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    [GeneratedComInterface]
    [Guid("15AD37D2-F641-4188-824B-0D68CB4F6C17")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    partial interface IFabricMeterProvider
    {
        [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricMeter>))]
        IFabricMeter CreateMeter([MarshalAs(UnmanagedType.LPWStr)] string metricNamespace, [MarshalAs(UnmanagedType.LPWStr)] string name, uint totalDimensionsCount, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] dimensionNames, uint fixedDimensionCount, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] fixedDimensionValues);
    }
}
