// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.FabricTransport.V2.Runtime
{
    internal interface IFabricTransportConnectionHandler
    {
        Task ConnectAsync(FabricTransportCallbackClient fabricTransportServiceRemotingCallback, TimeSpan timeout);

        Task DisconnectAsync(string clientId, TimeSpan timeout);

        FabricTransportCallbackClient GetCallBack(string clientId);
    }
}
