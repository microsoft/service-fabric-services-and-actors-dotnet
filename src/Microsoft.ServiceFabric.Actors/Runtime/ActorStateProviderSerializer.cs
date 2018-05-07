// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.Remoting;

    internal class ActorStateProviderSerializer
    {
        private readonly ConcurrentDictionary<Type, DataContractSerializer> actorStateSerializerCache;

        internal ActorStateProviderSerializer()
        {
            this.actorStateSerializerCache = new ConcurrentDictionary<Type, DataContractSerializer>();
        }

        internal byte[] Serialize<T>(Type stateType, T state)
        {
            var serializer = this.actorStateSerializerCache.GetOrAdd(
                stateType,
                CreateDataContractSerializer);

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    serializer.WriteObject(writer, state);
                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }

        internal T Deserialize<T>(byte[] buffer)
        {
            if ((buffer == null) || (buffer.Length == 0))
            {
                return default(T);
            }

            var serializer = this.actorStateSerializerCache.GetOrAdd(
                typeof(T),
                CreateDataContractSerializer);

            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    return (T)serializer.ReadObject(reader);
                }
            }
        }

        private static DataContractSerializer CreateDataContractSerializer(Type actorStateType)
        {
            return new DataContractSerializer(
                actorStateType,
                new DataContractSerializerSettings
                {
                    MaxItemsInObjectGraph = int.MaxValue,
#if !DotNetCoreClr
                    DataContractSurrogate = ActorDataContractSurrogate.Instance,
#endif
                    KnownTypes = new[] { typeof(ActorReference) },
                });
        }
    }
}
