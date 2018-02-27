// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "msgBody", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceRemotingRequestMessageBody : IServiceRemotingRequestMessageBody
    {
        public ServiceRemotingRequestMessageBody(int parameterInfos)
        {
            this.parameters = new Dictionary<string, object>(parameterInfos);
        }

        [DataMember] private Dictionary<string, object> parameters;

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
