// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;

    /// <summary>
    /// Exception handler.
    /// </summary>
    internal interface IExceptionHandler
    {
        /// <summary>
        /// Handler for the observed exception
        /// </summary>
        /// <param name="exception">Observed exception.</param>
        /// <param name="isTransient">True if the exception is transient, false otherwise.</param>
        /// <returns>True if the exception is handled, false otherwise.</returns>
        bool TryHandleException(
            Exception exception,
            out bool isTransient);
    }
}
