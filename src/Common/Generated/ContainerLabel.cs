// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a container label.
    /// </summary>
    public partial class ContainerLabel
    {
        /// <summary>
        /// Initializes a new instance of the ContainerLabel class.
        /// </summary>
        /// <param name="name">The name of the container label.</param>
        /// <param name="value">The value of the container label.</param>
        public ContainerLabel(
            string name,
            string value)
        {
            name.ThrowIfNull(nameof(name));
            value.ThrowIfNull(nameof(value));
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the container label.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the container label.
        /// </summary>
        public string Value { get; }
    }
}
