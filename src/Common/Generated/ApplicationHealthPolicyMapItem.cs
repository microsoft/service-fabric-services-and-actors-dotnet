// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an item in ApplicationHealthPolicyMap.
    /// </summary>
    public partial class ApplicationHealthPolicyMapItem
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthPolicyMapItem class.
        /// </summary>
        /// <param name="key">The key of the application health policy map item. This is the name of the application.</param>
        /// <param name="value">The value of the application health policy map item. This is the ApplicationHealthPolicy for
        /// this application.</param>
        public ApplicationHealthPolicyMapItem(
            ApplicationName key,
            ApplicationHealthPolicy value)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key of the application health policy map item. This is the name of the application.
        /// </summary>
        public ApplicationName Key { get; }

        /// <summary>
        /// Gets the value of the application health policy map item. This is the ApplicationHealthPolicy for this application.
        /// </summary>
        public ApplicationHealthPolicy Value { get; }
    }
}
