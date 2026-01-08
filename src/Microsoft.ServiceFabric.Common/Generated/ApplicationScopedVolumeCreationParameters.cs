// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes parameters for creating application-scoped volumes.
    /// </summary>
    public abstract partial class ApplicationScopedVolumeCreationParameters
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationScopedVolumeCreationParameters class.
        /// </summary>
        /// <param name="kind">Specifies the application-scoped volume kind.</param>
        /// <param name="description">User readable description of the volume.</param>
        protected ApplicationScopedVolumeCreationParameters(
            ApplicationScopedVolumeKind? kind,
            string description = default(string))
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
            this.Description = description;
        }

        /// <summary>
        /// Gets user readable description of the volume.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets specifies the application-scoped volume kind.
        /// </summary>
        public ApplicationScopedVolumeKind? Kind { get; }
    }
}
