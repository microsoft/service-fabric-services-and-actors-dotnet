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
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    /// <summary>
    /// Describes the interface for the user services from the adapter perspective.
    /// The stateless service adapter uses this interface to make calls that should be forwarded to the user service implementation.
    /// </summary>
    internal interface IStatelessUserServiceInstance
    {
        /// <summary>
        /// Sets the addresses of all communication listeners that are opened by the adapter.
        /// </summary>
        IReadOnlyDictionary<string, string> Addresses { set; }

        /// <summary>
        /// Sets the partition to which the user service instance belongs to.
        /// </summary>
        IStatelessServicePartition Partition { set; }

        /// <summary>
        /// Get the service instance listeners that the user service wants to open.
        /// </summary>
        /// <returns>A list of service instance listener that should be opened by the adapter.</returns>
        IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners();

        /// <summary>
        /// Calls RunAsync method on the user service at the appropriate time.
        /// </summary>
        /// <param name="cancellationToken">Token which will be cancelled when the runtime closes this service instances.</param>
        /// <returns>Task that represents the outstanding execution of the RunAsync method.</returns>
        Task RunAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code as part of the opening of the service instance.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation may still be completed even if it is canceled.
        /// </param>
        /// <returns>A <see cref="Task"/>that represents the outstanding operation.</returns>
        Task OnOpenAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code when the service instance is being closed gracefully.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object that the operation is observing.
        /// It can be used to send a notification that the operation should be canceled. Note that cancellation is advisory and that the operation may still be completed even if it is canceled.
        /// </param>
        /// <returns>A <see cref="Task"/>that represents the outstanding operation.</returns>
        Task OnCloseAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Allows the user service to execute custom code when the service instance is being terminated ungracefully.
        /// </summary>
        void OnAbort();
    }
}
