// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes how the scaling should be performed
    /// </summary>
    public partial class ScalingPolicyDescription
    {
        /// <summary>
        /// Initializes a new instance of the ScalingPolicyDescription class.
        /// </summary>
        /// <param name="scalingTrigger">Specifies the trigger associated with this scaling policy</param>
        /// <param name="scalingMechanism">Specifies the mechanism associated with this scaling policy</param>
        public ScalingPolicyDescription(
            ScalingTriggerDescription scalingTrigger,
            ScalingMechanismDescription scalingMechanism)
        {
            scalingTrigger.ThrowIfNull(nameof(scalingTrigger));
            scalingMechanism.ThrowIfNull(nameof(scalingMechanism));
            this.ScalingTrigger = scalingTrigger;
            this.ScalingMechanism = scalingMechanism;
        }

        /// <summary>
        /// Gets specifies the trigger associated with this scaling policy
        /// </summary>
        public ScalingTriggerDescription ScalingTrigger { get; }

        /// <summary>
        /// Gets specifies the mechanism associated with this scaling policy
        /// </summary>
        public ScalingMechanismDescription ScalingMechanism { get; }
    }
}
