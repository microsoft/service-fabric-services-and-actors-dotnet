// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Wcf.Runtime
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceFabric.Services.Remoting.V1;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Runtime;

    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    internal sealed class WcfServiceRemotingRequestContext : IServiceRemotingRequestContext
    {
        public IServiceRemotingCallbackClient GetCallbackClient()
        {
            return new WcfServiceRemotingCommunicationCallback(
                OperationContext.Current.GetCallbackChannel<IServiceRemotingCallbackContract>());
        }
    }
}
