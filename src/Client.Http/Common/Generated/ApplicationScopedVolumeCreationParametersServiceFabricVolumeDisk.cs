// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes parameters for creating application-scoped volumes provided by Service Fabric Volume Disks
    /// </summary>
    public partial class ApplicationScopedVolumeCreationParametersServiceFabricVolumeDisk : ApplicationScopedVolumeCreationParameters
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationScopedVolumeCreationParametersServiceFabricVolumeDisk class.
        /// </summary>
        /// <param name="sizeDisk">Volume size. Possible values include: 'Small', 'Medium', 'Large'</param>
        /// <param name="description">User readable description of the volume.</param>
        public ApplicationScopedVolumeCreationParametersServiceFabricVolumeDisk(
            SizeTypes? sizeDisk,
            string description = default(string))
            : base(
                Common.ApplicationScopedVolumeKind.ServiceFabricVolumeDisk,
                description)
        {
            sizeDisk.ThrowIfNull(nameof(sizeDisk));
            this.SizeDisk = sizeDisk;
        }

        /// <summary>
        /// Gets volume size. Possible values include: 'Small', 'Medium', 'Large'
        /// </summary>
        public SizeTypes? SizeDisk { get; }
    }
}
