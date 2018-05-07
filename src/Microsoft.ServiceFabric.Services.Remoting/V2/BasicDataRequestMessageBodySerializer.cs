// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal class BasicDataRequestMessageBodySerializer : IServiceRemotingRequestMessageBodySerializer
    {
        private readonly DataContractSerializer serializer;

        public BasicDataRequestMessageBodySerializer(
            IEnumerable<Type> parameterInfo)
        {
            this.serializer = new DataContractSerializer(
                typeof(ServiceRemotingRequestMessageBody),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = parameterInfo,
                });
        }

        public IOutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            if (serviceRemotingRequestMessageBody == null)
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    this.serializer.WriteObject(writer, serviceRemotingRequestMessageBody);
                    writer.Flush();
                    var bytes = stream.ToArray();
                    var segments = new List<ArraySegment<byte>>
                    {
                        new ArraySegment<byte>(bytes),
                    };
                    return new OutgoingMessageBody(segments);
                }
            }
        }

        public IServiceRemotingRequestMessageBody Deserialize(IIncomingMessageBody messageBody)
        {
            if ((messageBody == null) || (messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0))
            {
                return null;
            }

            using (var reader = XmlDictionaryReader.CreateBinaryReader(
                messageBody.GetReceivedBuffer(),
                XmlDictionaryReaderQuotas.Max))
            {
                return (ServiceRemotingRequestMessageBody)this.serializer.ReadObject(reader);
            }
        }
    }
}
