// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Fabric.Interop;

namespace Microsoft.ServiceFabric.FabricTransport.V2
{
    internal static class FabricTransportSettingsExtension
    {
        internal static IntPtr ToNativeV2(this FabricTransportSettings transportSettings, PinCollection pin)
        {
            var nativeObj = new NativeFabricTransport.FABRIC_TRANSPORT_SETTINGS();
            nativeObj.Reserved = IntPtr.Zero;

            if (transportSettings.SecurityCredentials != null)
            {
                nativeObj.SecurityCredentials = transportSettings.SecurityCredentials.ToNative(pin);
            }
            else
            {
                nativeObj.SecurityCredentials = IntPtr.Zero;
            }

            if (transportSettings.OperationTimeout.TotalSeconds < 0)
            {
                nativeObj.OperationTimeoutInSeconds = 0;
            }
            else
            {
                nativeObj.OperationTimeoutInSeconds = (uint) transportSettings.OperationTimeout.TotalSeconds;
            }

            if (transportSettings.KeepAliveTimeout.TotalSeconds < 0)
            {
                nativeObj.KeepAliveTimeoutInSeconds = 0;
            }
            else
            {
                nativeObj.KeepAliveTimeoutInSeconds = (uint) transportSettings.KeepAliveTimeout.TotalSeconds;
            }


            Helper.ThrowIfValueOutOfBounds(transportSettings.MaxMessageSize, "MaxMessageSize");

            nativeObj.MaxMessageSize = (uint) transportSettings.MaxMessageSize;

            Helper.ThrowIfValueOutOfBounds(transportSettings.MaxConcurrentCalls, "MaxConcurrentCalls");
            nativeObj.MaxConcurrentCalls = (uint) transportSettings.MaxConcurrentCalls;

            Helper.ThrowIfValueOutOfBounds(transportSettings.MaxQueueSize, "MaxQueueSize");

            nativeObj.MaxQueueSize = (uint) transportSettings.MaxQueueSize;

            var ex1settings = new NativeFabricTransport.FABRIC_TRANSPORT_SETTINGS_EX1();

            if (transportSettings.ConnectTimeout.TotalMilliseconds < 0)
            {
                ex1settings.ConnectTimeoutInMilliseconds = (uint) FabricTransportSettings.DefaultConnectTimeout.TotalMilliseconds;
            }
            else
            {
                ex1settings.ConnectTimeoutInMilliseconds = (uint) transportSettings.ConnectTimeout.TotalMilliseconds;
            }

            var ex2settings = new NativeFabricTransport.FABRIC_TRANSPORT_SETTINGS_EX2();
            ex2settings.EnableMaxConcurrentCalls = NativeTypes.ToBOOLEAN(transportSettings.MaxConcurrentCalls > 0);

            ex1settings.Reserved = pin.AddBlittable(ex2settings);

            nativeObj.Reserved = pin.AddBlittable(ex1settings);

            return pin.AddBlittable(nativeObj);
        }
    }
}
