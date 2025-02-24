// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1
{
    using System;
    using System.Runtime.Serialization;

    [Obsolete(DeprecationMessage.RemotingV1)]
    [DataContract(Name = "msgBody", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class ServiceRemotingMessageBody
    {
        [DataMember(Name = "val", IsRequired = false, Order = 1)]
        public object Value { get; set; }
    }
}
