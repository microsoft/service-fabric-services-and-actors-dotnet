// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;

    internal class WcfAsyncThreadExceptionHandler : ExceptionHandler
    {
        private readonly string traceType = "WcfAsyncThreadExceptionHandler";

        public override bool HandleException(Exception exception)
        {
            ServiceTrace.Source.WriteWarning(
                this.traceType,
                "Exception {0}  Stacktrace {1} occured in Wcf Service Background Threads",
                exception.Message,
                exception.StackTrace);

            if (exception is FaultException)
            {
                return false;
            }

            return true;
        }
    }
}
