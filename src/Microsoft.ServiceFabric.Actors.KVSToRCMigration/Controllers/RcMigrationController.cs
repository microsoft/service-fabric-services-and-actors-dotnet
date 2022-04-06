// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;

    /// <summary>
    /// Represents the controller class for KVS migration REST API.
    /// </summary>
    [Route("[controller]")]
    internal class RcMigrationController : MigrationControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RcMigrationController"/> class.
        /// </summary>
        /// <param name="migrationOrchestrator">Target Migration orchestrator</param>
        public RcMigrationController(IMigrationOrchestrator migrationOrchestrator)
            : base(migrationOrchestrator)
        {
        }

        /// <summary>
        /// Gets migration status
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetMigrationStatus")]
        public async Task<MigrationResult> GetMigrationStatusAsync(CancellationToken cancellationToken)
        {
            return await ((TargetMigrationOrchestrator)this.MigrationOrchestrator).GetResultAsync(cancellationToken);
        }

        /// <summary>
        /// Validates Migrated data
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("VerifyMigration")]
        public string VerifyMigrationAsync()
        {
            throw new NotImplementedException();
        }
    }
}
