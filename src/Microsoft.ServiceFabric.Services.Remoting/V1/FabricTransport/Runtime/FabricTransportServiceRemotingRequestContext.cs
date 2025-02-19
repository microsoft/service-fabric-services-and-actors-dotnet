// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime
{
    using System;
    using System.Globalization;
    using Microsoft.ServiceFabric.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Runtime;

    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    internal class FabricTransportServiceRemotingRequestContext : IServiceRemotingRequestContext
    {
        private readonly FabricTransportRequestContext requestContext;
        private readonly string id;
        private IServiceRemotingCallbackClient callback = null;

        public FabricTransportServiceRemotingRequestContext(FabricTransportRequestContext requestContext)
        {
            this.requestContext = requestContext;
            this.id = requestContext.ClientId;
        }

        public IServiceRemotingCallbackClient GetCallbackClient()
        {
            if (this.callback == null)
            {
                var nativeCallback = this.requestContext.GetCallbackClient();
                this.callback = new FabricTransportServiceRemotingCallback(nativeCallback);
            }

            if (this.callback == null)
            {
                throw new FabricTransportCallbackNotFoundException(string.Format(
                    CultureInfo.CurrentCulture,
                    SR.ErrorClientCallbackChannelNotFound,
                    this.id));
            }

            return this.callback;
        }
    }
}
