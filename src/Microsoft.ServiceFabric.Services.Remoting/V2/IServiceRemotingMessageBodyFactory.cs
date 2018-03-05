// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    /// <summary>
    /// Defines the interface that must be implemented for providing factory for creating remtoing request body and response body objects.
    /// </summary>
    public interface IServiceRemotingMessageBodyFactory
    {
        /// <summary>
        /// Creates a remoting request message body.
        /// </summary>
        /// <param name="interfaceName"> This is FullName for the service interface for which request body is being constructed</param>
        /// <param name="methodName">MethodName for the service interface for which request will be sent to</param>
        /// <param name="numberOfParameters">Number of Parameters in that Method</param>
        /// <returns>An <see cref="IServiceRemotingRequestMessageBody"/>.</returns>
        IServiceRemotingRequestMessageBody CreateRequest(string interfaceName, string methodName, int numberOfParameters);

        /// <summary>
        /// Creates a remoting response message body.
        /// </summary>
        /// <param name="interfaceName"> This is FullName for the service interface for which request body is being constructed</param>
        /// <param name="methodName">MethodName for the service interface for which request will be sent to</param>
        /// <returns>IServiceRemotingResponseMessageBody</returns>
        /// <returns>An <see cref="IServiceRemotingResponseMessageBody"/>.</returns>
        IServiceRemotingResponseMessageBody CreateResponse(string interfaceName, string methodName);
    }
}
