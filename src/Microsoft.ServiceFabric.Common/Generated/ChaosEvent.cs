// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an event generated during a Chaos run.
    /// </summary>
    public abstract partial class ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the ChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="kind">The kind of Chaos event.
        /// </param>
        protected ChaosEvent(
            DateTime? timeStampUtc,
            ChaosEventKind? kind)
        {
            timeStampUtc.ThrowIfNull(nameof(timeStampUtc));
            kind.ThrowIfNull(nameof(kind));
            this.TimeStampUtc = timeStampUtc;
            this.Kind = kind;
        }

        /// <summary>
        /// Gets the UTC timestamp when this Chaos event was generated.
        /// </summary>
        public DateTime? TimeStampUtc { get; }

        /// <summary>
        /// Gets the kind of Chaos event.
        /// </summary>
        public ChaosEventKind? Kind { get; }
    }
}
