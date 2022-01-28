// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Controllers
{
    using System;
    using System.Fabric;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors.Generator;
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
        private static readonly HttpClient Client = new HttpClient();
        private StatefulServiceContext serviceContext;
        private ActorTypeInformation actorTypeInformation;
        private KVStoRCMigrationActorStateProvider kvsToRcMigrationActorStateProvider;
        private string kvsEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="RcMigrationController"/> class.
        /// </summary>
        /// <param name="context">StatefulServiceContext</param>
        /// <param name="actorTypeInfo">ActorTypeInformation</param>
        /// <param name="stateProvider">KvsActorStateProvider</param>
        public RcMigrationController(StatefulServiceContext context, ActorTypeInformation actorTypeInfo, KVStoRCMigrationActorStateProvider stateProvider)
        {
            this.serviceContext = context;
            this.actorTypeInformation = actorTypeInfo;
            this.kvsToRcMigrationActorStateProvider = stateProvider;
            this.GetUserSettingsOrDefault();
        }

        /// <summary>
        /// Gets migration status
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("GetMigrationStatus")]
        public async Task<MigrationStatus> GetMigrationStatusAsync(CancellationToken cancellationToken)
        {
            var migrationStatus = await this.kvsToRcMigrationActorStateProvider.GetMigrationStatusAsync(cancellationToken);

            return migrationStatus;
        }

        /// <summary>
        /// Validates Migrated data
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("VerifyMigration")]
        public string VerifyMigrationAsync()
        {
            return "TODO: Implementaion pending.";
        }

        /// <summary>
        /// Calls ResumeWritesOnKVS API on KVS service to resume accepting write calls
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("ResumeWritesOnKVSService")]
        public async Task ResumeWritesOnKVSServiceAsync()
        {
            await Client.PutAsync(this.kvsEndpoint + MigrationConstants.ResumeWritesAPIEndpoint, null);
        }

        private void GetUserSettingsOrDefault()
        {
            var configPackageName = ActorNameFormat.GetConfigPackageName();
            try
            {
                var configPackageObj = this.serviceContext.CodePackageActivationContext.GetConfigurationPackageObject(configPackageName);
                var migrationConfigLabel = ActorNameFormat.GetMigrationConfigSectionName(this.actorTypeInformation.ImplementationType);
                if (configPackageObj.Settings.Sections.Contains(migrationConfigLabel))
                {
                    var migrationSettings = configPackageObj.Settings.Sections[migrationConfigLabel];
                    if (migrationSettings.Parameters.Contains("KVSActorServiceUri"))
                    {
                        this.kvsEndpoint = migrationSettings.Parameters["KVSActorServiceUri"].Value;
                    }
                }
            }
            catch (Exception e)
            {
                ActorTrace.Source.WriteError("RcMigrationController", e.Message);
            }
        }
    }
}
