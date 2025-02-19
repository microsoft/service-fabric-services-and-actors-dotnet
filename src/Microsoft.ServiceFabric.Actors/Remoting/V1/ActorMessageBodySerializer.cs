// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;

    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    internal static class ActorMessageBodySerializer
    {
        private static readonly IEnumerable<Type> DefaultKnownTypes = new[]
        {
            typeof(ActorReference),
        };

        public static DataContractSerializer GetActorMessageSerializer(IEnumerable<Type> knownTypes)
        {
            var types = new List<Type>(knownTypes);

            types.AddRange(DefaultKnownTypes);
#if DotNetCoreClr
            DataContractSerializer dataContractSerializer = new DataContractSerializer(
                typeof(ActorMessageBody),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = types
                });
            dataContractSerializer.SetSerializationSurrogateProvider(new ActorDataContractSurrogate());
#else
            var dataContractSerializer = new DataContractSerializer(
                typeof(ActorMessageBody),
                new DataContractSerializerSettings()
                {
                    DataContractSurrogate = new ActorDataContractSurrogate(),
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = types,
                });
#endif

            return dataContractSerializer;
        }
    }
}
