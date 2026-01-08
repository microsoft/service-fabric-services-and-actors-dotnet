// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a volume whose lifetime is scoped to the application's lifetime.
    /// </summary>
    public partial class ApplicationScopedVolume : VolumeReference
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationScopedVolume class.
        /// </summary>
        /// <param name="name">Name of the volume being referenced.</param>
        /// <param name="destinationPath">The path within the container at which the volume should be mounted. Only valid path
        /// characters are allowed.</param>
        /// <param name="creationParameters">Describes parameters for creating application-scoped volumes.</param>
        /// <param name="readOnly">The flag indicating whether the volume is read only. Default is 'false'.</param>
        public ApplicationScopedVolume(
            string name,
            string destinationPath,
            ApplicationScopedVolumeCreationParameters creationParameters,
            bool? readOnly = default(bool?))
            : base(
                name,
                destinationPath,
                readOnly)
        {
            creationParameters.ThrowIfNull(nameof(creationParameters));
            this.CreationParameters = creationParameters;
        }

        /// <summary>
        /// Gets parameters for creating application-scoped volumes.
        /// </summary>
        public ApplicationScopedVolumeCreationParameters CreationParameters { get; }
    }
}
