// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Services.Communication;

    internal class ExceptionConvertorHelper
    {
        private IList<IExceptionConvertor> convertors;

        public Exception FromServiceException(ServiceException serviceException)
        {
            List<Exception> innerExceptions = new List<Exception>();
            if (serviceException.ActualInnerExceptions != null && serviceException.ActualInnerExceptions.Count > 0)
            {
                foreach (var inner in serviceException.ActualInnerExceptions)
                {
                    innerExceptions.Add(this.FromServiceException(inner));
                }
            }

            Exception actualEx = null;
            foreach (var convertor in this.convertors)
            {
                try
                {
                    if (innerExceptions.Count == 0)
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, out actualEx))
                        {
                            break;
                        }
                    }
                    else if (innerExceptions.Count == 1)
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, innerExceptions[0], out actualEx))
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (convertor.TryConvertFromServiceException(serviceException, innerExceptions.ToArray(), out actualEx))
                        {
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // Throw
                }
            }

            return actualEx != null ? actualEx : serviceException;
        }
    }
}
