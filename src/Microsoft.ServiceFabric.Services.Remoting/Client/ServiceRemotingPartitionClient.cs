// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Microsoft.ServiceFabric.Services.Remoting;

    /// <summary>
    /// Specifies the Service partition client for Remoting communication.
    /// </summary>
    internal class ServiceRemotingPartitionClient : ServicePartitionClient<IServiceRemotingClient>, IServiceRemotingPartitionClient
    {
        private const string TraceType = "ServiceRemotingPartitionClient";
       
        public ServiceRemotingPartitionClient(
            IServiceRemotingClientFactory remotingClientFactory,
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null,
            OperationRetrySettings retrySettings = null)
            : base(
            remotingClientFactory, 
            serviceUri, 
            partitionKey,
            targetReplicaSelector,
            listenerName, 
            retrySettings)
        {
        }

        internal async Task<byte[]> InvokeAsync(
            ServiceRemotingMessageHeaders headers, 
            byte[] requestMsgBody,
            CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                return await this.InvokeWithRetryAsync(
                    client => client.RequestResponseAsync(headers, requestMsgBody),
                    cancellationToken);
            }

            //
            // Remote calls that can be canceled need to be identifiable. So set the call context if
            // the higher layer hasn't already set one.
            //
            if (headers.InvocationId == null)
            {
                headers.InvocationId = Guid.NewGuid().ToString();
            }

            //
            // Create a TaskCompletionSource that completes with false on cancellation.
            //
           
            var tcs = new TaskCompletionSource<bool>();

            // Using statement will make sure that we dispose registerationtoken which in turn  un-register cancellationtoken.

            using (cancellationToken.Register(() => tcs.TrySetResult(false)))
            {
                var innerTask = this.InvokeWithRetryAsync(
                    client => client.RequestResponseAsync(headers, requestMsgBody),
                    cancellationToken);

                var completedTask = await Task.WhenAny(innerTask, tcs.Task);

                if (completedTask != innerTask)
                {
                    // Task has been canceled.
                    if (cancellationToken.IsCancellationRequested)
                    {
                        //
                        // Invoke the cancellation logic.
                        // Adding a cancellation header indicates that the request that was sent with
                        // for the interface, method and identified by the call-context should be canceled.
                        //
                        ServiceTrace.Source.WriteInfo(
                            TraceType,
                            "Cancellation requested for CallContext : {0}, MethodId : {1}, InterfaceId : {2}",
                            headers.InvocationId,
                            headers.MethodId,
                            headers.InterfaceId);

                        headers.AddHeader(ServiceRemotingMessageHeaders.CancellationHeaderName, new byte[0]);

                        // Cancellation token is not sent in this call that means that cancellation *will* be 
                        // delivered.
                        await this.InvokeWithRetryAsync(
                            client => client.RequestResponseAsync(headers, requestMsgBody),
                            CancellationToken.None);

                        ServiceTrace.Source.WriteInfo(
                            TraceType,
                            "Cancellation delivered for CallContext : {0}, MethodId : {1}, InterfaceId : {2}",
                            headers.InvocationId,
                            headers.MethodId,
                            headers.InterfaceId);
                    }
                }

                tcs.TrySetResult(true);
                return await innerTask;
            }
        }
    }
}
