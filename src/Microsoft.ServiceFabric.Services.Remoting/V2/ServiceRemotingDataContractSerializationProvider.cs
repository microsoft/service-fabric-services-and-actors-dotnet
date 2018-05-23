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

    /// <summary>
    /// This is the default implmentation  for <see cref="IServiceRemotingMessageSerializationProvider"/>used by remoting service and client during
    /// request/response serialization . It used DataContract for serialization.
    /// </summary>
    public class ServiceRemotingDataContractSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        private readonly IBufferPoolManager bodyBufferPoolManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingDataContractSerializationProvider"/> class
        /// with default IBufferPoolManager implementation.
        /// </summary>
        public ServiceRemotingDataContractSerializationProvider()
            : this(new BufferPoolManager(Constants.DefaultMessageBufferSize, Constants.DefaultMaxBufferCount))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingDataContractSerializationProvider"/> class
        /// with specified IBufferPoolManager implementation.
        /// </summary>
        /// <param name="bodyBufferPoolManager">The buffer pool manager for serializing the remoting message bodies.</param>
        public ServiceRemotingDataContractSerializationProvider(
            IBufferPoolManager bodyBufferPoolManager)
        {
            this.bodyBufferPoolManager = bodyBufferPoolManager;
        }

        /// <summary>
        /// Creates IServiceRemotingRequestMessageBodySerializer for a serviceInterface using DataContract implementation
        /// </summary>
        /// <param name="serviceInterfaceType">The remoted service interface.</param>
        /// <param name="methodParameterTypes">The union of parameter types of all of the methods of the specified interface.</param>
        /// <param name="wrappedMessageTypes">Wrapped Request Types for all Methods</param>
        /// <returns>
        /// An instance of the <see cref="IServiceRemotingRequestMessageBodySerializer" /> that can serialize the service
        /// remoting request message body to a messaging body for transferring over the transport.
        /// </returns>
        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> methodParameterTypes,
            IEnumerable<Type> wrappedMessageTypes = null)
        {
            DataContractSerializer serializer = this.CreateRemotingRequestMessageBodyDataContractSerializer(
                    typeof(ServiceRemotingRequestMessageBody),
                    methodParameterTypes);

            return this
                .CreateRemotingRequestMessageSerializer<ServiceRemotingRequestMessageBody,
                    ServiceRemotingResponseMessageBody>(serializer);
        }

        /// <summary>
        /// Creates IServiceRemotingResponseMessageBodySerializer for a serviceInterface using DataContract implementation
        /// </summary>
        /// <param name="serviceInterfaceType">The remoted service interface.</param>
        /// <param name="methodReturnTypes">The return types of all of the methods of the specified interface.</param>
        /// <param name="wrappedMessageTypes">Wrapped Response Types for all remoting methods</param>
        /// <returns>
        /// An instance of the <see cref="IServiceRemotingResponseMessageBodySerializer" /> that can serialize the service
        /// remoting response message body to a messaging body for transferring over the transport.
        /// </returns>
        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> methodReturnTypes,
            IEnumerable<Type> wrappedMessageTypes = null)
        {
            DataContractSerializer serializer =
                this.CreateRemotingRequestMessageBodyDataContractSerializer(
                    typeof(ServiceRemotingResponseMessageBody),
                    methodReturnTypes);

            return this
                .CreateRemotingResponseMessageSerializer<ServiceRemotingRequestMessageBody,
                    ServiceRemotingResponseMessageBody>(serializer);
        }

        /// <summary>
        /// Creates a MessageFactory for DataContract Remoting Types. This is used to create Remoting Request/Response objects.
        /// </summary>
        /// <returns>
        /// <see cref="IServiceRemotingMessageBodyFactory" /> that provides an instance of the factory for creating
        /// remoting request and response message bodies.
        /// </returns>
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new DataContractRemotingMessageFactory();
        }

        internal IServiceRemotingRequestMessageBodySerializer CreateRemotingRequestMessageSerializer<TRequest, TResponse>(
         DataContractSerializer serializer)
           where TRequest : IServiceRemotingRequestMessageBody
           where TResponse : IServiceRemotingResponseMessageBody
        {
            if (this.bodyBufferPoolManager != null)
            {
                return new PooledBufferMessageBodySerializer<TRequest,
                    TResponse>(
                    this,
                    this.bodyBufferPoolManager,
                    serializer);
            }

            return new MemoryStreamMessageBodySerializer(this, serializer);
        }

        internal IServiceRemotingResponseMessageBodySerializer CreateRemotingResponseMessageSerializer<TRequest, TResponse>(
          DataContractSerializer serializer)
            where TRequest : IServiceRemotingRequestMessageBody
            where TResponse : IServiceRemotingResponseMessageBody
        {
            if (this.bodyBufferPoolManager != null)
            {
                return new PooledBufferMessageBodySerializer<TRequest,
                    TResponse>(
                    this,
                    this.bodyBufferPoolManager,
                    serializer);
            }

            return new MemoryStreamMessageBodySerializer(this, serializer);
        }

        /// <summary>
        ///     Gets the settings used to create DataContractSerializer for serializing and de-serializing request message body.
        /// </summary>
        /// <param name="remotingRequestType">Remoting RequestMessageBody Type</param>
        /// <param name="knownTypes">The return types of all of the methods of the specified interface.</param>
        /// <returns><see cref="DataContractSerializerSettings" /> for serializing and de-serializing request message body.</returns>
        protected internal virtual DataContractSerializer CreateRemotingRequestMessageBodyDataContractSerializer(
            Type remotingRequestType,
            IEnumerable<Type> knownTypes)
        {
            return new DataContractSerializer(
                remotingRequestType,
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = knownTypes,
                });
        }

        /// <summary>
        ///     Gets the settings used to create DataContractSerializer for serializing and de-serializing request message body.
        /// </summary>
        /// <param name="remotingResponseType">Remoting ResponseMessage Type</param>
        /// <param name="knownTypes">The return types of all of the methods of the specified interface.</param>
        /// <returns><see cref="DataContractSerializerSettings" /> for serializing and de-serializing request message body.</returns>
        protected internal virtual DataContractSerializer CreateRemotingResponseMessageBodyDataContractSerializer(
            Type remotingResponseType,
            IEnumerable<Type> knownTypes)
        {
            return new DataContractSerializer(
                remotingResponseType,
                new DataContractSerializerSettings()
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = knownTypes,
                });
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
        protected internal virtual XmlDictionaryWriter CreateXmlDictionaryWriter(Stream outputStream)
        {
            return XmlDictionaryWriter.CreateBinaryWriter(outputStream);
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
        protected internal virtual XmlDictionaryReader CreateXmlDictionaryReader(Stream inputStream)
        {
            return XmlDictionaryReader.CreateBinaryReader(inputStream, XmlDictionaryReaderQuotas.Max);
        }

        /// <summary>
        ///     Default serializer for service remoting request and response message body that uses the
        ///     memory stream to create outgoing message buffers.
        /// </summary>
        private class MemoryStreamMessageBodySerializer :
            IServiceRemotingRequestMessageBodySerializer,
            IServiceRemotingResponseMessageBodySerializer
        {
            private readonly ServiceRemotingDataContractSerializationProvider serializationProvider;
            private readonly DataContractSerializer serializer;

            public MemoryStreamMessageBodySerializer(
                ServiceRemotingDataContractSerializationProvider serializationProvider,
                DataContractSerializer serializer)
            {
                this.serializationProvider = serializationProvider;
                this.serializer = serializer;
            }

            IOutgoingMessageBody IServiceRemotingRequestMessageBodySerializer.Serialize(
                IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
            {
                if (serviceRemotingRequestMessageBody == null)
                {
                    return null;
                }

                using (var stream = new MemoryStream())
                {
                    using (var writer = this.CreateXmlDictionaryWriter(stream))
                    {
                        this.serializer.WriteObject(writer, serviceRemotingRequestMessageBody);
                        writer.Flush();
                        return new OutgoingMessageBody(
                            new[]
                                {
                                    new ArraySegment<byte>(stream.ToArray()),
                                });
                    }
                }
            }

            IServiceRemotingRequestMessageBody IServiceRemotingRequestMessageBodySerializer.Deserialize(
                IIncomingMessageBody messageBody)
            {
                if (messageBody == null || messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
                {
                    return null;
                }

                using (var stream = new DisposableStream(messageBody.GetReceivedBuffer()))
                {
                    using (var reader = this.CreateXmlDictionaryReader(stream))
                    {
                        return (ServiceRemotingRequestMessageBody)this.serializer.ReadObject(reader);
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

                using (var stream = new MemoryStream())
                {
                    using (var writer = this.CreateXmlDictionaryWriter(stream))
                    {
                        this.serializer.WriteObject(writer, serviceRemotingResponseMessageBody);
                        writer.Flush();
                        return new OutgoingMessageBody(
                            new[]
                                {
                                    new ArraySegment<byte>(stream.ToArray()),
                                });
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
                        return (ServiceRemotingResponseMessageBody)this.serializer.ReadObject(reader);
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
}
