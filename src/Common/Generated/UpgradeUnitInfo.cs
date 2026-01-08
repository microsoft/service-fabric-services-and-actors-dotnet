// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about an upgrade unit.
    /// </summary>
    public partial class UpgradeUnitInfo
    {
        /// <summary>
        /// Initializes a new instance of the UpgradeUnitInfo class.
        /// </summary>
        /// <param name="name">The name of the upgrade unit</param>
        /// <param name="state">The state of the upgrade unit. Possible values include: 'Invalid', 'Pending', 'InProgress',
        /// 'Completed', 'Failed'</param>
        public UpgradeUnitInfo(
            string name = default(string),
            UpgradeUnitState? state = default(UpgradeUnitState?))
        {
            this.Name = name;
            this.State = state;
        }

        /// <summary>
        /// Gets the name of the upgrade unit
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the state of the upgrade unit. Possible values include: 'Invalid', 'Pending', 'InProgress', 'Completed',
        /// 'Failed'
        /// </summary>
        public UpgradeUnitState? State { get; }
    }
}
