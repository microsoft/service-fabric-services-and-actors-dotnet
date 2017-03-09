// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Wcf
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;
    /// <summary>
    ///    Interface that defines the contract for WCF service remoting.
    /// </summary>
    [ServiceContract(
        Namespace = WcfConstants.Namespace, 
        CallbackContract = typeof(IServiceRemotingCallbackContract))]
    public interface IServiceRemotingContract
    {
        /// <summary>
        ///    Sends a message to the client and gets the response.
        /// </summary>
        /// <param name="messageHeaders">Message Headers contains the information needed to deserialize request and to dispatch message to the service.</param>
        /// <param name="requestBody"> Message Body contains a request in a serialized form.</param>
        /// <returns>Response Body is a serialized response recived by the client</returns>
        #pragma warning disable 108
        [OperationContract]
        [FaultContract(typeof(RemoteExceptionInformation))]
        Task<byte[]> RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody);

        /// <summary>
        ///    Sends a one way message to the client.
        /// </summary>
        /// <param name="messageHeaders">Message Headers contains the information needed to deserialize request and to dispatch message to the service.</param>
        /// <param name="requestBody"> Message Body contains a serialized message</param>
        [OperationContract(IsOneWay = true)]
        void OneWayMessage(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody);

    }
}