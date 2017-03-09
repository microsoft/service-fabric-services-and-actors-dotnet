// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    /// <summary>
    /// Defines the interface for handling the exceptions encountered in communicating with service fabric services. 
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Method that examines the exception and determines how that exception can be handled. 
        /// </summary>
        /// <param name="exceptionInformation">Information about the exception</param>
        /// <param name="retrySettings">The operation retry preferences.</param>
        /// <param name="result">Result of the exception handling</param>
        /// <returns>true if the exception is handled, false otherwise</returns>
        bool TryHandleException(
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings, 
            out ExceptionHandlingResult result);
    }
}