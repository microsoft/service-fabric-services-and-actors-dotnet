// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The container state.
    /// </summary>
    public partial class ContainerState
    {
        /// <summary>
        /// Initializes a new instance of the ContainerState class.
        /// </summary>
        /// <param name="state">The state of this container</param>
        /// <param name="startTime">Date/time when the container state started.</param>
        /// <param name="exitCode">The container exit code.</param>
        /// <param name="finishTime">Date/time when the container state finished.</param>
        /// <param name="detailStatus">Human-readable status of this state.</param>
        public ContainerState(
            string state = default(string),
            DateTime? startTime = default(DateTime?),
            string exitCode = default(string),
            DateTime? finishTime = default(DateTime?),
            string detailStatus = default(string))
        {
            this.State = state;
            this.StartTime = startTime;
            this.ExitCode = exitCode;
            this.FinishTime = finishTime;
            this.DetailStatus = detailStatus;
        }

        /// <summary>
        /// Gets the state of this container
        /// </summary>
        public string State { get; }

        /// <summary>
        /// Gets date/time when the container state started.
        /// </summary>
        public DateTime? StartTime { get; }

        /// <summary>
        /// Gets the container exit code.
        /// </summary>
        public string ExitCode { get; }

        /// <summary>
        /// Gets date/time when the container state finished.
        /// </summary>
        public DateTime? FinishTime { get; }

        /// <summary>
        /// Gets human-readable status of this state.
        /// </summary>
        public string DetailStatus { get; }
    }
}
