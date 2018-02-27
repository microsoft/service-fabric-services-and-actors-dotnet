// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication
{
    using System.Runtime.Serialization;

    [DataContract(Name = "ServiceExceptionData", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceExceptionData
    {
        public ServiceExceptionData(string type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        [DataMember]
        public string Type { get; private set; }

        [DataMember]
        public string Message { get; private set; }
    }
}
