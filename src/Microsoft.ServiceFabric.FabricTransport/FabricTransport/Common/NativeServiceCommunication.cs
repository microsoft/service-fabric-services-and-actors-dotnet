// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.FabricTransport
{
    using System;
    using System.Fabric.Interop;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using HRESULT = System.Int32;
#if NET
    using System.Runtime.InteropServices.Marshalling;
#else
    using GeneratedComInterfaceAttribute = System.Runtime.InteropServices.ComImportAttribute;
    using LibraryImportAttribute = System.Runtime.InteropServices.DllImportAttribute;
#endif

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
                out IFabricServiceCommunicationClient client);
        }

        //// ----------------------------------------------------------------------------
        //// Interfaces
        ///     
        [GeneratedComInterface]
        [Guid("7e010010-80b2-453c-aab3-a73f0790dfac")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricCommunicationMessageHandler
        {
            NativeCommon.IFabricAsyncOperationContext BeginProcessRequest(
                IntPtr clientId,
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            IFabricServiceCommunicationMessage EndProcessRequest(
                NativeCommon.IFabricAsyncOperationContext context);

            void HandleOneWay(
                IntPtr clientId,
                IFabricServiceCommunicationMessage message);
        }

        [GeneratedComInterface]
        [Guid("fdf2bcd7-14f9-463f-9b70-ae3b5ff9d83f")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricCommunicationMessageSender
        {
            NativeCommon.IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            IFabricServiceCommunicationMessage EndRequest(
                NativeCommon.IFabricAsyncOperationContext context);

            void SendMessage(
                IFabricServiceCommunicationMessage message);
        }

        [GeneratedComInterface]
        [Guid("60ae1ab3-5f00-404d-8f89-96485c8b013e")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricClientConnection : IFabricCommunicationMessageSender
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new NativeCommon.IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            new IFabricServiceCommunicationMessage EndRequest(
                NativeCommon.IFabricAsyncOperationContext context);

            new void SendMessage(
                IFabricServiceCommunicationMessage message);
#endif
            [PreserveSig]
            IntPtr get_ClientId();
        }

        [GeneratedComInterface]
        [Guid("b069692d-e8f0-4f25-a3b6-b2992598a64c")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceConnectionHandler
        {
            NativeCommon.IFabricAsyncOperationContext BeginProcessConnect(
                IFabricClientConnection clientConnection,
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
        [Guid("255ecbe8-96b8-4f47-9e2c-1235dba3220a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal partial interface IFabricServiceCommunicationClient : IFabricCommunicationMessageSender
        {
#if NETFRAMEWORK // Base methods must be redefined. Legacy NetFx interop doesn't support COM interface inheritance.
            new NativeCommon.IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            new IFabricServiceCommunicationMessage EndRequest(
                NativeCommon.IFabricAsyncOperationContext context);

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
            new NativeCommon.IFabricAsyncOperationContext BeginRequest(
                IFabricServiceCommunicationMessage message,
                uint timeoutMilliseconds,
                NativeCommon.IFabricAsyncOperationCallback callback);

            new IFabricServiceCommunicationMessage EndRequest(
                NativeCommon.IFabricAsyncOperationContext context);

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