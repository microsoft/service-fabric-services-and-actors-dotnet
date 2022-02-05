// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Migration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Represents the controller class for KVS migration REST API.
    /// </summary>
    [Route("[controller]")]
#pragma warning disable CS3009 // Base type is not CLS-compliant
    public class RcMigrationController : ControllerBase
#pragma warning restore CS3009 // Base type is not CLS-compliant
    {
        private IMigrationOrchestrator migrationOrchestrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RcMigrationController"/> class.
        /// </summary>
        /// <param name="migrationOrchestrator">Migration orchestrator</param>
        public RcMigrationController(IMigrationOrchestrator migrationOrchestrator)
        {
            this.migrationOrchestrator = migrationOrchestrator;
        }

        /// <summary>
        /// Gets migration status
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetMigrationStatus")]
        public Task<MigrationStatus> GetMigrationStatusAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// Calls ResumeWritesOnKVS API on KVS service to resume accepting write calls
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("ResumeWritesOnKVSService")]
        public async Task ResumeWritesOnKVSServiceAsync(CancellationToken cancellationToken)
        {
            await this.migrationOrchestrator.InvokeResumeWritesAsync(cancellationToken);
        }
    }
}
