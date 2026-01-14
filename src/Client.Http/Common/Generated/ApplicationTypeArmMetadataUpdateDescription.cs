// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Arm metadata to be associated with a specific application type
    /// </summary>
    public partial class ApplicationTypeArmMetadataUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeArmMetadataUpdateDescription class.
        /// </summary>
        /// <param name="armMetadata">Common ArmMetadata associated with Service Fabric Entities.</param>
        public ApplicationTypeArmMetadataUpdateDescription(
            ArmMetadata armMetadata)
        {
            armMetadata.ThrowIfNull(nameof(armMetadata));
            this.ArmMetadata = armMetadata;
        }

        /// <summary>
        /// Gets common ArmMetadata associated with Service Fabric Entities.
        /// </summary>
        public ArmMetadata ArmMetadata { get; }
    }
}
