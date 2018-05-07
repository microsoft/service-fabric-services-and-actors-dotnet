// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime
{
    using System.Fabric;

    internal class FabricTransportCallbackNotFoundException : FabricException
    {
        public FabricTransportCallbackNotFoundException(string message)
            : base(message)
        {
        }
    }
}
