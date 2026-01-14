// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about an upgrade domain.
    /// </summary>
    public partial class UpgradeDomainInfo
    {
        /// <summary>
        /// Initializes a new instance of the UpgradeDomainInfo class.
        /// </summary>
        /// <param name="name">The name of the upgrade domain</param>
        /// <param name="state">The state of the upgrade domain. Possible values include: 'Invalid', 'Pending', 'InProgress',
        /// 'Completed'</param>
        public UpgradeDomainInfo(
            string name = default(string),
            UpgradeDomainState? state = default(UpgradeDomainState?))
        {
            this.Name = name;
            this.State = state;
        }

        /// <summary>
        /// Gets the name of the upgrade domain
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the state of the upgrade domain. Possible values include: 'Invalid', 'Pending', 'InProgress', 'Completed'
        /// </summary>
        public UpgradeDomainState? State { get; }
    }
}
