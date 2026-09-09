// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Collections
{
    using System;
    using System.Fabric;

    /// <summary>
    /// Represents the exception thrown by <see cref="IReliableConcurrentQueue{T}.EnqueueAsync"/> when the queue capacity
    /// has been reached.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Retriable; when encountering this exception, the caller should wait some time for additional dequeue operations
    /// before retrying the enqueue.
    /// </para>
    /// <para>
    /// The default capacity limit is the maximum value of <see langword="int"/> and is not currently configurable.
    /// </para>
    /// </remarks>
    public class QueueFullException : FabricTransientException
    {
        /// <inheritdoc path="/summary" cref="QueueFullException(string, Exception)"/>
        public QueueFullException()
        {
        }

        /// <inheritdoc path="/summary" cref="QueueFullException(string, Exception)"/>
        public QueueFullException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueFullException"/> class.
        /// </summary>
        public QueueFullException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
    }
}
