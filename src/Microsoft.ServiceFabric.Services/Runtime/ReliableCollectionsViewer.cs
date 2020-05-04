// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{


    public abstract class ReliableCollectionsViewer : StatefulService
    {
        public async Task<IActionResult> GetCollections()
        {
            var collectionsEnumerator = this.stateManager.GetAsyncEnumerator();
            CancellationToken ct = new CancellationToken();

            var collections = new List<string>();

            while (await collectionsEnumerator.MoveNextAsync(ct))
            {
                var current = collectionsEnumerator.Current;

                string collectionName = current.Name.AbsolutePath;
                string collectionType = GetCollectionType(current);
                String collection = String.Format("Collection name: {0} Type: {1}", collectionName, collectionType);

                collections.Add(collection);
            }

            return this.Json(collections);
        }

        public async Task<IActionResult> GetItem(string collectionName, string dllPath, string serializerTypeStr, byte[] serializedKey, string keyTypeStr, string valueTypeStr)
        {
            var result = this.Json(String.Empty);

            var tryGetAsyncTask = await stateManager.TryGetAsync<IReliableState>(collectionName).ConfigureAwait(false);

            if (!tryGetAsyncTask.HasValue)
            {
                result = this.Json(String.Format("{0} does not exist.", collectionName));

                return this.Json(result);
            }

            var collection = tryGetAsyncTask.Value;

            using (ITransaction txn = this.stateManager.CreateTransaction())
            {
                var assembly = Assembly.LoadFrom(dllPath);

                var serializer = GetSerializer(assembly, collection.Name);

                var serializerType = assembly.GetType(serializerTypeStr);
                var keyType = assembly.GetType(keyTypeStr);
                var valueType = assembly.GetType(valueTypeStr);

                dynamic item = null;

                if (collection.ToString().Contains("DistributedDictionary"))
                {
                    var deserializedKey = GetDeserializedItem(serializer, serializerType, serializedKey);

                    Type reliableDictionaryType = typeof(IReliableDictionary<,>).MakeGenericType(keyType, valueType);

                    MethodInfo tryGetValueAsyncMethod = reliableDictionaryType.GetMethod("TryGetValueAsync", new[] { typeof(ITransaction), keyType, typeof(LockMode) });
                    var tryGetValueAsyncTask = (Task)tryGetValueAsyncMethod.Invoke(collection, new object[] { txn, deserializedKey, LockMode.Update });
                    await tryGetValueAsyncTask.ConfigureAwait(false);

                    item = ((dynamic)tryGetValueAsyncTask).Result;
                }
                else if (collection.ToString().Contains("DistributedQueue"))
                {
                    Type reliableQueueType = typeof(IReliableQueue<>).MakeGenericType(keyType);

                    MethodInfo tryPeekAsyncMethod = reliableQueueType.GetMethod("TryPeekAsync", new[] { typeof(ITransaction), typeof(LockMode) });
                    var tryPeekAsyncTask = (Task)tryPeekAsyncMethod.Invoke(collection, new object[] { txn, LockMode.Update });
                    await tryPeekAsyncTask.ConfigureAwait(false);

                    item = ((dynamic)tryPeekAsyncTask).Result;
                }
                else if (collection.ToString().Contains("ReliableConcurrentQueue"))
                {
                    Type reliableConcurrentQueueType = typeof(IReliableConcurrentQueue<>).MakeGenericType(keyType);

                    MethodInfo tryDequeueAsyncMethod = reliableConcurrentQueueType.GetMethod("TryDequeueAsync", new[] { typeof(ITransaction), typeof(CancellationToken), typeof(Nullable<TimeSpan>) });
                    var tryDequeueAsyncTask = (Task)tryDequeueAsyncMethod.Invoke(collection, new object[] { txn, null, null });
                    await tryDequeueAsyncTask.ConfigureAwait(false);

                    item = ((dynamic)tryDequeueAsyncTask).Result;

                    txn.Abort();
                }

                if (!item.HasValue)
                {
                    result = this.Json(String.Format("Item does not exist in {0}", collectionName));

                    return result;
                }

                var value = item.Value;
                var serializedValue = GetSerializedItem(serializer, serializerType, value, valueType);
                result = this.Json(serializedValue);
            }

            return result;
        }

        public async Task<IActionResult> GetItems(string collectionName, string dllPath, string serializerTypeStr, byte[] serializedKey, string keyTypeStr, string valueTypeStr, int count)
        {
            var list = new List<string>();
            var result = this.Json(list);

            var tryGetAsyncTask = await stateManager.TryGetAsync<IReliableState>(collectionName).ConfigureAwait(false);

            if (!tryGetAsyncTask.HasValue)
            {
                result = this.Json(String.Format("{0} does not exist.", collectionName));

                return this.Json(result);
            }

            var collection = tryGetAsyncTask.Value;

            using (ITransaction txn = this.stateManager.CreateTransaction())
            {
                var assembly = Assembly.LoadFrom(dllPath);

                var serializer = GetSerializer(assembly, collection.Name);

                var serializerType = assembly.GetType(serializerTypeStr);
                var keyType = assembly.GetType(keyTypeStr);
                var valueType = assembly.GetType(valueTypeStr);

                if (collection.ToString().Contains("DistributedDictionary"))
                {
                    Type reliableDictionaryType = typeof(IReliableDictionary<,>).MakeGenericType(keyType, valueType);

                    MethodInfo createEnumerableAsyncMethod = reliableDictionaryType.GetMethod("CreateEnumerableAsync", new[] { typeof(ITransaction) });
                    var createEnumerableAsyncTask = (Task)createEnumerableAsyncMethod.Invoke(collection, new object[] { txn });
                    await createEnumerableAsyncTask.ConfigureAwait(false);
                    var enumerable = ((dynamic)createEnumerableAsyncTask).Result;

                    Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);

                    Type iAsyncEnumerableType = typeof(IAsyncEnumerable<>).MakeGenericType(keyValuePairType);
                    var getAsyncEnumeratorMethod = iAsyncEnumerableType.GetMethod("GetAsyncEnumerator");
                    var enumerator = getAsyncEnumeratorMethod.Invoke(enumerable, null);

                    Type iAsyncEnumeratorType = typeof(IAsyncEnumerator<>).MakeGenericType(keyValuePairType);
                    var moveNextAsyncMethod = iAsyncEnumeratorType.GetMethod("MoveNextAsync", new[] { typeof(CancellationToken) });

                    var isAdvanced = false;
                    CancellationToken ct = new CancellationToken();

                    do
                    {
                        var moveNextAsyncMethodInvokeResult = moveNextAsyncMethod.Invoke(enumerator, new object[] { ct });
                        isAdvanced = moveNextAsyncMethodInvokeResult.Result;

                        if (isAdvanced)
                        {
                            var current = enumerator.GetType().GetProperty("Current").GetValue(enumerator);
                            var currentDeserializedValue = current.GetType().GetProperty("Value").GetValue(current);
                            var currentSerializedValue = GetSerializedItem(serializer, serializerType, currentDeserializedValue, valueType);

                            list.Add(this.Json(currentSerializedValue));

                            count--;
                        }

                    } while (isAdvanced && count > 0);
                }

                result = this.Json(list);
            }

            return result;
        }

        private string GetCollectionType(IReliableState reliableState)
        {
            var collectionFullType = reliableState.GetType().ToString();
            var collectionType = String.Empty;

            if (collectionFullType.Contains("DistributedDictionary"))
            {
                collectionType = "IReliableDictionary";
            }
            else if (collectionFullType.Contains("DistributedQueue"))
            {
                collectionType = "IReliableQueue";
            }
            else if (collectionFullType.Contains("ReliableConcurrentQueue"))
            {
                collectionType = "IReliableConcurrentQueue";
            }

            return collectionType;
        }

        private object GetDeserializedItem(object serializer, Type serializerType, byte[] serializedItem)
        {
            var readMethod = serializerType.GetMethod("Read", new[] { typeof(BinaryReader) });

            object deserializedItem = null;
            using (var memoryStream = new MemoryStream(serializedItem))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    deserializedItem = readMethod.Invoke(serializer, new object[] { binaryReader });
                }
            }

            return deserializedItem;
        }

        private object GetSerializer(Assembly assembly, Uri collectionUriName)
        {
            var stateManagerType = typeof(IReliableStateManager);
            var stateManagerReplicatorField = stateManagerType.GetField("transactionalReplicator", BindingFlags.NonPublic | BindingFlags.Instance);

            var replicator = stateManagerReplicatorField.GetValue(stateManager);
            var replicatorType = stateManagerReplicatorField.FieldType;

            var getStateSerializerMethod = replicatorType.GetMethod("GetStateSerializer", new[] { typeof(Uri) });
            var serializer = getStateSerializerMethod.Invoke(replicator, new object[] { collectionUriName });

            return serializer;
        }

        private object GetSerializedItem(object serializer, Type serializerType, object value, Type itemType)
        {
            var writeMethod = serializerType.GetMethod("Write", new[] { itemType, typeof(BinaryWriter) });

            object serializedItem = null;
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    serializedItem = writeMethod.Invoke(serializer, new object[] { value, binaryWriter });
                }
            }

            return serializedItem;
        }
    }
}
