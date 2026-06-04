// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
struct FABRIC_METER_DESCRIPTION
{
    public IntPtr Namespace;
    public IntPtr Name;
    public uint TotalDimensionsCount;
    public IntPtr DimensionNames;
    public uint FixedDimensionCount;
    public IntPtr FixedDimensionValues;
    public IntPtr Reserved;
}
