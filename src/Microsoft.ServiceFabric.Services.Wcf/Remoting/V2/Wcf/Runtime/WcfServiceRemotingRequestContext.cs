// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    using System.ServiceModel;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

    internal sealed class WcfServiceRemotingRequestContext : IServiceRemotingRequestContext
    {
        private ServiceRemotingMessageSerializersManager serializerManager;

        public WcfServiceRemotingRequestContext(ServiceRemotingMessageSerializersManager serializerManager)
        {
            this.serializerManager = serializerManager;
        }

        public IServiceRemotingCallbackClient GetCallBackClient()
        {
            return new WcfServiceRemotingCallbackClient(
                OperationContext.Current.GetCallbackChannel<IServiceRemotingCallbackContract>(),
                this.serializerManager
                );
        }
    }
}