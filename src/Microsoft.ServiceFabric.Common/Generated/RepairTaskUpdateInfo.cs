// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the result of an operation that created or updated a repair task.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class RepairTaskUpdateInfo
    {
        /// <summary>
        /// Initializes a new instance of the RepairTaskUpdateInfo class.
        /// </summary>
        /// <param name="version">The new version of the repair task.</param>
        public RepairTaskUpdateInfo(
            string version)
        {
            version.ThrowIfNull(nameof(version));
            this.Version = version;
        }

        /// <summary>
        /// Gets the new version of the repair task.
        /// </summary>
        public string Version { get; }
    }
}
