// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base
{
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;

    internal class Helper
    {
        public static bool IsCancellationRequest(IServiceRemotingRequestMessageHeader requestMessageHeaders)
        {
            if (requestMessageHeaders.InvocationId != null &&
                requestMessageHeaders.TryGetHeaderValue(
                    ServiceRemotingRequestMessageHeader.CancellationHeaderName,
                    out var headerValue))
            {
                return true;
            }

            return false;
        }
    }
}
