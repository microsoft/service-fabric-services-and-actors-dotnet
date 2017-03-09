// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using System.Runtime.Serialization;

    /// <summary>
    ///  Body of the actor messages.
    /// </summary>
    [DataContract(Name = "msgBody", Namespace = Constants.Namespace)]
    internal class ActorMessageBody
    {
        [DataMember(Name = "val", IsRequired = false, Order = 1)]
        public object Value { get; set; }
    }
}