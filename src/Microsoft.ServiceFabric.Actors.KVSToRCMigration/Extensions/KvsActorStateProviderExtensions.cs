// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Runtime.Serialization.Json;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
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
                                var keyHash = new SHA512Managed();
                                var valueHash = new SHA512Managed();

                                while (hasData
                                    && !endSequenceNumberReached
                                    && (pairs.Count < request.ChunkSize || !sequenceNumberFullyDrained))
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                    if (firstSNInChunk == null)
                                    {
                                        firstSNInChunk = enumerator.Current.Metadata.SequenceNumber;
                                    }

                                    var keyValuePair = await MakeKeyValuePairAsync(stateProvider, enumerator.Current, request, cancellationToken);
                                    if (request.ComputeHash
                                        && !keyValuePair.IsDeleted
                                        && !MigrationUtility.IgnoreKey(keyValuePair.Key))
                                    {
                                        // var keyBuffer = Encoding.UTF8.GetBytes(keyValuePair.Key);
                                        // keyHash.TransformBlock(keyBuffer, 0, keyBuffer.Length, null, 0);
                                        valueHash.TransformBlock(keyValuePair.Value, 0, keyValuePair.Value.Length, null, 0);
                                    }

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

                                keyHash.TransformFinalBlock(new byte[0], 0, 0);
                                valueHash.TransformFinalBlock(new byte[0], 0, 0);

                                var enumerationResponse = new EnumerationResponse()
                                {
                                    KeyValuePairs = pairs,
                                    KeyHash = keyHash.Hash,
                                    ValueHash = valueHash.Hash,
                                };

                                await WriteKeyValuePairsToResponseAsync(
                                    new EnumerationResponse
                                    {
                                        KeyValuePairs = pairs,
                                        EndSequenceNumberReached = endSequenceNumberReached,
                                        ResolveActorIdsForStateKVPairs = request.ResolveActorIdsForStateKVPairs,
                                        KeyHash = keyHash.Hash,
                                        ValueHash = valueHash.Hash,
                                    },
                                    response);
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
                () =>
                {
                    if (!stateProvider.GetStoreReplica().TryAbortExistingTransactionsAndRejectWrites())
                    {
                        throw new FabricTransientException("Unable to abort exiting transactions.");
                    }

                    return Task.CompletedTask;
                },
                "TryAbortExistingTransactionsAndRejectWrites",
                cancellationToken);

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

        private static async Task WriteKeyValuePairsToResponseAsync(EnumerationResponse enumerationResponse, HttpResponse httpResponse)
        {
            var byteArray = SerializationUtility.Serialize(responseSerializer, enumerationResponse);
            var newLine = Encoding.UTF8.GetBytes("\n");

            ActorTrace.Source.WriteNoise(TraceType, $"ByteArray: {byteArray} ArrayLength: {byteArray.Length}");

            if (string.IsNullOrEmpty(httpResponse.ContentType))
            {
                // Set the content type
                httpResponse.ContentType = "application/json; charset=utf-8";
            }

            await httpResponse.Body.WriteAsync(byteArray, 0, byteArray.Length);
            await httpResponse.Body.WriteAsync(newLine, 0, newLine.Length);
            await httpResponse.Body.FlushAsync();
        }

        private static async Task<KeyValuePair> MakeKeyValuePairAsync(this KvsActorStateProvider stateProvider, KeyValueStoreItem item, EnumerationRequest request, CancellationToken cancellationToken)
        {
            bool isDeleted = item.Metadata.ValueSizeInBytes < 0;
            var result = new KeyValuePair
            {
                IsDeleted = isDeleted,
                Version = item.Metadata.SequenceNumber,
                Key = item.Metadata.Key,
                Value = isDeleted ? new byte[0] : item.Value,
            };

            var ambiguousActoIdHandler = new KVSAmbiguousActorIdHandler(stateProvider);
            if (request.ResolveActorIdsForStateKVPairs && item.Metadata.Key.StartsWith("Actor_"))
            {
                var cv = await ambiguousActoIdHandler.TryResolveActorIdAsync(item.Metadata.Key, cancellationToken);
                result.ActorId = cv.HasValue ? cv.Value : null;
            }

            return result;
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
