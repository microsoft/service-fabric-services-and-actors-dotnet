// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.ServiceFabric.FabricTransport
{
    [GeneratedComClass]
    sealed partial class FabricServiceCommunicationMessage : NativeServiceCommunication.IFabricServiceCommunicationMessage,
        IDisposable
    {
        private PinCollection pin;
        private readonly IntPtr nativeHeaders;
        private readonly IntPtr nativeBody;

        public FabricServiceCommunicationMessage(byte[] headers, byte[] body)
        {
            // Create pinned objects for header and body.
            this.pin = new PinCollection();
            var nativeObj = new NativeTypes.FABRIC_MESSAGE_BUFFER();
            var nativeValue = NativeTypes.ToNativeBytes(this.pin, headers);
            nativeObj.BufferSize = nativeValue.Item1;
            nativeObj.Buffer = nativeValue.Item2;
            this.nativeHeaders = this.pin.AddBlittable(nativeObj);

            nativeObj = new NativeTypes.FABRIC_MESSAGE_BUFFER();
            nativeValue = NativeTypes.ToNativeBytes(this.pin, body);
            nativeObj.BufferSize = nativeValue.Item1;
            nativeObj.Buffer = nativeValue.Item2;
            this.nativeBody = this.pin.AddBlittable(nativeObj);
        }

        public IntPtr Get_Body()
        {
            return this.nativeBody;
        }

        public IntPtr Get_Headers()
        {
            return this.nativeHeaders;
        }

        public void Dispose()
        {
            if (pin != null)
            {
                pin.Dispose();
                pin = null;
            }
        }
    }
}
