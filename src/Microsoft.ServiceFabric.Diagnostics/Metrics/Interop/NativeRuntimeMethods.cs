// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Interop
{
    internal static class NativeRuntimeMethods
    {
        private const string FabricTelemetryLib = "FabricTelemetry";

        [DllImport(FabricTelemetryLib, PreserveSig = false)]
        internal unsafe static extern int FabricCreateMeterProvider(
            [MarshalAs(UnmanagedType.Interface)] out IFabricMeterProvider meterProvider);
    }
}
