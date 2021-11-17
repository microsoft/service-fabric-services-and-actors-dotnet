// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an information about an exception from the service. This exception is thrown when the actual
    /// exception from the service cannot be serialized for transferring to client.
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        public ServiceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException" /> class with appropriate message.
        /// </summary>
        /// <param name="actualExceptionType">the ActualExceptionType of exception thrown</param>
        /// <param name="message">The error message that explains the reason for this exception
        /// </param>
        public ServiceException(string actualExceptionType, string message)
            : base(message)
        {
            this.ActualExceptionType = actualExceptionType;
        }

        /// <summary>
        /// Gets the ActualExceptionType is the type of actual exception thrown.
        /// </summary>
        public string ActualExceptionType { get; private set; }

        /// <summary>
        /// Gets or sets the actual exception stack trace.
        /// </summary>
        public string ActualExceptionStackTrace { get; set; }

        /// <summary>
        /// Gets or sets additional data about the actual exception.
        /// </summary>
        public IDictionary<object, object> ActualExceptionData { get; set; }

        /// <summary>
        /// Gets or sets the list of inner exceptions.
        /// </summary>
        public IList<ServiceException> ActualInnerExceptions { get; set; }
    }
}
