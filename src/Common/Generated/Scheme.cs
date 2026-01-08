// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for Scheme.
    /// </summary>
    public enum Scheme
    {
        /// <summary>
        /// Indicates that the probe is http.
        /// </summary>
        Http,

        /// <summary>
        /// Indicates that the probe is https. No cert validation.
        /// </summary>
        Https,
    }
}
