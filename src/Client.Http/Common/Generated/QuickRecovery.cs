// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for QuickRecovery.
    /// </summary>
    public enum QuickRecovery
    {
        /// <summary>
        /// Quick Recovery is disabled. Use normal restore process with manual intervention.
        /// </summary>
        Disabled,

        /// <summary>
        /// Recover from primary replica when primary has newer or equal data in case of Partial Data Loss. Requires manual
        /// intervention when backup has newer data.
        /// </summary>
        FromPrimary,
    }
}
