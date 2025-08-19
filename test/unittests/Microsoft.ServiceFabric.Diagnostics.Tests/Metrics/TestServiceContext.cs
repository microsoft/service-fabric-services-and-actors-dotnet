using System;
using System.Fabric;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class TestServiceContext : ServiceContext
    {
        public TestServiceContext(NodeContext nodeContext, ICodePackageActivationContext codePackageActivationContext, string serviceTypeName, Uri serviceName, byte[] initializationData, Guid partitionId, long replicaId)
            : base(nodeContext, codePackageActivationContext, serviceTypeName, serviceName, initializationData, partitionId, replicaId)
        {
        }
    }
}
