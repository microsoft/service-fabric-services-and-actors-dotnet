// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// 
    /// </summary>
    public interface IServiceRemotingClient : ICommunicationClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestRequestMessage"></param>
        /// <returns></returns>
        Task<IServiceRemotingResponseMessage> RequestResponseAsync(IServiceRemotingRequestMessage requestRequestMessage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMessage"></param>
        void SendOneWay(IServiceRemotingRequestMessage requestMessage);
    }
}
