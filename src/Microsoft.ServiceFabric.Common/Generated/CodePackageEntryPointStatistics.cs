// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Statistics about setup or main entry point  of a code package deployed on a Service Fabric node.
    /// </summary>
    public partial class CodePackageEntryPointStatistics
    {
        /// <summary>
        /// Initializes a new instance of the CodePackageEntryPointStatistics class.
        /// </summary>
        /// <param name="lastExitCode">The last exit code of the entry point.</param>
        /// <param name="lastActivationTime">The last time (in UTC) when Service Fabric attempted to run the entry
        /// point.</param>
        /// <param name="lastExitTime">The last time (in UTC) when the entry point finished running.</param>
        /// <param name="lastSuccessfulActivationTime">The last time (in UTC) when the entry point ran successfully.</param>
        /// <param name="lastSuccessfulExitTime">The last time (in UTC) when the entry point finished running
        /// gracefully.</param>
        /// <param name="activationCount">Number of times the entry point has run.</param>
        /// <param name="activationFailureCount">Number of times the entry point failed to run.</param>
        /// <param name="continuousActivationFailureCount">Number of times the entry point continuously failed to run.</param>
        /// <param name="exitCount">Number of times the entry point finished running.</param>
        /// <param name="exitFailureCount">Number of times the entry point failed to exit gracefully.</param>
        /// <param name="continuousExitFailureCount">Number of times the entry point continuously failed to exit
        /// gracefully.</param>
        public CodePackageEntryPointStatistics(
            string lastExitCode = default(string),
            DateTime? lastActivationTime = default(DateTime?),
            DateTime? lastExitTime = default(DateTime?),
            DateTime? lastSuccessfulActivationTime = default(DateTime?),
            DateTime? lastSuccessfulExitTime = default(DateTime?),
            string activationCount = default(string),
            string activationFailureCount = default(string),
            string continuousActivationFailureCount = default(string),
            string exitCount = default(string),
            string exitFailureCount = default(string),
            string continuousExitFailureCount = default(string))
        {
            this.LastExitCode = lastExitCode;
            this.LastActivationTime = lastActivationTime;
            this.LastExitTime = lastExitTime;
            this.LastSuccessfulActivationTime = lastSuccessfulActivationTime;
            this.LastSuccessfulExitTime = lastSuccessfulExitTime;
            this.ActivationCount = activationCount;
            this.ActivationFailureCount = activationFailureCount;
            this.ContinuousActivationFailureCount = continuousActivationFailureCount;
            this.ExitCount = exitCount;
            this.ExitFailureCount = exitFailureCount;
            this.ContinuousExitFailureCount = continuousExitFailureCount;
        }

        /// <summary>
        /// Gets the last exit code of the entry point.
        /// </summary>
        public string LastExitCode { get; }

        /// <summary>
        /// Gets the last time (in UTC) when Service Fabric attempted to run the entry point.
        /// </summary>
        public DateTime? LastActivationTime { get; }

        /// <summary>
        /// Gets the last time (in UTC) when the entry point finished running.
        /// </summary>
        public DateTime? LastExitTime { get; }

        /// <summary>
        /// Gets the last time (in UTC) when the entry point ran successfully.
        /// </summary>
        public DateTime? LastSuccessfulActivationTime { get; }

        /// <summary>
        /// Gets the last time (in UTC) when the entry point finished running gracefully.
        /// </summary>
        public DateTime? LastSuccessfulExitTime { get; }

        /// <summary>
        /// Gets number of times the entry point has run.
        /// </summary>
        public string ActivationCount { get; }

        /// <summary>
        /// Gets number of times the entry point failed to run.
        /// </summary>
        public string ActivationFailureCount { get; }

        /// <summary>
        /// Gets number of times the entry point continuously failed to run.
        /// </summary>
        public string ContinuousActivationFailureCount { get; }

        /// <summary>
        /// Gets number of times the entry point finished running.
        /// </summary>
        public string ExitCount { get; }

        /// <summary>
        /// Gets number of times the entry point failed to exit gracefully.
        /// </summary>
        public string ExitFailureCount { get; }

        /// <summary>
        /// Gets number of times the entry point continuously failed to exit gracefully.
        /// </summary>
        public string ContinuousExitFailureCount { get; }
    }
}
