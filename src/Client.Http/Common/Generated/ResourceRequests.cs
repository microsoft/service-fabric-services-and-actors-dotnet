// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This type describes the requested resources for a given container. It describes the least amount of resources
    /// required for the container. A container can consume more than requested resources up to the specified limits before
    /// being restarted. Currently, the requested resources are treated as limits.
    /// </summary>
    public partial class ResourceRequests
    {
        /// <summary>
        /// Initializes a new instance of the ResourceRequests class.
        /// </summary>
        /// <param name="memoryInGB">The memory request in GB for this container.</param>
        /// <param name="cpu">Requested number of CPU cores. At present, only full cores are supported.</param>
        public ResourceRequests(
            double? memoryInGB,
            double? cpu)
        {
            memoryInGB.ThrowIfNull(nameof(memoryInGB));
            cpu.ThrowIfNull(nameof(cpu));
            this.MemoryInGB = memoryInGB;
            this.Cpu = cpu;
        }

        /// <summary>
        /// Gets the memory request in GB for this container.
        /// </summary>
        public double? MemoryInGB { get; }

        /// <summary>
        /// Gets requested number of CPU cores. At present, only full cores are supported.
        /// </summary>
        public double? Cpu { get; }
    }
}
