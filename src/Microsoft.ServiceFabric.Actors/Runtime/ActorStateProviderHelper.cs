// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Query;

    /// <summary>
    /// The code  in this class is shared by the different actor state providers (Kvs, RD, Volatile and Null).
    /// If you are adding any code/behavior that is common to different actor state provider(s), please add
    /// it to this class.
    /// </summary>
    internal sealed class ActorStateProviderHelper
    {
        private const int RetryCountThreshold = 3;
        private const long DefaultMaxPrimaryReplicationQueueSize = 8192;
        private const long DefaultMaxSecondaryReplicationQueueSize = 16384;

        private readonly IActorStateProviderInternal owner;

        internal ActorStateProviderHelper(IActorStateProviderInternal owner)
        {
            this.owner = owner;
        }

        internal const string ActorPresenceStorageKeyPrefix = "@@";
        internal const string ReminderCompletedStorageKeyPrefix = "RC@@";

        
        internal Task ExecuteWithRetriesAsync(
            Func<Task> func,
            string functionNameTag, 
            CancellationToken userCancellationToken)
        {
            return this.ExecuteWithRetriesAsync(
                async () =>
                {
                    await func.Invoke();
                    return (object) null;
                },
                functionNameTag,
                userCancellationToken);
        }

        internal async Task<TResult> ExecuteWithRetriesAsync<TResult>(
            Func<Task<TResult>> func,
            string functionNameTag,
            CancellationToken userCancellationToken)
        {
            int retryCount = 0;
            var effectiveRetryDelay = this.owner.TransientErrorRetryDelay;

            while (true)
            {
                try
                {
                    var res = await func.Invoke();

                    if (retryCount > RetryCountThreshold)
                    {
                        ActorTrace.Source.WriteInfoWithId(
                            this.owner.TraceType,
                            this.owner.TraceId,
                            "ExecuteWithRetriesAsync: FunctionNameTag={0} completed with RetryCount={1}.",
                            functionNameTag,
                            retryCount);
                    }

                    return res;
                }
                catch (FabricTransientException ex)
                {
                    if (ex.ErrorCode == FabricErrorCode.ReplicationQueueFull)
                    {
                        effectiveRetryDelay = new TimeSpan(this.owner.TransientErrorRetryDelay.Seconds * 2);

                        ActorTrace.Source.WriteWarningWithId(
                            this.owner.TraceType,
                            this.owner.TraceId,
                            "ExecuteWithRetriesAsync: FunctionNameTag={0} encountered ReplicationQueueFull NewRetryDelay={1}s.",
                            functionNameTag,
                            effectiveRetryDelay.Seconds);
                    }

                    // fall-through and retry
                }
                catch (FabricNotPrimaryException)
                {
                    if (this.owner.CurrentReplicaRole != ReplicaRole.Primary)
                    {
                        throw;
                    }

                    // fall-through and retry
                }
                catch (FabricObjectClosedException)
                {
                    // During close of a primary replica, the user code may try to use the 
                    // KVS after it has been closed. This causes KVS to throw FabricObjectClosedException.
                    // RC already converts it to FabricNotPrimaryException.
                    if (this.owner is KvsActorStateProvider)
                    {
                        throw new FabricNotPrimaryException();
                    }

                    throw;
                }
                catch (FabricException ex)
                {
                    // KVS aborts all active transaction(s) when changing role from primary to secondary
                    // or if replica is primary and is closing.
                    if (this.owner is KvsActorStateProvider && 
                        ex.ErrorCode == FabricErrorCode.TransactionAborted)
                    {
                        throw new FabricNotPrimaryException();
                    }

                    throw;
                }
                catch (OperationCanceledException)
                {
                    if (userCancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }

                    if (this.owner.CurrentReplicaRole != ReplicaRole.Primary)
                    {
                        throw new FabricNotPrimaryException();
                    }

                    // fall-through and retry
                }
                catch(InvalidOperationException)
                {
                    if(!(this.owner is ReliableCollectionsActorStateProvider))
                    {
                        throw;
                    }

                    // fall-through and retry
                }

                retryCount++;

                if (retryCount > RetryCountThreshold) 
                {
                    ActorTrace.Source.WriteWarningWithId(
                        this.owner.TraceType,
                        this.owner.TraceId,
                        "ExecuteWithRetriesAsync: FunctionNameTag={0}, RetryCount={1}, NextRetryDelay={2}s.",
                        functionNameTag,
                        retryCount,
                        effectiveRetryDelay.Seconds);
                }

                await Task.Delay(effectiveRetryDelay, userCancellationToken);

                // Reset effective retry delay to orginal value.
                effectiveRetryDelay = this.owner.TransientErrorRetryDelay;
            }
        }
        
        internal Task<PagedResult<ActorId>> GetStoredActorIdsAsync<T>(
            int itemsCount,
            ContinuationToken continuationToken,
            Func<IEnumerator<T>> getEnumeratorFunc,
            Func<T, string> getStorageKeyFunc,
            CancellationToken cancellationToken)
        {
            var previousActorCount = continuationToken == null ? 0 : long.Parse((string) continuationToken.Marker);

            long currentActorCount = 0;
            var actorIdList = new List<ActorId>();
            var actorQueryResult = new PagedResult<ActorId>();

            // KVS enumerates its entries in alphabetical order.
            var enumerator = getEnumeratorFunc();

            // Move the enumerator to point to first entry
            var enumHasMoreEntries = enumerator.MoveNext();

            if (!enumHasMoreEntries)
            {
                return Task.FromResult(actorQueryResult);
            }

            // Skip the previous returned entries
            while (currentActorCount < previousActorCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                enumHasMoreEntries = enumerator.MoveNext();
                currentActorCount++;

                if (!enumHasMoreEntries)
                {
                    // We are here means the current snapshot that enumerator represents
                    // has less number of entries that what ContinuationToken contains.
                    return Task.FromResult(actorQueryResult);
                }
            }

            while (enumHasMoreEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var storageKey = getStorageKeyFunc(enumerator.Current);
                var actorId = GetActorIdFromPresenceStorageKey(storageKey);

                if (actorId != null)
                {
                    actorIdList.Add(actorId);
                }
                else
                {
                    ActorTrace.Source.WriteWarningWithId(
                        this.owner.TraceType,
                        this.owner.TraceId,
                        string.Format("Failed to parse ActorId from storage key: {0}", storageKey));
                }

                enumHasMoreEntries = enumerator.MoveNext();
                currentActorCount++;

                if (actorIdList.Count == itemsCount)
                {
                    actorQueryResult.Items = actorIdList.AsReadOnly();

                    // If enumerator has more elements, then set the continuation token
                    if (enumHasMoreEntries)
                    {
                        actorQueryResult.ContinuationToken = new ContinuationToken(currentActorCount.ToString());
                    }

                    return Task.FromResult(actorQueryResult);
                }
            }

            // We are here means 'actorIdList' contains less than 'itemsCount' 
            // item or it is empty. The continuation token will remain null.
            actorQueryResult.Items = actorIdList.AsReadOnly();

            return Task.FromResult(actorQueryResult);
        }
        
        #region Static Methods

        internal static IActorStateProvider CreateDefaultStateProvider(ActorTypeInformation actorTypeInfo)
        {
            // KvsActorStateProvider is used only when: 
            //    1. Actor's [StatePersistenceAttribute] attribute has StatePersistence.Persisted.
            // VolatileActorStateProvider is used when:
            //    1. Actor's [StatePersistenceAttribute] attribute has StatePersistence.Volatile
            // NullActorStateProvider is used when:
            //    2. Actor's [StatePersistenceAttribute] attribute has StatePersistence.None OR
            //    3. Actor doesn't have [StatePersistenceAttribute] attribute.

            IActorStateProvider stateProvider = new NullActorStateProvider();
            if (actorTypeInfo.StatePersistence.Equals(StatePersistence.Persisted))
            {
                stateProvider = new KvsActorStateProvider();
            }
            else if (actorTypeInfo.StatePersistence.Equals(StatePersistence.Volatile))
            {
                stateProvider = new VolatileActorStateProvider();
            }

            // Get state provider override from settings if specified, used by tests to override state providers.
            var stateProviderOverride = GetActorStateProviderOverride();

            if (stateProviderOverride != null)
            {
                stateProvider = stateProviderOverride;
            }

            return stateProvider;
        }

        internal static IActorStateProvider GetActorStateProviderOverride()
        {
            IActorStateProvider stateProvider = null;

            try
            {
                var configurationPackageName = ActorNameFormat.GetConfigPackageName();
                var stateProviderOverrideSectionName = ActorNameFormat.GetActorStateProviderOverrideSectionName();
                var attributeTypeKey = ActorNameFormat.GetActorStateProviderOverrideKeyName();

                // Load the ActorStateProviderAttribute Type from the Configuration settings
                var context = FabricRuntime.GetActivationContext();
                var config = context.GetConfigurationPackageObject(configurationPackageName);

                if ((config.Settings.Sections != null) &&
                    (config.Settings.Sections.Contains(stateProviderOverrideSectionName)))
                {
                    var section = config.Settings.Sections[stateProviderOverrideSectionName];
                    var stateProviderType = section.Parameters[attributeTypeKey].Value;
                    stateProvider = Activator.CreateInstance(Type.GetType(stateProviderType)) as IActorStateProvider;
                }
            }
            catch (Exception)
            {
                // ignore
            }

            return stateProvider;
        }

        /// <summary>
        /// This is used by Kvs and Volatile actor state provider.
        /// </summary>
        /// <param name="codePackage"></param>
        /// <param name="actorImplType"></param>
        /// <returns></returns>
        internal static ReplicatorSettings GetActorReplicatorSettings(CodePackageActivationContext codePackage, Type actorImplType)
        {
            var settings = ReplicatorSettings.LoadFrom(
                codePackage,
                ActorNameFormat.GetConfigPackageName(actorImplType),
                ActorNameFormat.GetFabricServiceReplicatorConfigSectionName(actorImplType));

            settings.SecurityCredentials = SecurityCredentials.LoadFrom(
                codePackage,
                ActorNameFormat.GetConfigPackageName(actorImplType),
                ActorNameFormat.GetFabricServiceReplicatorSecurityConfigSectionName(actorImplType));

            var nodeContext = FabricRuntime.GetNodeContext();
            var endpoint = codePackage.GetEndpoint(ActorNameFormat.GetFabricServiceReplicatorEndpointName(actorImplType));

            settings.ReplicatorAddress = string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}",
                nodeContext.IPAddressOrFQDN,
                endpoint.Port);

            if (!settings.MaxPrimaryReplicationQueueSize.HasValue)
            {
                settings.MaxPrimaryReplicationQueueSize = DefaultMaxPrimaryReplicationQueueSize;
            }

            if (!settings.MaxSecondaryReplicationQueueSize.HasValue)
            {
                settings.MaxSecondaryReplicationQueueSize = DefaultMaxSecondaryReplicationQueueSize;
            }

            return settings;
        }

        internal static string CreateActorPresenceStorageKey(ActorId actorId)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}",
                ActorPresenceStorageKeyPrefix,
                actorId.GetStorageKey());
        }

        internal static string CreateReminderCompletedStorageKey(ActorId actorId, string reminderName)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}_{2}",
                ReminderCompletedStorageKeyPrefix,
                actorId.GetStorageKey(),
                reminderName);
        }

        internal static string CreateReminderCompletedStorageKeyPrefix(ActorId actorId)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}_",
                ReminderCompletedStorageKeyPrefix,
                actorId.GetStorageKey());
        }

        internal static ActorId GetActorIdFromPresenceStorageKey(string presenceStorageKey)
        {
            // ActorPresenceStoragekey is of following format:
            // @@_<actor Kind>_<actor ID>
            //
            // See CreateActorPresenceStorageKey for how it is generated.

            var storageKey = presenceStorageKey.Substring(ActorPresenceStorageKeyPrefix.Length + 1);
            return ActorId.TryGetActorIdFromStorageKey(storageKey);
        }

        #endregion Static Methods
    }
}