// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

#if NET
using System.Runtime.InteropServices.Marshalling;
#else
using GeneratedComInterfaceAttribute = System.Runtime.InteropServices.ComImportAttribute;
#endif

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    [GeneratedComInterface]
    [Guid("15AD37D2-F641-4188-824B-0D68CB4F6C17")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    partial interface IFabricMeterProvider
    {
        IFabricMeter CreateMeter([MarshalAs(UnmanagedType.LPWStr)] string metricNamespace, [MarshalAs(UnmanagedType.LPWStr)] string name, uint count, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] dimensionNames);
    }
}
