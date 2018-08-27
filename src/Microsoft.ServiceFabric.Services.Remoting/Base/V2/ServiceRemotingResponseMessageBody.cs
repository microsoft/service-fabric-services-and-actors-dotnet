// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "msgResponse", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceRemotingResponseMessageBody : IServiceRemotingResponseMessageBody
    {
        [DataMember]
        private object response;

        public void Set(object response)
        {
            this.response = response;
        }

        public object Get(Type paramType)
        {
            return this.response;
        }
    }
}
