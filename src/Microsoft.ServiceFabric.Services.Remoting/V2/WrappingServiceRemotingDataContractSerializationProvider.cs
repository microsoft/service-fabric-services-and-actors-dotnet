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
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// This is the  implmentation  for <see cref="IServiceRemotingMessageSerializationProvider"/>used by remoting service and client during
    /// request/response serialization . It uses request Wrapping and data contract for serialization.
    /// </summary>
    public class WrappingServiceRemotingDataContractSerializationProvider : IServiceRemotingMessageSerializationProvider
    {
        private ServiceRemotingDataContractSerializationProvider internalprovider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappingServiceRemotingDataContractSerializationProvider"/> class
        /// with default IBufferPoolManager implementation.
        /// </summary>
        public WrappingServiceRemotingDataContractSerializationProvider()
        {
            this.internalprovider = new ServiceRemotingDataContractSerializationProvider();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappingServiceRemotingDataContractSerializationProvider"/> class
        /// with specified IBufferPoolManager implementation.
        /// </summary>
        /// <param name="bodyBufferPoolManager">The buffer pool manager for serializing the remoting message bodies.</param>
        public WrappingServiceRemotingDataContractSerializationProvider(
            IBufferPoolManager bodyBufferPoolManager)
        {
            this.internalprovider = new ServiceRemotingDataContractSerializationProvider(bodyBufferPoolManager);
        }

        /// <summary>
        /// Creates a MessageFactory for Wrapped Message DataContract Remoting Types. This is used to create Remoting Request/Response objects.
        /// </summary>
        /// <returns>
        /// <see cref="IServiceRemotingMessageBodyFactory" /> that provides an instance of the factory for creating
        /// remoting request and response message bodies.
        /// </returns>
        public IServiceRemotingMessageBodyFactory CreateMessageBodyFactory()
        {
            return new WrappedRequestMessageFactory();
        }

        /// <summary>
        /// Creates IServiceRemotingRequestMessageBodySerializer for a serviceInterface using Wrapped Message DataContract implementation
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
                typeof(WrappedRemotingMessageBody),
                wrappedMessageTypes);

            return this.internalprovider.CreateRemotingRequestMessageSerializer<WrappedRemotingMessageBody, WrappedRemotingMessageBody>(
              serializer);
        }

        /// <summary>
        /// Creates IServiceRemotingResponseMessageBodySerializer for a serviceInterface using Wrapped Message DataContract implementation
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
            DataContractSerializer serializer = this.CreateRemotingResponseMessageBodyDataContractSerializer(
                typeof(WrappedRemotingMessageBody),
                wrappedMessageTypes);
            return this.internalprovider
                .CreateRemotingResponseMessageSerializer<WrappedRemotingMessageBody, WrappedRemotingMessageBody>(
                    serializer);
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
            return this.internalprovider.CreateXmlDictionaryWriter(outputStream);
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
            return this.internalprovider.CreateXmlDictionaryReader(inputStream);
        }

        /// <summary>
        ///     Gets the settings used to create DataContractSerializer for serializing and de-serializing request message body.
        /// </summary>
        /// <param name="remotingRequestType">Remoting RequestMessageBody Type</param>
        /// <param name="knownTypes">The return types of all of the methods of the specified interface.</param>
        /// <returns><see cref="DataContractSerializerSettings" /> for serializing and de-serializing request message body.</returns>
        protected virtual DataContractSerializer CreateRemotingRequestMessageBodyDataContractSerializer(
            Type remotingRequestType,
            IEnumerable<Type> knownTypes)
        {
            return this.internalprovider.CreateRemotingRequestMessageBodyDataContractSerializer(
                remotingRequestType,
                knownTypes);
        }

        /// <summary>
        ///     Gets the settings used to create DataContractSerializer for serializing and de-serializing request message body.
        /// </summary>
        /// <param name="remotingResponseType">Remoting ResponseMessage Type</param>
        /// <param name="knownTypes">The return types of all of the methods of the specified interface.</param>
        /// <returns><see cref="DataContractSerializerSettings" /> for serializing and de-serializing request message body.</returns>
        protected virtual DataContractSerializer CreateRemotingResponseMessageBodyDataContractSerializer(
            Type remotingResponseType,
            IEnumerable<Type> knownTypes)
        {
            return this.internalprovider.CreateRemotingResponseMessageBodyDataContractSerializer(
                remotingResponseType,
                knownTypes);
        }
    }
}
