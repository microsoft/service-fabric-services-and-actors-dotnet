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
    /// Exception to indicate actor state provider is invalid and cannot partitipate in migration.
    /// </summary>
    [Serializable]
    public sealed class InvalidMigrationStateProviderException : FabricException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationStateProviderException"/> class.
        /// </summary>
        public InvalidMigrationStateProviderException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationStateProviderException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidMigrationStateProviderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationStateProviderException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidMigrationStateProviderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationStateProviderException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private InvalidMigrationStateProviderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
