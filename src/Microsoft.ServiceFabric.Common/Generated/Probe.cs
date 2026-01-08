// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Probes have a number of fields that you can use to control their behavior.
    /// </summary>
    public partial class Probe
    {
        /// <summary>
        /// Initializes a new instance of the Probe class.
        /// </summary>
        /// <param name="initialDelaySeconds">The initial delay in seconds to start executing probe once codepackage has
        /// started.</param>
        /// <param name="periodSeconds">Periodic seconds to execute probe.</param>
        /// <param name="timeoutSeconds">Period after which probe is considered as failed if it hasn't completed
        /// successfully.</param>
        /// <param name="successThreshold">The count of successful probe executions after which probe is considered
        /// success.</param>
        /// <param name="failureThreshold">The count of failures after which probe is considered failed.</param>
        /// <param name="exec">Exec command to run inside the container.</param>
        /// <param name="httpGet">Http probe for the container.</param>
        /// <param name="tcpSocket">Tcp port to probe inside the container.</param>
        public Probe(
            int? initialDelaySeconds = 0,
            int? periodSeconds = 10,
            int? timeoutSeconds = 1,
            int? successThreshold = 1,
            int? failureThreshold = 3,
            ProbeExec exec = default(ProbeExec),
            ProbeHttpGet httpGet = default(ProbeHttpGet),
            ProbeTcpSocket tcpSocket = default(ProbeTcpSocket))
        {
            this.InitialDelaySeconds = initialDelaySeconds;
            this.PeriodSeconds = periodSeconds;
            this.TimeoutSeconds = timeoutSeconds;
            this.SuccessThreshold = successThreshold;
            this.FailureThreshold = failureThreshold;
            this.Exec = exec;
            this.HttpGet = httpGet;
            this.TcpSocket = tcpSocket;
        }

        /// <summary>
        /// Gets the initial delay in seconds to start executing probe once codepackage has started.
        /// </summary>
        public int? InitialDelaySeconds { get; }

        /// <summary>
        /// Gets periodic seconds to execute probe.
        /// </summary>
        public int? PeriodSeconds { get; }

        /// <summary>
        /// Gets period after which probe is considered as failed if it hasn't completed successfully.
        /// </summary>
        public int? TimeoutSeconds { get; }

        /// <summary>
        /// Gets the count of successful probe executions after which probe is considered success.
        /// </summary>
        public int? SuccessThreshold { get; }

        /// <summary>
        /// Gets the count of failures after which probe is considered failed.
        /// </summary>
        public int? FailureThreshold { get; }

        /// <summary>
        /// Gets exec command to run inside the container.
        /// </summary>
        public ProbeExec Exec { get; }

        /// <summary>
        /// Gets http probe for the container.
        /// </summary>
        public ProbeHttpGet HttpGet { get; }

        /// <summary>
        /// Gets tcp port to probe inside the container.
        /// </summary>
        public ProbeTcpSocket TcpSocket { get; }
    }
}
