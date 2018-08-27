// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;
    using ActorEventSubscription = Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime.ActorEventSubscription;

    internal class ActorRemotingSerializationManager : Services.Remoting.V2.ServiceRemotingSerializationManager
    {
        public ActorRemotingSerializationManager(
            IServiceRemotingMessageSerializationProvider serializationProvider,
            IServiceRemotingMessageHeaderSerializer headerSerializer,
            bool useWrappedMessage = false)
            : base(
                  GetSerializationProvider(serializationProvider, useWrappedMessage),
                  headerSerializer,
                  useWrappedMessage)
        {
        }

        // Custom Serializer needs to be used only for ActorDispatch scenario
        internal override CacheEntry CreateSerializers(int interfaceId)
        {
            if (interfaceId == ActorEventSubscription.InterfaceId)
            {
                IServiceRemotingMessageSerializationProvider actorRemotingSerializationProvider = null;
                actorRemotingSerializationProvider = new ActorRemotingDataContractSerializationProvider();

                var cacheEntry = new CacheEntry(
                    actorRemotingSerializationProvider.CreateRequestMessageSerializer(
                        null,
                        new[] { typeof(EventSubscriptionRequestBody) }),
                    actorRemotingSerializationProvider.CreateResponseMessageSerializer(
                        null,
                        new[] { typeof(EventSubscriptionRequestBody) }));

                return cacheEntry;
            }

            return base.CreateSerializers(interfaceId);
        }

        internal override InterfaceDetails GetInterfaceDetails(int interfaceId)
        {
            if (ActorCodeBuilder.TryGetKnownTypes(interfaceId, out var interfaceDetails))
            {
                return interfaceDetails;
            }

            // if not found in Actor Store, Check if its there in service store for actor service request
            return base.GetInterfaceDetails(interfaceId);
        }

        private static IServiceRemotingMessageSerializationProvider GetSerializationProvider(IServiceRemotingMessageSerializationProvider serializationProvider, bool useWrappedMessage)
        {
            if (serializationProvider == null)
            {
                if (useWrappedMessage)
                {
                    return new ActorRemotingWrappingDataContractSerializationProvider();
                }

                return new ActorRemotingDataContractSerializationProvider();
            }

            return serializationProvider;
        }
    }
}
