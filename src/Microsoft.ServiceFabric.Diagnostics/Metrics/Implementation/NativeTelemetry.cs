// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using HRESULT = System.Int32;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    internal static class NativeTelemetry
    {
        const string FabricTelemetryLib = "FabricTelemetry";

        internal static IFabricMeterProvider FabricCreateMeterProvider()
        {
            Marshal.ThrowExceptionForHR(FabricCreateMeterProvider(out IFabricMeterProvider meterProvider));
            return meterProvider;
        }

        [DllImport(FabricTelemetryLib)]
        static extern HRESULT FabricCreateMeterProvider([MarshalAs(UnmanagedType.Interface)] out IFabricMeterProvider meterProvider);

    }
}
