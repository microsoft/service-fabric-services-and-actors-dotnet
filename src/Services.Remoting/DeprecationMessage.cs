using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Remoting
{
    internal static class DeprecationMessage
    {
        public const string RemotingV1 = "Service Remoting V1 is deprecated. Please use Service Remoting V2 instead. " +
            "Please refer to the release notes for more details: https://github.com/microsoft/service-fabric/blob/master/release_notes/Deprecated/RemotingV1.md";
    }
}
