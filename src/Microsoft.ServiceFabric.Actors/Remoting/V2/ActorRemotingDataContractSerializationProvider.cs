// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Messaging;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    /// <summary>
    ///     This is the default implmentation  for <see cref="IServiceRemotingMessageSerializationProvider" />used by actor
    ///     remoting.
    ///     It uses DataContractSerializer for serialization of remoting request and response message bodies.
    /// </summary>
    public class ActorRemotingDataContractSerializationProvider : ServiceRemotingDataContractSerializationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorRemotingDataContractSerializationProvider"/> class.
        /// with default IBufferPoolManager
        /// </summary>
        public ActorRemotingDataContractSerializationProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorRemotingDataContractSerializationProvider"/> class.
        /// with user specified IBufferPoolManager.If the specified buffer pool manager is null, the buffer pooling will be turned off.
        /// </summary>
        /// <param name="bodyBufferPoolManager">The buffer pool manager for serializing the remoting message bodies.</param>
        public ActorRemotingDataContractSerializationProvider(
            IBufferPoolManager bodyBufferPoolManager)
            : base(bodyBufferPoolManager)
        {
        }

        /// <inheritdoc />
        protected internal override DataContractSerializer CreateRemotingRequestMessageBodyDataContractSerializer(
            Type remotingRequestType,
            IEnumerable<Type> knownTypes)
        {
#if DotNetCoreClr
            var serializer =
                base.CreateRemotingRequestMessageBodyDataContractSerializer(remotingRequestType, knownTypes);

            serializer.SetSerializationSurrogateProvider(new ActorDataContractSurrogate());
            return serializer;

#else
            return new DataContractSerializer(
                remotingRequestType,
                new DataContractSerializerSettings
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = knownTypes,
                    DataContractSurrogate = new ActorDataContractSurrogate(),
                });
#endif

        }

        /// <inheritdoc />
        protected internal override DataContractSerializer CreateRemotingResponseMessageBodyDataContractSerializer(
            Type remotingResponseType,
            IEnumerable<Type> knownTypes)
        {
#if DotNetCoreClr
            var serializer =
                base.CreateRemotingResponseMessageBodyDataContractSerializer(remotingResponseType, knownTypes);
            serializer.SetSerializationSurrogateProvider(new ActorDataContractSurrogate());
            return serializer;

#else
            return new DataContractSerializer(
                remotingResponseType,
                new DataContractSerializerSettings
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = knownTypes,
                    DataContractSurrogate = new ActorDataContractSurrogate(),
                });
#endif

        }
    }
}
