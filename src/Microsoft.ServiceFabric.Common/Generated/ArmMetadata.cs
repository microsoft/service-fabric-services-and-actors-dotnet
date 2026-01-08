// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Common ArmMetadata associated with Service Fabric Entities.
    /// </summary>
    public partial class ArmMetadata
    {
        /// <summary>
        /// Initializes a new instance of the ArmMetadata class.
        /// </summary>
        /// <param name="armResourceId">A string containing the ArmResourceId.</param>
        public ArmMetadata(
            string armResourceId = default(string))
        {
            this.ArmResourceId = armResourceId;
        }

        /// <summary>
        /// Gets a string containing the ArmResourceId.
        /// </summary>
        public string ArmResourceId { get; }
    }
}
