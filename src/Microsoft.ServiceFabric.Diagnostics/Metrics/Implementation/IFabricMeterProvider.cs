// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    [ComImport]
    [Guid("15AD37D2-F641-4188-824B-0D68CB4F6C17")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IFabricMeterProvider
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IFabricMeter CreateMeter([In][MarshalAs(UnmanagedType.LPWStr)] string metricNamespace, [In][MarshalAs(UnmanagedType.LPWStr)] string name, [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] dimensionNames, [In] uint count);
    }
}
