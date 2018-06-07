// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.V1.Builder;

    /// <summary>
    /// The class is used by actor remoting code generator to generate a type that dispatches requests to actor
    /// object by invoking right method on it.
    /// </summary>
    public abstract class ActorMethodDispatcherBase : MethodDispatcherBaseWithSerializer
    {
        internal override DataContractSerializer CreateRequestMessageBodySerializer(
            IEnumerable<Type> requestBodyValueTypes)
        {
            return ActorMessageBodySerializer.GetActorMessageSerializer(requestBodyValueTypes);
        }

        internal override DataContractSerializer CreateResponseMessageBodySerializer(
            IEnumerable<Type> responseBodyValueTypes)
        {
            return ActorMessageBodySerializer.GetActorMessageSerializer(responseBodyValueTypes);
        }

        internal override object GetRequestMessageBodyValue(object requestMessageBody)
        {
            return ((ActorMessageBody)requestMessageBody).Value;
        }

        internal override object CreateResponseMessageBody(object responseMessageBodyValue)
        {
            return new ActorMessageBody() { Value = responseMessageBodyValue };
        }
    }
}
