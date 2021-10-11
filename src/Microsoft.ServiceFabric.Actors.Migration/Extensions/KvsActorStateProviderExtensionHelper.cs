// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal static class KvsActorStateProviderExtensionHelper
    {
        internal static async Task<long> GetFirstSequeceNumberAsync(this KvsActorStateProvider stateProvider, CancellationToken cancellationToken)
        {
            var storeReplica = stateProvider.GetStoreReplica();
            var lsn = storeReplica.GetLastCommittedSequenceNumber();
            return await stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync<long>(
                () =>
                {
                    using (var txn = storeReplica.CreateTransaction())
                    {
                        var enumerator = storeReplica.EnumerateBySequenceNumber(txn, 0);
                        var hasData = enumerator.MoveNext();

                        while (hasData)
                        {
                            if (enumerator.Current.Metadata.ValueSizeInBytes > 0)
                            {
                                return Task.FromResult(enumerator.Current.Metadata.SequenceNumber);
                            }

                            hasData = enumerator.MoveNext();
                        }
                    }

                    return Task.FromResult(lsn);
                },
                "GetFirstSequeceNumberAsync",
                cancellationToken);
        }

        internal static long GetLastSequeceNumber(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().GetLastCommittedSequenceNumber();
        }

        internal static Task EnumerateAsync(this KvsActorStateProvider stateProvider, EnumerationRequest request)
        {
            var storeReplica = stateProvider.GetStoreReplica();
            var lsn = storeReplica.GetLastCommittedSequenceNumber();
            return stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync(
                async () =>
                {
                    using (var txn = storeReplica.CreateTransaction())
                    {
                        IEnumerator<KeyValueStoreItem> enumerator;

                        if (request.IncludeDeletes)
                        {
                            enumerator = storeReplica.EnumerateKeysAndTombstonesBySequenceNumber(txn, request.StartSN);
                        }
                        else
                        {
                            enumerator = storeReplica.EnumerateBySequenceNumber(txn, request.StartSN);
                        }

                        var hasData = enumerator.MoveNext();
                        long enumerationKeyCount = 0;

                        while (hasData && enumerationKeyCount < request.NoOfItems)
                        {
                            var pairs = new List<KeyValuePair>();

                            var sequenceNumberFullyDrained = true;
                            while (hasData && (pairs.Count < request.ChunkSize || !sequenceNumberFullyDrained))
                            {
                                var keyValuePair = MakeKeyValuePair(enumerator.Current);
                                var currentSequenceNumber = keyValuePair.Version;

                                pairs.Add(keyValuePair);
                                enumerationKeyCount++;

                                hasData = enumerator.MoveNext();

                                if (hasData)
                                {
                                    var nextKeyValuePair = enumerator.Current;
                                    var nextKeySequenceNumber = nextKeyValuePair.Metadata.SequenceNumber;
                                    sequenceNumberFullyDrained = !(nextKeySequenceNumber == currentSequenceNumber);
                                }
                            }

                            ////await responseStream.WriteAsync(pairs);
                            await Task.Delay(1);
                        }
                    }
                },
                "EnumerateAsync",
                CancellationToken.None);
        }

        internal static bool TryAbortExistingTransactionsAndRejectWrites(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().TryAbortExistingTransactionsAndRejectWrites();
        }

        internal static async Task SaveKvsRejectWriteStatusAsync(this KvsActorStateProvider stateProvider, bool ready)
        {
            using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
            {
                if (stateProvider.GetStoreReplica().TryGet(tx, Constants.RejectWritesKey) != null)
                {
                    stateProvider.GetStoreReplica().TryUpdate(tx, Constants.RejectWritesKey, BitConverter.GetBytes(ready));
                }
                else
                {
                    stateProvider.GetStoreReplica().TryAdd(tx, Constants.RejectWritesKey, BitConverter.GetBytes(ready));
                }

                await tx.CommitAsync();
            }
        }

        internal static bool GetKvsRejectWriteStatusAsync(this KvsActorStateProvider stateProvider)
    {
        using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
        {
            var result = stateProvider.GetStoreReplica().TryGet(tx, Constants.RejectWritesKey);

            if (result != null)
            {
                return BitConverter.ToBoolean(result.Value, 0);
            }
        }

        return false;
    }

        private static KeyValuePair MakeKeyValuePair(KeyValueStoreItem item)
        {
            bool isDeleted = item.Metadata.ValueSizeInBytes < 0;

            return new KeyValuePair
            {
                IsDeleted = isDeleted,
                Version = item.Metadata.SequenceNumber,
                Key = item.Metadata.Key,
                Value = isDeleted ? new byte[0] : item.Value,
            };
        }
    }
}
