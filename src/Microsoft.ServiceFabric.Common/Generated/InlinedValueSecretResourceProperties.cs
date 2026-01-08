// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the properties of a secret resource whose value is provided explicitly as plaintext. The secret resource
    /// may have multiple values, each being uniquely versioned. The secret value of each version is stored encrypted, and
    /// delivered as plaintext into the context of applications referencing it.
    /// </summary>
    public partial class InlinedValueSecretResourceProperties : SecretResourceProperties
    {
        /// <summary>
        /// Initializes a new instance of the InlinedValueSecretResourceProperties class.
        /// </summary>
        /// <param name="description">User readable description of the secret.</param>
        /// <param name="contentType">The type of the content stored in the secret value. The value of this property is opaque
        /// to Service Fabric. Once set, the value of this property cannot be changed.</param>
        public InlinedValueSecretResourceProperties(
            string description = default(string),
            string contentType = default(string))
            : base(
                Common.SecretKind.InlinedValue,
                description,
                contentType)
        {
        }
    }
}
