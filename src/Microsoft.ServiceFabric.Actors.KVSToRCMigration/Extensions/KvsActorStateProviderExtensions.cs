// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.CodeAnalysis;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.MigrationConstants;

    internal static class KvsActorStateProviderExtensions
    {
        public static readonly string TombstoneCleanupMessage = "KeyValueStoreReplicaSettings.DisableTombstoneCleanup is either not enabled or set to false";
        private static readonly string TraceType = typeof(KvsActorStateProviderExtensions).Name;
        private static DataContractJsonSerializer responseSerializer = new DataContractJsonSerializer(typeof(EnumerationResponse), new[] { typeof(List<KeyValuePair>) });

        internal static async Task<long> GetFirstSequenceNumberAsync(this KvsActorStateProvider stateProvider, string traceId, CancellationToken cancellationToken)
        {
            stateProvider.ThrowIfTombCleanupIsNotEnabled();
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
                            cancellationToken.ThrowIfCancellationRequested();
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

        internal static async Task<long> GetLastSequenceNumberAsync(this KvsActorStateProvider stateProvider, string traceId, CancellationToken cancellationToken)
        {
            stateProvider.ThrowIfTombCleanupIsNotEnabled();
            cancellationToken.ThrowIfCancellationRequested();
            return await stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync<long>(
                async () =>
                {
                    return await Task.Run(() =>
                    {
                        return stateProvider.GetStoreReplica().GetLastCommittedSequenceNumber();
                    });
                },
                "GetLastSequenceNumber",
                cancellationToken);
        }

        internal static Task EnumerateAsync(this KvsActorStateProvider stateProvider, EnumerationRequest request, HttpResponse response, string traceId, CancellationToken cancellationToken)
        {
            stateProvider.ThrowIfTombCleanupIsNotEnabled();
            var storeReplica = stateProvider.GetStoreReplica();
            var lsn = storeReplica.GetLastCommittedSequenceNumber();
            return stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync(
                async () =>
                {
                    try
                    {
                        bool hasData;
                        bool endSequenceNumberReached = false;

                        using (var txn = storeReplica.CreateTransaction())
                        {
                            IEnumerator<KeyValueStoreItem> enumerator;

                            if (request.IncludeDeletes)
                            {
                                enumerator = storeReplica.EnumerateKeysAndTombstonesBySequenceNumber(txn, request.StartSequenceNumber);
                            }
                            else
                            {
                                enumerator = storeReplica.EnumerateBySequenceNumber(txn, request.StartSequenceNumber);
                            }

                            hasData = enumerator.MoveNext();
                            int chunk = 1;
                            while (chunk <= request.NumberOfChunksPerEnumeration && !endSequenceNumberReached)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var pairs = new List<KeyValuePair>();
                                var sequenceNumberFullyDrained = true;
                                long? firstSNInChunk = null;
                                long? endSNInChunk = null;

                                while (hasData
                                    && !endSequenceNumberReached
                                    && (pairs.Count < request.ChunkSize || !sequenceNumberFullyDrained))
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                    if (firstSNInChunk == null)
                                    {
                                        firstSNInChunk = enumerator.Current.Metadata.SequenceNumber;
                                    }

                                    var keyValuePair = MakeKeyValuePair(enumerator.Current);
                                    var currentSequenceNumber = keyValuePair.Version;
                                    pairs.Add(keyValuePair);
                                    hasData = enumerator.MoveNext();

                                    if (hasData)
                                    {
                                        var nextKeyValuePair = enumerator.Current;
                                        var nextKeySequenceNumber = nextKeyValuePair.Metadata.SequenceNumber;
                                        endSequenceNumberReached = nextKeySequenceNumber > request.EndSequenceNumber;
                                        sequenceNumberFullyDrained = !(nextKeySequenceNumber == currentSequenceNumber);
                                        //// TODO : sequenceNumberFullyDrained??
                                    }

                                    endSNInChunk = currentSequenceNumber;
                                }

                                await WriteKeyValuePairsToResponse(pairs, endSequenceNumberReached, response);
                                ++chunk;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ActorTrace.Source.WriteErrorWithId(TraceType, traceId,  $"{e.Message} {e.StackTrace}");
                        if (e.InnerException != null)
                        {
                            ActorTrace.Source.WriteErrorWithId(TraceType, traceId, $"{e.InnerException.Message} {e.InnerException.StackTrace}/n");
                        }
                    }
                },
                "EnumerateAsync",
                cancellationToken);
        }

        internal static async Task<bool> TryAbortExistingTransactionsAndRejectWritesAsync(this KvsActorStateProvider stateProvider, string traceId, CancellationToken cancellationToken)
        {
            stateProvider.ThrowIfTombCleanupIsNotEnabled();
            cancellationToken.ThrowIfCancellationRequested();
            return await stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync<bool>(
                async () =>
                {
                    return await Task.Run(() =>
                    {
                        return stateProvider.GetStoreReplica().TryAbortExistingTransactionsAndRejectWrites();
                    });
                },
                "TryAbortExistingTransactionsAndRejectWrites",
                cancellationToken);
        }

        internal static async Task RejectWritesAsync(this KvsActorStateProvider stateProvider, string traceId, CancellationToken cancellationToken)
        {
            stateProvider.ThrowIfTombCleanupIsNotEnabled();
            cancellationToken.ThrowIfCancellationRequested();
            await stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync(
                async () =>
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
                },
                "RejectWritesAsync",
                cancellationToken);
        }

        internal static async Task ResumeWritesAsync(this KvsActorStateProvider stateProvider, string traceId, CancellationToken cancellationToken)
        {
            stateProvider.ThrowIfTombCleanupIsNotEnabled();
            cancellationToken.ThrowIfCancellationRequested();
            await stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync(
               async () =>
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
               },
               "ResumeWritesAsync",
               cancellationToken);
        }

        internal static async Task<bool> GetRejectWriteStateAsync(this KvsActorStateProvider stateProvider, string traceId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await stateProvider.GetActorStateProviderHelper().ExecuteWithRetriesAsync<bool>(
               async () =>
               {
                   return await Task.Run(() =>
                   {
                       using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
                       {
                           var result = stateProvider.GetStoreReplica().TryGet(tx, RejectWritesKey);

                           if (result != null)
                           {
                               return BitConverter.ToBoolean(result.Value, 0);
                           }
                       }

                       return false;
                   });
               },
               "GetRejectWriteStateAsync",
               cancellationToken);
        }

        internal static bool IsTombstoneCleanupDisabled(this KvsActorStateProvider stateProvider)
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
                                foreach (var key in keys)
                                {
                                    using var tx = stateProvider.GetStoreReplica().CreateTransaction();
                                    var result = stateProvider.GetStoreReplica().TryGet(tx, key);
                                    await tx.CommitAsync();

                                    if (result == null)
                                    {
                                        throw new MigrationDataValidationException($"Could not find key: {key} in KVS");
                                    }

                                    pairs.Add(new KeyValuePair() { Key = key, Value = result.Value });
                                }
                            }

                            await WriteKeyValuePairsToResponse(pairs, true, response); // TODO: Remove this when merging data validation PR
                        },
                        "GetValueByKeyAsync",
                        cancellationToken);
        }

        private static async Task WriteKeyValuePairsToResponse(List<KeyValuePair> pairs, bool endSequenceNumberReached, HttpResponse response)
        {
            var byteArray = SerializationUtility.Serialize(responseSerializer, new EnumerationResponse
            {
                KeyValuePairs = pairs,
                EndSequenceNumberReached = endSequenceNumberReached,
            });
            var newLine = Encoding.UTF8.GetBytes("\n");

            ActorTrace.Source.WriteNoise(TraceType, $"ByteArray: {byteArray} ArrayLength: {byteArray.Length}");

            if (string.IsNullOrEmpty(response.ContentType))
            {
                // Set the content type
                response.ContentType = "application/json; charset=utf-8";
            }

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

        private static void ThrowIfTombCleanupIsNotEnabled(this KvsActorStateProvider stateProvider)
        {
            if (!stateProvider.IsTombstoneCleanupDisabled())
            {
                throw new InvalidMigrationConfigException(TombstoneCleanupMessage);
            }
        }
    }
}
