// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an item in ServiceTypeHealthPolicyMap.
    /// </summary>
    public partial class ServiceTypeHealthPolicyMapItem
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeHealthPolicyMapItem class.
        /// </summary>
        /// <param name="key">The key of the service type health policy map item. This is the name of the service type.</param>
        /// <param name="value">The value of the service type health policy map item. This is the ServiceTypeHealthPolicy for
        /// this service type.</param>
        public ServiceTypeHealthPolicyMapItem(
            string key,
            ServiceTypeHealthPolicy value)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key of the service type health policy map item. This is the name of the service type.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value of the service type health policy map item. This is the ServiceTypeHealthPolicy for this service
        /// type.
        /// </summary>
        public ServiceTypeHealthPolicy Value { get; }
    }
}
