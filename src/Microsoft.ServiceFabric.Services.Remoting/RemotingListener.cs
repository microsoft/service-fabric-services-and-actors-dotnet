// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    /// <summary>
    /// Determines the remoting stack for server/listener when using remoting provider attribuite to determine the remoting client.
    /// </summary>
    public enum RemotingListener
    {
#if !DotNetCoreClr

        /// <summary>
        /// This is selected to create V1 Listener.V1 is an old (soon to be deprecated) Remoting Stack.
        /// </summary>
        V1Listener,

        /// <summary>
        /// This is selected to create Listener which creates both V1 and V2 Listener to support both V1 and V2 Clients.
        /// This is useful in case of upgrade from V1 to V2 Listener.
        /// </summary>
        CompatListener,
#endif

        /// <summary>
        /// This is selected to create V2 Listener.V2 is a new Remoting Stack.
        /// </summary>
        V2Listener,
    }
}
