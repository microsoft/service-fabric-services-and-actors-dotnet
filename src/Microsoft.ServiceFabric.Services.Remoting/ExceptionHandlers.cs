using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class ExceptionHandler : IExceptionHandler
    {
        bool IExceptionHandler.TryHandleException(
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            var fabricException = exceptionInformation.Exception as FabricException;
            if (fabricException != null)
            {
                return TryHandleFabricException(
                    fabricException,
                    retrySettings,
                    out result);
            }

            result = null;
            return false;
        }

        private static bool TryHandleFabricException(
            FabricException fabricException,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            if ((fabricException is FabricCannotConnectException) ||
                (fabricException is FabricEndpointNotFoundException)
                )
            {
                result = new ExceptionHandlingRetryResult(
                    fabricException,
                    false,
                    retrySettings,
                    int.MaxValue);
                return true;
            }

            if (fabricException.ErrorCode.Equals(FabricErrorCode.ServiceTooBusy))
            {
                result = new ExceptionHandlingRetryResult(
                    fabricException,
                    true,
                    retrySettings,
                    int.MaxValue);
                return true;
            }

            result = null;
            return false;
        }
    }
}
