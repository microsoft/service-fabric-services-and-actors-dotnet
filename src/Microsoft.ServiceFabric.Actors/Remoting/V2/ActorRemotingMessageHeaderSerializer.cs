// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal class ActorRemotingMessageHeaderSerializer : ServiceRemotingMessageHeaderSerializer
    {
        public ActorRemotingMessageHeaderSerializer(int headerBufferSize, int headerBufferCount)
        : base(
            new BufferPoolManager(headerBufferSize, headerBufferCount),
            new DataContractSerializer(
                typeof(IServiceRemotingRequestMessageHeader),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = new[]
                    {
                        typeof(ServiceRemotingRequestMessageHeader),
                        typeof(ActorRemotingMessageHeaders),
                    },
                }))
        {
        }
    }
}
