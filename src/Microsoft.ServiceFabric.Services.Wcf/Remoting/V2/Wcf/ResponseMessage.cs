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
        /// Creates Response with Empty Headers and Body
        /// </summary>
        public ResponseMessage()
        {
            this.MessageHeaders = new ArraySegment<byte>();
            this.ResponseBody = new List<ArraySegment<byte>>();
        }

        /// <summary>
        /// Headers in the response Message
        /// </summary>
        [DataMember(Name = "Headers")]
        public ArraySegment<byte> MessageHeaders { get; set; }

        /// <summary>
        /// Message body in the response Message
        /// </summary>
        [DataMember(Name = "ResponseBody")]
        public IEnumerable<ArraySegment<byte>> ResponseBody { get; set; }
    }
}
