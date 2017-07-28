// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System;

    /// <summary>
    /// Represents the base Client side interface for Remoting. The framework provides the
    /// Remoting infrastructure for all the service contracts inheriting from IService through
    /// ServiceRemotingListener and ServiceProxy.
    /// </summary>
    public interface IServiceProxy
    {
        /// <summary>
        /// Gets the interface type that is being remoted.
        /// </summary>
        /// <value>The Service interface type.</value>
        Type ServiceInterfaceType { get; }

        /// <summary>
        /// Gets the service partition client used to send requests to the service.
        /// </summary>
        /// <value>The ServicePartitionClient used by the ServiceProxy.</value>
         IServiceRemotingPartitionClient ServicePartitionClient { get; }
    }
}
