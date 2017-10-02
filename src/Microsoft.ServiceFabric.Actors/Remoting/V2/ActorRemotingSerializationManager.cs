// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;
    using ActorEventSubscription = Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime.ActorEventSubscription;

    class ActorRemotingSerializationManager : ServiceRemotingMessageSerializersManager
    {
        public ActorRemotingSerializationManager(IServiceRemotingMessageSerializationProvider serializationProvider,
            IServiceRemotingMessageHeaderSerializer headerSerializer) : base(serializationProvider, headerSerializer)
        {
        }

        //Custom Serializer needs to be used only for ActorDispatch scenario
        internal override CacheEntry CreateSerializers(int interfaceId)
        {
            if (interfaceId == ActorEventSubscription.InterfaceId)
            {
                return new CacheEntry(
                    new BasicDataRequestMessageBodySerializer(
                        new[]
                        {
                            typeof(EventSubscriptionRequestBody)
                        }), 
                    new BasicDataResponsetMessageBodySerializer(
                        new[]
                        {
                            typeof(EventSubscriptionRequestBody)
                        }));
                
            }

            return base.CreateSerializers(interfaceId);
        }

        internal override InterfaceDetails GetInterfaceDetails(int interfaceId)
        {
            InterfaceDetails interfaceDetails;
            if (ActorCodeBuilder.TryGetKnownTypes(interfaceId, out interfaceDetails))
            {
                return interfaceDetails;
            }
            //if not found in Actor Store, Check if its there in service store for actor service request
            return base.GetInterfaceDetails(interfaceId);
        }
    }
}