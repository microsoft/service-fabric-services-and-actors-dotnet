// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Extensions
{
    using System;
    using System.Fabric;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal static class ServiceParitionClientExtensions
    {
        private static readonly DataContractJsonSerializer ErrorSerializer = new DataContractJsonSerializer(typeof(ErrorResponse));

        public static async Task<HttpResponseMessage> InvokeWebRequestWithRetryAsync(
            this ServicePartitionClient<HttpCommunicationClient> partitionClient,
            Func<HttpCommunicationClient, Task<HttpResponseMessage>> asyncFunc,
            string funcTag,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await partitionClient.InvokeWithRetryAsync(async client =>
            {
                var response = await asyncFunc.Invoke(client);
                await ThrowIfErrorResponseAsync(response);

                return response;
            });
        }

        private static async Task ThrowIfErrorResponseAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var buffer = await response.Content.ReadAsByteArrayAsync();
                var error = SerializationUtility.Deserialize<ErrorResponse>(ErrorSerializer, buffer);
                Exception ex = null;
                if (error.IsFabricError)
                {
                    ex = new FabricException(error.Message, error.ErrorCode);
                }
                else
                {
                    ex = new Exception(error.Message);
                }

                ex.Data.Add("ActualExceptionType", error.ExceptionType);
                ex.Data.Add("IsErrorAtSource", true);
                throw ex;
            }
        }
    }
}
