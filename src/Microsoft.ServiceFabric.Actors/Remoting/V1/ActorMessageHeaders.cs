// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Remoting.V1;

    /// <summary>
    /// Represents the header for the actor messages.
    /// </summary>
    [Obsolete(Services.Remoting.DeprecationMessage.RemotingV1)]
    [DataContract(Name = "addr", Namespace = Actors.Remoting.Constants.Namespace)]
    internal class ActorMessageHeaders
    {
#pragma warning disable SA1401 // Fields should be private
        [DataMember(IsRequired = true, Order = 0)]
        public int InterfaceId;

        [DataMember(IsRequired = true, Order = 1)]
        public int MethodId;

        [DataMember(IsRequired = false, Order = 2)]
        public ActorId ActorId;

        [DataMember(IsRequired = false, Order = 3)]
        public string CallContext;
#pragma warning restore SA1401 // Fields should be private

        private const string ActorMessageHeaderName = "ActorMessageHeader";
        private static readonly DataContractSerializer Serializer =
            new DataContractSerializer(typeof(ActorMessageHeaders));

        public static bool TryFromServiceMessageHeaders(ServiceRemotingMessageHeaders headers, out ActorMessageHeaders actorHeaders)
        {
            actorHeaders = null;
            if (!headers.TryGetHeaderValue(ActorMessageHeaderName, out var headerValue))
            {
                return false;
            }

            actorHeaders = Deserialize(headerValue);
            return true;
        }

        public ServiceRemotingMessageHeaders ToServiceMessageHeaders()
        {
            var serviceMessageHeaders = new ServiceRemotingMessageHeaders();
            serviceMessageHeaders.AddHeader(ActorMessageHeaderName, this.Serialize());

            return serviceMessageHeaders;
        }

        private static ActorMessageHeaders Deserialize(byte[] headerBytes)
        {
            using (var memoryStream = new MemoryStream(headerBytes))
            {
                var reader = XmlDictionaryReader.CreateBinaryReader(memoryStream, XmlDictionaryReaderQuotas.Max);
                return (ActorMessageHeaders)Serializer.ReadObject(reader);
            }
        }

        private byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
                Serializer.WriteObject(writer, this);
                writer.Flush();

                return memoryStream.ToArray();
            }
        }
    }
}
