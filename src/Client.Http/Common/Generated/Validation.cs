// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for Validation.
    /// </summary>
    public enum Validation
    {
        /// <summary>
        /// Backup validation is disabled. Backups taken will not be validated.
        /// </summary>
        Disabled,

        /// <summary>
        /// Backup validation is enabled. Backups taken will be validated.
        /// </summary>
        Enabled,
    }
}
