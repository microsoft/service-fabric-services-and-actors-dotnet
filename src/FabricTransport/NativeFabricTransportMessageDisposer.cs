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
    /// <summary>
    /// Disposes native <see cref="IFabricTransportMessage"/> instances handed back across the COM boundary by the
    /// Service Fabric runtime.
    /// </summary>
    [GeneratedComClass]
    sealed partial class NativeFabricTransportMessageDisposer : IFabricTransportMessageDisposer
    {
        readonly int sizeOfPtr = Marshal.SizeOf(typeof(IntPtr));

        /// <summary>
        /// Disposes the <see cref="IFabricTransportMessage"/> instances in the native array referenced by
        /// <paramref name="messages"/>.
        /// </summary>
        /// <param name="count">The number of message pointers in the array referenced by <paramref name="messages"/>.</param>
        /// <param name="messages">A pointer to a contiguous array of <see cref="IFabricTransportMessage"/> COM interface pointers to dispose.</param>
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
