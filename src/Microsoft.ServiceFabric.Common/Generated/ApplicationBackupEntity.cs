// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifies the Service Fabric application which is being backed up.
    /// </summary>
    public partial class ApplicationBackupEntity : BackupEntity
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationBackupEntity class.
        /// </summary>
        /// <param name="applicationName">The name of the application, including the 'fabric:' URI scheme.</param>
        public ApplicationBackupEntity(
            ApplicationName applicationName = default(ApplicationName))
            : base(
                Common.BackupEntityKind.Application)
        {
            this.ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName ApplicationName { get; }
    }
}
