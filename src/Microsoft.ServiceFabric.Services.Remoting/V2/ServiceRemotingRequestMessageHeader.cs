// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Runtime.Serialization;

    [DataContract(Name = "ServiceMessageHeaders", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceRemotingRequestMessageHeader : IServiceRemotingRequestMessageHeader
    {
        internal const string CancellationHeaderName = "CancellationHeader";

        [DataMember(Name = "Headers", IsRequired = true, Order = 2)]
        private Dictionary<string, byte[]> headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingRequestMessageHeader"/> class.
        /// </summary>
        public ServiceRemotingRequestMessageHeader()
        {
            this.headers = new Dictionary<string, byte[]>();
            this.InvocationId = null;
        }

        /// <summary>
        /// Gets or sets the methodId of the remote method
        /// </summary>
        /// <value>Method id</value>
        [DataMember(Name = "MethodId", IsRequired = true, Order = 0)]
        public int MethodId { get; set; }

        /// <summary>
        /// Gets or sets the interface id of the remote interface.
        /// </summary>
        /// <value>Interface id</value>
        [DataMember(Name = "InterfaceId", IsRequired = true, Order = 1)]
        public int InterfaceId { get; set; }

        /// <summary>
        /// Gets or sets identifier for the remote method invocation
        /// </summary>
        [DataMember(Name = "InvocationId", IsRequired = false, Order = 3, EmitDefaultValue = false)]
        public string InvocationId { get; set; }

        public void AddHeader(string headerName, byte[] headerValue)
        {
            if (this.headers.ContainsKey(headerName))
            {
                throw new FabricElementAlreadyExistsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorHeaderAlreadyExists,
                        headerName));
            }

            this.headers[headerName] = headerValue;
        }

        public bool TryGetHeaderValue(string headerName, out byte[] headerValue)
        {
            headerValue = null;

            if (this.headers == null)
            {
                return false;
            }

            return this.headers.TryGetValue(headerName, out headerValue);
        }
    }
}
