// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Interop
{
    [ComImport]
    [Guid("15ad37d2-f641-4188-824b-0d68cb4f6c17")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFabricMeterProvider
    {
        [PreserveSig]
        int CreateMeter(
            [In, MarshalAs(UnmanagedType.LPWStr)] string namespaceName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string name,
            [In] NativeTypes.FABRIC_STRING_LIST dimensionNames,
            [MarshalAs(UnmanagedType.Interface)] out IFabricMeter meter);
    }
}
