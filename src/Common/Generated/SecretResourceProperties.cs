// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the properties of a secret resource.
    /// </summary>
    public partial class SecretResourceProperties : SecretResourcePropertiesBase
    {
        /// <summary>
        /// Initializes a new instance of the SecretResourceProperties class.
        /// </summary>
        /// <param name="kind">Describes the kind of secret.</param>
        /// <param name="description">User readable description of the secret.</param>
        /// <param name="contentType">The type of the content stored in the secret value. The value of this property is opaque
        /// to Service Fabric. Once set, the value of this property cannot be changed.</param>
        public SecretResourceProperties(
            SecretKind? kind,
            string description = default(string),
            string contentType = default(string))
            : base(
                kind)
        {
            this.Description = description;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets user readable description of the secret.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets status of the resource. Possible values include: 'Unknown', 'Ready', 'Upgrading', 'Creating', 'Deleting',
        /// 'Failed'
        /// </summary>
        public ResourceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets additional information about the current status of the secret.
        /// </summary>
        public string StatusDetails { get; internal set; }

        /// <summary>
        /// Gets the type of the content stored in the secret value. The value of this property is opaque to Service Fabric.
        /// Once set, the value of this property cannot be changed.
        /// </summary>
        public string ContentType { get; }
    }
}
