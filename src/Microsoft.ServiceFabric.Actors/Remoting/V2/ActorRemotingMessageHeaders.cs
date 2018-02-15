// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Header for the actor messages.
    /// </summary>
    [DataContract(Name = "ActorHeader", Namespace = Constants.Namespace)]
    internal class ActorRemotingMessageHeaders : IActorRemotingMessageHeaders
    {
        public ActorRemotingMessageHeaders()
        {
            this.headers = new Dictionary<string, byte[]>();
            this.InvocationId = null;
        }

        /// <summary>
        /// The methodId of the remote method
        /// </summary>
        /// <value>Method id</value>
        [DataMember(Name = "MethodId", IsRequired = true, Order = 0)]
        public int MethodId { get; set; }

        /// <summary>
        /// The interface id of the remote interface.
        /// </summary>
        /// <value>Interface id</value>
        [DataMember(Name = "InterfaceId", IsRequired = true, Order = 1)]
        public int InterfaceId { get; set; }

        /// <summary>
        /// Identifier for the remote method invocation
        /// </summary>
        [DataMember(Name = "InvocationId", IsRequired = false, Order = 2, EmitDefaultValue = false)]
        public string InvocationId { get; set; }


        [DataMember(IsRequired = false, Order = 3)]
        public ActorId ActorId { get; set; }

        [DataMember(IsRequired = false, Order = 4)]
        public string CallContext { get; set; }

        [DataMember(Name = "Headers", IsRequired = true, Order = 5)] private Dictionary<string, byte[]> headers;

        public void AddHeader(string headerName, byte[] headerValue)
        {
            if (this.headers.ContainsKey(headerName))
            {
                throw new FabricElementAlreadyExistsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Services.Remoting.SR.ErrorHeaderAlreadyExists,
                        headerName));
            }

            this.headers[headerName] = headerValue;
        }

        public bool TryGetHeaderValue(string headerName, out byte[] headerValue)
        {
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
}
