// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    internal class AmbiguousActorIdHandler
    {
        internal static readonly string MetadataTableName = "store://nonambiguousactorid//metadataTable";
        internal static readonly string MetaDataKey = "nonambiguousactorid";
        private KVStoRCMigrationActorStateProvider stateProvider;
        private ActorTypeInformation actorTypeInfo;
        private IReliableDictionary<string, HashSet<string>> metadataTable;
        private bool isMetadataTableInitialized = false;
        private HashSet<string> metadataCache;

        public AmbiguousActorIdHandler(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo)
        {
            this.stateProvider = stateProvider;
            this.actorTypeInfo = actorTypeInfo;
            this.metadataCache = new HashSet<string>();
        }

        public string TraceType
        {
            get { return "AmbiguousActorIdHandler"; }
        }

        public async Task PopulateCache(CancellationToken cancellationToken)
        {
            await this.InitializeMetadataTableAsync(cancellationToken);

            ConditionalValue<HashSet<string>> metadataTableValue;
            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                metadataTableValue = await this.metadataTable.TryGetValueAsync(tx, MetaDataKey, TimeSpan.FromSeconds(60), cancellationToken);
            }

            if (metadataTableValue.HasValue)
            {
                this.metadataCache = metadataTableValue.Value;
            }
        }

        private async Task InitializeMetadataTableAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (this.isMetadataTableInitialized)
            {
                return;
            }

            IReliableDictionary<string, HashSet<string>> metadataTab = null;

            using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
            {
                try
                {
                    metadataTab = await this.stateProvider.GetStateManager().GetOrAddAsync<IReliableDictionary2<string, HashSet<string>>>(tx, MetadataTableName);
                    cancellationToken.ThrowIfCancellationRequested();
                    await tx.CommitAsync();
                }
                catch (Exception e)
                {
                    ActorTrace.Source.WriteError(this.TraceType, e.Message);
                }
            }

            this.metadataTable = metadataTab;

            Volatile.Write(ref this.isMetadataTableInitialized, true);
        }
    }
}
