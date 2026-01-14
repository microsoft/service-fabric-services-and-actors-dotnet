// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an item in ApplicationTypeHealthPolicyMap.
    /// </summary>
    public partial class ApplicationTypeHealthPolicyMapItem
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeHealthPolicyMapItem class.
        /// </summary>
        /// <param name="key">The key of the application type health policy map item. This is the name of the application
        /// type.</param>
        /// <param name="value">The value of the application type health policy map item.
        /// The max percent unhealthy applications allowed for the application type. Must be between zero and 100.
        /// </param>
        public ApplicationTypeHealthPolicyMapItem(
            string key,
            int? value)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key of the application type health policy map item. This is the name of the application type.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value of the application type health policy map item.
        /// The max percent unhealthy applications allowed for the application type. Must be between zero and 100.
        /// </summary>
        public int? Value { get; }
    }
}
