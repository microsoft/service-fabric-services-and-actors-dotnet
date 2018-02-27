// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
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
        /// Initializes a new instance of the <see cref="ExceptionInformation"/> class using the given exception and the default target replica selector.
        /// </summary>
        /// <param name="exception">The exception that was received</param>
        public ExceptionInformation(Exception exception)
            : this(exception, TargetReplicaSelector.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInformation"/> class using the given exception and target replica selector.
        /// </summary>
        /// <param name="exception">The exception that was received</param>
        /// <param name="targetReplica">The target replica information</param>
        public ExceptionInformation(Exception exception, TargetReplicaSelector targetReplica)
        {
            this.exception = exception;
            this.targetReplica = targetReplica;
        }

        /// <summary>
        /// Gets exception that was seen on the communication channel
        /// </summary>
        /// <value>The <see cref="System.Exception"/> that was seen.</value>
        public Exception Exception
        {
            get { return this.exception; }
        }

        /// <summary>
        /// Gets the target replica identifier to which the communication channel was established.
        /// </summary>
        /// <value>A <see cref="TargetReplicaSelector"/> that specifies gives information about the target replica</value>
        public TargetReplicaSelector TargetReplica
        {
            get { return this.targetReplica; }
        }
    }
}
