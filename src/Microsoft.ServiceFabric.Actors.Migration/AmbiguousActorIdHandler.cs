// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Health;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    /// <summary>
    /// Provides an implementaion of AmbiguousActorIdHandler
    /// </summary>
    public class AmbiguousActorIdHandler
    {
        internal static readonly string MetadataTableName = "store://nonambiguousactorid//metadataTable";
        internal static readonly string MetaDataKey = "nonambiguousactorid";
        private static readonly string AmbiguousActorIdHandlerSourceId = "AmbiguousActorIdHandler";
        private static readonly string CannotResolveAmbiguousActorIdHealthProperty = "CannotResolveAmbiguousActorId";
        private KVStoRCMigrationActorStateProvider stateProvider;
        private ActorTypeInformation actorTypeInfo;
        private IReliableDictionary<string, HashSet<string>> metadataTable;
        private bool isMetadataTableInitialized = false;
        private HashSet<string> metadataCache;
        private List<Type> resolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousActorIdHandler"/> class.
        /// </summary>
        /// <param name="stateProvider">State Provider</param>
        /// <param name="actorTypeInfo">Actor Tyoe</param>
        public AmbiguousActorIdHandler(KVStoRCMigrationActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo)
        {
            this.stateProvider = stateProvider;
            this.actorTypeInfo = actorTypeInfo;
            this.metadataCache = new HashSet<string>();
            this.resolvers = this.GetActorIdResolvers();
        }

        /// <summary>
        /// Gets Trace Type
        /// </summary>
        private string TraceType
        {
            get { return "AmbiguousActorIdHandler"; }
        }

        /// <summary>
        /// Populates the Ambigious Actor Id cache on failover
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task</returns>
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

        internal void AddRange(List<string> stringActorIds)
        {
            stringActorIds.ForEach(x => this.Add(x));
        }

        internal async void Add(string stringActorId)
        {
            if (stringActorId.StartsWith(MigrationConstants.StringTypeActorIdPrefix))
            {
                stringActorId = stringActorId.Substring(MigrationConstants.StringTypeActorIdPrefix.Length, stringActorId.Length - MigrationConstants.StringTypeActorIdPrefix.Length - 1);
            }

            if (!this.IsAmbiguousActorId(stringActorId))
            {
                this.metadataCache.Add(stringActorId);
                await this.InitializeMetadataTableAsync(CancellationToken.None);
                using (var tx = this.stateProvider.GetStateManager().CreateTransaction())
                {
                    await this.metadataTable.AddOrUpdateAsync(tx, MetaDataKey, this.metadataCache, (k, v) => this.metadataCache);
                    await tx.CommitAsync();
                }
            }
            else
            {
                this.ReportPartitionHealthAndThrow();
            }
        }

        internal bool IsAmbiguousActorId(string stringActorId)
        {
            var count = 0;
            var tempStr = stringActorId;

            // drop last token. which could be statename or reminder name
            if (stringActorId.Contains("_"))
            {
                var lastIndexOfUnderscore = stringActorId.LastIndexOf('_');
                tempStr = stringActorId.Substring(0, lastIndexOfUnderscore);
            }

            for (int iterations = 0; iterations < stringActorId.Split('_').Count(); iterations++)
            {
                count += this.metadataCache.Select(n => n.Contains(tempStr)).Count();

                var lastIndexOfUnderscore = tempStr.LastIndexOf('_');
                tempStr = tempStr.Substring(0, lastIndexOfUnderscore);
            }

            if (count > 1)
            {
                foreach (Type resolver in this.resolvers)
                {
                    var resolverObj = Activator.CreateInstance(resolver, new object[] { });
                    var tryResolveActorIdAndStateNameMethodInfo = resolver.GetMethod("TryResolveActorIdAndStateName");
                    var tryResolveActorIdAndReminderNameMethodInfo = resolver.GetMethod("TryResolveActorIdAndReminderName");

                    string actorId = string.Empty;
                    string stateName = string.Empty;
                    string reminderName = string.Empty;

                    if ((bool)tryResolveActorIdAndStateNameMethodInfo.Invoke(resolverObj, new object[] { stringActorId, actorId, stateName })
                        || (bool)tryResolveActorIdAndReminderNameMethodInfo.Invoke(resolverObj, new object[] { stringActorId, actorId, reminderName }))
                    {
                        // actor Id is not ambigious as resolver was successful
                        return false;
                    }
                }
            }

            return true;
        }

        internal ActorId GetActorId(string key)
        {
            if (!this.IsAmbiguousActorId(key))
            {
                var count = 0;
                var tempStr = key;
                var actorId = key;

                // drop last token. which could be statename or reminder name
                if (key.Contains("_"))
                {
                    var lastIndexOfUnderscore = key.LastIndexOf('_');
                    tempStr = key.Substring(0, lastIndexOfUnderscore);
                    actorId = tempStr;
                }

                for (int iterations = 0; iterations < actorId.Split('_').Count(); iterations++)
                {
                    count += this.metadataCache.Select(n => n.Contains(tempStr)).Count();

                    var lastIndexOfUnderscore = tempStr.LastIndexOf('_');
                    tempStr = tempStr.Substring(0, lastIndexOfUnderscore);
                }

                if (count <= 1)
                {
                    return new ActorId(actorId);
                }
                else
                {
                    foreach (Type resolver in this.resolvers)
                    {
                        var resolverObj = Activator.CreateInstance(resolver, new object[] { });
                        var tryResolveActorIdAndStateNameMethodInfo = resolver.GetMethod("TryResolveActorIdAndStateName");
                        var tryResolveActorIdAndReminderNameMethodInfo = resolver.GetMethod("TryResolveActorIdAndReminderName");

                        string stateName = string.Empty;
                        string reminderName = string.Empty;

                        if ((bool)tryResolveActorIdAndStateNameMethodInfo.Invoke(resolverObj, new object[] { key, actorId, stateName })
                            || (bool)tryResolveActorIdAndReminderNameMethodInfo.Invoke(resolverObj, new object[] { key, actorId, reminderName }))
                        {
                            // actor Id is not ambigious as resolver was successful
                            break;
                        }
                    }
                }

                return new ActorId(actorId);
            }
            else
            {
                this.ReportPartitionHealthAndThrow();
                return null; // this is unreachable code added to avoid compile time error.
            }
        }

        private void ReportPartitionHealthAndThrow()
        {
            var description = string.Format(
                "Migration failed due to ambiguous Actor Ids present in source location and no resolver failing to resolve them. Please check <link to documntation>");

            var healthInfo = new HealthInformation(AmbiguousActorIdHandlerSourceId, CannotResolveAmbiguousActorIdHealthProperty, HealthState.Error)
            {
                TimeToLive = TimeSpan.MaxValue,
                RemoveWhenExpired = false,
                Description = description,
            };

            this.stateProvider.ReportPartitionHealth(healthInfo);
            throw new InvalidOperationException(description);
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

        private List<Type> GetActorIdResolvers()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(ActorIdResolverAttribute), true).Length > 0).ToList();
            }

            return new List<Type>();
        }
    }
}
