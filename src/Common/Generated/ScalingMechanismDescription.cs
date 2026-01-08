// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the mechanism for performing a scaling operation.
    /// </summary>
    public abstract partial class ScalingMechanismDescription
    {
        /// <summary>
        /// Initializes a new instance of the ScalingMechanismDescription class.
        /// </summary>
        /// <param name="kind">Enumerates the ways that a service can be scaled.</param>
        protected ScalingMechanismDescription(
            ScalingMechanismKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets enumerates the ways that a service can be scaled.
        /// </summary>
        public ScalingMechanismKind? Kind { get; }
    }
}
