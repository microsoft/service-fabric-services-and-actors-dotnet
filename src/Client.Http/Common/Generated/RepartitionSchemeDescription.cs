// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes how the service is repartitioned.
    /// </summary>
    public abstract partial class RepartitionSchemeDescription
    {
        /// <summary>
        /// Initializes a new instance of the RepartitionSchemeDescription class.
        /// </summary>
        /// <param name="kind">Enumerates the ways that a service can be partitioned.</param>
        protected RepartitionSchemeDescription(
            RepartitionScheme? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets enumerates the ways that a service can be partitioned.
        /// </summary>
        public RepartitionScheme? Kind { get; }
    }
}
