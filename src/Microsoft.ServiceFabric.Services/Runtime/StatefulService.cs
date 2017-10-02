// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Data;

    /// <summary>
    /// Represents the base class for Microsoft Service Fabric based stateful reliable service
    /// which provides an <see cref="IReliableStateManager"/> to manage service's state. 
    /// Derive from this class to implement a Microsoft Service Fabric based stateful reliable service.
    /// </summary>
    public abstract class StatefulService : StatefulServiceBase
    {
        private readonly IReliableStateManager stateManager;

        /// <summary>
        /// Gets this replica's <see cref="IReliableStateManager"/>.
        /// </summary>
        /// <value>The <see cref="IReliableStateManager"/> of the replica.</value>
        public IReliableStateManager StateManager
        {
            get { return this.stateManager; }
        }

        /// <summary>
        /// Creates a new <see cref="StatefulService"/> with default <see cref="IReliableStateManager"/>: <see cref="ReliableStateManager"/>.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> describes the stateful service context, which it provides information like replica ID, partition ID, and service name.
        /// </param>
        protected StatefulService(StatefulServiceContext serviceContext)
            : this(serviceContext, new ReliableStateManager(serviceContext))
        {
        }

        /// <summary>
        /// Creates a new stateful service. 
        /// Override this method to create a new stateful service with non-default state manager replica.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> describes the stateful service context, which it provides information like replica ID, partition ID, and service name.
        /// </param>
        /// <param name="reliableStateManagerReplica">
        /// A <see cref="IReliableStateManagerReplica2"/> represents a reliable state provider replica.
        /// </param>
        protected StatefulService(StatefulServiceContext serviceContext, IReliableStateManagerReplica2 reliableStateManagerReplica)
            : base(serviceContext, reliableStateManagerReplica)
        {
            this.stateManager = reliableStateManagerReplica;
        }
    }
}