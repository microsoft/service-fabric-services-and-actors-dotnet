// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;
    using Microsoft.ServiceFabric.Services.Remoting.V1;

    /// <summary>
    ///  Defines the interface that must be implemented for providing callback mechanism
    ///  from the wcf remoting listener to the client.
    /// </summary>
    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    [ServiceContract(Namespace = WcfConstants.Namespace)]
    public interface IServiceRemotingCallbackContract
    {
        /// <summary>
        ///  Sends a message to the client and gets the response.
        /// </summary>
        /// <param name="messageHeaders">Message Headers contains the information needed to deserialize request and to dispatch message to the client.</param>
        /// <param name="requestBody"> Message Body contains a request in a serialized form.</param>
        /// <returns>Response Body is a serialized response received by the service.</returns>
#pragma warning disable 108
        [OperationContract]
        [FaultContract(typeof(RemoteExceptionInformation))]
        Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody);

        /// <summary>
        /// Sends a one way message to the client.
        /// </summary>
        /// <param name="messageHeaders">Message Headers contains the information needed to deserialize request and to dispatch message to the client.</param>
        /// <param name="requestBody"> Message Body contains a request in a serialized form.</param>
        [OperationContract(IsOneWay = true)]
        void SendOneWay(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody);
    }
}
