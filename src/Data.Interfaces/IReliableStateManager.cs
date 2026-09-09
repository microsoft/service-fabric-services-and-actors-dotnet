// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Data.Notifications;

    /// <summary>
    /// Manages all <see cref="IReliableState"/> for a service replica.
    /// Each replica in a service has its own state manager and thus its own set of <see cref="IReliableState"/>.
    /// </summary>
    public interface IReliableStateManager : IAsyncEnumerable<IReliableState>
    {
        /// <summary>
        /// Occurs when an <see cref="ITransaction"/> changes state.
        /// For example, when a transaction is committed.
        /// </summary>
        /// <exception cref="FabricObjectClosedException">The Reliable State Manager is closed.</exception>
        event EventHandler<NotifyTransactionChangedEventArgs> TransactionChanged;

        /// <summary>
        /// Occurs when the <see cref="IReliableStateManager"/>'s state changes.
        /// For example, creation or deletion of an <see cref="IReliableState"/> or rebuild of the reliable state manager.
        /// </summary>
        /// <exception cref="FabricObjectClosedException">The Reliable State Manager is closed.</exception>
        event EventHandler<NotifyStateManagerChangedEventArgs> StateManagerChanged;

        /// <summary>
        /// Tries to register a custom <paramref name="stateSerializer"/> for all reliable collections and returns <see langword="true"/>
        /// if it was successfully registered; otherwise returns <see langword="false"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a reliable collection needs to serialize an object, it asks the <see cref="IReliableStateManager"/> for a serializer for the given type.
        /// The state manager will first check if there is a custom serializer registered for the input type. If not, it will check if one of the built-in
        /// serializers can serialize the type. The state manager has built-in serializers for the following types: <see cref="Guid"/>, <see langword="bool"/>, <see langword="byte"/>, <see langword="sbyte"/>, <see langword="char"/>, <see langword="decimal"/>,
        /// <see langword="double"/>, <see langword="float"/>, <see langword="int"/>, <see langword="uint"/>, <see langword="long"/>, <see langword="ulong"/>, <see langword="short"/>, <see langword="ushort"/>, <see langword="string"/> and <see langword="byte"/>[]. If not, it will use <see cref="DataContractSerializer"/>.
        /// </para>
        /// <para>
        /// Serializers must be infinitely forwards and backwards compatible. For the types that are using built-in serializers, Service Fabric ensures
        /// forwards and backwards compatibility. However, if a custom serializer is added for a type with a built-in serializer, the custom serializer
        /// must be compatible with the built-in serialization format for that type.
        /// </para>
        /// <para>
        /// This method should be called from the constructor of the Stateful Service.
        /// This ensures that the Reliable Collections have the necessary serializers before recovery of the persisted state begins.
        /// </para>
        /// </remarks>
        bool TryAddStateSerializer<T>(IStateSerializer<T> stateSerializer);

        /// <summary>
        /// Returns a new, started <see cref="ITransaction"/> that can be used to group operations to be performed atomically.
        /// </summary>
        /// <remarks>
        /// Operations are added to the transaction by passing the <see cref="ITransaction"/> object in to reliable state methods.
        /// </remarks>
        ITransaction CreateTransaction();

        /// <summary>
        /// Asynchronously gets an <see cref="IReliableState"/> of the given type <typeparamref name="T"/> and with the given name if it exists, or creates one
        /// and returns it if it doesn't already exist.
        /// </summary>
        /// <remarks>
        /// This is an atomic operation. When an <see cref="IReliableState"/> needs to be created, it will either complete and return successfully
        /// or it will not be created. If this method throws an exception, the transaction must be aborted.
        /// </remarks>
        /// <typeparam name="T">
        /// When specifying the <see cref="IReliableState"/> type, you may ask for either a class type or an interface type.
        /// <para>
        /// If specifying a class type, the system will attempt to return an instance of that type.
        /// If an instance of that type cannot be instantiated (e.g., abstract class, no parameterless constructor), a <see cref="MissingMethodException"/> is thrown.
        /// </para>
        /// <para>
        /// If specifying an interface type, the manager will resolve the interface to its default concrete implementation.
        /// If the given interface type does not have a default implementation, or the type is invalid, this method will throw <see cref="ArgumentException"/>.
        /// </para>
        /// </typeparam>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="name">
        /// The name of the <see cref="IReliableState"/>. This name must be unique in this <see cref="IReliableStateManager"/>
        /// across <see cref="IReliableState"/> types, including unrelated types.
        /// </param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <exception cref="ArgumentException">The interface type <typeparamref name="T"/> cannot be resolved to a concrete type, the existing <see cref="IReliableState"/> instance is not of type <typeparamref name="T"/>, or <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableStateManager"/> is not in the <see cref="ReplicaRole.Primary"/> role.</exception>
        /// <exception cref="FabricObjectClosedException">The Reliable State Manager is closed.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="MissingMethodException">The class type <typeparamref name="T"/> cannot be instantiated; for example, it is abstract or has no parameterless constructor.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task<T> GetOrAddAsync<T>(ITransaction tx, Uri name, TimeSpan timeout) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="ArgumentException">The interface type <typeparamref name="T"/> cannot be resolved to a concrete type, or the existing <see cref="IReliableState"/> instance is not of type <typeparamref name="T"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(ITransaction tx, Uri name) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <remarks>
        /// This is an atomic operation. When an <see cref="IReliableState"/> needs to be created, it will either complete and return successfully
        /// or it will not be created.
        /// </remarks>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='timeout']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.TimeoutException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(Uri name, TimeSpan timeout) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="GetOrAddAsync{T}(Uri, TimeSpan)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="ArgumentException">The interface type <typeparamref name="T"/> cannot be resolved to a concrete type, or the existing <see cref="IReliableState"/> instance is not of type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(Uri name) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='timeout']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.TimeoutException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(ITransaction tx, string name, TimeSpan timeout) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='tx']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="ArgumentException">The interface type <typeparamref name="T"/> cannot be resolved to a concrete type, or the existing <see cref="IReliableState"/> instance is not of type <typeparamref name="T"/>.</exception>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(ITransaction tx, string name) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="GetOrAddAsync{T}(Uri, TimeSpan)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='timeout']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.TimeoutException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(string name, TimeSpan timeout) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="GetOrAddAsync{T}(Uri, TimeSpan)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/param[@name='name']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <exception cref="ArgumentException">The interface type <typeparamref name="T"/> cannot be resolved to a concrete type, or the existing <see cref="IReliableState"/> instance is not of type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.MissingMethodException']" cref="GetOrAddAsync{T}(ITransaction, Uri, TimeSpan)"/>
        Task<T> GetOrAddAsync<T>(string name) where T : IReliableState;

        /// <summary>
        /// Asynchronously removes the <see cref="IReliableState"/> with the given name from this state manager. The state is
        /// permanently removed from persistent storage and all replicas when the transaction is committed.
        /// </summary>
        /// <remarks>
        /// This is an atomic operation. The <see cref="IReliableState"/> will be successfully removed along with all state
        /// or be left intact. If this method throws an exception, the transaction must be aborted.
        /// </remarks>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <exception cref="ArgumentException">An <see cref="IReliableState"/> with the given name does not exist, or <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tx"/> is <see langword="null"/>, or <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotPrimaryException">The <see cref="IReliableStateManager"/> is not in the <see cref="ReplicaRole.Primary"/> role.</exception>
        /// <exception cref="FabricObjectClosedException">The Reliable State Manager is closed.</exception>
        /// <exception cref="InvalidOperationException">
        /// A method call is invalid for the object's current state.
        /// For example, the transaction used is already terminated: committed or aborted by the user.
        /// If this exception is thrown, it is highly likely that there is a bug in the service's use of transactions.
        /// </exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the given timeout.</exception>
        /// <exception cref="TransactionFaultedException">The transaction has been internally faulted by the system. Retry the operation on a new transaction.</exception>
        Task RemoveAsync(ITransaction tx, Uri name, TimeSpan timeout);

        /// <inheritdoc path="/summary" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <exception cref="ArgumentException">An <see cref="IReliableState"/> with the given name does not exist.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(ITransaction tx, Uri name);

        /// <summary>
        /// Asynchronously removes the <see cref="IReliableState"/> with the given name from this state manager. The state is
        /// permanently removed from persistent storage and all replicas.
        /// </summary>
        /// <remarks>
        /// This is an atomic operation. The <see cref="IReliableState"/> will be successfully removed along with all state
        /// or be left intact.
        /// </remarks>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.TimeoutException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(Uri name, TimeSpan timeout);

        /// <inheritdoc path="/summary" cref="RemoveAsync(Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="RemoveAsync(Uri, TimeSpan)"/>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <exception cref="ArgumentException">An <see cref="IReliableState"/> with the given name does not exist.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(Uri name);

        /// <inheritdoc path="/summary" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.TimeoutException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(ITransaction tx, string name, TimeSpan timeout);

        /// <inheritdoc path="/summary" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <param name="tx">The transaction to associate this operation with.</param>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <exception cref="ArgumentException">An <see cref="IReliableState"/> with the given name does not exist.</exception>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.TransactionFaultedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.InvalidOperationException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(ITransaction tx, string name);

        /// <inheritdoc path="/summary" cref="RemoveAsync(Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="RemoveAsync(Uri, TimeSpan)"/>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <param name="timeout">The amount of time to wait for the operation to complete before throwing a <see cref="TimeoutException"/>. Primarily used to prevent deadlocks.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.TimeoutException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(string name, TimeSpan timeout);

        /// <inheritdoc path="/summary" cref="RemoveAsync(Uri, TimeSpan)"/>
        /// <inheritdoc path="/remarks" cref="RemoveAsync(Uri, TimeSpan)"/>
        /// <param name="name">The name of the <see cref="IReliableState"/> to remove.</param>
        /// <exception cref="ArgumentException">An <see cref="IReliableState"/> with the given name does not exist.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <exception cref="TimeoutException">The operation failed to complete within the default timeout.</exception>
        /// <exception cref="TransactionFaultedException">The operation has been internally faulted by the system. Retry the operation.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotPrimaryException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="RemoveAsync(ITransaction, Uri, TimeSpan)"/>
        Task RemoveAsync(string name);

        /// <summary>
        /// Asynchronously attempts to get an <see cref="IReliableState"/> of the given type <typeparamref name="T"/> and with
        /// the given name, and returns a <see cref="ConditionalValue{T}"/> indicating whether it was found and containing
        /// the instance if it was.
        /// </summary>
        /// <typeparam name="T">
        /// When specifying the type, you may ask for either a concrete type or an interface type. The retrieved object will
        /// be cast to the given type.
        /// </typeparam>
        /// <param name="name">
        /// The name of the <see cref="IReliableState"/>. This name must be unique in this <see cref="IReliableStateManager"/>
        /// across <see cref="IReliableState"/> types, including unrelated types.
        /// </param>
        /// <exception cref="ArgumentException">The <see cref="IReliableState"/> instance is not convertible to type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="FabricNotReadableException">
        /// The <see cref="IReliableStateManager"/> cannot retrieve the requested <see cref="IReliableState"/>.
        /// This exception can be thrown in all <see cref="ReplicaRole"/>s.
        /// For example, a <see cref="ReplicaRole.Primary"/> loses <see cref="IStatefulServicePartition.ReadStatus"/>,
        /// or an <see cref="ReplicaRole.ActiveSecondary"/>'s local state is not yet readable.</exception>
        /// <exception cref="FabricObjectClosedException">The Reliable State Manager is closed.</exception>
        Task<ConditionalValue<T>> TryGetAsync<T>(Uri name) where T : IReliableState;

        /// <inheritdoc path="/summary" cref="TryGetAsync{T}(Uri)"/>
        /// <inheritdoc path="/typeparam[@name='T']" cref="TryGetAsync{T}(Uri)"/>
        /// <inheritdoc path="/param[@name='name']" cref="TryGetAsync{T}(Uri)"/>
        /// <exception cref="FormatException"><paramref name="name"/> begins with the reserved <c>urn:</c> prefix.</exception>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentException']" cref="TryGetAsync{T}(Uri)"/>
        /// <inheritdoc path="/exception[@cref='T:System.ArgumentNullException']" cref="TryGetAsync{T}(Uri)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricNotReadableException']" cref="TryGetAsync{T}(Uri)"/>
        /// <inheritdoc path="/exception[@cref='T:System.Fabric.FabricObjectClosedException']" cref="TryGetAsync{T}(Uri)"/>
        Task<ConditionalValue<T>> TryGetAsync<T>(string name) where T : IReliableState;
    }
}
