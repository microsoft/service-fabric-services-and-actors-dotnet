// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception indicates that an Actor received the duplicate message while waiting for to acquire 
    /// the turn based concurrency lock.
    /// </summary>
    [Serializable]
    public sealed class DuplicateMessageException : FabricException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateMessageException"/> class.
        /// </summary>
        public DuplicateMessageException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateMessageException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DuplicateMessageException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateMessageException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DuplicateMessageException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private DuplicateMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
