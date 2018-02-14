// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;

    internal abstract class ProxyGeneratorWithSerializer : ProxyGenerator
    {
        private readonly IDictionary<int, DataContractSerializer> requestMessageBodySerializers;
        private readonly IDictionary<int, DataContractSerializer> responseMessageBodySerializers;

        protected ProxyGeneratorWithSerializer(
            Type proxyInterfaceType,
            IDictionary<int, DataContractSerializer> requestMessageBodySerializers,
            IDictionary<int, DataContractSerializer> responseMessageBodySerializers) : base(proxyInterfaceType)
        {
            this.requestMessageBodySerializers = requestMessageBodySerializers;
            this.responseMessageBodySerializers = responseMessageBodySerializers;
        }

        public DataContractSerializer GetRequestMessageBodySerializer(int interfaceId)
        {
            return this.requestMessageBodySerializers[interfaceId];
        }

        public DataContractSerializer GetResponseMessageBodySerializer(int interfaceId)
        {
            return this.responseMessageBodySerializers[interfaceId];
        }
    }
}
