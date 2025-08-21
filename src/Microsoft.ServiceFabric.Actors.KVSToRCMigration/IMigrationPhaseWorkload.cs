// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;

    /// <summary>
    /// Interface definition for migration workload by phase.
    /// </summary>
    internal interface IMigrationPhaseWorkload
    {
        /// <summary>
        /// Gets the current migration phase.
        /// </summary>
        MigrationPhase Phase { get; }

        /// <summary>
        /// Starts a new phase or resumes the current migration phase after failover.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the task.</param>
        /// <returns>Returns the migration result.</returns>
        Task<PhaseResult> StartOrResumeMigrationAsync(CancellationToken cancellationToken);
    }
}
