// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;

    /// <summary>
    ///  Defines the interface that must be implemented for providing callback mechanism
    ///  from the wcf remoting listener to the client.
    /// </summary>
    [ServiceContract(Namespace = WcfConstants.Namespace)]
    public interface IServiceRemotingCallbackContract
    {

        /// <summary>
        /// Sends a one way message to the client.
        /// </summary>
        /// <param name="messageHeaders">Message Headers contains the information needed to deserialize request and to dispatch message to the client.</param>
        /// <param name="requestBody"> Message Body contains a request in a serialized form.</param>
        [OperationContract(IsOneWay = true)]
        void SendOneWay(ArraySegment<byte> messageHeaders, IEnumerable<ArraySegment<byte>> requestBody);
    }
}
