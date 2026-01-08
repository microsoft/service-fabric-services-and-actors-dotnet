// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes properties of secret value resource.
    /// </summary>
    public partial class SecretValueProperties
    {
        /// <summary>
        /// Initializes a new instance of the SecretValueProperties class.
        /// </summary>
        /// <param name="value">The actual value of the secret.</param>
        public SecretValueProperties(
            string value = default(string))
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the actual value of the secret.
        /// </summary>
        public string Value { get; }
    }
}
