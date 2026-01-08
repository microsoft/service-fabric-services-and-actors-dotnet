// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes the resource limits for a given container. It describes the most amount of resources a
    /// container is allowed to use before being restarted.
    /// </summary>
    public partial class ResourceLimits
    {
        /// <summary>
        /// Initializes a new instance of the ResourceLimits class.
        /// </summary>
        /// <param name="memoryInGB">The memory limit in GB.</param>
        /// <param name="cpu">CPU limits in cores. At present, only full cores are supported.</param>
        public ResourceLimits(
            double? memoryInGB = default(double?),
            double? cpu = default(double?))
        {
            this.MemoryInGB = memoryInGB;
            this.Cpu = cpu;
        }

        /// <summary>
        /// Gets the memory limit in GB.
        /// </summary>
        public double? MemoryInGB { get; }

        /// <summary>
        /// Gets CPU limits in cores. At present, only full cores are supported.
        /// </summary>
        public double? Cpu { get; }
    }
}
