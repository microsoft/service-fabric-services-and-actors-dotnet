// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Fabric.Description;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Data.Notifications;

    /// <summary>
    /// Manages all <see cref="IReliableState"/> for a service replica.
    /// </summary>
    /// <remarks>
    /// Each replica in a service has its own <see cref="ReliableStateManager"/> and thus its own set of <see cref="IReliableState"/>.
    /// <see cref="IReliableState"/> can include <see cref="IReliableDictionary{TKey, TValue}"/>,
    /// <see cref="IReliableQueue{T}"/>, or any <see cref="IReliableCollection{T}"/> types.
    /// </remarks>
    public class ReliableStateManager : IReliableStateManagerReplica2
    {
        private readonly IReliableStateManagerReplica2 _impl;

        #region TransactionChanged variables
        private EventHandler<NotifyTransactionChangedEventArgs> userTransactionChangedEventHandler;

        private readonly object eventHandlerRegistrationLock = new object();

        private bool isTransactionEventHandlerRegistered = false;
        #endregion

        #region StateManagerChanged variables
        private EventHandler<NotifyStateManagerChangedEventArgs> userStateManagerChangedEventHandler;

        private bool isStateManagerEventHandlerRegistered = false;

        private bool isClosingOrAborting = false;

        #endregion

        internal IStateProviderReplica2 Replica
        {
            get
            {
                return this._impl;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableStateManager"/> class.
        /// </summary>
        /// <param name="serviceContext">The context that the replica operates under.</param>
        /// <param name="configuration">
        /// The configuration used to create the state manager, or <see langword="null"/> to use a default
        /// <see cref="ReliableStateManagerConfiguration"/>.
        /// </param>
        /// <exception cref="FileNotFoundException">The <c>Microsoft.ServiceFabric.Data.Impl</c> assembly could not be loaded.</exception>
        public ReliableStateManager(StatefulServiceContext serviceContext, ReliableStateManagerConfiguration configuration = null)
        {
            configuration = configuration ?? new ReliableStateManagerConfiguration();

            Assembly dataImpl = FabricRuntime.LoadAssembly("Microsoft.ServiceFabric.Data.Impl") ?? throw new FileNotFoundException();
            Type entryPoints = dataImpl.GetType("Microsoft.ServiceFabric.Data.EntryPoints");
            MethodInfo createReliableStateManager2 = entryPoints.GetMethod("CreateReliableStateManager2");
            this._impl = (IReliableStateManagerReplica2)createReliableStateManager2.Invoke(null, new object[] { serviceContext, configuration });
        }

        /// <inheritdoc/>
        public event EventHandler<NotifyTransactionChangedEventArgs> TransactionChanged
        {
            add
            {
                // MCoskun: Order of the following operations is significant.
                // user event handler is set before the event handler is registered to ensure if a notification comes it can be plumbed.
                this.userTransactionChangedEventHandler += value;

                this.RegisterTransactionEventHandlerIfNecessary();

                ReleaseAssert.AssertIfNot(this.isTransactionEventHandlerRegistered, "transaction event handler must have been registered");
            }
            remove
            {
                this.userTransactionChangedEventHandler -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<NotifyStateManagerChangedEventArgs> StateManagerChanged
        {
            add
            {
                // MCoskun: Order of the following operations is significant.
                // user event handler is set before the event handler is registered to ensure if a notification comes it can be plumbed.
                this.userStateManagerChangedEventHandler += value;

                this.RegisterStateManagerEventHandlerIfNecessary();


                ReleaseAssert.AssertIfNot(this.isStateManagerEventHandlerRegistered, "state manager event handler must have been registered");
            }
            remove
            {
                this.userStateManagerChangedEventHandler -= value;
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerator<IReliableState> GetAsyncEnumerator() { return this._impl.GetAsyncEnumerator(); }

        /// <inheritdoc/>
        public Task BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback) { return this._impl.BackupAsync(backupCallback); }

        /// <inheritdoc/>
        Task<ConditionalValue<T>> IReliableStateManager.TryGetAsync<T>(string name) { return this._impl.TryGetAsync<T>(name); }

        /// <inheritdoc/>
        Task<ConditionalValue<T>> IReliableStateManager.TryGetAsync<T>(Uri name) { return this._impl.TryGetAsync<T>(name); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(string name) { return this._impl.RemoveAsync(name); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(string name, TimeSpan timeout) { return this._impl.RemoveAsync(name, timeout); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(ITransaction tx, string name) { return this._impl.RemoveAsync(tx, name); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(ITransaction tx, string name, TimeSpan timeout) { return this._impl.RemoveAsync(tx, name, timeout); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(Uri name) { return this._impl.RemoveAsync(name); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(Uri name, TimeSpan timeout) { return this._impl.RemoveAsync(name, timeout); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(ITransaction tx, Uri name) { return this._impl.RemoveAsync(tx, name); }

        /// <inheritdoc/>
        Task IReliableStateManager.RemoveAsync(ITransaction tx, Uri name, TimeSpan timeout) { return this._impl.RemoveAsync(tx, name, timeout); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(string name) { return this._impl.GetOrAddAsync<T>(name); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(string name, TimeSpan timeout) { return this._impl.GetOrAddAsync<T>(name, timeout); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(ITransaction tx, string name) { return this._impl.GetOrAddAsync<T>(tx, name); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(ITransaction tx, string name, TimeSpan timeout) { return this._impl.GetOrAddAsync<T>(tx, name, timeout); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(Uri name) { return this._impl.GetOrAddAsync<T>(name); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(Uri name, TimeSpan timeout) { return this._impl.GetOrAddAsync<T>(name, timeout); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(ITransaction tx, Uri name) { return this._impl.GetOrAddAsync<T>(tx, name); }

        /// <inheritdoc/>
        Task<T> IReliableStateManager.GetOrAddAsync<T>(ITransaction tx, Uri name, TimeSpan timeout) { return this._impl.GetOrAddAsync<T>(tx, name, timeout); }

        /// <inheritdoc/>
        ITransaction IReliableStateManager.CreateTransaction() { return this._impl.CreateTransaction(); }

        /// <inheritdoc/>
        bool IReliableStateManager.TryAddStateSerializer<T>(IStateSerializer<T> stateSerializer)
        {
            return this._impl.TryAddStateSerializer(stateSerializer);
        }

        /// <inheritdoc/>
        public Task RestoreAsync(string backupFolderPath) { return this._impl.RestoreAsync(backupFolderPath); }

        /// <inheritdoc/>
        public Task RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy, CancellationToken cancellationToken)
        {
            return this._impl.RestoreAsync(backupFolderPath, restorePolicy, cancellationToken);
        }

        /// <inheritdoc/>
        public Task BackupAsync(
            BackupOption option,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            return this._impl.BackupAsync(option, timeout, cancellationToken, backupCallback);
        }

        /// <inheritdoc/>
        public Func<CancellationToken, Task<bool>> OnDataLossAsync
        {
            set { this._impl.OnDataLossAsync = value; }
        }

        /// <inheritdoc/>
        public Func<CancellationToken, Task> OnRestoreCompletedAsync
        {
            set { this._impl.OnRestoreCompletedAsync = value; }
        }

        /// <inheritdoc/>
        void IStateProviderReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            this.Replica.Initialize(initializationParameters);
        }

        /// <inheritdoc/>
        Task<IReplicator> IStateProviderReplica.OpenAsync(ReplicaOpenMode openMode, IStatefulServicePartition partition, CancellationToken cancellationToken)
        {
            return this.Replica.OpenAsync(openMode, partition, cancellationToken);
        }

        /// <inheritdoc/>
        Task IStateProviderReplica.ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            return this.Replica.ChangeRoleAsync(newRole, cancellationToken);
        }

        /// <inheritdoc/>
        Task IStateProviderReplica.CloseAsync(CancellationToken cancellationToken)
        {
            lock (this.eventHandlerRegistrationLock)
            {
                this.isClosingOrAborting = true;
            }

            this.CleanUpEventHandlers();
            return this.Replica.CloseAsync(cancellationToken);
        }

        /// <inheritdoc/>
        void IStateProviderReplica.Abort()
        {
            lock (this.eventHandlerRegistrationLock)
            {
                this.isClosingOrAborting = true;
            }

            this.CleanUpEventHandlers();
            this.Replica.Abort();
        }

        // Transaction changed forwarder.
        private void OnTransactionChangedHandler(object sender, NotifyTransactionChangedEventArgs args)
        {
            var snappedUserDictionaryChangedEventHandler = this.userTransactionChangedEventHandler;

            if (snappedUserDictionaryChangedEventHandler != null)
            {
                snappedUserDictionaryChangedEventHandler.Invoke(this, args);
            }
        }

        // State manager changed forwarder.
        private void OnStateManagerChangedHandler(object sender, NotifyStateManagerChangedEventArgs args)
        {
            var snappedUserStateManagerChangedEventHandler = this.userStateManagerChangedEventHandler;

            if (snappedUserStateManagerChangedEventHandler != null)
            {
                snappedUserStateManagerChangedEventHandler.Invoke(this, args);
            }
        }

        private void RegisterTransactionEventHandlerIfNecessary()
        {
            if (this.isTransactionEventHandlerRegistered)
            {
                return;
            }

            lock (this.eventHandlerRegistrationLock)
            {
                this.ThrowIfClosingOrAbortingCallerHoldsLock();

                if (this.isTransactionEventHandlerRegistered == false)
                {
                    this._impl.TransactionChanged += this.OnTransactionChangedHandler;
                    this.isTransactionEventHandlerRegistered = true;
                }
            }
        }

        private void RegisterStateManagerEventHandlerIfNecessary()
        {
            if (this.isStateManagerEventHandlerRegistered)
            {
                return;
            }

            lock (this.eventHandlerRegistrationLock)
            {
                this.ThrowIfClosingOrAbortingCallerHoldsLock();

                if (this.isStateManagerEventHandlerRegistered == false)
                {
                    this._impl.StateManagerChanged += this.OnStateManagerChangedHandler;
                    this.isStateManagerEventHandlerRegistered = true;
                }
            }
        }

        private void UnRegisterTransactionEventHandlerIfNecessary()
        {
            ReleaseAssert.AssertIfNot(this.isClosingOrAborting, "Unregisteration only happens during close or abort.");

            // lock not required since close/abort is signaled, no registeration can come in.
            if (this.isTransactionEventHandlerRegistered)
            {
                this._impl.TransactionChanged -= this.OnTransactionChangedHandler;
                this.isTransactionEventHandlerRegistered = false;
            }
        }

        private void UnRegisterStateManagerEventHandlerIfNecessary()
        {
            ReleaseAssert.AssertIfNot(this.isClosingOrAborting, "Unregisteration only happens during close or abort.");

            // lock not required since close/abort is signaled, no registeration can come in.
            if (this.isStateManagerEventHandlerRegistered)
            {
                this._impl.StateManagerChanged -= this.OnStateManagerChangedHandler;
                this.isStateManagerEventHandlerRegistered = false;
            }
        }

        private void CleanUpEventHandlers()
        {
            this.UnRegisterTransactionEventHandlerIfNecessary();
            this.UnRegisterStateManagerEventHandlerIfNecessary();
        }

        private void ThrowIfClosingOrAbortingCallerHoldsLock()
        {
            if (this.isClosingOrAborting)
            {
                throw new FabricObjectClosedException("Reliable State Manager is closed.");
            }
        }
    }
}
