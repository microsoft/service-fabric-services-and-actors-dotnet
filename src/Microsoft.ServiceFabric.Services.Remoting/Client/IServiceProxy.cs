// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System;

    /// <summary>
    /// This is the base Client side interface for Remoting. The framework provides the
    /// Remoting infrastructure for all the service contracts inheriting from IService through
    /// ServiceRemotingListener and ServiceProxy.
    /// </summary>
    public interface IServiceProxy
    {
        /// <summary>
        /// Gets the interface type that is being remoted.
        /// </summary>
        /// <value>Service interface type.</value>
        Type ServiceInterfaceType { get; }

#if !DotNetCoreClr
        /// <summary>
        /// Gets the service partition client used to send requests to the service.
        /// </summary>
        /// <value>ServicePartitionClient used by the ServiceProxy.</value>
        Microsoft.ServiceFabric.Services.Remoting.V1.Client.IServiceRemotingPartitionClient ServicePartitionClient { get; }
#endif

        /// <summary>
        /// Gets the service partition client used to send requests to the service.
        /// </summary>
        /// <value>ServicePartitionClient used by the ServiceProxy.</value>
        Microsoft.ServiceFabric.Services.Remoting.V2.Client.IServiceRemotingPartitionClient ServicePartitionClient2 { get; }
    }
}
