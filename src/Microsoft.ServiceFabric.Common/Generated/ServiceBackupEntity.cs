// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifies the Service Fabric stateful service which is being backed up.
    /// </summary>
    public partial class ServiceBackupEntity : BackupEntity
    {
        /// <summary>
        /// Initializes a new instance of the ServiceBackupEntity class.
        /// </summary>
        /// <param name="serviceName">The full name of the service with 'fabric:' URI scheme.</param>
        public ServiceBackupEntity(
            ServiceName serviceName = default(ServiceName))
            : base(
                Common.BackupEntityKind.Service)
        {
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the full name of the service with 'fabric:' URI scheme.
        /// </summary>
        public ServiceName ServiceName { get; }
    }
}
