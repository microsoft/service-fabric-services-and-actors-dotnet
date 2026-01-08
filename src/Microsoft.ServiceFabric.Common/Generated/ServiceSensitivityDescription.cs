// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes sensitivities for different types of replicas.
    /// </summary>
    public partial class ServiceSensitivityDescription
    {
        /// <summary>
        /// Initializes a new instance of the ServiceSensitivityDescription class.
        /// </summary>
        /// <param name="primaryDefaultSensitivity">Default sensitivity for a primary replica.</param>
        /// <param name="secondaryDefaultSensitivity">Default sensitivity for a secondary replica.</param>
        /// <param name="auxiliaryDefaultSensitivity">Default sensitivity for an auxiliary replica.</param>
        /// <param name="isMaximumSensitivity">Denotes whether the sensitivities should be set to maximum. If true, other
        /// values are ignored.</param>
        public ServiceSensitivityDescription(
            int? primaryDefaultSensitivity = default(int?),
            int? secondaryDefaultSensitivity = default(int?),
            int? auxiliaryDefaultSensitivity = default(int?),
            bool? isMaximumSensitivity = default(bool?))
        {
            this.PrimaryDefaultSensitivity = primaryDefaultSensitivity;
            this.SecondaryDefaultSensitivity = secondaryDefaultSensitivity;
            this.AuxiliaryDefaultSensitivity = auxiliaryDefaultSensitivity;
            this.IsMaximumSensitivity = isMaximumSensitivity;
        }

        /// <summary>
        /// Gets default sensitivity for a primary replica.
        /// </summary>
        public int? PrimaryDefaultSensitivity { get; }

        /// <summary>
        /// Gets default sensitivity for a secondary replica.
        /// </summary>
        public int? SecondaryDefaultSensitivity { get; }

        /// <summary>
        /// Gets default sensitivity for an auxiliary replica.
        /// </summary>
        public int? AuxiliaryDefaultSensitivity { get; }

        /// <summary>
        /// Gets denotes whether the sensitivities should be set to maximum. If true, other values are ignored.
        /// </summary>
        public bool? IsMaximumSensitivity { get; }
    }
}
