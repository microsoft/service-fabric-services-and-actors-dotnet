// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.AspNetCore.Http;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;

    internal static class KvsActorStateProviderExtensions
    {
        private static DataContractSerializer keyValuePairSerializer = new DataContractSerializer(typeof(List<KeyValuePair>));

        internal static async Task<long> GetFirstSequenceNumberAsync(this KvsActorStateProvider stateProvider, CancellationToken cancellationToken)
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

        internal static long GetLastSequenceNumber(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().GetLastCommittedSequenceNumber();
        }

        internal static Task EnumerateAsync(this KvsActorStateProvider stateProvider, EnumerationRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            var storeReplica = stateProvider.GetStoreReplica();
            var lsn = storeReplica.GetLastCommittedSequenceNumber();
            return stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync(
                async () =>
                {
                    try
                    {
                        bool hasData;
                        long enumerationKeyCount = 0;

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

                            hasData = enumerator.MoveNext();

                            do
                            {
                                var pairs = new List<KeyValuePair>();
                                var sequenceNumberFullyDrained = true;
                                long? firstSNInChunk = null;
                                long? endSNInChunk = null;

                                while (hasData && (pairs.Count < request.ChunkSize || !sequenceNumberFullyDrained))
                                {
                                    if (firstSNInChunk == null)
                                    {
                                        firstSNInChunk = enumerator.Current.Metadata.SequenceNumber;
                                    }

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

                                    endSNInChunk = currentSequenceNumber;
                                }

                                await WriteKeyValuePairsToResponse(pairs, response);
                            }
                            while (hasData && enumerationKeyCount < request.NoOfItems);
                        }
                    }
                    catch (Exception e)
                    {
                        ActorTrace.Source.WriteError("KvsActorStateProviderExtensionHelper", $"{e.Message} {e.StackTrace}");
                        if (e.InnerException != null)
                        {
                            ActorTrace.Source.WriteError("KvsActorStateProviderExtensionHelper", $"{e.InnerException.Message} {e.InnerException.StackTrace}/n");
                        }
                    }
                },
                "EnumerateAsync",
                cancellationToken);
        }

        internal static bool TryAbortExistingTransactionsAndRejectWrites(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().TryAbortExistingTransactionsAndRejectWrites();
        }

        internal static async Task RejectWritesAsync(this KvsActorStateProvider stateProvider)
        {
            using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
            {
                if (stateProvider.GetStoreReplica().TryGet(tx, RejectWritesKey) != null)
                {
                    stateProvider.GetStoreReplica().TryUpdate(tx, RejectWritesKey, BitConverter.GetBytes(true));
                }
                else
                {
                    stateProvider.GetStoreReplica().TryAdd(tx, RejectWritesKey, BitConverter.GetBytes(true));
                }

                await tx.CommitAsync();
            }
        }

        internal static async Task ResumeWritesAsync(this KvsActorStateProvider stateProvider)
        {
            using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
            {
                if (stateProvider.GetStoreReplica().TryGet(tx, RejectWritesKey) != null)
                {
                    stateProvider.GetStoreReplica().TryUpdate(tx, RejectWritesKey, BitConverter.GetBytes(false));
                }
                else
                {
                    stateProvider.GetStoreReplica().TryAdd(tx, RejectWritesKey, BitConverter.GetBytes(false));
                }

                await tx.CommitAsync();
            }
        }

        internal static bool GetRejectWriteState(this KvsActorStateProvider stateProvider)
        {
            using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
            {
                // TODO: Consider caching this value.
                var result = stateProvider.GetStoreReplica().TryGet(tx, RejectWritesKey);

                if (result != null)
                {
                    return BitConverter.ToBoolean(result.Value, 0);
                }
            }

            return false;
        }

        internal static bool GetDisableTombstoneCleanupSetting(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().KeyValueStoreReplicaSettings.DisableTombstoneCleanup;
        }

        internal static Task GetValueByKeysAsync(this KvsActorStateProvider stateProvider, List<string> keys, HttpResponse response, CancellationToken cancellationToken)
        {
            var pairs = new List<KeyValuePair>();
            return stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync(
                        async () =>
                        {
                            if (keys.Any())
                            {
                                using var tx = stateProvider.GetStoreReplica().CreateTransaction();

                                foreach (var key in keys)
                                {
                                    var result = stateProvider.GetStoreReplica().TryGet(tx, key);

                                    if (result == null)
                                    {
                                        throw new ActorStateMigratedDataValidationFailedException($"Could not find key: {key} in KVS");
                                    }

                                    pairs.Add(new KeyValuePair() { Key = key, Value = result.Value });
                                }
                            }

                            await WriteKeyValuePairsToResponse(pairs, response);
                        },
                        "GetValueByKeyAsync",
                        cancellationToken);
        }

        private static async Task WriteKeyValuePairsToResponse(List<KeyValuePair> pairs, HttpResponse response)
        {
            using var memoryStream = new MemoryStream();
            var binaryWriter = XmlDictionaryWriter.CreateTextWriter(memoryStream);
            keyValuePairSerializer.WriteObject(binaryWriter, pairs);
            binaryWriter.Flush();

            var byteArray = memoryStream.ToArray();
            var newLine = Encoding.ASCII.GetBytes("\n");

            ActorTrace.Source.WriteNoise("KvsActorStateProviderExtensionHelper", $"ByteArray: {byteArray} ArrayLength: {byteArray.Length} StreamLength: {memoryStream.Length}");

            // Set the content type
            response.ContentType = "application/xml; charset=utf-8";
            await response.Body.WriteAsync(byteArray, 0, byteArray.Length);
            await response.Body.WriteAsync(newLine, 0, newLine.Length);
            await response.Body.FlushAsync();
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
