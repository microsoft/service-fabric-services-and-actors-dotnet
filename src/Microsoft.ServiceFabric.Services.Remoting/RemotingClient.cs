// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    /// <summary>
    /// Determines the remoting stack for client
    /// </summary>
    public enum RemotingClient
    {
#if !DotNetCoreClr
        /// <summary>
        /// This is selected to create V1 Client. V1 is an old(soon to be deprecated) Remoting Stack.
        /// </summary>
        V1Client,
#endif

        /// <summary>
        /// This is selected to create V2 Client. V2 is a new Remoting Stack.
        /// </summary>
        V2Client,
    }
}
