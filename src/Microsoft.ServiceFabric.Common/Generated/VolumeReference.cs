// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a reference to a volume resource.
    /// </summary>
    public partial class VolumeReference
    {
        /// <summary>
        /// Initializes a new instance of the VolumeReference class.
        /// </summary>
        /// <param name="name">Name of the volume being referenced.</param>
        /// <param name="destinationPath">The path within the container at which the volume should be mounted. Only valid path
        /// characters are allowed.</param>
        /// <param name="readOnly">The flag indicating whether the volume is read only. Default is 'false'.</param>
        public VolumeReference(
            string name,
            string destinationPath,
            bool? readOnly = default(bool?))
        {
            name.ThrowIfNull(nameof(name));
            destinationPath.ThrowIfNull(nameof(destinationPath));
            this.Name = name;
            this.DestinationPath = destinationPath;
            this.ReadOnly = readOnly;
        }

        /// <summary>
        /// Gets name of the volume being referenced.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the flag indicating whether the volume is read only. Default is 'false'.
        /// </summary>
        public bool? ReadOnly { get; }

        /// <summary>
        /// Gets the path within the container at which the volume should be mounted. Only valid path characters are allowed.
        /// </summary>
        public string DestinationPath { get; }
    }
}
