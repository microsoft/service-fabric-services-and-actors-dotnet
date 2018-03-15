// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal class BasicDataContractActorHeaderSerializer : BasicDataContractHeaderSerializer
    {

        public BasicDataContractActorHeaderSerializer()
            : base(
                new DataContractSerializer(
                    typeof(IServiceRemotingRequestMessageHeader),
                    new DataContractSerializerSettings()
                    {
                        MaxItemsInObjectGraph = int.MaxValue,
                        KnownTypes = new[] { typeof(ServiceRemotingRequestMessageHeader),
                            typeof(ActorRemotingMessageHeaders) }
                    }))
        { }

    }
}
