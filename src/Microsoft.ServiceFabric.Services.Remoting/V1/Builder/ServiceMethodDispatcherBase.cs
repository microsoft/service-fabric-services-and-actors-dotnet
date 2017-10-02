// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Service method dispatcher class. Internal use only.
    /// </summary>
    public abstract class 
        ServiceMethodDispatcherBase : MethodDispatcherBaseWithSerializer
    {

        internal override DataContractSerializer CreateRequestMessageBodySerializer(IEnumerable<Type> requestBodyValueTypes)
        {
            return ServiceRemotingMessageSerializer.GetMessageBodySerializer(requestBodyValueTypes);
        }


        internal override DataContractSerializer CreateResponseMessageBodySerializer(IEnumerable<Type> responseBodyValueTypes)
        {
            return ServiceRemotingMessageSerializer.GetMessageBodySerializer(responseBodyValueTypes);
        }

        internal override object GetRequestMessageBodyValue(object requestMessageBody)
        {
            return ((ServiceRemotingMessageBody) requestMessageBody).Value;
        }

        internal override object CreateResponseMessageBody(object responseMessageBodyValue)
        {
            return new ServiceRemotingMessageBody {Value = responseMessageBodyValue};
        }
    }
}
