// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class CatchupPhaseWorkload : MigrationPhaseWorkloadBase
    {
        public CatchupPhaseWorkload(
            int curentIteration,
            KVStoRCMigrationActorStateProvider stateProvider,
            ServicePartitionClient<HttpCommunicationClient> servicePartitionClient,
            StatefulServiceContext statefulServiceContext,
            MigrationSettings migrationSettings,
            ActorTypeInformation actorTypeInfo,
            string traceId)
            : base(MigrationPhase.Catchup, curentIteration, 1, stateProvider, servicePartitionClient, statefulServiceContext, migrationSettings, actorTypeInfo, traceId)
        {
        }
    }
}
