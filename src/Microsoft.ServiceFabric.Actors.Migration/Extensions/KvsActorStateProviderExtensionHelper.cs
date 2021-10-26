// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
#if DotNetCoreClr
    using Microsoft.AspNetCore.Http;
#endif
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

        internal static long GetLastSequenceNumber(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().GetLastCommittedSequenceNumber();
        }

#if DotNetCoreClr
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
                        var keyvaluepairserializer = new DataContractSerializer(typeof(List<KeyValuePair>));

                        do
                        {
                            var pairs = new List<KeyValuePair>();
                            var sequenceNumberFullyDrained = true;

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
                            }

                            using var memoryStream = new MemoryStream();
                            var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
                            keyvaluepairserializer.WriteObject(binaryWriter, pairs);
                            binaryWriter.Flush();

                            var byteArray = memoryStream.ToArray();

                            ActorTrace.Source.WriteInfo("KvsActorStateProviderExtensionHelper", $"ByteArray: {byteArray} ArrayLength: {byteArray.Length} StreamLength: {memoryStream.Length}");

                            // Set the content type
                            response.ContentType = "application/xml; charset=utf-8";
                            await response.Body.WriteAsync(byteArray, 0, byteArray.Length);
                            await response.Body.FlushAsync();
                        }
                        while (hasData && enumerationKeyCount < request.NoOfItems);
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
#endif

        internal static bool TryAbortExistingTransactionsAndRejectWrites(this KvsActorStateProvider stateProvider)
        {
            return stateProvider.GetStoreReplica().TryAbortExistingTransactionsAndRejectWrites();
        }

        internal static async Task RejectWritesAsync(this KvsActorStateProvider stateProvider)
        {
            using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
            {
                if (stateProvider.GetStoreReplica().TryGet(tx, Constants.RejectWritesKey) != null)
                {
                    stateProvider.GetStoreReplica().TryUpdate(tx, Constants.RejectWritesKey, BitConverter.GetBytes(true));
                }
                else
                {
                    stateProvider.GetStoreReplica().TryAdd(tx, Constants.RejectWritesKey, BitConverter.GetBytes(true));
                }

                await tx.CommitAsync();
            }
        }

        internal static async Task ResumeWritesAsync(this KvsActorStateProvider stateProvider)
        {
            using (var tx = stateProvider.GetStoreReplica().CreateTransaction())
            {
                if (stateProvider.GetStoreReplica().TryGet(tx, Constants.RejectWritesKey) != null)
                {
                    stateProvider.GetStoreReplica().TryUpdate(tx, Constants.RejectWritesKey, BitConverter.GetBytes(false));
                }
                else
                {
                    stateProvider.GetStoreReplica().TryAdd(tx, Constants.RejectWritesKey, BitConverter.GetBytes(false));
                }

                await tx.CommitAsync();
            }
        }

        internal static bool GetWriteStatusAsync(this KvsActorStateProvider stateProvider)
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
