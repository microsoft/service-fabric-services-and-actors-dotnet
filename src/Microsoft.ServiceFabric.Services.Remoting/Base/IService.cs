// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base
{
    using Microsoft.ServiceFabric.Services.Remoting.Base.Runtime;

    /// <summary>
    /// Defines the marker interface for enabling interface Remoting for services.
    /// An <see cref="IServiceRemotingListener"/> provides remoting
    /// for all interfaces implemented by the service that derive from this interface. The remoted interfaces can be
    /// accessed via IServiceProxy on the client side.
    /// </summary>
    public interface IService
    {
    }
}
