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
    [Guid("A0D80970-C062-4C59-A3CA-7AEEB901B49C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    partial interface IFabricMeter
    {
        void Record(long value, uint count, IntPtr dimensionValues);
    }
}
