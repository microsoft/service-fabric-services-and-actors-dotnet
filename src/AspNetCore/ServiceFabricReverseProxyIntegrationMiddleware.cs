// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore
{
    /// <summary>
    /// A middleware to be used with Service Fabric stateful and stateless services hosted in Kestrel or HttpSys.
    /// This middleware automatically adds X-ServiceFabric ResourceNotFound header, required by the Service Fabric Reverse Proxy, when 404 status code is returned.
    /// </summary>
    public class ServiceFabricReverseProxyIntegrationMiddleware
    {
        private const string XServiceFabricHeader = "X-ServiceFabric";
        private const string XServiceFabricResourceNotFoundValue = "ResourceNotFound";

        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricReverseProxyIntegrationMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request handler in pipeline.</param>
        public ServiceFabricReverseProxyIntegrationMiddleware(RequestDelegate next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            this.next = next;
        }

        /// <summary>
        /// Invoke.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>Task for the execution by next middleware in pipeline.</returns>
        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.OnStarting(
                state =>
                {
                    var response = (HttpResponse)state;
                    if (response.StatusCode == StatusCodes.Status404NotFound)
                    {
                        response.Headers[XServiceFabricHeader] = XServiceFabricResourceNotFoundValue;
                    }

                    return Task.CompletedTask;
                },
                context.Response);

            return this.next(context);
        }
    }
}
