// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    /// <summary>
    /// Determines the remoting stack for client
    /// </summary>
    public enum RemotingClientVersion
    {
        /// <summary>
        /// This is selected to create V2 Client. V2 is a new Remoting Stack.
        /// </summary>
        V2 = 2,

        /// <summary>
        /// This is selected to create  Client using WrapMessage for the parameters. This version is interface compatible to V1 interface.
        /// </summary>
        V2_1 = 4,
    }
}
