// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Services.Communication.Wcf;

    /// <summary>
    /// This represent response received during wcf remoting.
    /// </summary>
    [DataContract(Name = "ResponseMessage", Namespace = WcfConstants.Namespace)]
    public class ResponseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMessage"/> class with Empty Headers and Body
        /// </summary>
        public ResponseMessage()
        {
            this.MessageHeaders = default(ArraySegment<byte>);
            this.ResponseBody = new List<ArraySegment<byte>>();
        }

        /// <summary>
        /// Gets or sets headers in the response Message
        /// </summary>
        [DataMember(Name = "Headers")]
        public ArraySegment<byte> MessageHeaders { get; set; }

        /// <summary>
        /// Gets or sets message body in the response Message
        /// </summary>
        [DataMember(Name = "ResponseBody")]
        public IEnumerable<ArraySegment<byte>> ResponseBody { get; set; }
    }
}
