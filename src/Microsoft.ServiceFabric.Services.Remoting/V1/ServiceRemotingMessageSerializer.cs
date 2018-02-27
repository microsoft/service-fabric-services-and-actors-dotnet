// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal static class ServiceRemotingMessageSerializer
    {
        public static DataContractSerializer GetMessageBodySerializer(IEnumerable<Type> knownTypes)
        {
            return new DataContractSerializer(
                typeof(ServiceRemotingMessageBody),
                new DataContractSerializerSettings()
                {
                    KnownTypes = knownTypes,
                    MaxItemsInObjectGraph = int.MaxValue,
                });
        }

        public static DataContractSerializer GetMessageHeaderSerializer()
        {
            // When additional items are added to the service message headers,
            // the known type list in the serializer Settings should be updated.
            return new DataContractSerializer(
                typeof(ServiceRemotingMessageHeaders),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                });
        }
    }
}
