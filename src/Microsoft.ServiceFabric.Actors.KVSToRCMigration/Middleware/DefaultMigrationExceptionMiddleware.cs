// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Middleware
{
    using System;
    using System.Fabric;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.ServiceFabric.Actors.Migration;

    internal class DefaultMigrationExceptionMiddleware
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(ErrorResponse));
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
            var error = new ErrorResponse
            {
                Message = exception.Message,
                ErrorCode = 0,
                ExceptionType = exception.GetType().FullName,
                IsFabricError = false,
            };

            if (exception is FabricException)
            {
                var fabEx = exception as FabricException;
                error.IsFabricError = true;
                error.ErrorCode = fabEx.ErrorCode;
            }

            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var buffer = SerializationUtility.Serialize(Serializer, error);
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
            await response.Body.FlushAsync();
        }
    }
}
