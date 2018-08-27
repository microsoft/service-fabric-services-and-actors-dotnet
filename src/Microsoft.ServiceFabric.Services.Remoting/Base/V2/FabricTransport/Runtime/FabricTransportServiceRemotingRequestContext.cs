// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.FabricTransport.Runtime
{
    using System.Globalization;
    using Microsoft.ServiceFabric.FabricTransport.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Runtime;

    internal class FabricTransportServiceRemotingRequestContext : IServiceRemotingRequestContext
    {
        private readonly FabricTransportRequestContext requestContext;
        private readonly ServiceRemotingMessageSerializationManager serializersManager;
        private string id;
        private IServiceRemotingCallbackClient callback = null;

        public FabricTransportServiceRemotingRequestContext(
            FabricTransportRequestContext requestContext,
            ServiceRemotingMessageSerializationManager serializersManager)
        {
            this.requestContext = requestContext;
            this.serializersManager = serializersManager;
            this.id = requestContext.ClientId;
        }

        public IServiceRemotingCallbackClient GetCallBackClient()
        {
            if (this.callback == null)
            {
                var nativeCallback = this.requestContext.GetCallbackClient();
                this.callback = new FabricTransportServiceRemotingCallbackClient(nativeCallback, this.serializersManager);
            }

            if (this.callback == null)
            {
                throw new FabricTransportCallbackNotFoundException(string.Format(CultureInfo.CurrentCulture, SR.ErrorClientCallbackChannelNotFound, this.id));
            }

            return this.callback;
        }
    }
}
