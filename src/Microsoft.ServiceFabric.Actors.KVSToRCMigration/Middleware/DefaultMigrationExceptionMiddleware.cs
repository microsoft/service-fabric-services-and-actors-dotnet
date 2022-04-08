// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Middleware
{
    using System;
    using System.Fabric;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.ServiceFabric.Actors.Migration;

    internal class DefaultMigrationExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public DefaultMigrationExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = exception.Message;
            var errorCode = FabricErrorCode.Unknown;

            if (exception is FabricException)
            {
                var fabEx = exception as FabricException;
                message = fabEx.Message;
                errorCode = fabEx.ErrorCode;
            }

            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            await response.WriteAsync(new ErrorResponse()
            {
                Message = message,
                ErrorCode = errorCode,
            }.ToString());
        }
    }
}
