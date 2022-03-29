// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationUtility;

    internal class CopyPhaseWorkload : MigrationPhaseWorkloadBase
    {
        private static readonly string TraceType = typeof(CopyPhaseWorkload).Name;

        public CopyPhaseWorkload(
            KVStoRCMigrationActorStateProvider stateProvider,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            StatefulServiceContext statefulServiceContext,
            MigrationSettings migrationSettings,
            ActorTypeInformation actorTypeInfo,
            string traceId)
            : base(MigrationPhase.Copy, 1, migrationSettings.CopyPhaseParallelism, stateProvider, servicePartitionClient, statefulServiceContext, migrationSettings, actorTypeInfo, traceId)
        {
        }

        protected override async Task<long> GetStartSequenceNumberAsync(CancellationToken cancellationToken)
        {
            long startSequenceNumber;
            using (var tx = this.Transaction)
            {
                var cond = await this.MetaDataDictionary.TryGetValueAsync(
                        tx,
                        MigrationLastAppliedSeqNum,
                        DefaultRCTimeout,
                        cancellationToken);
                await tx.CommitAsync();

                if (cond.HasValue)
                {
                    startSequenceNumber = ParseLong(cond.Value, this.TraceId);
                }
                else
                {
                    return (await ParseLongAsync(
                       () => this.ServicePartitionClient.InvokeWithRetryAsync<string>(
                       async client =>
                       {
                           return await client.HttpClient.GetStringAsync($"{KVSMigrationControllerName}/{GetStartSNEndpoint}");
                       },
                       cancellationToken),
                       this.TraceId)).Value;
                }
            }

            return startSequenceNumber;
        }
    }
}
