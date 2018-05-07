// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Defines a base communication interface that enables interface remoting for
    /// stateless and stateful services.
    /// </summary>
    public interface IServiceRemotingListener : ICommunicationListener
    {
    }
}
