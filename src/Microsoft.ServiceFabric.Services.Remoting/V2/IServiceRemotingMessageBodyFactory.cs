// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    /// <summary>
    /// Defines the interface that must be implemented for providing factory for creating remtoing request body and response body objects.
    /// </summary>
    public interface IServiceRemotingMessageBodyFactory
    {
        /// <summary>
        /// Creates a Remoting Request Body
        /// </summary>
        /// <param name="interfaceName"> This is FullName for the service interface for which request body is being constructed</param>
        /// <param name="methodName">MethodName for the service interface for which request will be sent to</param>
        /// <param name="numberOfParameters">Number of Parameters in that Method</param>
        /// <returns>IServiceRemotingRequestMessageBody</returns>
        IServiceRemotingRequestMessageBody CreateRequest(string interfaceName, string methodName, int numberOfParameters);

        /// <summary>
        /// 
        ///</summary> 
        /// <param name="interfaceName"> This is FullName for the service interface for which request body is being constructed</param>
        /// <param name="methodName">MethodName for the service interface for which request will be sent to</param>
        /// <returns>IServiceRemotingResponseMessageBody</returns>
        IServiceRemotingResponseMessageBody CreateResponse(string interfaceName, string methodName);
    }
}
