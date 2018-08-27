// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "msgBody", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceRemotingRequestMessageBody : IServiceRemotingRequestMessageBody
    {
        [DataMember]
        private Dictionary<string, object> parameters;

        public ServiceRemotingRequestMessageBody(int parameterInfos)
        {
            this.parameters = new Dictionary<string, object>(parameterInfos);
        }

        public void SetParameter(int position, string paramName, object parameter)
        {
            this.parameters[paramName] = parameter;
        }

        public object GetParameter(int position, string paramName, Type paramType)
        {
            return this.parameters[paramName];
        }
    }
}
