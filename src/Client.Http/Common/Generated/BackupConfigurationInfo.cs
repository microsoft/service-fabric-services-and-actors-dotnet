// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the backup configuration information.
    /// </summary>
    public abstract partial class BackupConfigurationInfo
    {
        /// <summary>
        /// Initializes a new instance of the BackupConfigurationInfo class.
        /// </summary>
        /// <param name="kind">The entity type of a Service Fabric entity such as Application, Service or a Partition where
        /// periodic backups can be enabled.
        /// </param>
        /// <param name="policyName">The name of the backup policy which is applicable to this Service Fabric application or
        /// service or partition.</param>
        /// <param name="policyInheritedFrom">Specifies the scope at which the backup policy is applied.
        /// . Possible values include: 'Invalid', 'Partition', 'Service', 'Application'</param>
        /// <param name="suspensionInfo">Describes the backup suspension details.
        /// </param>
        protected BackupConfigurationInfo(
            BackupEntityKind? kind,
            string policyName = default(string),
            BackupPolicyScope? policyInheritedFrom = default(BackupPolicyScope?),
            BackupSuspensionInfo suspensionInfo = default(BackupSuspensionInfo))
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
            this.PolicyName = policyName;
            this.PolicyInheritedFrom = policyInheritedFrom;
            this.SuspensionInfo = suspensionInfo;
        }

        /// <summary>
        /// Gets the name of the backup policy which is applicable to this Service Fabric application or service or partition.
        /// </summary>
        public string PolicyName { get; }

        /// <summary>
        /// Gets specifies the scope at which the backup policy is applied.
        /// . Possible values include: 'Invalid', 'Partition', 'Service', 'Application'
        /// </summary>
        public BackupPolicyScope? PolicyInheritedFrom { get; }

        /// <summary>
        /// Gets the backup suspension details.
        /// </summary>
        public BackupSuspensionInfo SuspensionInfo { get; }

        /// <summary>
        /// Gets the entity type of a Service Fabric entity such as Application, Service or a Partition where periodic backups
        /// can be enabled.
        /// </summary>
        public BackupEntityKind? Kind { get; }
    }
}
