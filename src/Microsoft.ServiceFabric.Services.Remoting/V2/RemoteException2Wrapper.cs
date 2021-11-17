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
        public RemoteException2[] Exceptions { get; set; }

        [DataContract(Name = "RemoteException2", Namespace = Constants.ServiceCommunicationNamespace)]
        public class RemoteException2
        {
            [DataMember(Name = "Type", Order = 0)]
            public string Type { get; set; }

            [DataMember(Name = "Message", Order = 1)]
            public string Message { get; set; }

            [DataMember(Name = "StackTrace", Order = 2)]
            public string StackTrace { get; set; }

            [DataMember(Name = "Data", Order = 3)]
            public Dictionary<string, string> Data { get; set; }

            [DataMember(Name = "InnerExceptions", Order = 4)]
            public RemoteException2[] InnerExceptions { get; set; }
        }
    }
}
