// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// 
    /// </summary>
    public interface IServiceRemotingClientFactory : ICommunicationClientFactory<IServiceRemotingClient>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory();
    }
}