// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Specifies result of validating a cluster upgrade.
    /// </summary>
    public partial class ValidateClusterUpgradeResult
    {
        /// <summary>
        /// Initializes a new instance of the ValidateClusterUpgradeResult class.
        /// </summary>
        /// <param name="serviceHostUpgradeImpact">The expected impact of the upgrade. Possible values include: 'Invalid',
        /// 'None', 'ServiceHostRestart', 'UnexpectedServiceHostRestart'</param>
        /// <param name="validationDetails">A string containing additional details for the Fabric upgrade validation
        /// result.</param>
        public ValidateClusterUpgradeResult(
            ServiceHostUpgradeImpact? serviceHostUpgradeImpact = default(ServiceHostUpgradeImpact?),
            string validationDetails = default(string))
        {
            this.ServiceHostUpgradeImpact = serviceHostUpgradeImpact;
            this.ValidationDetails = validationDetails;
        }

        /// <summary>
        /// Gets the expected impact of the upgrade. Possible values include: 'Invalid', 'None', 'ServiceHostRestart',
        /// 'UnexpectedServiceHostRestart'
        /// </summary>
        public ServiceHostUpgradeImpact? ServiceHostUpgradeImpact { get; }

        /// <summary>
        /// Gets a string containing additional details for the Fabric upgrade validation result.
        /// </summary>
        public string ValidationDetails { get; }
    }
}
