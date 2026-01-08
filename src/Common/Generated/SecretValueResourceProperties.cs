// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes properties of a secret value resource.
    /// </summary>
    public partial class SecretValueResourceProperties : SecretValueProperties
    {
        /// <summary>
        /// Initializes a new instance of the SecretValueResourceProperties class.
        /// </summary>
        /// <param name="value">The actual value of the secret.</param>
        public SecretValueResourceProperties(
            string value = default(string))
            : base(
                value)
        {
        }
    }
}
