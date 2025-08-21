// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    sealed class DefaultExceptionConvertor : IExceptionConvertor
    {
        public Exception[] GetInnerExceptions(Exception exception)
        {
            return exception.InnerException == null ? null : new Exception[] { exception.InnerException };
        }

        public bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
        {
            serviceException = new ServiceException(originalException.GetType().FullName, originalException.Message);
            serviceException.ActualExceptionStackTrace = originalException.StackTrace;
            serviceException.ActualExceptionData = new Dictionary<string, string>()
            {
                { "HResult", originalException.HResult.ToString() },
            };

            if (originalException is FabricException fabricEx)
                serviceException.ActualExceptionData.Add("FabricErrorCode", ((long)fabricEx.ErrorCode).ToString());

            return true;
        }
    }
}
