// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for updating an ongoing application upgrade.
    /// </summary>
    public partial class ApplicationUpgradeUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationUpgradeUpdateDescription class.
        /// </summary>
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="upgradeKind">The kind of upgrade out of the following possible values. Possible values include:
        /// 'Invalid', 'Rolling'</param>
        /// <param name="applicationHealthPolicy">Defines a health policy used to evaluate the health of an application or one
        /// of its children entities.
        /// </param>
        /// <param name="updateDescription">Describes the parameters for updating a rolling upgrade of application or
        /// cluster.</param>
        public ApplicationUpgradeUpdateDescription(
            ApplicationName name,
            UpgradeKind? upgradeKind = Common.UpgradeKind.Rolling,
            ApplicationHealthPolicy applicationHealthPolicy = default(ApplicationHealthPolicy),
            RollingUpgradeUpdateDescription updateDescription = default(RollingUpgradeUpdateDescription))
        {
            name.ThrowIfNull(nameof(name));
            upgradeKind.ThrowIfNull(nameof(upgradeKind));
            this.Name = name;
            this.UpgradeKind = upgradeKind;
            this.ApplicationHealthPolicy = applicationHealthPolicy;
            this.UpdateDescription = updateDescription;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName Name { get; }

        /// <summary>
        /// Gets the kind of upgrade out of the following possible values. Possible values include: 'Invalid', 'Rolling'
        /// </summary>
        public UpgradeKind? UpgradeKind { get; }

        /// <summary>
        /// Gets defines a health policy used to evaluate the health of an application or one of its children entities.
        /// </summary>
        public ApplicationHealthPolicy ApplicationHealthPolicy { get; }

        /// <summary>
        /// Gets the parameters for updating a rolling upgrade of application or cluster.
        /// </summary>
        public RollingUpgradeUpdateDescription UpdateDescription { get; }
    }
}
