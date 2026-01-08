// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the trigger for performing auto scaling operation.
    /// </summary>
    public abstract partial class AutoScalingTrigger
    {
        /// <summary>
        /// Initializes a new instance of the AutoScalingTrigger class.
        /// </summary>
        /// <param name="kind">Enumerates the triggers for auto scaling.</param>
        protected AutoScalingTrigger(
            AutoScalingTriggerKind? kind)
        {
            kind.ThrowIfNull(nameof(kind));
            this.Kind = kind;
        }

        /// <summary>
        /// Gets enumerates the triggers for auto scaling.
        /// </summary>
        public AutoScalingTriggerKind? Kind { get; }
    }
}
