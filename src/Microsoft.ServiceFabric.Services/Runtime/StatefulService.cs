// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
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
        /// Initializes a new instance of the <see cref="StatefulService"/> class with default reliable state manager (<see cref="ReliableStateManager"/>).
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> describes the stateful service context, which it provides information like replica ID, partition ID, and service name.
        /// </param>
        protected StatefulService(StatefulServiceContext serviceContext)
            : this(serviceContext, new ReliableStateManager(serviceContext))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulService"/> class with non-default reliable state manager replica.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> describes the stateful service context, which it provides information like replica ID, partition ID, and service name.
        /// </param>
        /// <param name="reliableStateManagerReplica">
        /// A <see cref="IReliableStateManagerReplica"/> represents a reliable state provider replica.
        /// </param>
        protected StatefulService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica)
            : base(serviceContext, reliableStateManagerReplica)
        {
            this.stateManager = reliableStateManagerReplica;
        }

        /// <summary>
        /// Gets this replica's <see cref="IReliableStateManager"/>.
        /// </summary>
        /// <value>The <see cref="IReliableStateManager"/> of the replica.</value>
        public IReliableStateManager StateManager
        {
            get { return this.stateManager; }
        }
    }
}
