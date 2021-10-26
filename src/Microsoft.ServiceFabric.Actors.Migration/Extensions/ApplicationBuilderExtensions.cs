// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
#if DotNetCoreClr
    using System;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Class containing extension methods for IApplicationBuilder.
    /// </summary>
    internal static class ApplicationBuilderExtensions
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
    }
#endif
}
