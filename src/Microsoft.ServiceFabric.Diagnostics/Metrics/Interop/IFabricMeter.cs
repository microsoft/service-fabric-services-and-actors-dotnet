// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Interop
{
    [ComImport]
    [Guid("a0d80970-c062-4c59-a3ca-7aeeb901b49c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFabricMeter
    {
        [PreserveSig]
        int Record(
            [In] long value,
            [In, MarshalAs(UnmanagedType.Struct)] FabricStringList dimensionValues);
    }
}
