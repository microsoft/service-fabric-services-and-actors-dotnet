// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
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
    /// <typeparam name="TCommunicationClient">The type of the communication client.</typeparam>
    internal class CommunicationClientCache<TCommunicationClient>
    : IDisposable
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
            ServiceTrace.Source.WriteNoise(TraceType, "{0} ctor", traceId);

            this.clientCache = new ConcurrentDictionary<Guid, PartitionClientCache>();
            this.traceId = traceId;
            this.random = new Random();

            // Don't capture the current ExecutionContext and its AsyncLocals onto the timer
            bool restoreFlow = false;
            AsyncFlowControl asyncFlowControl = default(AsyncFlowControl);
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    asyncFlowControl = ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                this.cacheCleanupTimer = new Timer(
                state => this.CacheCleanupTimerCallback(),
                null,
                this.GetNextCleanupTimerDueTimeSeconds(),
                TimeSpan.FromMilliseconds(-1));
            }
            finally
            {
                // Restore the current ExecutionContext
                if (restoreFlow)
                {
                    asyncFlowControl.Undo();
                }
            }
        }

        public CommunicationClientCacheEntry<TCommunicationClient> GetOrAddClientCacheEntry(
            Guid partitionId,
            ResolvedServiceEndpoint endpoint,
            string listenerName,
            ResolvedServicePartition rsp)
        {
            var partitionClientCache = this.clientCache.GetOrAdd(
                                                partitionId,
                                                this.CreatePartitionClientCache);

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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.cacheCleanupTimer != null)
                {
                    this.cacheCleanupTimer.Dispose();
                    this.clientCache.Clear();
                    this.cacheCleanupTimer = null;
                }
            }
        }

        // Returns a random value between 120 and 150 seconds.
        private TimeSpan GetNextCleanupTimerDueTimeSeconds()
        {
            return TimeSpan.FromSeconds(this.cleanupTimerIntervalSeconds + this.random.Next(0, this.cleanupTimerMaxRandomizationInterval));
        }

        private PartitionClientCache CreatePartitionClientCache(Guid partitionId)
        {
            return new PartitionClientCache(partitionId, this.traceId);
        }

        private void CacheCleanupTimerCallback()
        {
            var totalItemsInCache = 0;
            var totalItemsCleaned = 0;
            foreach (var item in this.clientCache)
            {
                item.Value.CleanupCacheEntries(out var totalItemsPerEntry, out var itemsCleanedPerEntry);
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
                }// We are delaying the delete partition for next cleanup event to reduce the case of false positive partitions (which are still in use), since removing partition means acquiring lock on it.
                else if (item.Value.IsEmpty())
                {
                    // delete partition from cache only if when number of entry is zero.
                    var emptyItem = new PartitionClientCache(item.Key, this.traceId);
                    var isDeleted = ((IDictionary<Guid, PartitionClientCache>)this.clientCache).Remove(new KeyValuePair<Guid, PartitionClientCache>(item.Key, emptyItem));
                    if (isDeleted)
                    {
                        ServiceTrace.Source.WriteInfo(
                        TraceType,
                        "{0} Deleted Cache Entry for partition {1}",
                        this.traceId,
                        item.Key);
                    }
                }

                totalItemsInCache += totalItemsPerEntry;
            }

            if (totalItemsCleaned != 0)
            {
                ServiceTrace.Source.WriteInfo(
                    TraceType,
                    "{0} CacheCleanupTimer: {1} items out of {2} cleaned",
                    this.traceId,
                    totalItemsCleaned,
                    totalItemsInCache);
            }

            // Re-arm the clean-up timer.
            this.cacheCleanupTimer.Change(this.GetNextCleanupTimerDueTimeSeconds(), TimeSpan.FromMilliseconds(-1));
        }

        private sealed class PartitionClientCache
        {
            private readonly string traceId;
            private readonly Guid partitionId;

            private TimeSpan cacheEntryLockWaitTimeForCleanup = TimeSpan.FromMilliseconds(500);

            // The max size of the dictionary is
            // the number of Listeners per replica * number of replicas in a partition.
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
                        Rsp = rsp,
                    });
            }

            public override bool Equals(object obj)
            {
                var other = obj as PartitionClientCache;

                // Two Empty cache entries for same partition are equal. This we needed for removing empty entries
                if (other.cache.Count == 0 && this.cache.Count == 0)
                {
                    if (this.partitionId.Equals(other.partitionId))
                    {
                        return true;
                    }
                }

                return base.Equals(obj);
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

            // Cleaning up of cache entries needs to synchronize with the code that uses the cache entry and sets the
            // communicationClient inside the cache entry. So the removal of the cache-entry should set entry.IsInCache to false
            // and also remove the entry while holding the entry's semaphore, so that we don't purge a cache entry that has a valid
            // communication client.
            public void CleanupCacheEntries(out int totalNumberOfItems, out int numberOfEntriesCleaned)
            {
                totalNumberOfItems = 0;
                numberOfEntriesCleaned = 0;
                foreach (var entry in this.cache)
                {
                    ++totalNumberOfItems;

                    if (!entry.Value.IsCommunicationClientValid())
                    {
                        if (!entry.Value.Semaphore.Wait(this.cacheEntryLockWaitTimeForCleanup))
                        {
                            // If the wait cannot be satisfied in a short time, then it indicates usage for
                            // this cache entry, so it is ok to skip this entry in the current cleanup run.
                            continue;
                        }

                        try
                        {
                            ServiceTrace.Source.WriteNoise(
                                TraceType,
                                "{0} CleanupCacheEntries for partitionid {1} endpoint {2} : {3}",
                                this.traceId,
                                this.partitionId,
                                entry.Key.ListenerName,
                                entry.Key.Endpoint);

                            entry.Value.IsInCache = false;

                            this.cache.TryRemove(entry.Key, out var removedValue);
                            ++numberOfEntriesCleaned;
                        }
                        finally
                        {
                            entry.Value.Semaphore.Release();
                        }
                    }

                }
            }

            public bool IsEmpty()
            {
                return this.cache.IsEmpty;
            }

            public override int GetHashCode()
            {
                return this.partitionId.GetHashCode();
            }
        }

        private sealed class PartitionClientCacheKey
        {
            private readonly ResolvedServiceEndpoint endpoint;
            private readonly string listenerName;

            public PartitionClientCacheKey(ResolvedServiceEndpoint endpoint, string listenerName)
            {
                this.endpoint = endpoint;
                this.listenerName = listenerName;
            }

            public ResolvedServiceEndpoint Endpoint
            {
                get
                {
                    return this.endpoint;
                }
            }

            public string ListenerName
            {
                get
                {
                    return this.listenerName;
                }
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
