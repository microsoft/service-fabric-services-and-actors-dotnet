// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception is thrown by actor runtime when more than one reentrant call chain is active for an
    /// actor at the same time.
    /// <para>
    /// This can happen in scenario where actor A calls actor B, C and D in parallel and then B, C and D
    /// try to call back A at the same time.
    /// </para>
    /// </summary>
    [Serializable]
    public sealed class InvalidReentrantCallException : FabricException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidReentrantCallException"/> class.
        /// </summary>
        public InvalidReentrantCallException()
            : base(SR.InvalidReentrantCall)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidReentrantCallException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidReentrantCallException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidReentrantCallException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidReentrantCallException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private InvalidReentrantCallException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
