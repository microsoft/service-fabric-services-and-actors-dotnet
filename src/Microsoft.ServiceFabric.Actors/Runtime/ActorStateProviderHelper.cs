// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Query;
    using Microsoft.ServiceFabric.Actors.Remoting;

    /// <summary>
    /// Represents the code shared by the different actor state providers (Kvs, RD, Volatile and Null).
    /// If you are adding any code/behavior that is common to different actor state provider(s), please add
    /// it to this class.
    /// </summary>
    internal sealed class ActorStateProviderHelper
    {
        internal const string ActorPresenceStorageKeyPrefix = "@@";
        internal const string ReminderCompletedStorageKeyPrefix = "RC@@";
        private const string ActorsMigrationAssemblyName = "Microsoft.ServiceFabric.Actors.Migration";
        private const string KVSToRCMigrationActorStateProviderClassFullName = "Microsoft.ServiceFabric.Actors.Migration.KVStoRCMigrationActorStateProvider";
        private const long DefaultMaxPrimaryReplicationQueueSize = 8192;
        private const long DefaultMaxSecondaryReplicationQueueSize = 16384;
        private readonly IActorStateProviderInternal owner;

        internal ActorStateProviderHelper(IActorStateProviderInternal owner)
        {
            this.owner = owner;
        }

        private bool CurrentReplicaRoleNotPrimary
        {
            get
            {
                return (this.owner.CurrentReplicaRole != ReplicaRole.Primary);
            }
        }

        #region Static Methods

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

                    ActorTrace.Source.WriteInfo(
                        "ActorStateProviderHelper",
                        "Overridding actor state provider: '{0}'",
                        stateProviderType);

                    stateProvider = Activator.CreateInstance(Type.GetType(stateProviderType)) as IActorStateProvider;
                }
            }
            catch (Exception)
            {
                // ignore
            }

            return stateProvider;
        }

        internal static string CreateActorPresenceStorageKey(ActorId actorId)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}",
                ActorPresenceStorageKeyPrefix,
                actorId.GetStorageKey());
        }

        internal static DataContractSerializer CreateDataContractSerializer(Type actorStateType)
        {
            var dataContractSerializer = new DataContractSerializer(
                actorStateType,
                new DataContractSerializerSettings
                {
                    MaxItemsInObjectGraph = int.MaxValue,
#if !DotNetCoreClr
                    DataContractSurrogate = ActorDataContractSurrogate.Instance,
#endif
                    KnownTypes = new[]
                    {
                        typeof(ActorReference),
                    },
                });
#if DotNetCoreClr
            dataContractSerializer.SetSerializationSurrogateProvider(ActorDataContractSurrogate.Instance);
#endif
            return dataContractSerializer;
        }

        internal static bool TryGetConfigSection(
            ICodePackageActivationContext activationContext,
            string configPackageName,
            string sectionName,
            out ConfigurationSection section)
        {
            section = null;

            var config = activationContext.GetConfigurationPackageObject(configPackageName);

            if ((config.Settings.Sections == null) || (!config.Settings.Sections.Contains(sectionName)))
            {
                return false;
            }

            section = config.Settings.Sections[sectionName];

            return true;
        }

        internal static TimeSpan GetTimeConfigInSecondsAsTimeSpan(ConfigurationSection section, string parameterName, TimeSpan defaultValue)
        {
            if (section.Parameters.Contains(parameterName) &&
               !string.IsNullOrWhiteSpace(section.Parameters[parameterName].Value))
            {
                var timeInSeconds = double.Parse(section.Parameters[parameterName].Value);
                return TimeSpan.FromSeconds(timeInSeconds);
            }

            return defaultValue;
        }

        /// <summary>
        /// Used by Kvs and Volatile actor state provider.
        /// </summary>
        /// <param name="codePackage">The code package.</param>
        /// <param name="actorImplType">The type of actor.</param>
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
                bool isMigrationTarget = Utility.IsMigrationTarget(new List<Type>() { actorTypeInfo.ImplementationType });
                bool isMigrationSource = Utility.IsMigrationSource(new List<Type>() { actorTypeInfo.ImplementationType });
