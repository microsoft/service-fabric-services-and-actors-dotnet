// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Mesh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2;
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Mesh.Builder;

    internal class ServiceRemotingSerializationManager : Base.V2.ServiceRemotingMessageSerializationManager
    {
        public ServiceRemotingSerializationManager(IServiceRemotingMessageSerializationProvider serializationProvider, IServiceRemotingMessageHeaderSerializer headerSerializer, bool useWrappedMessage = false)
            : base(serializationProvider, headerSerializer, useWrappedMessage)
        {
        }

        internal override InterfaceDetails GetInterfaceDetails(int interfaceId)
        {
            if (!ServiceCodeBuilder.TryGetKnownTypes(interfaceId, out var interfaceDetails))
            {
                throw new ArgumentException("No interface found with this Id  " + interfaceId);
            }

            return interfaceDetails;
        }
    }
}
