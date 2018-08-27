// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2
{
    using System.Runtime.Serialization;

    /// <summary>
    /// This is a marker class indicating the remoting request / response is wrapped or not.
    /// </summary>
    [DataContract(Name = "msgBodywrapped", Namespace = Constants.ServiceCommunicationNamespace)]
    public abstract class WrappedMessage
    {
        /// <summary>
        /// Gets or sets  the wrapped object.
        /// </summary>
        [DataMember(Name = "value", IsRequired = true, Order = 1)]
        public object Value
        {
            get;
            set;
        }
    }
}
