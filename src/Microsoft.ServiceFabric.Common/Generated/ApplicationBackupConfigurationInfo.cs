// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Backup configuration information for a specific Service Fabric application specifying what backup policy is being
    /// applied and suspend description, if any.
    /// </summary>
    public partial class ApplicationBackupConfigurationInfo : BackupConfigurationInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationBackupConfigurationInfo class.
        /// </summary>
        /// <param name="policyName">The name of the backup policy which is applicable to this Service Fabric application or
        /// service or partition.</param>
        /// <param name="policyInheritedFrom">Specifies the scope at which the backup policy is applied.
        /// . Possible values include: 'Invalid', 'Partition', 'Service', 'Application'</param>
        /// <param name="suspensionInfo">Describes the backup suspension details.
        /// </param>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        public ApplicationBackupConfigurationInfo(
            string policyName = default(string),
            BackupPolicyScope? policyInheritedFrom = default(BackupPolicyScope?),
            BackupSuspensionInfo suspensionInfo = default(BackupSuspensionInfo),
            ApplicationName applicationName = default(ApplicationName))
            : base(
                Common.BackupEntityKind.Application,
                policyName,
                policyInheritedFrom,
                suspensionInfo)
        {
            this.ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }
    }
}
