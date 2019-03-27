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
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Represents the base class for Microsoft Service Fabric based stateful reliable service.
    /// </summary>
    public abstract class StatefulServiceBase : IStatefulUserServiceReplica
    {
        private readonly RestoreContext restoreContext;
        private readonly StatefulServiceContext serviceContext;
        private readonly IStateProviderReplica stateProviderReplica;

        private IReadOnlyDictionary<string, string> addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulServiceBase"/> class.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> that this service is created under. The context provides information like replica ID, partition ID, and service name.
        /// </param>
        /// <param name="stateProviderReplica">
        /// A <see cref="IStateProviderReplica2"/> represents a reliable state provider replica.
        /// </param>
        protected StatefulServiceBase(
            StatefulServiceContext serviceContext,
            IStateProviderReplica stateProviderReplica)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            if (stateProviderReplica == null)
            {
                throw new ArgumentNullException(nameof(stateProviderReplica));
            }

            this.stateProviderReplica = stateProviderReplica;
            this.stateProviderReplica.OnDataLossAsync = this.OnDataLossAsync;
            if (this.stateProviderReplica is IStateProviderReplica2)
            {
                ((IStateProviderReplica2)this.stateProviderReplica).OnRestoreCompletedAsync = this.OnRestoreCompletedAsync;
            }

            this.restoreContext = new RestoreContext(this.stateProviderReplica);
            this.serviceContext = serviceContext;
            this.addresses = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        /// <summary>
        /// Gets the service context that this stateful service is operating under.
        /// It provides information like replica ID, partition ID, service name etc.
        /// </summary>
        /// <value>
        /// A <see cref="StatefulServiceContext"/> describes the service context, which it provides information like replica ID, partition ID, and service name.
        /// </value>
        public StatefulServiceContext Context
        {
            get { return this.serviceContext; }
        }

        /// <inheritdoc/>
        IStatefulServicePartition IStatefulUserServiceReplica.Partition
        {
            set { this.Partition = value; }
        }

        /// <inheritdoc/>
        IReadOnlyDictionary<string, string> IStatefulUserServiceReplica.Addresses
        {
            set { Volatile.Write(ref this.addresses, value); }
        }

        internal IStateProviderReplica StateProviderReplica
        {
            get { return this.stateProviderReplica; }
        }

        /// <summary>
        /// Gets the service partition to which current service replica belongs.
        /// </summary>
        /// <value>
        /// An <see cref="IStatefulServicePartition"/> that represents the
        /// partition to which this service replica belongs.
        /// </value>
        protected IStatefulServicePartition Partition { get; private set; }

        #region Backup and Restore APIs

        /// <summary>
        /// Performs a backup of all reliable state managed by this <see cref="StatefulServiceBase"/>.
        /// </summary>
        /// <param name="backupDescription">
        /// A <see cref="BackupDescription"/> describing the backup request.
        /// </param>
        /// <returns>Task that represents the asynchronous backup operation.</returns>
        public Task BackupAsync(BackupDescription backupDescription)
        {
            return this.StateProviderReplica.BackupAsync(
                backupDescription.Option,
                TimeSpan.FromHours(1),
                CancellationToken.None,
                backupDescription.BackupCallback);
        }

        /// <summary>
        /// Performs a backup of all reliable state managed by this <see cref="StatefulServiceBase"/>.
        /// </summary>
        /// <param name="backupDescription">A <see cref="BackupDescription"/> describing the backup request.</param>
        /// <param name="timeout">The timeout for this operation.</param>
        /// <param name="cancellationToken">The cancellation token is used to monitor for cancellation requests.</param>
        /// <returns>Task that represents the asynchronous backup operation.</returns>
        /// <remarks>
        /// Boolean returned by the backupCallback indicate whether the service was able to successfully move the backup folder to an external location.
        /// If false is returned, BackupAsync throws InvalidOperationException with the relevant message indicating backupCallback returned false.
        /// Also, backup will be marked as unsuccessful.
        /// </remarks>
        public Task BackupAsync(
            BackupDescription backupDescription,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            return this.StateProviderReplica.BackupAsync(
                backupDescription.Option,
                timeout,
                cancellationToken,
                backupDescription.BackupCallback);
        }

        #endregion

        /// <inheritdoc/>
        Task IStatefulUserServiceReplica.RunAsync(CancellationToken cancellationToken)
        {
            return this.RunAsync(cancellationToken);
        }

        /// <inheritdoc/>
        Task IStatefulUserServiceReplica.OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            return this.OnChangeRoleAsync(newRole, cancellationToken);
        }

        /// <inheritdoc/>
        Task IStatefulUserServiceReplica.OnCloseAsync(CancellationToken cancellationToken)
        {
            return this.OnCloseAsync(cancellationToken);
        }

        /// <inheritdoc/>
        void IStatefulUserServiceReplica.OnAbort()
        {
            this.OnAbort();
        }

        /// <inheritdoc/>
        IEnumerable<ServiceReplicaListener> IStatefulUserServiceReplica.CreateServiceReplicaListeners()
        {
            return this.CreateServiceReplicaListeners();
        }

        /// <inheritdoc/>
        IStateProviderReplica IStatefulUserServiceReplica.CreateStateProviderReplica()
        {
            return this.StateProviderReplica;
        }

        /// <inheritdoc/>
        Task IStatefulUserServiceReplica.OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken)
        {
            return this.OnOpenAsync(openMode, cancellationToken);
        }

        /// <summary>
        /// Gets the list of all the addresses for this service replica
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
        /// Override this method to supply the communication listeners for the service replica. The endpoints returned by the communication listener
        /// are stored as a JSON string of ListenerName, Endpoint string pairs like.
        /// <code>{"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}</code>
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle.
        /// </para>
        /// </summary>
        /// <returns>List of <see cref="ServiceReplicaListener"/>. </returns>
        protected virtual IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return Enumerable.Empty<ServiceReplicaListener>();
        }

        /// <summary>
        /// This method is called when the replica is being opened and it is the final step of opening the service.
        /// Override this method to be notified that Open has completed for this replica's internal components.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle.
        /// </para>
        /// </summary>
        /// <param name="openMode"><see cref="ReplicaOpenMode"/> for this service replica.</param>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected virtual Task OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is called when role of the replica is changing and it is the final step before completing <see cref="IStatefulServiceReplica.ChangeRoleAsync"/>.
        /// Override this method to be notified that ChangeRole has completed for this replica's internal components.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle.
        /// </para>
        /// </summary>
        /// <param name="newRole">New <see cref="ReplicaRole"/> for this service replica.</param>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents outstanding operation.
        /// </returns>
        protected virtual Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is implemented as a processing loop and will only be called when the replica is primary with write status.
        /// Override this method with the application logic.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="Task">Task</see> that represents outstanding operation.
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
        ///                 Service Fabric runtime will restart this service replica. A health warning will be appear in Service Fabric
        ///                 Explorer containing details about unhandled exception.
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
        ///                 the process that is hosting this service replica is brought down. This will impact all other service replicas
        ///                 that are hosted by the same process. The details about unhandled exceptions can be viewed in Windows Event Viewer.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                 If an exception of any other type escapes from <see cref="RunAsync"/> then the process that is hosting this
        ///                 service replica is brought down. This will impact all other service replicas that are hosted by the
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
        /// This method is called as the final step of closing the service gracefully.
        /// Override this method to be notified that Close has completed for this replica's internal components.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected virtual Task OnCloseAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// The notification that the service is being aborted. RunAsync MAY be running concurrently
        /// with the execution of this method, as cancellation is not awaited on the abort path.
        /// <para>
        /// For information about Reliable Services life cycle please see
        /// https://docs.microsoft.com/azure/service-fabric/service-fabric-reliable-services-lifecycle.
        /// </para>
        /// </summary>
        protected virtual void OnAbort()
        {
        }

        /// <summary>
        /// This method is called during suspected data loss.
        /// You can override this method to restore the service in case of data loss.
        /// </summary>
        /// <param name="restoreCtx">
        /// A <see cref="RestoreContext"/> to be used to restore the service.
        /// </param>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A Task that represents the asynchronous restore operation.
        /// True indicates that the state has been restored.
        /// False indicates that the replica's state has not been modified.
        /// </returns>
        protected virtual Task<bool> OnDataLossAsync(RestoreContext restoreCtx, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// This method is called when replica's state has been restored successfully via the Backup Restore service.
        /// This is only supported when the reliable state provider replica object passed in the constructor is derived from <see cref="IStateProviderReplica2"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// <see cref="CancellationToken"/> to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// A Task that represents the asynchronous operation.
        /// </returns>
        protected virtual Task OnRestoreCompletedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        #region OnDataLoss

        /// <summary>
        /// This method is called during suspected data-loss.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token which will be signaled if the runtime cancels the execution of this API.</param>
        /// <returns>
        /// Task that represents the asynchronous operation.
        /// True indicates that the state has been restored.
        /// </returns>
        private Task<bool> OnDataLossAsync(CancellationToken cancellationToken)
        {
            return this.OnDataLossAsync(this.restoreContext, cancellationToken);
        }

        #endregion
    }
}
