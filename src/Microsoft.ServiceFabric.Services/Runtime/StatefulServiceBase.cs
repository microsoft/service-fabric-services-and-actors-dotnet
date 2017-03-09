// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
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
    /// Represents base class for Microsoft Service Fabric based stateful reliable service.
    /// </summary>
    public abstract class StatefulServiceBase : IStatefulUserServiceReplica
    {
        private readonly RestoreContext restoreContext;
        private readonly StatefulServiceContext serviceContext;
        private readonly IStateProviderReplica stateProviderReplica;

        private IReadOnlyDictionary<string, string> addresses;

        /// <summary>
        /// Creates a new StatefulService.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> that describes the service context.
        /// </param>
        /// <param name="stateProviderReplica">
        /// A <see cref="IStateProviderReplica"/> that represents a reliable state provider replica.
        /// </param>
        protected StatefulServiceBase(
            StatefulServiceContext serviceContext,
            IStateProviderReplica stateProviderReplica)
        {
            this.stateProviderReplica = stateProviderReplica;
            this.stateProviderReplica.OnDataLossAsync = this.OnDataLossAsync;
            this.restoreContext = new RestoreContext(this.stateProviderReplica);
            this.serviceContext = serviceContext;
            this.addresses = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        internal IStateProviderReplica StateProviderReplica
        {
            get { return this.stateProviderReplica; }
        }

        /// <summary>
        /// Gets the service context that this stateful service is operating under. It provides
        /// information like ReplicaId, PartitionId, ServiceName etc.
        /// </summary>
        /// <value>
        /// A <see cref="StatefulServiceContext"/> that describes the service context.
        /// </value>
        public StatefulServiceContext Context
        {
            get { return this.serviceContext; }
        }
        
        /// <summary>
        /// Gets list of all the addresses for this service replica
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
        /// Service partition to which current service replica belongs. 
        /// </summary>
        /// <value>
        /// An <see cref="IStatefulServicePartition"/> that represents the 
        /// partition to which this service replica belongs.
        /// </value>
        protected IStatefulServicePartition Partition { get; private set; }

        /// <summary>
        /// Override this method to supply the communication listeners for the service replica. The endpoints returned by the communication listener's
        /// are stored as a JSON string of ListenerName, Endpoint string pairs like 
        /// {"Endpoints":{"Listener1":"Endpoint1","Listener2":"Endpoint2" ...}}
        /// </summary>
        /// <returns>List of ServiceReplicaListeners</returns>
        protected virtual IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return Enumerable.Empty<ServiceReplicaListener>();
        }

        /// <summary>
        /// This method is called as the final step of opening the service.
        /// Override this method to be notified that Open has completed for this replica's internal components.
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
        /// This method is called as the final step before completing <see cref="IStatefulServiceReplica.ChangeRoleAsync"/>.
        /// Override this method to be notified that ChangeRole has completed for this replica's internal components.
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
        /// Services that want to implement a processing loop which runs when it is primary and has write status,
        /// just override this method with their logic.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected virtual Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// This method is called as the final step of closing the service.
        /// Override this method to be notified that Close has completed for this replica's internal components.
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
        /// Notification that the service is being aborted.  RunAsync MAY be running concurrently
        /// with the execution of this method, as cancellation is not awaited on the abort path.
        /// </summary>
        protected virtual void OnAbort()
        {
        }
        
        IReadOnlyDictionary<string, string> IStatefulUserServiceReplica.Addresses
        {
            set { Volatile.Write(ref this.addresses, value); }
        }

        IStatefulServicePartition IStatefulUserServiceReplica.Partition
        {
            set { this.Partition = value; }
        }

        IStateProviderReplica IStatefulUserServiceReplica.CreateStateProviderReplica()
        {
            return this.StateProviderReplica;
        }

        IEnumerable<ServiceReplicaListener> IStatefulUserServiceReplica.CreateServiceReplicaListeners()
        {
            return this.CreateServiceReplicaListeners();
        }

        Task IStatefulUserServiceReplica.OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken)
        {
            return this.OnOpenAsync(openMode, cancellationToken);
        }

        Task IStatefulUserServiceReplica.RunAsync(CancellationToken cancellationToken)
        {
            return this.RunAsync(cancellationToken);
        }

        Task IStatefulUserServiceReplica.OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            return this.OnChangeRoleAsync(newRole, cancellationToken);
        }

        Task IStatefulUserServiceReplica.OnCloseAsync(CancellationToken cancellationToken)
        {
            return this.OnCloseAsync(cancellationToken);
        }

        void IStatefulUserServiceReplica.OnAbort()
        {
            this.OnAbort();
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
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
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

        #region OnDataLoss

        /// <summary>
        /// This method is called during suspected data-loss.
        /// </summary>
        /// <param name="cancellationToken"></param>
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