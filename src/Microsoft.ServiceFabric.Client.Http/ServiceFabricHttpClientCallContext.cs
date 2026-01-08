// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class to get and set a call context to a given asynchronous control flow.
    /// Users of ServiceFabricHttpClient can use this to add a correlation id to the request which will be logged in events traced by ServiceFabricHttpClient.
    /// </summary>
    public static class ServiceFabricHttpClientCallContext
    {
        private static AsyncLocal<string> callContextAsyncLocal = new AsyncLocal<string>();

        /// <summary>
        /// Gets CorrelationId from <see cref="System.Threading.AsyncLocal{T}"/>
        /// </summary>
        /// <param name="correlationId">CorrelationId value if its available in asynchronous control flow else null.</param>
        /// <returns>true if CorrelationId was found in asynchronous control flow .</returns>
        public static bool TryGetCorrelationId(out string correlationId)
        {
            correlationId = callContextAsyncLocal.Value;
            return (correlationId != null);
        }

        /// <summary>
        /// Sets a CorrelationId for ServiceFabricHttpClient in asynchronous control flow.
        /// </summary>
        /// <param name="correlationId">Value for correlation id</param>
        public static void SetCorrelationId(string correlationId)
        {
            callContextAsyncLocal.Value = correlationId;
        }

        /// <summary>
        /// Clears correlation id from <see cref="System.Threading.AsyncLocal{T}"/>
        /// </summary>
        public static void ClearCorrelationId()
        {
            callContextAsyncLocal.Value = null;
        }
    }
}
