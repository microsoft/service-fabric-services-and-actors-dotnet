// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes the properties of a secret resource, including its kind.
    /// </summary>
    public abstract partial class SecretResourcePropertiesBase
    {
        /// <summary>
        /// Initializes a new instance of the SecretResourcePropertiesBase class.
        /// </summary>
        /// <param name="kind">Describes the kind of secret.</param>
        protected SecretResourcePropertiesBase(
            SecretKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the kind of secret.
        /// </summary>
        public SecretKind? Kind { get; }
    }
}
