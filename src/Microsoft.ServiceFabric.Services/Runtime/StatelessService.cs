// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Represents the Microsoft Service Fabric based stateless reliable service base class.
    /// Derive from this class to implement a Microsoft Service Fabric based stateless reliable service.
    /// </summary>
    public abstract class StatelessService : IStatelessUserServiceInstance
    {
        private readonly StatelessServiceContext serviceContext;

        private IReadOnlyDictionary<string, string> addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatelessService"/> class.
        /// </summary>
        /// <param name="serviceContext">A <see cref="StatelessServiceContext"/> that describes the context in which service is created.</param>
        protected StatelessService(StatelessServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            this.serviceContext = serviceContext;
            this.addresses = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

            ServiceTelemetry.StatelessServiceInitializeEvent(this.serviceContext);
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

        /// <inheritdoc/>
        IReadOnlyDictionary<string, string> IStatelessUserServiceInstance.Addresses
        {
            set { Volatile.Write(ref this.addresses, value); }
        }

        /// <inheritdoc/>
        IStatelessServicePartition IStatelessUserServiceInstance.Partition
        {
            set { this.Partition = value; }
        }

        /// <summary>
        /// Gets service partition that this service instance belongs to.
        /// </summary>
        /// <value>
        /// An <see cref="IStatelessServicePartition"/> that represents the
        /// partition to which this service replica belongs.
        /// </value>
        protected IStatelessServicePartition Partition { get; private set; }

        /// <inheritdoc/>
        IEnumerable<ServiceInstanceListener> IStatelessUserServiceInstance.CreateServiceInstanceListeners()
        {
            return this.CreateServiceInstanceListeners();
        }

        /// <inheritdoc/>
        Task IStatelessUserServiceInstance.OnOpenAsync(CancellationToken cancellationToken)
        {
            return this.OnOpenAsync(cancellationToken);
        }

        /// <inheritdoc/>
        Task IStatelessUserServiceInstance.RunAsync(CancellationToken cancellationToken)
        {
            return this.RunAsync(cancellationToken);
        }

        /// <inheritdoc/>
        Task IStatelessUserServiceInstance.OnCloseAsync(CancellationToken cancellationToken)
        {
            ServiceTelemetry.StatelessServiceInstanceCloseEvent(this.serviceContext);
            return this.OnCloseAsync(cancellationToken);
        }

        /// <inheritdoc/>
        void IStatelessUserServiceInstance.OnAbort()
        {
            this.OnAbort();
        }

        /// <summary>
        /// Gets the list of all the addresses for this service instance
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
        /// Override this method to supply the communication listeners for the service instance. The endpoints returned by the communication listener's
        /// are stored as a JSON string of ListenerName, Endpoint string pairs like
        /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle
        /// </para>
        /// </summary>
        /// <returns>List of ServiceInstanceListeners</returns>
        protected virtual IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return Enumerable.Empty<ServiceInstanceListener>();
        }

        /// <summary>
        /// This method is called as the final step of opening the service.
        /// Override this method to be notified that Open has completed for this instance's internal components.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle
        /// </para>
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
        /// Services that want to implement a background task, which runs when the service comes up,
        /// should override this method with their logic.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        /// <remarks>
        /// Please ensure you follow these guidelines when overriding <see cref="RunAsync"/>:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///         Make sure <paramref name="cancellationToken"/> passed to <see cref="RunAsync"/> is honored and once
        ///         it has been signaled, <see cref="RunAsync"/> exits gracefully as soon as possible. Please note that
        ///         if <see cref="RunAsync"/> has finished its intended work, it does not need to wait for
        ///         <paramref name="cancellationToken"/> to be signaled and can return gracefully.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///         Service Fabric runtime does not handle all exception(s) escaping from <see cref="RunAsync"/>. If an unhandled
        ///         exception escapes from <see cref="RunAsync"/>, then Service Fabric runtime takes following action(s):
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                 If a <see cref="FabricException"/> (or one of its derived exception) escapes from <see cref="RunAsync"/>,
        ///                 Service Fabric runtime will drop this service instance and a new instance will be created. Furthermore, a
        ///                 health warning will appear in Service Fabric Explorer containing details about unhandled exception.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                 If an <see cref="OperationCanceledException"/> escapes from <see cref="RunAsync"/> and Service Fabric runtime
        ///                 has requested cancellation by signaling <paramref name="cancellationToken"/> passed to <see cref="RunAsync"/>,
        ///                 Service Fabric runtime handles this exception and considers it as graceful completion of <see cref="RunAsync"/>.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                 If an <see cref="OperationCanceledException"/> escapes from <see cref="RunAsync"/> and Service Fabric runtime
        ///                 has NOT requested cancellation by signaling <paramref name="cancellationToken"/> passed to <see cref="RunAsync"/>,
        ///                 the process that is hosting this service instance is brought down. This will impact all other service instances
        ///                 that are hosted by the same process. The details about unhandled exceptions can be viewed in Windows Event Viewer.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                 If an exception of any other type escapes from <see cref="RunAsync"/> then the process that is hosting this
        ///                 service instance is brought down. This will impact all other service instances that are hosted by the
        ///                 same process. The details about unhandled exceptions can be viewed in Windows Event Viewer.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///         </description>
        ///     </item>
        /// </list>
        /// <para>
        /// Failing to conform to these guidelines can cause fail-over, reconfiguration or upgrade of your service to get stuck
        /// and can impact availability of your service.
        /// </para>
        /// </remarks>
        protected virtual Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is called as the final step of closing the service.
        /// Override this method to be notified that Close has completed for this instance's internal components.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle
        /// </para>
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
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle
        /// </para>
        /// </summary>
        protected virtual void OnAbort()
        {
        }
    }
}
