// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;

    internal class DataValidationPhaseWorkload : MigrationPhaseWorkloadBase
    {
        private static readonly string TraceType = typeof(DataValidationPhaseWorkload).Name;

        public DataValidationPhaseWorkload(
            KVStoRCMigrationActorStateProvider stateProvider,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            StatefulServiceContext statefulServiceContext,
            MigrationSettings migrationSettings,
            ActorTypeInformation actorTypeInfo,
            string traceId)
            : base(MigrationPhase.DataValidation, 1, migrationSettings.MigratedDataValidationPhaseParallelism, stateProvider, servicePartitionClient, statefulServiceContext, migrationSettings, actorTypeInfo, traceId)
        {
        }

        protected override Task<long> GetStartSequenceNumberAsync(CancellationToken cancellationToken)
        {
            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"DataValidationPhaseWorkload.GetStartSequenceNumberAsync: [{0L}]");

            return Task.FromResult(0L);
        }

        protected override async Task<long> GetEndSequenceNumberAsync(CancellationToken cancellationToken)
        {
            long eSN = -1L;
            using (var tx = this.Transaction)
            {
                var cond = await this.MetaDataDictionary.TryGetValueAsync(
                        tx,
                        MigrationNoOfKeysMigrated,
                        DefaultRCTimeout,
                        cancellationToken);
                await tx.CommitAsync();

                if (cond.HasValue)
                {
                    eSN = ParseLong(cond.Value, this.TraceId);
                }
            }

            ActorTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.TraceId,
                        $"DataValidationPhaseWorkload.GetEndSequenceNumberAsync: [{eSN}]");

            return eSN;
        }
    }
}
