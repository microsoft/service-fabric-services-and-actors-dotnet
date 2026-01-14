// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Metadata about an Analysis Event.
    /// </summary>
    public partial class AnalysisEventMetadata
    {
        /// <summary>
        /// Initializes a new instance of the AnalysisEventMetadata class.
        /// </summary>
        /// <param name="delay">The analysis delay.</param>
        /// <param name="duration">The duration of analysis.</param>
        public AnalysisEventMetadata(
            TimeSpan? delay = default(TimeSpan?),
            TimeSpan? duration = default(TimeSpan?))
        {
            this.Delay = delay;
            this.Duration = duration;
        }

        /// <summary>
        /// Gets the analysis delay.
        /// </summary>
        public TimeSpan? Delay { get; }

        /// <summary>
        /// Gets the duration of analysis.
        /// </summary>
        public TimeSpan? Duration { get; }
    }
}
