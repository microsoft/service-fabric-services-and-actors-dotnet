// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Runtime
{
    using System.ServiceModel;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Runtime;

    internal sealed class WcfServiceRemotingRequestContext : IServiceRemotingRequestContext
    {
        public IServiceRemotingCallbackClient GetCallbackClient()
        {
            return new WcfServiceRemotingCommunicationCallback(
                OperationContext.Current.GetCallbackChannel<IServiceRemotingCallbackContract>());
        }
    }
}
