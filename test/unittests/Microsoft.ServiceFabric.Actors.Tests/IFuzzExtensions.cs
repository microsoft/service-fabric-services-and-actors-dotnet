using System;
using System.Fabric;
using System.Numerics;
using Fuzzy;
using Inspector;
using Moq;

namespace Microsoft.ServiceFabric.Actors.Tests
{
    static class IFuzzExtensions
    {
        internal static ActorId ActorId(this IFuzz fuzzy)
        {
            switch(fuzzy.Enum<ActorIdKind>())
            {
                case ActorIdKind.Guid: return new ActorId(Guid.NewGuid());
                case ActorIdKind.Long: return new ActorId(fuzzy.Int64());
                case ActorIdKind.String: return new ActorId(fuzzy.String());
                default: throw new NotSupportedException();
            }
        }

        internal static ICodePackageActivationContext ICodePackageActivationContext(this IFuzz fuzzy)
        {
            var result = new Mock<ICodePackageActivationContext>();
            result.Setup(_ => _.ApplicationName).Returns(fuzzy.String());
            result.Setup(_ => _.ApplicationTypeName).Returns(fuzzy.String());
            return result.Object;
        }

        internal static NodeContext NodeContext(this IFuzz fuzzy)
        {
            string nodeName = fuzzy.String();
            BigInteger instanceId = fuzzy.Int64();
            string nodeType = fuzzy.String();
            string ipAddress = fuzzy.String();
            return new NodeContext(nodeName, fuzzy.NodeId(), instanceId, nodeType, ipAddress);
        }

        internal static NodeId NodeId(this IFuzz fuzzy)
        {
            BigInteger high = fuzzy.UInt64();
            BigInteger low = fuzzy.UInt64();
            return new NodeId(high, low);
        }

        internal static ServiceContext ServiceContext(this IFuzz fuzzy)
        {
            NodeContext nodeContext = fuzzy.NodeContext();
            ICodePackageActivationContext activationContext = fuzzy.ICodePackageActivationContext();
            string serviceTypeName = fuzzy.String();
            Uri serviceName = fuzzy.Uri();
            byte[] initializationData = fuzzy.Array(fuzzy.Byte);
            Guid partitionId = Guid.NewGuid();
            long replicaOrInstanceId = fuzzy.Int64();
            return new Mock<ServiceContext>(nodeContext, activationContext, serviceTypeName, serviceName, initializationData, partitionId, replicaOrInstanceId).Object;
        }
    }
}
