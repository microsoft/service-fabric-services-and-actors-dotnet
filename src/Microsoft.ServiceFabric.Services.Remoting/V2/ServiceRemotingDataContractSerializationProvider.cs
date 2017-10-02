// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Common;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// This is the default implmentation  for <see cref="IServiceRemotingMessageSerializationProvider"/>used by remoting service and client during
    /// request/response serialization . It used DataContract for serialization.
    /// </summary>
    public class ServiceRemotingDataContractSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        
        private readonly IBufferPoolManager bodyBufferPoolManager;

        /// <summary>
        /// Creates a ServiceRemotingDataContractSerializationProvider with default IBufferPoolManager 
        /// </summary>
        public ServiceRemotingDataContractSerializationProvider()
            : this(new BufferPoolManager(Constants.DefaultMessageBufferSize,Constants.DefaultMaxBufferCount))
        {
        }

        /// <summary>
        /// Creates a ServiceRemotingDataContractSerializationProvider with user specified IBufferPoolManager
        /// </summary>
        /// <param name="bodyBufferPoolManager"></param>
        public ServiceRemotingDataContractSerializationProvider(
            IBufferPoolManager bodyBufferPoolManager)
        {
            this.bodyBufferPoolManager = bodyBufferPoolManager;
        }

        /// <summary>
        /// Creates IServiceRemotingRequestMessageBodySerializer for a serviceInterface using DataContract implementation
        /// </summary>
        /// <param name="serviceInterfaceType">User service interface</param>
        /// <param name="requestBodyTypes">Parameters for all the methods in the serviceInterfaceType</param>
        /// <returns></returns>
        public IServiceRemotingRequestMessageBodySerializer CreateRequestMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> requestBodyTypes)
        {
            return new ServiceRemotingRequestMessageBodySerializer(
                this.bodyBufferPoolManager,
                requestBodyTypes);
        }


        /// <summary>
        /// Creates IServiceRemotingResponseMessageBodySerializer for a serviceInterface using DataContract implementation
        /// </summary>
        /// <param name="serviceInterfaceType">User service interface</param>
        /// <param name="responseBodyTypes">Return Types for all the methods in the serviceInterfaceType</param>
        /// <returns></returns>
        public IServiceRemotingResponseMessageBodySerializer CreateResponseMessageSerializer(
            Type serviceInterfaceType,
            IEnumerable<Type> responseBodyTypes)
        {
            return new ServiceRemotingResponseMessageBodySerializer(
                this.bodyBufferPoolManager,
                responseBodyTypes);
        }
  
        /// <summary>
        /// Creates a MessageFactory for DataContract Remoting Types. This is used to create Remoting Request/Response objects.
        /// </summary>
        /// <returns></returns>
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
                IEnumerable<Type>  parameterInfo)
            {
                this.bufferPoolManager = bufferPoolManager;
                this.serializer = new DataContractSerializer(
                    typeof(ServiceRemotingRequestMessageBody),
                    new DataContractSerializerSettings()
                    {
                        MaxItemsInObjectGraph = int.MaxValue,
                        KnownTypes = parameterInfo
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
                if (messageBody == null || messageBody.GetReceivedBuffer()==null || messageBody.GetReceivedBuffer().Length==0)
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
                        KnownTypes = parameterInfo
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

    [DataContract(Name = "msgResponse", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceRemotingResponseMessageBody : IServiceRemotingResponseMessageBody
    {
        [DataMember] private object response;
        public void Set(object response)
        {
            this.response = response;
        }

        public object Get(Type paramType)
        {
            return this.response;
        }
    }
}