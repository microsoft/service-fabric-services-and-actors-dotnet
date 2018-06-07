// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    ///     Default serializdr for service remoting request and response message body that uses the
    ///     buffer pool manager to create outgoing message buffers.
    /// </summary>
    internal class PooledBufferMessageBodySerializer<TRequest, TResponse> :
        IServiceRemotingRequestMessageBodySerializer,
        IServiceRemotingResponseMessageBodySerializer
        where TRequest : IServiceRemotingRequestMessageBody
        where TResponse : IServiceRemotingResponseMessageBody
    {
        private readonly ServiceRemotingDataContractSerializationProvider serializationProvider;
        private readonly IBufferPoolManager bufferPoolManager;
        private readonly DataContractSerializer serializer;

        public PooledBufferMessageBodySerializer(
            ServiceRemotingDataContractSerializationProvider serializationProvider,
            IBufferPoolManager bufferPoolManager,
            DataContractSerializer serializer)
        {
            this.serializationProvider = serializationProvider;
            this.bufferPoolManager = bufferPoolManager;
            this.serializer = serializer;
        }

        IOutgoingMessageBody IServiceRemotingRequestMessageBodySerializer.Serialize(
            IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            if (serviceRemotingRequestMessageBody == null)
            {
                return null;
            }

            using (var stream = new SegmentedPoolMemoryStream(this.bufferPoolManager))
            {
                using (var writer = this.CreateXmlDictionaryWriter(stream))
                {
                    this.serializer.WriteObject(writer, serviceRemotingRequestMessageBody);
                    writer.Flush();
                    return new OutgoingMessageBody(stream.GetBuffers());
                }
            }
        }

        IServiceRemotingRequestMessageBody IServiceRemotingRequestMessageBodySerializer.Deserialize(
            IIncomingMessageBody messageBody)
        {
            if (messageBody?.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
            {
                return null;
            }

            using (var stream = new DisposableStream(messageBody.GetReceivedBuffer()))
            {
                using (var reader = this.CreateXmlDictionaryReader(stream))
                {
                    return (TRequest)this.serializer.ReadObject(reader);
                }
            }
        }

        IOutgoingMessageBody IServiceRemotingResponseMessageBodySerializer.Serialize(
            IServiceRemotingResponseMessageBody serviceRemotingResponseMessageBody)
        {
            if (serviceRemotingResponseMessageBody == null)
            {
                return null;
            }

            using (var stream = new SegmentedPoolMemoryStream(this.bufferPoolManager))
            {
                using (var writer = this.CreateXmlDictionaryWriter(stream))
                {
                    this.serializer.WriteObject(writer, serviceRemotingResponseMessageBody);
                    writer.Flush();
                    return new OutgoingMessageBody(stream.GetBuffers());
                }
            }
        }

        IServiceRemotingResponseMessageBody IServiceRemotingResponseMessageBodySerializer.Deserialize(
            IIncomingMessageBody messageBody)
        {
            if (messageBody?.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
            {
                return null;
            }

            using (var stream = new DisposableStream(messageBody.GetReceivedBuffer()))
            {
                using (var reader = this.CreateXmlDictionaryReader(stream))
                {
                    return (TResponse)this.serializer.ReadObject(reader);
                }
            }
        }

        /// <summary>
        ///     Create the writer to write to the stream. Use this method to customize how the serialized contents are written to
        ///     the stream.
        /// </summary>
        /// <param name="outputStream">The stream on which to write the serialized contents.</param>
        /// <returns>
        ///     An <see cref="System.Xml.XmlDictionaryWriter" /> using which the serializer will write the object on the
        ///     stream.
        /// </returns>
        private XmlDictionaryWriter CreateXmlDictionaryWriter(Stream outputStream)
        {
            return this.serializationProvider.CreateXmlDictionaryWriter(outputStream);
        }

        /// <summary>
        ///     Create the reader to read from the input stream. Use this method to customize how the serialized contents are read
        ///     from the stream.
        /// </summary>
        /// <param name="inputStream">The stream from which to read the serialized contents.</param>
        /// <returns>
        ///     An <see cref="System.Xml.XmlDictionaryReader" /> using which the serializer will read the object from the
        ///     stream.
        /// </returns>
        private XmlDictionaryReader CreateXmlDictionaryReader(Stream inputStream)
        {
            return this.serializationProvider.CreateXmlDictionaryReader(inputStream);
        }
    }
}
