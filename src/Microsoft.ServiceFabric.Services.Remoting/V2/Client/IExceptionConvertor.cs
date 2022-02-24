// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System;
    using Microsoft.ServiceFabric.Services.Communication;

    /// <summary>
    /// Defines an interface to convert service exception to user exception.
    /// </summary>
    public interface IExceptionConvertor
    {
        /// <summary>
        /// Converts the service exception to user exception.
        /// </summary>
        /// <param name="serviceException">Service exception.</param>
        /// <param name="actualException">User exception.</param>
        /// <returns>True if converted, false otherwise.</returns>
        bool TryConvertFromServiceException(ServiceException serviceException, out Exception actualException);

        /// <summary>
        /// Converts the service exception to user exception.
        /// </summary>
        /// <param name="serviceException">Service exception.</param>
        /// <param name="innerException">Inner exception.</param>
        /// <param name="actualException">User exception.</param>
        /// <returns>True if converted, false otherwise.</returns>
        bool TryConvertFromServiceException(ServiceException serviceException, Exception innerException, out Exception actualException);

        /// <summary>
        /// Converts the service exception to user exception.
        /// </summary>
        /// <param name="serviceException">Service exception.</param>
        /// <param name="innerExceptions">List to inner exception.</param>
        /// <param name="actualException">User exception.</param>
        /// <returns>True if converted, false otherwise.</returns>
        bool TryConvertFromServiceException(ServiceException serviceException, Exception[] innerExceptions, out Exception actualException);
    }
}
