// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    internal class CacheEntry
    {
        private readonly IServiceRemotingRequestMessageBodySerializer requestBodySerializer;
        private readonly IServiceRemotingResponseMessageBodySerializer responseBodySerializer;

        public CacheEntry(
            IServiceRemotingRequestMessageBodySerializer requestBodySerializer,
            IServiceRemotingResponseMessageBodySerializer responseBodySerializer)
        {
            this.requestBodySerializer = requestBodySerializer;
            this.responseBodySerializer = responseBodySerializer;
        }

        public IServiceRemotingRequestMessageBodySerializer RequestBodySerializer
        {
            get { return this.requestBodySerializer; }
        }

        public IServiceRemotingResponseMessageBodySerializer ResponseBodySerializer
        {
            get { return this.responseBodySerializer; }
        }
    }
}
