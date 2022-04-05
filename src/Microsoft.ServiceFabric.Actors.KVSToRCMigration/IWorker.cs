// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Threading;
    using System.Threading.Tasks;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.PhaseInput;
    using static Microsoft.ServiceFabric.Actors.Migration.PhaseResult;

    /// <summary>
    /// Interface definition for workers.
    /// </summary>
    internal interface IWorker
    {
        /// <summary>
        /// Gets the worker input information.
        /// </summary>
        public WorkerInput Input { get; }

        /// <summary>
        /// Starts a new worker.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the task.</param>
        /// <returns>Returns the worker result.</returns>
        Task<WorkerResult> StartWorkAsync(CancellationToken cancellationToken);
    }
}
