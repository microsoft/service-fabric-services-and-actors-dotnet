// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Describes the interface for the user services from the adapter perspective.
    /// The stateful service adapter uses this interface to make calls that should be forwarded to the user service implementation.
    /// </summary>
    internal interface IStatefulUserServiceReplica
    {
        /// <summary>
        /// Sets the addresses of all communication listeners that are opened by the adapter.
        /// </summary>
        IReadOnlyDictionary<string, string> Addresses { set; }

        /// <summary>
        /// Sets the partition to which the user service replica belongs to.
        /// </summary>
        IStatefulServicePartition Partition { set; }

        /// <summary>
        /// Asks the user service to create an instance of the state provider.
        /// </summary>
        /// <returns>An instance of <see cref="IStateProviderReplica2"/> state provider replica.</returns>
        IStateProviderReplica2 CreateStateProviderReplica();

        /// <summary>
        /// Get the service replica listeners that the user service wants to open.
        /// </summary>
        /// <returns>A list of service replica listener that should be opened by the adapter.</returns>
        IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners();

        /// <summary>
        ///  Calls RunAsync method on the user service when the replica is primary with write status.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation may still be completed even if it is canceled.
        /// </param>
        /// <returns>A <see cref="Task"/>that represents the outstanding operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code when the service replica is opening.
        /// </summary>
        /// <param name="openMode">The mode under which the service replica should be opened. This supports the Service Fabric infrastructure and is not meant to be used directly from your code.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation may still be completed even if it is canceled.
        /// </param>
        /// <returns>A <see cref="Task"/>that represents the outstanding operation.</returns>
        Task OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code as part of changing the replica role.
        /// </summary>
        /// <param name="newRole">The update <see cref="ReplicaRole"/> that this replica is transitioning to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation may still be completed even if it is canceled.
        /// </param>
        /// <returns>A <see cref="Task"/>that represents the outstanding operation.</returns>
        Task OnChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code when the service replica is being closed gracefully.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation may still be completed even if it is canceled.
        /// </param>
        /// <returns>A <see cref="Task"/>that represents the outstanding operation.</returns>
        Task OnCloseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code when the service replica is being terminated ungracefully.
        /// </summary>
        void OnAbort();
    }
}
