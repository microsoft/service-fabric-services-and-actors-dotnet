// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V1
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;

    /// <summary>
    ///  Represents the body of the actor messages.
    /// </summary>
    [Obsolete("This class is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
    [DataContract(Name = "msgBody", Namespace = Constants.Namespace)]
    internal class ActorMessageBody
    {
        [DataMember(Name = "val", IsRequired = false, Order = 1)]
        public object Value { get; set; }
    }
}
