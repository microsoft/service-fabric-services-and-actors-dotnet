namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    class BasicDataContractActorHeaderSerializer  : BasicDataContractHeaderSerializer
    {

        public BasicDataContractActorHeaderSerializer() 
            :base(
                new DataContractSerializer(
                    typeof(IServiceRemotingRequestMessageHeader),
                    new DataContractSerializerSettings()
                    {
                        MaxItemsInObjectGraph = int.MaxValue,
                        KnownTypes = new[] { typeof(ServiceRemotingRequestMessageHeader),
                            typeof(ActorRemotingMessageHeaders) }
                    }))
        { }

    }
}
