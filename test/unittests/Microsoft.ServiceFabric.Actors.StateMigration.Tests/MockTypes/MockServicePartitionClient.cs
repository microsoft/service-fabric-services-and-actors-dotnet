// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class MockServicePartitionClient : ServicePartitionClient<HttpCommunicationClient>
    {
        internal static readonly long EnumerationSize = 100;

        public MockServicePartitionClient(
           ICommunicationClientFactory<HttpCommunicationClient> communicationClientFactory,
           Uri serviceUri,
           ServicePartitionKey partitionKey = null,
           TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
           string listenerName = null,
           OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }

        public override Task<TResult> InvokeWithRetryAsync<TResult>(
            Func<HttpCommunicationClient, Task<TResult>> func,
            params Type[] doNotRetryExceptionTypes)
        {
            return this.InvokeWithRetryAsync(func, CancellationToken.None, doNotRetryExceptionTypes);
        }

        public override async Task<TResult> InvokeWithRetryAsync<TResult>(
            Func<HttpCommunicationClient, Task<TResult>> func,
            CancellationToken cancellationToken,
            params Type[] doNotRetryExceptionTypes)
        {
            return await func.Invoke(this.GetMockCommunicationClient());
        }

        private HttpCommunicationClient GetMockCommunicationClient()
        {
            return new HttpCommunicationClient(new HttpClient(new MockHttpMessageHandler())
            {
                BaseAddress = new Uri("http://testapp:8080/"),
            });
        }

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var requestUri = request.RequestUri.ToString();
                if (requestUri.EndsWith(MigrationConstants.EnumeratebySNEndpoint))
                {
                    var kvPairList = new List<KVSToRCMigration.Models.KeyValuePair>();
                    for (int i = 0; i < EnumerationSize; i++)
                    {
                        kvPairList.Add(new KVSToRCMigration.Models.KeyValuePair
                        {
                            Version = i,
                        });
                    }

                    var response = new EnumerationResponse
                    {
                        EndSequenceNumberReached = true,
                        KeyValuePairs = kvPairList,
                    };

                    return Task.FromResult(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new ByteArrayContent(SerializationUtility.Serialize(KvsActorStateProviderExtensions.ResponseSerializer, response)),
                    });
                }

                return Task.FromResult(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                });
            }
        }
    }
}
