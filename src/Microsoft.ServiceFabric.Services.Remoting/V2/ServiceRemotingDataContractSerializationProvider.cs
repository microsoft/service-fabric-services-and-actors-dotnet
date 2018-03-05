// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
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
        /// <returns>
        /// An instance of the <see cref="IServiceRemotingRequestMessageBodySerializer" /> that can serialize the service
        /// remoting request message body to a messaging body for transferring over the transport.
        /// </returns>
        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> methodParameterTypes)
        {
            return new ServiceRemotingRequestMessageBodySerializer(
                this.bodyBufferPoolManager,
                methodParameterTypes);
        }

        /// <summary>
        /// Creates IServiceRemotingResponseMessageBodySerializer for a serviceInterface using DataContract implementation
        /// </summary>
        /// <param name="serviceInterfaceType">The remoted service interface.</param>
        /// <param name="methodReturnTypes">The return types of all of the methods of the specified interface.</param>
        /// <returns>
        /// An instance of the <see cref="IServiceRemotingResponseMessageBodySerializer" /> that can serialize the service
        /// remoting response message body to a messaging body for transferring over the transport.
        /// </returns>
        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> methodReturnTypes)
        {
            return new ServiceRemotingResponseMessageBodySerializer(
                this.bodyBufferPoolManager,
                methodReturnTypes);
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

        internal class ServiceRemotingRequestMessageBodySerializer : IServiceRemotingRequestMessageBodySerializer
        {
            private readonly IBufferPoolManager bufferPoolManager;
            private readonly DataContractSerializer serializer;

            public ServiceRemotingRequestMessageBodySerializer(
                IBufferPoolManager bufferPoolManager,
                IEnumerable<Type> parameterInfo)
            {
                this.bufferPoolManager = bufferPoolManager;
                this.serializer = new DataContractSerializer(
                    typeof(ServiceRemotingRequestMessageBody),
                    new DataContractSerializerSettings()
                    {
                        MaxItemsInObjectGraph = int.MaxValue,
                        KnownTypes = parameterInfo,
                    });
            }

            public OutgoingMessageBody Serialize(IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
            {
                if (serviceRemotingRequestMessageBody == null)
                {
                    return null;
                }

                using (var stream = new SegmentedPoolMemoryStream(this.bufferPoolManager))
                {
                    using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                    {
                        this.serializer.WriteObject(writer, serviceRemotingRequestMessageBody);
                        writer.Flush();
                        return new OutgoingMessageBody(stream.GetBuffers());
                    }
                }
            }

            public IServiceRemotingRequestMessageBody Deserialize(IncomingMessageBody messageBody)
            {
                if (messageBody == null || messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
                {
                    return null;
                }

                // Binary Reader Dispose also call stream dispose. Hence no need to call stream dispose.
                using (var reader = XmlDictionaryReader.CreateBinaryReader(
                    messageBody.GetReceivedBuffer(),
                    XmlDictionaryReaderQuotas.Max))
                {
                    return (ServiceRemotingRequestMessageBody)this.serializer.ReadObject(reader);
                }
            }
        }

        internal class ServiceRemotingResponseMessageBodySerializer : IServiceRemotingResponseMessageBodySerializer
        {
            private readonly IBufferPoolManager bufferPoolManager;
            private readonly DataContractSerializer serializer;

            public ServiceRemotingResponseMessageBodySerializer(
                IBufferPoolManager bufferPoolManager,
                IEnumerable<Type> parameterInfo)
            {
                this.bufferPoolManager = bufferPoolManager;
                this.serializer = new DataContractSerializer(
                    typeof(ServiceRemotingResponseMessageBody),
                    new DataContractSerializerSettings()
                    {
                        MaxItemsInObjectGraph = int.MaxValue,
                        KnownTypes = parameterInfo,
                    });
            }

            public OutgoingMessageBody Serialize(IServiceRemotingResponseMessageBody serviceRemotingResponseMessageBody)
            {
                if (serviceRemotingResponseMessageBody == null)
                {
                    return null;
                }

                using (var stream = new SegmentedPoolMemoryStream(this.bufferPoolManager))
                {
                    using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                    {
                        this.serializer.WriteObject(writer, serviceRemotingResponseMessageBody);
                        writer.Flush();
                        return new OutgoingMessageBody(stream.GetBuffers());
                    }
                }
            }

            public IServiceRemotingResponseMessageBody Deserialize(IncomingMessageBody messageBody)
            {
                if (messageBody == null || messageBody.GetReceivedBuffer() == null || messageBody.GetReceivedBuffer().Length == 0)
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
}
