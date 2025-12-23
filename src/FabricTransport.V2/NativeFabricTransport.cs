// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using BOOLEAN = System.SByte;
using HRESULT = System.Int32;
using REMOTING_REQUEST_ID = System.Guid;
#if NETFRAMEWORK
    using GeneratedComInterfaceAttribute = System.Runtime.InteropServices.ComImportAttribute;
    using LibraryImportAttribute = System.Runtime.InteropServices.DllImportAttribute;
#endif
using static System.Fabric.Interop.NativeCommon;

namespace Microsoft.ServiceFabric.FabricTransport.V2
{
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

            [LibraryImport(FabricTransportDll)] internal static
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
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportListener>))]
                out IFabricTransportListener listener);

            [LibraryImport(FabricTransportDll)] internal static
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
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportClient2>))]
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

        // This interface is implemented but not consumed in managed code.
        // COM outputs aren't used by managed code and don't need special marshalling.
        // COM inputs should be wrapped in unique ComObjects for deterministic release.
        // IFabricAsyncOperationContext inputs are managed and should be unwrapped.
        [GeneratedComInterface]
        [Guid("6815bdb4-1479-4c44-8b9d-57d6d0cc9d64")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportMessageHandler
        {
            IFabricAsyncOperationContext BeginProcessRequest(
                IntPtr clientId,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportMessage>))]
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationCallback>))]
                IFabricAsyncOperationCallback callback);

            IFabricTransportMessage EndProcessRequest(
                IFabricAsyncOperationContext context);

            void HandleOneWay(
                IntPtr clientId,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportMessage>))]
                IFabricTransportMessage message);
        }

        [GeneratedComInterface]
        [Guid("1b63a266-1eeb-4f3e-8886-521458980d10")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportListener
        {
            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginOpen(
                IFabricAsyncOperationCallback callback);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricStringResult>))]
            IFabricStringResult EndOpen(
                IFabricAsyncOperationContext context);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginClose(
                IFabricAsyncOperationCallback callback);

            void EndClose(
                IFabricAsyncOperationContext context);

            [PreserveSig]
            void Abort();
        }

        [GeneratedComInterface]
        [Guid("5b0634fe-6a52-4bd9-8059-892c72c1d73a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportClient
        {
            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginRequest(
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportMessage>))]
            IFabricTransportMessage EndRequest(
                IFabricAsyncOperationContext context);

            void Send(
                IFabricTransportMessage message);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginOpen(
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            void EndOpen(
                IFabricAsyncOperationContext context);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginClose(
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            void EndClose(
                IFabricAsyncOperationContext context);

            [PreserveSig]
            void Abort();
        }

        [GeneratedComInterface]
        [Guid("9a078db3-aa29-40b2-8ca1-2913bc966b7c")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportClient2 : IFabricTransportClient
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new IFabricAsyncOperationContext BeginRequest(
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            new IFabricTransportMessage EndRequest(
                IFabricAsyncOperationContext context);

            new void Send(
                IFabricTransportMessage message);

            new IFabricAsyncOperationContext BeginOpen(
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            new void EndOpen(
                IFabricAsyncOperationContext context);

            new IFabricAsyncOperationContext BeginClose(
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            new void EndClose(
                IFabricAsyncOperationContext context);

            [PreserveSig]
            new void Abort();
#endif

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginRequestWithId(
                REMOTING_REQUEST_ID requestId,
                IFabricTransportMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportMessage>))]
            IFabricTransportMessage EndRequestWithId(
                IFabricAsyncOperationContext context);
        }

        // This interface is implemented but not consumed in managed code.
        // COM inputs should be wrapped in unique ComObjects for deterministic release.
        [GeneratedComInterface]
        [Guid("9ba8ac7a-3464-4774-b9b9-1d7f0f1920ba")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportCallbackMessageHandler
        {
            void HandleOneWay(
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportMessage>))]
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

        // This interface is implemented but not consumed in managed code.
        // COM outputs aren't used by managed code and don't need special marshalling.
        // COM inputs should be wrapped in unique ComObjects for deterministic release.
        // IFabricAsyncOperationContext inputs are managed and should be unwrapped.
        [GeneratedComInterface]
        [Guid("b069692d-e8f0-4f25-a3b6-b2992598a64c")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricTransportConnectionHandler
        {
            IFabricAsyncOperationContext BeginProcessConnect(
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricTransportClientConnection>))]
                IFabricTransportClientConnection clientConnection,
                uint timeoutMilliseconds,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationCallback>))]
                IFabricAsyncOperationCallback callback);

            void EndProcessConnect(
                IFabricAsyncOperationContext context);

            IFabricAsyncOperationContext BeginProcessDisconnect(
                IntPtr clientId,
                uint timeoutMilliseconds,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationCallback>))]
                IFabricAsyncOperationCallback callback);

            void EndProcessDisconnect(
                IFabricAsyncOperationContext context);
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
