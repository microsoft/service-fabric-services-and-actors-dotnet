// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.FabricTransport.V2
{
    using System;
    using System.Fabric.Interop;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using BOOLEAN = System.SByte;
    using HRESULT = System.Int32;
    using REMOTING_REQUEST_ID = System.Guid;
#if NET
    using System.Runtime.InteropServices.Marshalling;
#else
    using GeneratedComInterfaceAttribute = System.Runtime.InteropServices.ComImportAttribute;
    using LibraryImportAttribute = System.Runtime.InteropServices.DllImportAttribute;
#endif

    static partial class NativeFabricTransport
    {
        // ------------------------------------------------------------------------
        // Fabric Transport Structures
        // ------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct FABRIC_TRANSPORT_SETTINGS
        {
            public UInt32 OperationTimeoutInSeconds;
            public UInt32 KeepAliveTimeoutInSeconds;
            public UInt32 MaxMessageSize;
            public UInt32 MaxConcurrentCalls;
            public UInt32 MaxQueueSize;
            public IntPtr SecurityCredentials;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct FABRIC_TRANSPORT_SETTINGS_EX1
        {
            public UInt32 ConnectTimeoutInMilliseconds;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct FABRIC_TRANSPORT_SETTINGS_EX2
        {
            public BOOLEAN EnableMaxConcurrentCalls;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct FABRIC_TRANSPORT_LISTEN_ADDRESS
        {
            public IntPtr IPAddressOrFQDN;
            public UInt32 Port;
            public IntPtr Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct FABRIC_TRANSPORT_MESSAGE_BUFFER
        {
            public UInt32 BufferSize;
            public IntPtr Buffer;
        }

        #region DLL Entry Points 

        internal static IFabricTransportListener CreateFabricTransportListener(
            ref Guid iid,
            IntPtr settings,
            IntPtr address,
            IFabricTransportMessageHandler messageHandler,
            IFabricTransportConnectionHandler connectionHandler,
            IFabricTransportMessageDisposer messageDisposer)
        {
            Marshal.ThrowExceptionForHR(
                PInvoke.CreateFabricTransportListener(ref iid, settings, address, messageHandler, connectionHandler, messageDisposer, out IFabricTransportListener listener));
            return listener;
        }

        internal static IFabricTransportClient2 CreateFabricTransportClient(
            ref Guid iid,
            IntPtr settings,
            IntPtr address,
            IFabricTransportCallbackMessageHandler messageHandler,
            IFabricTransportClientEventHandler eventHandler,
            IFabricTransportMessageDisposer messageDisposer)
        {
            Marshal.ThrowExceptionForHR(
                PInvoke.CreateFabricTransportClient(ref iid, settings, address, messageHandler, eventHandler, messageDisposer, out IFabricTransportClient2 client));
            return client;
        }

        #endregion

        static partial class PInvoke
        {
            const string FabricTransportDll = "FabricTransport";

            [LibraryImport(FabricTransportDll)]
            internal static
#if NET
            partial
#else
            extern
#endif
            HRESULT CreateFabricTransportListener(
                ref Guid iid,
                IntPtr settings,
                IntPtr address,
                IFabricTransportMessageHandler messageHandler,
                IFabricTransportConnectionHandler connectionHandler,
                IFabricTransportMessageDisposer messageDisposer,
                out IFabricTransportListener listener);

            [LibraryImport(FabricTransportDll)]
            internal static
#if NET
            partial
#else
            extern
#endif
            HRESULT CreateFabricTransportClient(
                ref Guid iid,
                IntPtr settings,
                IntPtr address,
                IFabricTransportCallbackMessageHandler messageHandler,
                IFabricTransportClientEventHandler eventHandler,
                IFabricTransportMessageDisposer messageDisposer,
                out IFabricTransportClient2 client);
        }

        [GeneratedComInterface]
        [Guid("b4357dab-ef06-465f-b453-938f3b0ad4b5")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportMessage
        {
            [PreserveSig]
            void GetHeaderAndBodyBuffer(
                out IntPtr HeaderPtr,
                out UInt32 bufferlength,
                out IntPtr bufferPtr);

            [PreserveSig]
            void Dispose();
        }

        [GeneratedComInterface]
        [Guid("914097f3-a821-46ea-b3d9-feafe5f7c4a9")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportMessageDisposer
        {
            [PreserveSig]
            void Dispose(
                UInt32 count,
                IntPtr messages);

        }

        //// ----------------------------------------------------------------------------
        //// Interfaces
        ///     
        /// 
        [GeneratedComInterface]
        [Guid("6815bdb4-1479-4c44-8b9d-57d6d0cc9d64")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportMessageHandler
        {
            NativeCommon.IFabricAsyncOperationContext BeginProcessRequest(
                IntPtr clientId,
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            IFabricTransportMessage EndProcessRequest(
                NativeCommon.IFabricAsyncOperationContext context);

            void HandleOneWay(
                IntPtr clientId,
                IFabricTransportMessage message);
        }

        [GeneratedComInterface]
        [Guid("1b63a266-1eeb-4f3e-8886-521458980d10")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportListener
        {
            NativeCommon.IFabricAsyncOperationContext BeginOpen(
                NativeCommon.IFabricAsyncOperationCallback callback);

            NativeCommon.IFabricStringResult EndOpen(
                NativeCommon.IFabricAsyncOperationContext context);

            NativeCommon.IFabricAsyncOperationContext BeginClose(
                NativeCommon.IFabricAsyncOperationCallback callback);

            void EndClose(
                NativeCommon.IFabricAsyncOperationContext context);

            [PreserveSig]
            void Abort();
        }


        [GeneratedComInterface]
        [Guid("5b0634fe-6a52-4bd9-8059-892c72c1d73a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportClient
        {
            NativeCommon.IFabricAsyncOperationContext BeginRequest(
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            IFabricTransportMessage EndRequest(
                NativeCommon.IFabricAsyncOperationContext context);

            void Send(
                IFabricTransportMessage message);

            NativeCommon.IFabricAsyncOperationContext BeginOpen(
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);


            void EndOpen(
                NativeCommon.IFabricAsyncOperationContext context);

            NativeCommon.IFabricAsyncOperationContext BeginClose(
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);


            void EndClose(
                NativeCommon.IFabricAsyncOperationContext context);

            [PreserveSig]
            void Abort();
        }

        [GeneratedComInterface]
        [Guid("9a078db3-aa29-40b2-8ca1-2913bc966b7c")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportClient2 : IFabricTransportClient
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new NativeCommon.IFabricAsyncOperationContext BeginRequest(
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            new IFabricTransportMessage EndRequest(
                NativeCommon.IFabricAsyncOperationContext context);

            new void Send(
                IFabricTransportMessage message);

            new NativeCommon.IFabricAsyncOperationContext BeginOpen(
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            new void EndOpen(
                NativeCommon.IFabricAsyncOperationContext context);

            new NativeCommon.IFabricAsyncOperationContext BeginClose(
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            new void EndClose(
                NativeCommon.IFabricAsyncOperationContext context);

            [PreserveSig]
            new void Abort();
#endif
            NativeCommon.IFabricAsyncOperationContext BeginRequestWithId(
                REMOTING_REQUEST_ID requestId,
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            IFabricTransportMessage EndRequestWithId(
                NativeCommon.IFabricAsyncOperationContext context);
        }

        [GeneratedComInterface]
        [Guid("9ba8ac7a-3464-4774-b9b9-1d7f0f1920ba")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportCallbackMessageHandler
        {
            void HandleOneWay(
                IFabricTransportMessage message);
        }

        [GeneratedComInterface]
        [Guid("a54c17f7-fe94-4838-b14d-e9b5c258e2d0")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportClientConnection
        {
            void Send(
                IFabricTransportMessage message);

            [PreserveSig]
            IntPtr get_ClientId();
        }

        [GeneratedComInterface]
        [Guid("b069692d-e8f0-4f25-a3b6-b2992598a64c")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportConnectionHandler
        {
            NativeCommon.IFabricAsyncOperationContext BeginProcessConnect(
                IFabricTransportClientConnection clientConnection,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);


            void EndProcessConnect(
                NativeCommon.IFabricAsyncOperationContext context);

            NativeCommon.IFabricAsyncOperationContext BeginProcessDisconnect(
                IntPtr clientId,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            void EndProcessDisconnect(
                NativeCommon.IFabricAsyncOperationContext context);
        }

        [GeneratedComInterface]
        [Guid("4935ab6f-a8bc-4b10-a69e-7a3ba3324892")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportClientEventHandler
        {
            void OnConnected(
                IntPtr connectionAddress);

            void OnDisconnected(
                IntPtr connectionAddress,
                int errorCode);
        }
    }
}
