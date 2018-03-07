// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal class BasicDataContractSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new DataContractRemotingMessageFactory();
        }

        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(Type serviceInterfaceType,
            IEnumerable<Type> requestBodyTypes)
        {
            return new BasicDataRequestMessageBodySerializer(requestBodyTypes);
        }

        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(Type serviceInterfaceType,
            IEnumerable<Type> responseBodyTypes)
        {
            return new BasicDataResponsetMessageBodySerializer(responseBodyTypes);
        }
    }

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
                    KnownTypes = parameterInfo
                });
        }
        public IMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
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
                        new ArraySegment<byte>(bytes)
                    };
                    return new OutgoingMessageBody(segments);
                }
            }
        }

        public IServiceRemotingRequestMessageBody Deserialize(IMessageBody messageBody)
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

    internal class BasicDataResponsetMessageBodySerializer : IServiceRemotingResponseMessageBodySerializer
    {
        private readonly DataContractSerializer serializer;

        public BasicDataResponsetMessageBodySerializer(
            IEnumerable<Type> parameterInfo)
        {
            this.serializer = new DataContractSerializer(
                typeof(ServiceRemotingResponseMessageBody),
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = parameterInfo
                });
        }
        public IMessageBody Serialize(IServiceRemotingResponseMessageBody serviceRemotingRequestMessageBody)
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
                        new ArraySegment<byte>(bytes)
                    };
                    return new OutgoingMessageBody(segments);
                }
            }
        }

        public IServiceRemotingResponseMessageBody Deserialize(IMessageBody messageBody)
        {
            if ((messageBody == null) || (messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0))
            {
                return null;
            }

            using (var reader = XmlDictionaryReader.CreateBinaryReader(
                messageBody.GetReceivedBuffer(),
                XmlDictionaryReaderQuotas.Max))
            {
                return (ServiceRemotingResponseMessageBody)this.serializer.ReadObject(reader);

            }
        }
    }
}
