// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an interface that exposes methods to start migration of KVS data to RC />.
    /// </summary>
    public interface IMigrationOrchestrator
    {
        /// <summary>
        /// Starts KVS to RC migration.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous migration operation.
        /// </returns>
        public Task StartMigration(CancellationToken cancellationToken);
    }
}
