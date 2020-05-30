// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the base class for Microsoft Service Fabric based stateful reliable service
    /// which provides an <see cref="IReliableStateManager"/> to manage service's state.
    /// Derive from this class to implement a Microsoft Service Fabric based stateful reliable service.
    /// </summary>
    public abstract class StatefulService : StatefulServiceBase
    {
        private readonly IReliableStateManager stateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulService"/> class with default reliable state manager (<see cref="ReliableStateManager"/>).
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> describes the stateful service context, which it provides information like replica ID, partition ID, and service name.
        /// </param>
        protected StatefulService(StatefulServiceContext serviceContext)
            : this(serviceContext, new ReliableStateManager(serviceContext))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulService"/> class with non-default reliable state manager replica.
        /// </summary>
        /// <param name="serviceContext">
        /// A <see cref="StatefulServiceContext"/> describes the stateful service context, which it provides information like replica ID, partition ID, and service name.
        /// </param>
        /// <param name="reliableStateManagerReplica">
        /// A <see cref="IReliableStateManagerReplica"/> represents a reliable state provider replica.
        /// </param>
        protected StatefulService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica)
            : base(serviceContext, reliableStateManagerReplica)
        {
            this.stateManager = reliableStateManagerReplica;
        }

        /// <summary>
        /// Gets this replica's <see cref="IReliableStateManager"/>.
        /// </summary>
        /// <value>The <see cref="IReliableStateManager"/> of the replica.</value>
        public IReliableStateManager StateManager
        {
            get { return this.stateManager; }
        }

        /// <summary>
        /// Gets all reliable collections.
        /// </summary>
        /// <returns>
        /// A serialized list of realiable collections.
        /// </returns>
        public async Task<string> GetCollections()
        {
            var collectionsEnumerator = this.stateManager.GetAsyncEnumerator();
            CancellationToken ct = CancellationToken.None;

            var collections = new List<string>();

            while (await collectionsEnumerator.MoveNextAsync(ct))
            {
                var current = collectionsEnumerator.Current;

                string collectionName = current.Name.AbsolutePath;
                string collectionType = GetCollectionType(current);
                string collection = string.Format("Collection name: {0} Type: {1}", collectionName, collectionType);

                collections.Add(collection);
            }

            return JsonConvert.SerializeObject(collections);
        }

        /// <summary>
        /// Gets the value corresponding to the given key in the case of the given reliable collection
        /// being a <see cref="IReliableDictionary{TKey, TValue}"/> and gets the first item from
        /// the given collection in the case of the collection being a <see cref="IReliableQueue{T}"/>
        /// or a <see cref="IReliableConcurrentQueue{T}"/>.
        /// </summary>
        /// <param name="collectionName">
        /// Name of the reliable collection.
        /// </param>
        /// <param name="keyTypeStr">
        /// Name of the type of key.
        /// </param>
        /// <param name="valueTypeStr">
        /// Name of the type of value.
        /// </param>
        /// <param name="keySerializerTypeStr">
        /// Name of the type of serializer correspoding to the key.
        /// </param>
        /// <param name="valueSerializerTypeStr">
        /// Name of the type of serializer corresponding to the value.
        /// </param>
        /// <param name="serializedKey">
        /// A key in the reliable collection.
        /// </param>
        /// <returns>
        /// A serialized value.
        /// </returns>
        public async Task<string> GetItem(string collectionName, string keyTypeStr, string valueTypeStr, string keySerializerTypeStr, string valueSerializerTypeStr, byte[] serializedKey)
        {
            var result = JsonConvert.SerializeObject(string.Empty);

            var tryGetAsyncTask = await this.stateManager.TryGetAsync<IReliableState>(collectionName).ConfigureAwait(false);
            if (!tryGetAsyncTask.HasValue)
            {
                result = JsonConvert.SerializeObject(string.Format("{0} does not exist.", collectionName));

                return result;
            }

            var collection = tryGetAsyncTask.Value;

            Type keySerializerType = keySerializerTypeStr != null ? Type.GetType(keySerializerTypeStr) : null;
            Type valueSerializerType = valueSerializerTypeStr != null ? Type.GetType(valueSerializerTypeStr) : null;
            Type keyType = keyTypeStr != null ? Type.GetType(keyTypeStr) : null;
            Type valueType = Type.GetType(valueTypeStr);

            if (!AreValidTypes(keyType, valueType, keySerializerType, valueSerializerType, true))
            {
                result = JsonConvert.SerializeObject("Invalid type(s).");

                return result;
            }

            object keySerializer = keySerializerType != null ? this.GetSerializer(collection.Name, keyType) : null;
            object valueSerializer = valueSerializerType != null ? this.GetSerializer(collection.Name, valueType) : null;

            var deserializedKey = GetDeserializedItem(keySerializer, keySerializerType, serializedKey);
            if (deserializedKey == null)
            {
                result = JsonConvert.SerializeObject(string.Format("Value is not deserialized.", collectionName));

                return result;
            }

            dynamic item = await this.TryGetItemAsync(collection, keyType, valueType, keySerializer, deserializedKey);
            if (item is null || !item.HasValue)
            {
                result = JsonConvert.SerializeObject(string.Format("Item does not exist in {0}", collectionName));

                return result;
            }

            var value = item.Value;
            var serializedValue = GetSerializedItem(valueSerializer, valueSerializerType, value, valueType);
            result = JsonConvert.SerializeObject(serializedValue);

            return result;
        }

        /// <summary>
        /// Gets the given count number of items, starting from the given first item, from the
        /// given relaible collection. Only supports <see cref="IReliableDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="collectionName">
        /// Name of the reliable collection.
        /// </param>
        /// <param name="keyTypeStr">
        /// Name of the type of key.
        /// </param>
        /// <param name="valueTypeStr">
        /// Name of the type of value.
        /// </param>
        /// <param name="keySerializerTypeStr">
        /// Name of the type of serializer correspoding to the key.
        /// </param>
        /// <param name="valueSerializerTypeStr">
        /// Name of the type of serializer corresponding to the value.
        /// </param>
        /// <param name="serializedFirstKey">
        /// The key starting with which items are to be obtained from the reliable collection.
        /// </param>
        /// <param name="count">
        /// Number of items to be obtained.
        /// </param>
        /// <returns>
        /// A serialized list of items.
        /// </returns>
        public async Task<string> GetItems(string collectionName, string keyTypeStr, string valueTypeStr, string keySerializerTypeStr, string valueSerializerTypeStr, byte[] serializedFirstKey, int count)
        {
            var list = new List<KeyValuePair<byte[], byte[]>>();
            var result = JsonConvert.SerializeObject(list);

            var tryGetAsyncTask = await this.stateManager.TryGetAsync<IReliableState>(collectionName).ConfigureAwait(false);
            if (!tryGetAsyncTask.HasValue)
            {
                result = JsonConvert.SerializeObject(string.Format("{0} does not exist.", collectionName));

                return JsonConvert.SerializeObject(result);
            }

            var collection = tryGetAsyncTask.Value;

            Type keySerializerType = keySerializerTypeStr != null ? Type.GetType(keySerializerTypeStr) : null;
            Type valueSerializerType = valueSerializerTypeStr != null ? Type.GetType(valueSerializerTypeStr) : null;
            Type keyType = keyTypeStr != null ? Type.GetType(keyTypeStr) : null;
            Type valueType = Type.GetType(valueTypeStr);

            if (!AreValidTypes(keyType, valueType, keySerializerType, valueSerializerType, IsDictionary(collection)))
            {
                result = JsonConvert.SerializeObject("Invalid type(s).");

                return result;
            }

            object keySerializer = keySerializerType != null ? this.GetSerializer(collection.Name, keyType) : null;
            object valueSerializer = valueSerializerType != null ? this.GetSerializer(collection.Name, valueType) : null;

            object deserializedFirstKey = null; // GetDeserializedItem(keySerializer, keySerializerType, serializedFirstKey);

            list = await this.TryGetItemsAsync(collection, keyType, valueType, keySerializerType, valueSerializerType, keySerializer, valueSerializer, deserializedFirstKey, count);
            result = JsonConvert.SerializeObject(list);

            return result;
        }

        private static bool AreValidTypes(Type keyType, Type valueType, Type keySerializerType, Type valueSerializerType, bool isDictionary)
        {
            if ((isDictionary && (keyType == null && keySerializerType == null)) ||
                (!isDictionary && (keyType != null || keySerializerType != null)) ||
                (isDictionary && IsSimpleType(keyType) && keySerializerType != null) ||
                (isDictionary && !IsSimpleType(keyType) && keySerializerType == null) ||
                valueType == null ||
                (IsSimpleType(valueType) && valueSerializerType != null) ||
                (!IsSimpleType(valueType) && valueSerializerType == null))
            {
                return false;
            }

            return true;
        }

        private static string GetCollectionType(IReliableState collection)
        {
            var collectionType = string.Empty;

            if (IsDictionary(collection))
            {
                collectionType = "IReliableDictionary";
            }
            else if (IsQueue(collection))
            {
                collectionType = "IReliableQueue";
            }
            else if (IsConcurrentQueue(collection))
            {
                collectionType = "IReliableConcurrentQueue";
            }

            return collectionType;
        }

        private static object GetDeserializedItem(object serializer, Type serializerType, byte[] serializedItem)
        {
            object deserializedItem = null;

            if (serializerType == null)
            {
                var formatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream(serializedItem))
                {
                    using (var binaryReader = new BinaryReader(memoryStream))
                    {
                        deserializedItem = formatter.Deserialize(memoryStream);
                    }
                }
            }
            else
            {
                var methods = serializerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

                // Invoke the deserializer function that takes in a binary reader as an argument
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Read") && method.GetParameters().Length == 1)
                    {
                        using (var memoryStream = new MemoryStream(serializedItem))
                        {
                            using (var binaryReader = new BinaryReader(memoryStream))
                            {
                                deserializedItem = method.Invoke(serializer, new object[] { binaryReader });
                            }
                        }
                    }
                }
            }

            return deserializedItem;
        }

        private static byte[] GetSerializedItem(dynamic serializer, Type serializerType, object value, Type itemType)
        {
            byte[] serializedItem = null;

            if (serializerType == null)
            {
                var formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, value);
                    serializedItem = stream.ToArray();
                }
            }
            else
            {
                var methods = serializerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

                // Invoke the serializer function that takes in an item (to be serialized) and a binary writer as arguments
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Write") && method.GetParameters().Length == 2)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var binaryWriter = new BinaryWriter(memoryStream))
                            {
                                method.Invoke(serializer, new object[] { value, binaryWriter });

                                serializedItem = memoryStream.ToArray();
                            }
                        }
                    }
                }
            }

            return serializedItem;
        }

        private static bool IsConcurrentQueue(IReliableState collection)
        {
            var collectionStr = collection.ToString();

            return collection.ToString().Contains("ReliableConcurrentQueue");
        }

        private static bool IsDictionary(IReliableState collection)
        {
            var collectionStr = collection.ToString();

            return collection.ToString().Contains("DistributedDictionary");
        }

        private static bool IsQueue(IReliableState collection)
        {
            var collectionStr = collection.ToString();

            return collection.ToString().Contains("DistributedQueue");
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }

        private object GetSerializer(Uri collectionUriName, Type keyType)
        {
            // Get DynamicStateManager type
            var stateManagerType = this.stateManager.GetType();
            var stateManagerImplField = stateManagerType.GetField("_impl", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManagerImpl = stateManagerImplField.GetValue(this.stateManager);
            var stateManagerImplType = stateManagerImpl.GetType();

            // Get replicator
            var stateManagerReplicatorProperty = stateManagerImplType.GetProperty("Replicator", BindingFlags.NonPublic | BindingFlags.Instance);

            var replicator = stateManagerReplicatorProperty.GetValue(stateManagerImpl);
            var replicatorType = stateManagerReplicatorProperty.PropertyType;

            var getStateSerializerMethod = replicatorType.GetMethod("GetStateSerializer");
            var getStateSerializerGenericMethod = getStateSerializerMethod.MakeGenericMethod(keyType);
            var serializer = getStateSerializerGenericMethod.Invoke(replicator, new object[] { collectionUriName });

            return serializer;
        }

        private async Task<dynamic> TryGetItemAsync(IReliableState collection, Type keyType, Type valueType, object serializer, object key)
        {
            dynamic item = null;

            using (ITransaction txn = this.stateManager.CreateTransaction())
            {
                if (IsDictionary(collection))
                {
                    Type reliableDictionaryType = typeof(IReliableDictionary<,>).MakeGenericType(keyType, valueType);

                    MethodInfo tryGetValueAsyncMethod = reliableDictionaryType.GetMethod("TryGetValueAsync", new[] { typeof(ITransaction), keyType, typeof(LockMode) });
                    if (tryGetValueAsyncMethod != null)
                    {
                        var tryGetValueAsyncTask = (Task)tryGetValueAsyncMethod.Invoke(collection, new object[] { txn, key, LockMode.Update });
                        await tryGetValueAsyncTask.ConfigureAwait(false);

                        item = ((dynamic)tryGetValueAsyncTask).Result;
                    }
                }
                else if (IsQueue(collection))
                {
                    Type reliableQueueType = typeof(IReliableQueue<>).MakeGenericType(valueType);

                    MethodInfo tryPeekAsyncMethod = reliableQueueType.GetMethod("TryPeekAsync", new[] { typeof(ITransaction), typeof(LockMode) });
                    if (tryPeekAsyncMethod != null)
                    {
                        var tryPeekAsyncTask = (Task)tryPeekAsyncMethod.Invoke(collection, new object[] { txn, LockMode.Update });
                        await tryPeekAsyncTask.ConfigureAwait(false);

                        item = ((dynamic)tryPeekAsyncTask).Result;
                    }
                }
                else if (IsConcurrentQueue(collection))
                {
                    Type reliableConcurrentQueueType = typeof(IReliableConcurrentQueue<>).MakeGenericType(valueType);

                    MethodInfo tryDequeueAsyncMethod = reliableConcurrentQueueType.GetMethod("TryDequeueAsync", new[] { typeof(ITransaction), typeof(CancellationToken), typeof(TimeSpan?) });
                    if (tryDequeueAsyncMethod != null)
                    {
                        var tryDequeueAsyncTask = (Task)tryDequeueAsyncMethod.Invoke(collection, new object[] { txn, null, null });
                        await tryDequeueAsyncTask.ConfigureAwait(false);

                        item = ((dynamic)tryDequeueAsyncTask).Result;

                        txn.Abort();
                    }
                }
            }

            return item;
        }

        private async Task<dynamic> TryGetItemsAsync(IReliableState collection, Type keyType, Type valueType, Type keySerializerType, Type valueSerializerType, object keySerializer, object valueSerializer, object key, int count)
        {
            var list = new List<KeyValuePair<byte[], byte[]>>();

            using (ITransaction txn = this.stateManager.CreateTransaction())
            {
                if (IsDictionary(collection))
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
                    CancellationToken ct = CancellationToken.None;

                    do
                    {
                        var moveNextAsyncMethodInvokeResult = moveNextAsyncMethod.Invoke(enumerator, new object[] { ct });
                        isAdvanced = moveNextAsyncMethodInvokeResult.Result;

                        if (isAdvanced)
                        {
                            var current = enumerator.GetType().GetProperty("Current").GetValue(enumerator);

                            var currentDeserializedKey = current.GetType().GetProperty("Key").GetValue(current);
                            var currentSerializedKey = GetSerializedItem(keySerializer, keySerializerType, currentDeserializedKey, keyType);

                            var currentDeserializedValue = current.GetType().GetProperty("Value").GetValue(current);
                            var currentSerializedValue = GetSerializedItem(valueSerializer, valueSerializerType, currentDeserializedValue, valueType);

                            var keyValuePair = new KeyValuePair<byte[], byte[]>(currentSerializedKey, currentSerializedValue);

                            list.Add(keyValuePair);

                            count--;
                        }
                    }
                    while (isAdvanced && count > 0);
                }
            }

            return list;
        }
    }
}
