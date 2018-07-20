// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

    /// <summary>
    /// This is the  implmentation  for <see cref="IServiceRemotingMessageSerializationProvider"/>used by actor service and client during
    /// request/response serialization . It uses request Wrapping and data contract for serialization.
    /// </summary>
    public class ActorRemotingWrappingDataContractSerializationProvider : WrappingServiceRemotingDataContractSerializationProvider
    {
        private static readonly IEnumerable<Type> DefaultKnownTypes = new[]
        {
            typeof(ActorReference),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorRemotingWrappingDataContractSerializationProvider"/> class with default IBufferPoolManager
        /// </summary>
        public ActorRemotingWrappingDataContractSerializationProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorRemotingWrappingDataContractSerializationProvider"/> class with user specified IBufferPoolManager.
        ///     If the specified buffer pool manager is null, the buffer pooling will be turned off.
        /// </summary>
        /// <param name="bodyBufferPoolManager">The buffer pool manager for serializing the remoting message bodies.</param>
        public ActorRemotingWrappingDataContractSerializationProvider(
        IBufferPoolManager bodyBufferPoolManager)
            : base(bodyBufferPoolManager)
        {
        }

        /// <inheritdoc />
        protected override DataContractSerializer CreateRemotingRequestMessageBodyDataContractSerializer(
            Type remotingRequestType,
            IEnumerable<Type> knownTypes)
        {
#if DotNetCoreClr
            var serializer = base.CreateRemotingRequestMessageBodyDataContractSerializer(remotingRequestType, AddDefaultKnownTypes(knownTypes));

            serializer.SetSerializationSurrogateProvider(new ActorDataContractSurrogate());
            return serializer;

#else
            return new DataContractSerializer(
                    remotingRequestType,
                    new DataContractSerializerSettings
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = AddDefaultKnownTypes(knownTypes),
                    DataContractSurrogate = new ActorDataContractSurrogate(),
                });
#endif

        }

        /// <inheritdoc />
        protected override DataContractSerializer CreateRemotingResponseMessageBodyDataContractSerializer(
            Type remotingResponseType,
            IEnumerable<Type> knownTypes)
        {
#if DotNetCoreClr
            var serializer = base.CreateRemotingResponseMessageBodyDataContractSerializer(remotingResponseType, AddDefaultKnownTypes(knownTypes));

            serializer.SetSerializationSurrogateProvider(new ActorDataContractSurrogate());
            return serializer;

#else
            return new DataContractSerializer(
                    remotingResponseType,
                    new DataContractSerializerSettings
                {
                    MaxItemsInObjectGraph = int.MaxValue,
                    KnownTypes = AddDefaultKnownTypes(knownTypes),
                    DataContractSurrogate = new ActorDataContractSurrogate(),
                });
#endif

        }

        private static IEnumerable<Type> AddDefaultKnownTypes(IEnumerable<Type> knownTypes)
        {
            var types = new List<Type>(knownTypes);
            types.AddRange(DefaultKnownTypes);
            return types;
        }
    }
}
