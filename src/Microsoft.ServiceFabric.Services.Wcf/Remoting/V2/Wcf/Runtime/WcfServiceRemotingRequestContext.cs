// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    using System.ServiceModel;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal sealed class WcfServiceRemotingRequestContext : IServiceRemotingRequestContext
    {
        private ServiceRemotingMessageSerializationManager serializerManager;

        public WcfServiceRemotingRequestContext(ServiceRemotingMessageSerializationManager serializerManager)
        {
            this.serializerManager = serializerManager;
        }

        public IServiceRemotingCallbackClient GetCallBackClient()
        {
            return new WcfServiceRemotingCallbackClient(
                OperationContext.Current.GetCallbackChannel<IServiceRemotingCallbackContract>(),
                this.serializerManager);
        }
    }
}
