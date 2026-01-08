// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Http headers.
    /// </summary>
    public partial class ProbeHttpGetHeaders
    {
        /// <summary>
        /// Initializes a new instance of the ProbeHttpGetHeaders class.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public ProbeHttpGetHeaders(
            string name,
            string value)
        {
            name.ThrowIfNull(nameof(name));
            value.ThrowIfNull(nameof(value));
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the header.
        /// </summary>
        public string Value { get; }
    }
}
