// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.
    /// </summary>
    [Serializable]
    public class ServiceFabricRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricRequestException"/> class.
        /// </summary>
        public ServiceFabricRequestException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricRequestException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ServiceFabricRequestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricRequestException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ServiceFabricRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricRequestException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual information
        /// about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="Exception.HResult"/> is zero (0)
        /// </exception>
        protected ServiceFabricRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
