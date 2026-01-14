// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes extension of a service type defined in the service manifest.
    /// </summary>
    public partial class ServiceTypeExtensionDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeExtensionDescription class.
        /// </summary>
        /// <param name="key">The name of the extension.</param>
        /// <param name="value">The extension value.</param>
        public ServiceTypeExtensionDescription(
            string key = default(string),
            string value = default(string))
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the extension value.
        /// </summary>
        public string Value { get; }
    }
}
