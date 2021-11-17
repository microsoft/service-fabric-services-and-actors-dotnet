// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using Microsoft.ServiceFabric.Services.Communication;

    /// <summary>
    /// Defines an interface to convert user exception to ServiceException.
    /// </summary>
    public interface IExceptionConvertor
    {
        /// <summary>
        /// Converts user exception to ServiceException.
        /// </summary>
        /// <param name="originalException">Actual exception observed.</param>
        /// <param name="serviceException">ServiceException converted from actual exception.</param>
        /// <returns>True if the exception is handled, false otherwise.</returns>
        bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException);
    }
}
