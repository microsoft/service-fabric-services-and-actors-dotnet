// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    using System;

    /// <summary>
    /// Determines the remoting stack for server/listener when using remoting provider attribuite to determine the remoting client.
    /// </summary>
    [Flags]
    public enum RemotingListenerVersion
    {
#if !DotNetCoreClr
        /// <summary>
        /// This is selected to create V1 Listener.V1 is an old (soon to be deprecated) Remoting Stack.
        /// </summary>
        [Obsolete("This is an old version of remoting stack and will be removed in future versions.")]
        V1 = 1,

#endif

        /// <summary>
        /// This is selected to create V2 Listener.V2 is a new Remoting Stack.
        /// </summary>
        V2 = 2,

        /// <summary>
            /// This is selected to create Listener using Wrap Message for the parameters.This version is interface compatible to V1 interface.
            /// </summary>
        V2_1 = 4,
    }
}
