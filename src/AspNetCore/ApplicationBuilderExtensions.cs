// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore
{
    /// <summary>
    /// Class containing extension methods for IApplicationBuilder.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Extension method to use ServiceFabricMiddleware for Service Fabric stateful or stateless service
        /// using Kestrel or HttpSys as WebServer.
        /// </summary>
        /// <param name="builder">Microsoft.AspNetCore.Builder.IApplicationBuilder.</param>
        /// <param name="urlSuffix">Url suffix to determine if the request is meant for current partition and replica.</param>
        /// <returns>IApplicationBuilder instance.</returns>
        public static IApplicationBuilder UseServiceFabricMiddleware(this IApplicationBuilder builder, string urlSuffix)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            if (urlSuffix == null)
            {
                throw new ArgumentNullException("urlSuffix");
            }

            return builder.UseMiddleware<ServiceFabricMiddleware>(urlSuffix);
        }

        /// <summary>
        /// Extension method to use ServiceFabricReverseProxyIntegrationMiddleware for Service Fabric stateful or stateless service
        /// using Kestrel or HttpSys as WebServer.
        /// </summary>
        /// <param name="builder">Microsoft.AspNetCore.Builder.IApplicationBuilder.</param>
        /// <returns>IApplicationBuilder instance.</returns>
        public static IApplicationBuilder UseServiceFabricReverseProxyIntegrationMiddleware(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<ServiceFabricReverseProxyIntegrationMiddleware>();
        }
    }
}
