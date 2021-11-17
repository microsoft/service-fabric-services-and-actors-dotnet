// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Runtime.InteropServices;
    using Microsoft.ServiceFabric.Services.Communication;

    /// <summary>
    /// Default convertor for FabricExceptions.
    /// </summary>
    internal class FabricExceptionConvertor : ExceptionConvertorBase
    {
        private const string HResultField = "HResult";
        private const string FabricErrorCodeField = "FabricErrorCode";

        public FabricExceptionConvertor(IList<IExceptionConvertor> convertors)
            : base(convertors)
        {
        }

        public override IList<Exception> GetInnerExceptions(Exception originalException)
        {
            return originalException.InnerException != null ? new List<Exception>() { originalException.InnerException } : null;
        }

        public override ServiceException ToServiceException(Exception originalException)
        {
            var fabricEx = originalException as FabricException;
            var serviceException = new ServiceException(fabricEx.GetType().ToString(), fabricEx.Message);
            serviceException.ActualExceptionStackTrace = fabricEx.StackTrace;
            serviceException.ActualExceptionData = new Dictionary<string, string>()
            {
                { HResultField, fabricEx.HResult.ToString() },
                { FabricErrorCodeField, fabricEx.ErrorCode.ToString() },
            };

            return serviceException;
        }

        public override bool IsKnownType(Exception originalException)
        {
            return originalException is FabricException;
        }
    }
}
