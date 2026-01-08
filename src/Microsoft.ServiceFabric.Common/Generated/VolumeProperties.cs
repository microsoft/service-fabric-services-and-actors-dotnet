// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes properties of a volume resource.
    /// </summary>
    public partial class VolumeProperties
    {
        /// <summary>
        /// Initializes a new instance of the VolumeProperties class.
        /// </summary>
        /// <param name="description">User readable description of the volume.</param>
        /// <param name="azureFileParameters">This type describes a volume provided by an Azure Files file share.</param>
        public VolumeProperties(
            string description = default(string),
            VolumeProviderParametersAzureFile azureFileParameters = default(VolumeProviderParametersAzureFile))
        {
            this.Description = description;
            this.AzureFileParameters = azureFileParameters;
        }

        /// <summary>
        /// Gets user readable description of the volume.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets status of the volume. Possible values include: 'Unknown', 'Ready', 'Upgrading', 'Creating', 'Deleting',
        /// 'Failed'
        /// 
        /// Status of the resource.
        /// </summary>
        public ResourceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets additional information about the current status of the volume.
        /// </summary>
        public string StatusDetails { get; internal set; }

        /// <summary>
        /// Gets this type describes a volume provided by an Azure Files file share.
        /// </summary>
        public VolumeProviderParametersAzureFile AzureFileParameters { get; }
    }
}
