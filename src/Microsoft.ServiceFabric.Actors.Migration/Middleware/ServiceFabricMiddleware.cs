// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// A middleware to be used with Service Fabric stateful and stateless services hosted in Kestrel or HttpSys.
    /// This middleware examines the Microsoft.AspNetCore.Http.HttpRequest.Path in request to determine if the request is intended for this replica.
    /// </summary>
    /// <remarks>
    /// This middleware when used with Kestrel and HttpSys based Service Fabric Communication Listeners allows handling of scenarios when
    /// the Replica1 listening on Node1 and por11 has moved and another Replica2 is opened on Node1 got Port1.
    /// A client which has resolved Replica1 before it moved, will send the request to Node1:Port1. Using this middleware
    /// Replica2 can reject calls which were meant for Replica1 by examining the Path in incoming request.
    /// </remarks>
    internal class ServiceFabricMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string urlSuffix;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request handler in pipeline.</param>
        /// <param name="urlSuffix">Url suffix to determine if the request is meant for current partition and replica.</param>
        public ServiceFabricMiddleware(RequestDelegate next, string urlSuffix)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (urlSuffix == null)
            {
                throw new ArgumentNullException("urlSuffix");
            }

            this.urlSuffix = urlSuffix;
            this.next = next;
        }

        /// <summary>
        /// Invoke.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>Task for the execution by next middleware in pipeline.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (this.urlSuffix.Length == 0)
            {
                // when urlSuffix is empty string, just call the next middleware in pipeline.
                await this.next(context);
            }
            else
            {
                // If this middleware is enabled by specifying UseServiceFabricIntegration(), CommunicationListnerBehavior is:
                //   With ServiceFabricIntegrationOptions.UseUniqueServiceUrl (urlSuffix is /PartitionId/ReplicaOrInstanceId)
                //      - Url given to WebServer is http://+:port
                //      - Url given to Service Fabric Runtime is http://ip:port/PartitionId/ReplicaOrInstanceId

                // Since when registering with IWebHost, only http://+:port is provided:
                //    - HttpRequest.Path contains everything in url after http://+:port, and it must start with urlSuffix

                // So short circuit and return StatusCode 410 if (message isn't intended for this replica,):
                //    - HttpRequest.Path doesn't start with urlSuffix
                if (!context.Request.Path.StartsWithSegments(this.urlSuffix, out var matchedPath, out var remainingPath))
                {
                    context.Response.StatusCode = StatusCodes.Status410Gone;
                    return;
                }

                // All good, change Path, PathBase and call next middleware in the pipeline
                var originalPath = context.Request.Path;
                var originalPathBase = context.Request.PathBase;
                context.Request.Path = remainingPath;
                context.Request.PathBase = originalPathBase.Add(matchedPath);

                try
                {
                    await this.next(context);
                }
                finally
                {
                    context.Request.Path = originalPath;
                    context.Request.PathBase = originalPathBase;
                }
            }
        }
    }
}
