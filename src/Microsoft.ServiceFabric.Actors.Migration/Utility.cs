// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System.Fabric;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class Utility
    {
        public GrpcCommunicationListener GetKVSGrpcCommunicationListener(StatefulServiceContext serviceContext, ActorTypeInformation actorTypeInformation, KvsActorStateProvider stateProvider)
        {
            return new GrpcCommunicationListener(serviceContext, actorTypeInformation, new[] { KvsMigration.BindService(new KvsMigrationService(stateProvider)) });
        }

        public bool RejectWrites(KvsActorStateProvider stateProvider)
        {
            if (stateProvider.GetKvsRejectWriteStatusAsync())
            {
                return stateProvider.TryAbortExistingTransactionsAndRejectWrites();
            }

            return false;
        }
    }
}
