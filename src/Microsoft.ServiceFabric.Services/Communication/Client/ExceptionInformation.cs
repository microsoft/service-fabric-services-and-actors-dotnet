// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the class that encapsulates the exception seen on the communication channel and additional information required to handle that exception.
    /// </summary>
    public sealed class ExceptionInformation
    {
        private readonly Exception exception;
        private readonly TargetReplicaSelector targetReplica;

        /// <summary>
        /// Instantiates the exception information using the given exception and the default target replica selector.
        /// </summary>
        /// <param name="exception">The exception that was received</param>
        public ExceptionInformation(Exception exception)
            : this(exception, TargetReplicaSelector.Default)
        {
        }

        /// <summary>
        /// Instantiates the exception information using the given exception and target replica selector.
        /// </summary>
        /// <param name="exception">The exception that was received</param>
        /// <param name="targetReplica">The target replica information</param>
        public ExceptionInformation(Exception exception, TargetReplicaSelector targetReplica)
        {
            this.exception = exception;
            this.targetReplica = targetReplica;
        }

        /// <summary>
        /// Exception that was seen on the communication channel
        /// </summary>
        /// <value>The <see cref="System.Exception"/> that was seen.</value>
        public Exception Exception
        {
            get { return this.exception; }
        }

        /// <summary>
        /// The target replica identifier to which the communication channel was established.
        /// </summary>
        /// <value>A <see cref="TargetReplicaSelector"/> that specifies gives information about the target replica</value>
        public TargetReplicaSelector TargetReplica
        {
            get { return this.targetReplica; }
        }
    }
}
