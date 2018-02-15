// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the exception handling result when the request from client to service cannot be retried
    /// </summary>
    public sealed class ExceptionHandlingThrowResult : ExceptionHandlingResult
    {
        /// <summary>
        /// The exception that should be thrown to the client.
        /// </summary>
        /// <value>Exception to throw</value>
        public Exception ExceptionToThrow { get; set; }
    }
}
