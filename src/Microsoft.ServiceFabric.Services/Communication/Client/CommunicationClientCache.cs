// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// This is the cache used by CommunicationClientFactory base class to store the communication channel's
    /// for the replicas or instances of a partition. 
    /// This is a 2 level cache of Partition Id->Endpoint Address->Client channel. The client channels are
    /// maintained as a weak reference and the cache entries whose weak references are not alive are cleaned
    /// up periodically.
    /// </summary>
    /// <typeparam name="TCommunicationClient"></typeparam>
    internal class CommunicationClientCache<TCommunicationClient>
        where TCommunicationClient : ICommunicationClient
    {
        private const string TraceType = "CommunicationClientCache";
        private readonly string traceId;
        private ConcurrentDictionary<Guid, PartitionClientCache> clientCache;
        private Timer cacheCleanupTimer;
        private double cleanupTimerIntervalSeconds = 120; // 2 minutes
        private int cleanupTimerMaxRandomizationInterval = 30;
        private Random random;

        public CommunicationClientCache(string traceId)
        {
            ServiceTrace.Source.WriteNoise(TraceType, "{0} ctor", this.traceId);

            this.clientCache = new ConcurrentDictionary<Guid, PartitionClientCache>();
            this.traceId = traceId;
            this.random = new Random();
            this.cacheCleanupTimer = new Timer(
                state => this.CacheCleanupTimerCallback(),
                null,
                this.GetNextCleanupTimerDueTimeSeconds(),
                TimeSpan.FromMilliseconds(-1));
        }

        public CommunicationClientCacheEntry<TCommunicationClient> GetOrAddClientCacheEntry(
            Guid partitionId,
            ResolvedServiceEndpoint endpoint,
            string listenerName,
            ResolvedServicePartition rsp)
        {
            var partitionClientCache = this.clientCache.GetOrAdd(
                partitionId,
                new PartitionClientCache(partitionId, this.traceId));

            return partitionClientCache.GetOrAddClientCacheEntry(endpoint, listenerName, rsp);
        }

        public bool TryGetClientCacheEntry(
            Guid partitionId,
            ResolvedServiceEndpoint endpoint,
            string listenerName,
            out CommunicationClientCacheEntry<TCommunicationClient> cacheEntry)
        {
            var partitionClientCache = this.clientCache.GetOrAdd(
                partitionId,
                new PartitionClientCache(partitionId, this.traceId));

            return partitionClientCache.TryGetClientCacheEntry(endpoint, listenerName, out cacheEntry);
        }

        public void ClearClientCacheEntries(Guid partitionId)
        {
            // Currently no op. To implement when we register for service change notifications.            
        }

        // Returns a random value between 120 and 150 seconds.
        private TimeSpan GetNextCleanupTimerDueTimeSeconds()
        {
            return TimeSpan.FromSeconds(this.cleanupTimerIntervalSeconds + this.random.Next(0, this.cleanupTimerMaxRandomizationInterval));
        }

        private void CacheCleanupTimerCallback()
        {
            ServiceTrace.Source.WriteInfo(TraceType, "{0} CacheCleanupTimerCallback", this.traceId);
            var totalItemsInCache = 0;
            var totalItemsCleaned = 0;
            foreach (var item in this.clientCache)
            {
                var itemsCleanedPerEntry = 0;
                var totalItemsPerEntry = 0;
                item.Value.CleanupCacheEntries(out totalItemsPerEntry, out itemsCleanedPerEntry);
                if (itemsCleanedPerEntry > 0)
                {
                    ServiceTrace.Source.WriteNoise(
                        TraceType,
                        "{0} CacheCleanupTimer: {1} items out of {2} cleaned up for partition {3}",
                        this.traceId,
                        itemsCleanedPerEntry,
                        totalItemsPerEntry,
                        item.Key);

                    totalItemsCleaned += itemsCleanedPerEntry;
                }

                totalItemsInCache += totalItemsPerEntry;
            }

            ServiceTrace.Source.WriteInfo(
                TraceType,
                "{0} CacheCleanupTimer: {1} items out of {2} cleaned",
                this.traceId,
                totalItemsCleaned,
                totalItemsInCache);

            // Re-arm the clean-up timer.
            this.cacheCleanupTimer.Change(this.GetNextCleanupTimerDueTimeSeconds(), TimeSpan.FromMilliseconds(-1));
        }

        private sealed class PartitionClientCache
        {
            private TimeSpan cacheEntryLockWaitTimeForCleanup = TimeSpan.FromMilliseconds(500);
            private readonly string traceId;
            private readonly Guid partitionId;
            //
            // The max size of the dictionary is 
            // the number of Listeners per replica * number of replicas in a partition.
            //
            private ConcurrentDictionary<PartitionClientCacheKey, CommunicationClientCacheEntry<TCommunicationClient>> cache;

            public PartitionClientCache(Guid partitionId, string traceId)
            {
                this.cache = new ConcurrentDictionary<PartitionClientCacheKey, CommunicationClientCacheEntry<TCommunicationClient>>();
                this.traceId = traceId;
                this.partitionId = partitionId;
            }

            public CommunicationClientCacheEntry<TCommunicationClient> GetOrAddClientCacheEntry(
                ResolvedServiceEndpoint endpoint,
                string listenerName,
                ResolvedServicePartition rsp)
            {
                var key = new PartitionClientCacheKey(endpoint, listenerName);

                return this.cache.GetOrAdd(
                    key,
                    new CommunicationClientCacheEntry<TCommunicationClient>()
                    {
                        Endpoint = endpoint,
                        ListenerName = listenerName,
                        Rsp = rsp
                    });
            }

            public bool TryGetClientCacheEntry(
                ResolvedServiceEndpoint endpoint,
                string listenerName,
                out CommunicationClientCacheEntry<TCommunicationClient> cacheEntry)
            {
                return this.cache.TryGetValue(
                    new PartitionClientCacheKey(endpoint, listenerName),
                    out cacheEntry);
            }

            //
            // Cleaning up of cache entries needs to synchronize with the code that uses the cache entry and sets the 
            // communicationClient inside the cache entry. So the removal of the cache-entry should set entry.IsInCache to false
            // and also remove the entry while holding the entry's semaphore, so that we don't purge a cache entry that has a valid 
            // communication client.
            //
            public void CleanupCacheEntries(out int totalNumberOfItems, out int numberOfEntriesCleaned)
            {
                totalNumberOfItems = 0;
                numberOfEntriesCleaned = 0;
                foreach (var entry in this.cache)
                {
                    ++totalNumberOfItems;
                    if (!entry.Value.Semaphore.Wait(this.cacheEntryLockWaitTimeForCleanup))
                    {
                        // If the wait cannot be satisfied in a short time, then it indicates usage for
                        // this cache entry, so it is ok to skip this entry in the current cleanup run.
                        continue;
                    }

                    if (!entry.Value.IsCommunicationClientValid())
                    {
                        ServiceTrace.Source.WriteNoise(
                            TraceType,
                            "{0} CleanupCacheEntries for partitionid {1} endpoint {2} : {3}",
                            this.traceId,
                            this.partitionId,
                            entry.Key.ListenerName,
                            entry.Key.Endpoint);

                        entry.Value.IsInCache = false;

                        CommunicationClientCacheEntry<TCommunicationClient> removedValue;
                        this.cache.TryRemove(entry.Key, out removedValue);
                        ++numberOfEntriesCleaned;
                    }
                    entry.Value.Semaphore.Release();
                }
            }
        }

        private sealed class PartitionClientCacheKey
        {
            public readonly ResolvedServiceEndpoint Endpoint;
            public readonly string ListenerName;

            public PartitionClientCacheKey(ResolvedServiceEndpoint endpoint, string listenerName)
            {
                this.Endpoint = endpoint;
                this.ListenerName = listenerName;
            }

            public override bool Equals(object obj)
            {
                var other = obj as PartitionClientCacheKey;
                
                return (other != null)
                    && (this.Endpoint.Role == other.Endpoint.Role)
                    && (CultureInfo.InvariantCulture.CompareInfo.Compare(this.ListenerName, other.ListenerName, CompareOptions.IgnoreCase) == 0)
                    && (CultureInfo.InvariantCulture.CompareInfo.Compare(this.Endpoint.Address, other.Endpoint.Address, CompareOptions.IgnoreCase) == 0);
            }

            public override int GetHashCode()
            {
                if (this.ListenerName != null)
                {
                    return this.ListenerName.GetHashCode() ^ this.Endpoint.Address.GetHashCode();
                }

                return this.Endpoint.Address.GetHashCode();
            }
        }
    }
}