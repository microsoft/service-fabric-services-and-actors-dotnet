// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Metadata associated with a specific application type.
    /// </summary>
    public partial class ApplicationTypeMetadata
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeMetadata class.
        /// </summary>
        /// <param name="applicationTypeProvisionTimestamp">The timestamp when the application type was provisioned.</param>
        /// <param name="armMetadata">Common ArmMetadata associated with Service Fabric Entities.</param>
        public ApplicationTypeMetadata(
            string applicationTypeProvisionTimestamp = default(string),
            ArmMetadata armMetadata = default(ArmMetadata))
        {
            this.ApplicationTypeProvisionTimestamp = applicationTypeProvisionTimestamp;
            this.ArmMetadata = armMetadata;
        }

        /// <summary>
        /// Gets the timestamp when the application type was provisioned.
        /// </summary>
        public string ApplicationTypeProvisionTimestamp { get; }

        /// <summary>
        /// Gets common ArmMetadata associated with Service Fabric Entities.
        /// </summary>
        public ArmMetadata ArmMetadata { get; }
    }
}
