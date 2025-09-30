// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// Exception thrown by actor runtime when reentrant call is made for an actor and its <see cref="ActorReentrancyMode"/> is set to <see cref="ActorReentrancyMode.Disallowed"/>.
    /// </summary>
    [Serializable]
    public sealed class ReentrancyModeDisallowedException : FabricException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReentrancyModeDisallowedException"/> class.
        /// </summary>
        public ReentrancyModeDisallowedException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReentrancyModeDisallowedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ReentrancyModeDisallowedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReentrancyModeDisallowedException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ReentrancyModeDisallowedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
