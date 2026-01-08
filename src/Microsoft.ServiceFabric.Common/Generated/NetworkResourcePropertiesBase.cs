// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes the properties of a network resource, including its kind.
    /// </summary>
    public abstract partial class NetworkResourcePropertiesBase
    {
        /// <summary>
        /// Initializes a new instance of the NetworkResourcePropertiesBase class.
        /// </summary>
        /// <param name="kind">The type of a Service Fabric container network.</param>
        protected NetworkResourcePropertiesBase(
            NetworkKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the type of a Service Fabric container network.
        /// </summary>
        public NetworkKind? Kind { get; }
    }
}
