// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Represents Microsoft Service Fabric based stateless reliable service base class. 
    /// Derive from this class to implement a Microsoft Service Fabric based stateless reliable service.
    /// </summary>
    public abstract class StatelessService : IStatelessUserServiceInstance
    {
        private readonly StatelessServiceContext serviceContext;

        private IReadOnlyDictionary<string, string> addresses;

        /// <summary>
        /// Creates a new <see cref="StatelessService"/> instance.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatelessServiceContext"/> that describes the service context.
        /// </param>
        protected StatelessService(StatelessServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
            this.addresses = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        /// <summary>
        /// Gets list of all the addresses for this service instance
        /// as (ListenerName, Endpoint) key-value pair.
        /// </summary>
        /// <returns>
        /// An <see cref="IReadOnlyDictionary{k,v}"/> containing list of addresses as
        /// (ListenerName, Endpoint) key-value pair.
        /// </returns>
        protected IReadOnlyDictionary<string, string> GetAddresses()
        {
            return this.addresses;
        }

        /// <summary>
        /// Gets the service context that this stateless service is operating under. It provides
        /// information like InstanceId, PartitionId, ServiceName etc.
        /// </summary>
        /// <value>
        /// A <see cref="StatelessServiceContext"/> that describes the service context.
        /// </value>
        public StatelessServiceContext Context
        {
            get { return this.serviceContext; }
        }
        
        /// <summary>
        /// Service partition to which current service instance belongs. 
        /// </summary>
        /// <value>
        /// An <see cref="IStatelessServicePartition"/> that represents the 
        /// partition to which this service replica belongs.
        /// </value>
        protected IStatelessServicePartition Partition { get; private set; }

        /// <summary>
        /// Override this method to supply the communication listeners for the service instance. The endpoints returned by the communication listener's
        /// are stored as a JSON string of ListenerName, Endpoint string pairs like 
        /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}
        /// </summary>
        /// <returns>List of ServiceInstanceListeners</returns>
        protected virtual IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return Enumerable.Empty<ServiceInstanceListener>();
        }

        /// <summary>
        /// This method is called as the final step of opening the service.
        /// Override this method to be notified that Open has completed for this instance's internal components.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected virtual Task OnOpenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Services that want to implement a background task which runs when it is opened,
        /// just override this method with their logic.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected virtual Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is called as the final step of closing the service.
        /// Override this method to be notified that Close has completed for this instance's internal components.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token provided to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected virtual Task OnCloseAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Notification that the service is being aborted.  RunAsync MAY be running concurrently
        /// with the execution of this method, as cancellation is not awaited on the abort path.
        /// </summary>
        protected virtual void OnAbort()
        {
        }
        
        IReadOnlyDictionary<string, string> IStatelessUserServiceInstance.Addresses
        {
            set { Volatile.Write(ref this.addresses, value); }
        }

        IStatelessServicePartition IStatelessUserServiceInstance.Partition
        {
            set { this.Partition = value; }
        }

        IEnumerable<ServiceInstanceListener> IStatelessUserServiceInstance.CreateServiceInstanceListeners()
        {
            return this.CreateServiceInstanceListeners();
        }

        Task IStatelessUserServiceInstance.OnOpenAsync(CancellationToken cancellationToken)
        {
            return this.OnOpenAsync(cancellationToken);
        }

        Task IStatelessUserServiceInstance.RunAsync(CancellationToken cancellationToken)
        {
            return this.RunAsync(cancellationToken);
        }

        Task IStatelessUserServiceInstance.OnCloseAsync(CancellationToken cancellationToken)
        {
            return this.OnCloseAsync(cancellationToken);
        }

        void IStatelessUserServiceInstance.OnAbort()
        {
            this.OnAbort();
        }
    }
}