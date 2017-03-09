// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal static class ActorMessageBodySerializer
    {
        private static readonly IEnumerable<Type> DefaultKnownTypes = new[]
        {
            typeof(ActorReference)
        };

        public static DataContractSerializer GetActorMessageSerializer(IEnumerable<Type> knownTypes)
        {
            var types = new List<Type>(knownTypes);
            
            types.AddRange(DefaultKnownTypes);
            DataContractSerializer dataContractSerializer = new DataContractSerializer(
                typeof(ActorMessageBody),
                new DataContractSerializerSettings()
                {
                    DataContractSurrogate = new ActorDataContractSurrogate(),
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = types
                });

            return dataContractSerializer;
        }
    }
}
