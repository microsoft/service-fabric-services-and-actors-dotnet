// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static Microsoft.ServiceFabric.FabricTransport.NativeFabricTransport;

namespace Microsoft.ServiceFabric.FabricTransport
{
    [GeneratedComClass]
    sealed partial class NativeFabricTransportMessageDisposer : IFabricTransportMessageDisposer
    {
        readonly int sizeOfPtr = Marshal.SizeOf(typeof(IntPtr));

        public void Dispose(UInt32 count, IntPtr messages)
        {
            for (var i = 0; i < count; i++)
            {
                IntPtr messagePtr = Marshal.ReadIntPtr(IntPtr.Add(messages, i * sizeOfPtr));
                object message = messagePtr.GetObjectForIUnknown();
                ((IFabricTransportMessage)message).Dispose();
            }

            GC.KeepAlive(messages);
        }
    }
}