#if DotNetCoreClr
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    if (isMigrationTarget)
                    {
                        var message = "Migration target attribute is valid only for Reliable Collection (RC) service.";
                        ActorTrace.Source.WriteWarning("ActorStateProviderHelper", message);
                    }

                    stateProvider = new KvsActorStateProvider();
                }
                else
                {
                    if (isMigrationTarget)
                    {
                        stateProvider = (IActorStateProvider)GetKVSToRCMigrationStateProvider();
                    }
                    else if (isMigrationSource)
                    {
                        var message = "Migration source attribute is valid only for KVS services.";
                        ActorTrace.Source.WriteWarning("ActorStateProviderHelper", message);
                        stateProvider = new ReliableCollectionsActorStateProvider();
                    }
                    else
                    {
                        stateProvider = new ReliableCollectionsActorStateProvider();
                    }
                }
#else
                if (isMigrationTarget)
                {
                    var message = "Migration target attribute is valid only for Reliable Collection (RC) service.";
                    ActorTrace.Source.WriteWarning("ActorStateProviderHelper", message);
                }

                stateProvider = new KvsActorStateProvider();
#endif
            }
            else if (actorTypeInfo.StatePersistence.Equals(StatePersistence.Volatile))
            {
                stateProvider = new VolatileActorStateProvider();
            }

            // Get state provider override from settings if specified, used by tests to override state providers.
            var stateProviderOverride = Runtime.ActorStateProviderHelper.GetActorStateProviderOverride();

            if (stateProviderOverride != null)
            {
                stateProvider = stateProviderOverride;
            }

            return stateProvider;
        }

        internal static IActorStateProvider GetStateProvider(IActorStateProvider stateProvider, ActorTypeInformation actorTypeInfo)
        {
            if (stateProvider == null)
            {
                return CreateDefaultStateProvider(actorTypeInfo);
            }

            if (Utility.IsMigrationTarget(new List<Type>() { actorTypeInfo.ImplementationType }))
            {
                if (stateProvider.GetType() == typeof(ReliableCollectionsActorStateProvider))
                {
                    return (IActorStateProvider)GetKVSToRCMigrationStateProvider((ReliableCollectionsActorStateProvider)stateProvider);
                }
                else
                {
                    var message = "Migration target attribute is valid only for Reliable Collection (RC) service";
                    ActorTrace.Source.WriteWarning("ActorStateProviderHelper", message);
                    return stateProvider;
                }
            }
            else if (Utility.IsMigrationSource(new List<Type>() { actorTypeInfo.ImplementationType }))
            {
                if (stateProvider.GetType() != typeof(KvsActorStateProvider))
                {
                    var message = "Migration source attribute is valid only for KVS service";
                    ActorTrace.Source.WriteWarning("ActorStateProviderHelper", message);
                }

                return stateProvider;
            }
            else
            {
                return stateProvider;
            }
        }

        internal static object GetKVSToRCMigrationStateProvider(ReliableCollectionsActorStateProvider reliableCollectionsActorStateProvider = null)
        {
            var currentAssembly = typeof(ActorStateProviderHelper).GetTypeInfo().Assembly;

            var actorsMigrationAssembly = new AssemblyName
            {
                Name = ActorsMigrationAssemblyName,
                Version = currentAssembly.GetName().Version,
#if !DotNetCoreClr
                CultureInfo = currentAssembly.GetName().CultureInfo,
#endif
                ProcessorArchitecture = currentAssembly.GetName().ProcessorArchitecture,
            };

            actorsMigrationAssembly.SetPublicKeyToken(currentAssembly.GetName().GetPublicKeyToken());

            var kvsToRCMigrationStateProviderTypeName = Helper.CreateQualifiedNameForAssembly(
                actorsMigrationAssembly.FullName,
                KVSToRCMigrationActorStateProviderClassFullName);

            var kvsToRCMigrationStateProviderType = Type.GetType(kvsToRCMigrationStateProviderTypeName, true);

            if (reliableCollectionsActorStateProvider == null)
            {
                return Activator.CreateInstance(kvsToRCMigrationStateProviderType);
            }
            else
            {
                return Activator.CreateInstance(kvsToRCMigrationStateProviderType, new object[] { reliableCollectionsActorStateProvider });
            }
        }
        #endregion Static Methods

        internal Task ExecuteWithRetriesAsync(
            Func<Task> func,
            string functionNameTag,
            CancellationToken userCancellationToken)
        {
            return this.ExecuteWithRetriesAsync(
                async () =>
                {
                    await func.Invoke();
                    return (object)null;
                },
                functionNameTag,
                userCancellationToken);
        }

        internal async Task<TResult> ExecuteWithRetriesAsync<TResult>(
            Func<Task<TResult>> func,
            string functionNameTag,
            CancellationToken userCancellationToken)
        {
            var retryCount = 0;
            var useLinearBackoff = false;
            var lastExceptionTag = string.Empty;
            var roleChangeTracker = this.owner.RoleChangeTracker;
            var operationId = Guid.NewGuid();
            var timeoutHelper = new TimeoutHelper(this.owner.OperationTimeout);

            while (true)
            {
                try
                {
                    // Actor operations only happen on a primary replica and are required not to span role
                    // change boundaries. This is required to ensure that for a given ActorId on a primary
                    // replica only one thread can make any state change. Any operation active for this ActorId
                    // when current replica was primary previously should fail to make any state change.
                    //
                    // When primary replica becomes secondary, all in-flight operations fail as replica do not
                    // have write status. However, in rare cases, it may happen that replica undergoes a P -> S -> P
                    // role change very quickly while an in-flight operation was undergoing back-off before next retry.
                    //
                    // Fail the operation if primary replica of partition has changed.
                    this.EnsureSamePrimary(roleChangeTracker);

                    useLinearBackoff = false;

                    var res = await func.Invoke();

                    if (retryCount > 0)
                    {
                        ActorTrace.Source.WriteInfoWithId(
                            this.owner.TraceType,
                            this.owner.TraceId,
                            "ExecuteWithRetriesAsync: FunctionNameTag={0}, OperationId={1} completed with RetryCount={2}.",
                            functionNameTag,
                            operationId,
                            retryCount);
                    }

                    return res;
                }
                catch (FabricTransientException ex)
                {
                    useLinearBackoff = (ex.ErrorCode == FabricErrorCode.ReplicationQueueFull);
                    lastExceptionTag = ex.ErrorCode.ToString();

                    // fall-through and retry
                }
                catch (FabricNotPrimaryException)
                {
                    if (timeoutHelper.HasTimedOut || this.CurrentReplicaRoleNotPrimary)
                    {
                        throw;
                    }

                    lastExceptionTag = "FabricNotPrimary";

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

                    if (this.CurrentReplicaRoleNotPrimary)
                    {
                        throw new FabricNotPrimaryException();
                    }

                    if (timeoutHelper.HasTimedOut)
                    {
                        throw;
                    }

                    lastExceptionTag = "OperationCanceled";

                    // fall-through and retry
                }
                catch (TransactionFaultedException)
                {
                    if (timeoutHelper.HasTimedOut)
                    {
                        throw;
                    }

                    lastExceptionTag = "TransactionFaulted";

                    // fall-through and retry
                }

                retryCount++;

                var effectiveRetryDelay = useLinearBackoff ?
                    TimeSpan.FromTicks(retryCount * this.owner.TransientErrorRetryDelay.Ticks) :
                    this.owner.TransientErrorRetryDelay;

                ActorTrace.Source.WriteInfoWithId(
                    this.owner.TraceType,
                    this.owner.TraceId,
                    "ExecuteWithRetriesAsync: FunctionNameTag={0}, OperationId={1}, RetryCount={2}, LastExceptionTag={3}, NextRetryDelay={4}s.",
                    functionNameTag,
                    operationId,
                    retryCount,
                    lastExceptionTag,
                    effectiveRetryDelay.Seconds);

                await Task.Delay(effectiveRetryDelay, userCancellationToken);
            }
        }

        internal Task<PagedResult<ActorId>> GetStoredActorIdsAsync<T>(
            int itemsCount,
            ContinuationToken continuationToken,
            Func<IEnumerator<T>> getEnumeratorFunc,
            Func<T, string> getStorageKeyFunc,
            CancellationToken cancellationToken)
        {
            var actorIdList = new List<ActorId>();
            var actorQueryResult = new PagedResult<ActorId>();

            // KVS enumerates its entries in alphabetical order.
            using var enumerator = getEnumeratorFunc();

            // Move the enumerator to point to first entry
            var enumHasMoreEntries = enumerator.MoveNext();

            if (!enumHasMoreEntries)
            {
                return Task.FromResult(actorQueryResult);
            }

            // Find continuation point for enumeration
            if (continuationToken != null)
            {
                long previousActorCount = 0L;
                if (long.TryParse((string)continuationToken.Marker, out previousActorCount))
                {
                    enumHasMoreEntries = this.GetContinuationPointByActorCount(previousActorCount, enumerator, cancellationToken);
                }
                else
                {
                    string lastSeenActorStorageKey = continuationToken.Marker.ToString();
                    enumHasMoreEntries = this.GetContinuationPointByActorStorageKey(lastSeenActorStorageKey, enumerator, getStorageKeyFunc, cancellationToken);
                }

                if (!enumHasMoreEntries)
                {
                    // We are here means the current snapshot that enumerator represents
                    // has less entries that what ContinuationToken contains.
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

                if (actorIdList.Count == itemsCount)
                {
                    actorQueryResult.Items = actorIdList.AsReadOnly();

                    // If enumerator has more elements, then set the continuation token.
                    if (enumHasMoreEntries)
                    {
                        actorQueryResult.ContinuationToken = new ContinuationToken(storageKey.ToString());
                    }

                    return Task.FromResult(actorQueryResult);
                }
            }

            // We are here means 'actorIdList' contains less than 'itemsCount'
            // item or it is empty. The continuation token will remain null.
            actorQueryResult.Items = actorIdList.AsReadOnly();

            return Task.FromResult(actorQueryResult);
        }

        #region Private Helpers

        private bool GetContinuationPointByActorCount<T>(
            long previousActorCount,
            IEnumerator<T> enumerator,
            CancellationToken cancellationToken)
        {
            long currentActorCount = 0L;
            bool enumHasMoreEntries = true;

            // Skip the previous returned entries
            while (currentActorCount < previousActorCount && enumHasMoreEntries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentActorCount++;
                enumHasMoreEntries = enumerator.MoveNext();
            }

            return enumHasMoreEntries;
        }

        private bool GetContinuationPointByActorStorageKey<T>(
            string lastSeenActorStorageKey,
            IEnumerator<T> enumerator,
            Func<T, string> getStorageKeyFunc,
            CancellationToken cancellationToken)
        {
            bool enumHasMoreEntries = true;
            var storageKey = getStorageKeyFunc(enumerator.Current);

            // Skip the previous returned entries
            while (enumHasMoreEntries && string.Compare(storageKey, lastSeenActorStorageKey, StringComparison.InvariantCultureIgnoreCase) <= 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                enumHasMoreEntries = enumerator.MoveNext();
                if (enumHasMoreEntries)
                {
                    storageKey = getStorageKeyFunc(enumerator.Current);
                }
            }

            return enumHasMoreEntries;
        }

        private void EnsureSamePrimary(long roleChangeTracker)
        {
            if (roleChangeTracker != this.owner.RoleChangeTracker)
            {
                throw new FabricNotPrimaryException();
            }
        }

        #endregion
    }

#pragma warning disable SA1201 // Elements should appear in the correct order
    internal struct TimeoutHelper
#pragma warning restore SA1201 // Elements should appear in the correct order
    {
        private readonly Stopwatch stopWatch;
        private readonly TimeSpan originalTimeout;

        public TimeoutHelper(TimeSpan timeout)
        {
            this.originalTimeout = timeout;
            this.stopWatch = Stopwatch.StartNew();
        }

        public bool HasRemainingTime
        {
            get
            {
                return (this.stopWatch.Elapsed < this.originalTimeout);
            }
        }

        public bool HasTimedOut
        {
            get
            {
                return !this.HasRemainingTime;
            }
        }
    }
}
