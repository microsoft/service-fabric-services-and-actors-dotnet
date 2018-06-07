// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    internal class ServiceRemotingMessageSerializersManager
    {
        private readonly ConcurrentDictionary<int, CacheEntry> cachedBodySerializers;
        private readonly IServiceRemotingMessageHeaderSerializer headerSerializer;
        private readonly IServiceRemotingMessageSerializationProvider serializationProvider;

        public ServiceRemotingMessageSerializersManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            IServiceRemotingMessageHeaderSerializer headerSerializer,
            bool useWrappedMessage = false)
        {
            if (headerSerializer == null)
            {
                headerSerializer = new ServiceRemotingMessageHeaderSerializer(new BufferPoolManager());
            }

            if (serializationProvider == null)
            {
                if (useWrappedMessage)
                {
                    serializationProvider = new WrappingServiceRemotingDataContractSerializationProvider();
                }
                else
                {
                    serializationProvider = new ServiceRemotingDataContractSerializationProvider();
                }
            }

            this.serializationProvider = serializationProvider;
            this.cachedBodySerializers = new ConcurrentDictionary<int, CacheEntry>();
            this.headerSerializer = headerSerializer;
        }

        public IServiceRemotingMessageSerializationProvider GetSerializationProvider()
        {
            return this.serializationProvider;
        }

        public IServiceRemotingMessageHeaderSerializer GetHeaderSerializer()
        {
            return this.headerSerializer;
        }

        public IServiceRemotingRequestMessageBodySerializer GetRequestBodySerializer(int interfaceId)
        {
            return this.cachedBodySerializers.GetOrAdd(interfaceId, this.CreateSerializers).RequestBodySerializer;
        }

        public IServiceRemotingResponseMessageBodySerializer GetResponseBodySerializer(int interfaceId)
        {
            return this.cachedBodySerializers.GetOrAdd(interfaceId, this.CreateSerializers).ResponseBodySerializer;
        }

        internal virtual CacheEntry CreateSerializers(int interfaceId)
        {
            var interfaceDetails = this.GetInterfaceDetails(interfaceId);

            // get the service interface type from the code gen layer
            var serviceInterfaceType = interfaceDetails.ServiceInterfaceType;

            // get the known types from the codegen layer
            var requestBodyTypes = interfaceDetails.RequestKnownTypes;

            // get the known types from the codegen layer
            var responseBodyTypes = interfaceDetails.ResponseKnownTypes;

            return new CacheEntry(
                this.serializationProvider.CreateRequestMessageSerializer(serviceInterfaceType, requestBodyTypes, interfaceDetails.RequestWrappedKnownTypes),
                this.serializationProvider.CreateResponseMessageSerializer(serviceInterfaceType, responseBodyTypes, interfaceDetails.ResponseWrappedKnownTypes));
        }

        internal virtual InterfaceDetails GetInterfaceDetails(int interfaceId)
        {
            if (!ServiceCodeBuilder.TryGetKnownTypes(interfaceId, out var interfaceDetails))
            {
                throw new ArgumentException("No interface found with this Id  " + interfaceId);
            }

            return interfaceDetails;
        }
    }
}
