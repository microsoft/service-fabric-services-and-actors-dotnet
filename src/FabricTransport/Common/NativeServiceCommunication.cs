// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using HRESULT = System.Int32;
using static System.Fabric.Interop.NativeCommon;
#if NETFRAMEWORK
    using GeneratedComInterfaceAttribute = System.Runtime.InteropServices.ComImportAttribute;
    using LibraryImportAttribute = System.Runtime.InteropServices.DllImportAttribute;
#endif

namespace Microsoft.ServiceFabric.FabricTransport
{
    static partial class NativeServiceCommunication
    {
        internal static IFabricServiceCommunicationListener CreateServiceCommunicationListener(
            ref Guid iid,
            IntPtr transportSettings,
            IntPtr listenerAddress,
            IFabricCommunicationMessageHandler messageHandler,
            IFabricServiceConnectionHandler connectionHandler)
        {
            Marshal.ThrowExceptionForHR(
                PInvoke.CreateServiceCommunicationListener(ref iid, transportSettings, listenerAddress, messageHandler, connectionHandler, out IFabricServiceCommunicationListener listener));
            return listener;
        }

        internal static IFabricServiceCommunicationClient CreateServiceCommunicationClient(
            ref Guid iid,
            IntPtr transportSettings,
            IntPtr connectionAddress,
            IFabricCommunicationMessageHandler notificationHandler,
            IFabricServiceConnectionEventHandler connectionHandler)
        {
            Marshal.ThrowExceptionForHR(
                PInvoke.CreateServiceCommunicationClient(ref iid, transportSettings, connectionAddress, notificationHandler, connectionHandler, out IFabricServiceCommunicationClient client));
            return client;
        }

        static partial class PInvoke
        {
            const string FabricServiceCommunicationDll = "FabricServiceCommunication";

            [LibraryImport(FabricServiceCommunicationDll)] internal static
#if NET
            partial
#else
            extern
#endif
            HRESULT CreateServiceCommunicationListener(
                ref Guid iid,
                IntPtr transportSettings,
                IntPtr listenerAddress,
                IFabricCommunicationMessageHandler messageHandler,
                IFabricServiceConnectionHandler connectionHandler,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricServiceCommunicationListener>))]
                out IFabricServiceCommunicationListener listener);

            [LibraryImport(FabricServiceCommunicationDll)] internal static
#if NET
            partial
#else
            extern
#endif
            HRESULT CreateServiceCommunicationClient(
                ref Guid iid,
                IntPtr transportSettings,
                IntPtr connectionAddress,
                IFabricCommunicationMessageHandler notificationHandler,
                IFabricServiceConnectionEventHandler connectionHandler,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricServiceCommunicationClient>))]
                out IFabricServiceCommunicationClient client);
        }

        //// ----------------------------------------------------------------------------
        //// Interfaces

        // This interface is implemented but not consumed in managed code.
        // COM outputs aren't used by managed code and don't need special marshalling.
        // COM inputs should be wrapped in unique ComObjects for deterministic release.
        // IFabricAsyncOperationContext inputs are managed and should be unwrapped.
        [GeneratedComInterface]
        [Guid("7e010010-80b2-453c-aab3-a73f0790dfac")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricCommunicationMessageHandler
        {
            IFabricAsyncOperationContext BeginProcessRequest(
                IntPtr clientId,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricServiceCommunicationMessage>))]
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationCallback>))]
                IFabricAsyncOperationCallback callback);

            IFabricServiceCommunicationMessage EndProcessRequest(
                IFabricAsyncOperationContext context);

            void HandleOneWay(
                IntPtr clientId,
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricServiceCommunicationMessage>))]
                IFabricServiceCommunicationMessage message);
        }

        [GeneratedComInterface]
        [Guid("fdf2bcd7-14f9-463f-9b70-ae3b5ff9d83f")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricCommunicationMessageSender
        {
            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricAsyncOperationContext>))]
            IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            [return: MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricServiceCommunicationMessage>))]
            IFabricServiceCommunicationMessage EndRequest(
                IFabricAsyncOperationContext context);

            void SendMessage(
                IFabricServiceCommunicationMessage message);
        }

        [GeneratedComInterface]
        [Guid("60ae1ab3-5f00-404d-8f89-96485c8b013e")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricClientConnection : IFabricCommunicationMessageSender
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            new IFabricServiceCommunicationMessage EndRequest(
                IFabricAsyncOperationContext context);

            new void SendMessage(
                IFabricServiceCommunicationMessage message);
#endif
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
        internal partial interface IFabricServiceConnectionHandler
        {
            IFabricAsyncOperationContext BeginProcessConnect(
                [MarshalUsing(typeof(UniqueComInterfaceMarshaller<IFabricClientConnection>))]
                IFabricClientConnection clientConnection,
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
        [Guid("dc6e168a-dbd4-4ce1-a3dc-5f33494f4972")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceCommunicationMessage
        {
            [PreserveSig]
            IntPtr Get_Body();

            [PreserveSig]
            IntPtr Get_Headers();
        }

        [GeneratedComInterface]
        [Guid("ad5d9f82-d62c-4819-9938-668540248e97")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceCommunicationListener
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
        [Guid("255ecbe8-96b8-4f47-9e2c-1235dba3220a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceCommunicationClient : IFabricCommunicationMessageSender
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            new IFabricServiceCommunicationMessage EndRequest(
                IFabricAsyncOperationContext context);

            new void SendMessage(
                IFabricServiceCommunicationMessage message);
#endif
        }

        [GeneratedComInterface]
        [Guid("73b2cac5-4278-475b-82e6-1e33ebe20767")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceCommunicationClient2 : IFabricServiceCommunicationClient
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                IFabricAsyncOperationCallback callback);

            new IFabricServiceCommunicationMessage EndRequest(
                IFabricAsyncOperationContext context);

            new void SendMessage(
                IFabricServiceCommunicationMessage message);
#endif
            [PreserveSig]
            void Abort();
        }

        [GeneratedComInterface]
        [Guid("77f434b1-f9e9-4cb1-b0c4-c7ea2984aa8d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceConnectionEventHandler
        {
            void OnConnected(
                IntPtr connectionAddress);

            void OnDisconnected(
                IntPtr connectionAddress,
                int errorCode);
        }
    }
}
