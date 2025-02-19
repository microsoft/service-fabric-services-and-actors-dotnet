// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Runtime
{
    /// <summary>
    /// Defines the interface that must be implemented to provide the request context for the IServiceRemotingMessageHandler.
    /// </summary>
    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    public interface IServiceRemotingRequestContext
    {
        /// <summary>
        /// Retrieves the client channel interface to use in cases where service wants to initiate calls to the client.
        /// </summary>
        /// <returns>The remoting callback client.</returns>
        IServiceRemotingCallbackClient GetCallbackClient();
    }
}
