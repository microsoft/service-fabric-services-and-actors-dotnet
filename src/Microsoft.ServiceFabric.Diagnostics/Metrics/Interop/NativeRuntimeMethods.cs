// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using HRESULT = System.Int32;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Interop
{
    internal static class NativeRuntimeMethods
    {

        internal static IFabricMeterProvider FabricCreateMeterProvider()
        {
            Marshal.ThrowExceptionForHR(PInvoke.FabricCreateMeterProvider(out IFabricMeterProvider meterProvider));
            return meterProvider;
        }

        static class PInvoke
        {
            private const string FabricTelemetryLib = "FabricTelemetry";

            [DllImport(FabricTelemetryLib)]
            internal static extern HRESULT FabricCreateMeterProvider(
                [MarshalAs(UnmanagedType.Interface)] out IFabricMeterProvider meterProvider);
        }
    }
}
