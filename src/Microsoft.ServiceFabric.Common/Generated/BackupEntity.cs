// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the Service Fabric entity that is configured for backup.
    /// </summary>
    public abstract partial class BackupEntity
    {
        /// <summary>
        /// Initializes a new instance of the BackupEntity class.
        /// </summary>
        /// <param name="entityKind">The entity type of a Service Fabric entity such as Application, Service or a Partition
        /// where periodic backups can be enabled.
        /// </param>
        protected BackupEntity(
            BackupEntityKind? entityKind)
        {
            entityKind.ThrowIfNull(nameof(entityKind));
            this.EntityKind = entityKind;
        }

        /// <summary>
        /// Gets the entity type of a Service Fabric entity such as Application, Service or a Partition where periodic backups
        /// can be enabled.
        /// </summary>
        public BackupEntityKind? EntityKind { get; }
    }
}
