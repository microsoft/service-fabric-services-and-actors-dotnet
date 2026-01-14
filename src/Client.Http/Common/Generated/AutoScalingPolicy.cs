// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the auto scaling policy
    /// </summary>
    public partial class AutoScalingPolicy
    {
        /// <summary>
        /// Initializes a new instance of the AutoScalingPolicy class.
        /// </summary>
        /// <param name="name">The name of the auto scaling policy.</param>
        /// <param name="trigger">Determines when auto scaling operation will be invoked.</param>
        /// <param name="mechanism">The mechanism that is used to scale when auto scaling operation is invoked.</param>
        public AutoScalingPolicy(
            string name,
            AutoScalingTrigger trigger,
            AutoScalingMechanism mechanism)
        {
            name.ThrowIfNull(nameof(name));
            trigger.ThrowIfNull(nameof(trigger));
            mechanism.ThrowIfNull(nameof(mechanism));
            this.Name = name;
            this.Trigger = trigger;
            this.Mechanism = mechanism;
        }

        /// <summary>
        /// Gets the name of the auto scaling policy.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets determines when auto scaling operation will be invoked.
        /// </summary>
        public AutoScalingTrigger Trigger { get; }

        /// <summary>
        /// Gets the mechanism that is used to scale when auto scaling operation is invoked.
        /// </summary>
        public AutoScalingMechanism Mechanism { get; }
    }
}
