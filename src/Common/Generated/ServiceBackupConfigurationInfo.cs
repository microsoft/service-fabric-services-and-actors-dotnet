// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Backup configuration information for a specific Service Fabric service specifying what backup policy is being
    /// applied and suspend description, if any.
    /// </summary>
    public partial class ServiceBackupConfigurationInfo : BackupConfigurationInfo
    {
        /// <summary>
        /// Initializes a new instance of the ServiceBackupConfigurationInfo class.
        /// </summary>
        /// <param name="policyName">The name of the backup policy which is applicable to this Service Fabric application or
        /// service or partition.</param>
        /// <param name="policyInheritedFrom">Specifies the scope at which the backup policy is applied.
        /// . Possible values include: 'Invalid', 'Partition', 'Service', 'Application'</param>
        /// <param name="suspensionInfo">Describes the backup suspension details.
        /// </param>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        public ServiceBackupConfigurationInfo(
            string policyName = default(string),
            BackupPolicyScope? policyInheritedFrom = default(BackupPolicyScope?),
            BackupSuspensionInfo suspensionInfo = default(BackupSuspensionInfo),
            ServiceName serviceName = default(ServiceName))
            : base(
                Common.BackupEntityKind.Service,
                policyName,
                policyInheritedFrom,
                suspensionInfo)
        {
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName ServiceName { get; }
    }
}
