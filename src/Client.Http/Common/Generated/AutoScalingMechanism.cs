// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the mechanism for performing auto scaling operation. Derived classes will describe the actual mechanism.
    /// </summary>
    public abstract partial class AutoScalingMechanism
    {
        /// <summary>
        /// Initializes a new instance of the AutoScalingMechanism class.
        /// </summary>
        /// <param name="kind">Enumerates the mechanisms for auto scaling.</param>
        protected AutoScalingMechanism(
            AutoScalingMechanismKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets enumerates the mechanisms for auto scaling.
        /// </summary>
        public AutoScalingMechanismKind? Kind { get; }
    }
}
