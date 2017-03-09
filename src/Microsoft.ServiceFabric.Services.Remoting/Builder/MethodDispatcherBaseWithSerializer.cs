// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Common;

    /// <summary>
    /// The class dispatches the requests from the client to the interface/method of the remoted objectts.
    /// This class is used by remoting code generator. This class caches the Serializer.
    /// </summary>
    public abstract class MethodDispatcherBaseWithSerializer : MethodDispatcherBase
    {
        private DataContractSerializer requestBodySerializer;
        private DataContractSerializer responseBodySerializer;

        internal override void SetRequestKnownTypes(IEnumerable<Type> requestBodyTypes, IEnumerable<Type> responseBodyTypes)
        {
            this.requestBodySerializer = this.CreateRequestMessageBodySerializer(requestBodyTypes);
            this.responseBodySerializer = this.CreateResponseMessageBodySerializer(responseBodyTypes);
        }

        internal object DeserializeRequestMessageBody(byte[] requestMsgBodyBytes)
        {
            return SerializationUtility.Deserialize(this.requestBodySerializer, requestMsgBodyBytes);
        }

        internal byte[] SerializeResponseMessageBody(object responseMsgBody)
        {
            return SerializationUtility.Serialize(this.responseBodySerializer, responseMsgBody);
        }

        internal abstract DataContractSerializer CreateRequestMessageBodySerializer(IEnumerable<Type> requestBodyValueTypes);

        internal abstract DataContractSerializer CreateResponseMessageBodySerializer(IEnumerable<Type> responseBodyValueTypes);
    }
}