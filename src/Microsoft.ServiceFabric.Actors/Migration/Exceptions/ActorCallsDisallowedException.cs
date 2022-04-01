// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Exceptions
{
    using System;
    using System.Fabric;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception to indicate the current migration service is not allowed to dispatch actor calls.
    /// Source Migration service is disallowed to accept calls in downtime phase.
    /// Target Migration service is disallowed to accept calls until all the sequence numbers are migrated successfully.
    /// </summary>
    [Serializable]
    public sealed class ActorCallsDisallowedException : FabricException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorCallsDisallowedException"/> class.
        /// </summary>
        public ActorCallsDisallowedException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorCallsDisallowedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ActorCallsDisallowedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorCallsDisallowedException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ActorCallsDisallowedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorCallsDisallowedException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private ActorCallsDisallowedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
