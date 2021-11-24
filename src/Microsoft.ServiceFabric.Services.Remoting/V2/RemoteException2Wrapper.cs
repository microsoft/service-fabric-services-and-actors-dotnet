// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "RemoteException2Wrapper", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class RemoteException2Wrapper
    {
        [DataMember(Name = "Exceptions", Order = 0)]
        public List<RemoteException2> Exceptions { get; set; }
    }
}
