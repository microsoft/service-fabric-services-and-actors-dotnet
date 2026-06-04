// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using HRESULT = System.Int32;

#if !NET
using LibraryImportAttribute = System.Runtime.InteropServices.DllImportAttribute;
#endif

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    static partial class NativeTelemetry
    {
        internal static IFabricMeterProvider2 FabricCreateMeterProvider()
        {
            Marshal.ThrowExceptionForHR(FabricCreateMeterProvider(out IFabricMeterProvider2 meterProvider));
            return meterProvider;
        }

        [LibraryImport("FabricTelemetry")]
        internal static
#if NET
        partial
#else
        extern
#endif
        HRESULT FabricCreateMeterProvider([MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricMeterProvider2>))] out IFabricMeterProvider2 meterProvider);

    }
}
