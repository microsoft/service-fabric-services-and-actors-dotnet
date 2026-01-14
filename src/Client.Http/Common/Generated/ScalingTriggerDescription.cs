// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the trigger for performing a scaling operation.
    /// </summary>
    public abstract partial class ScalingTriggerDescription
    {
        /// <summary>
        /// Initializes a new instance of the ScalingTriggerDescription class.
        /// </summary>
        /// <param name="kind">Enumerates the ways that a service can be scaled.</param>
        protected ScalingTriggerDescription(
            ScalingTriggerKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets enumerates the ways that a service can be scaled.
        /// </summary>
        public ScalingTriggerKind? Kind { get; }
    }
}
