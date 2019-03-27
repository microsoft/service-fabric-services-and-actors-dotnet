// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Specifies the Service partition client for Remoting communication
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

        public async Task<IServiceRemotingResponseMessage> InvokeAsync(
          IServiceRemotingRequestMessage remotingRequestMessage,
          string methodName,
          CancellationToken cancellationToken)
        {
            ServiceRemotingClientEvents.RaiseSendRequest(remotingRequestMessage, this.ServiceUri, methodName);

            Task<IServiceRemotingResponseMessage> returnValue;

            if (!cancellationToken.CanBeCanceled)
            {
                returnValue = this.InvokeWithRetryAsync(
                    client => client.RequestResponseAsync(remotingRequestMessage),
                    cancellationToken);
            }
            else
            {
                // Remote calls that can be canceled need to be identifiable. So set the call context if
                // the higher layer hasn't already set one.
                if (remotingRequestMessage.GetHeader().InvocationId == null)
                {
                    remotingRequestMessage.GetHeader().InvocationId = Guid.NewGuid().ToString();
                }

                // Create a TaskCompletionSource that completes with false on cancellation.
                var tcs = new TaskCompletionSource<bool>();
                using (cancellationToken.Register(() => tcs.TrySetResult(false)))
                {
                    var innerTask = this.InvokeWithRetryAsync(
                        client => client.RequestResponseAsync(remotingRequestMessage),
                        cancellationToken);

                    var completedTask = await Task.WhenAny(innerTask, tcs.Task);

                    if (completedTask != innerTask)
                    {
                        // Task has been canceled.
                        if (cancellationToken.IsCancellationRequested)
                        {
                            // Invoke the cancellation logic.
                            // Adding a cancellation header indicates that the request that was sent with
                            // for the interface, method and identified by the call-context should be canceled.
                            var headers = remotingRequestMessage.GetHeader();
                            ServiceTrace.Source.WriteInfo(
                                TraceType,
                                "Cancellation requested for CallContext : {0}, MethodId : {1}, InterfaceId : {2}",
                                headers.InvocationId,
                                headers.MethodId,
                                headers.InterfaceId);

                            headers.AddHeader(ServiceRemotingRequestMessageHeader.CancellationHeaderName, new byte[0]);

                            // Under normal service operation, we want to make sure that cancellation message is
                            // delivered to destination service. However if the destination service undergoes failover
                            // while cancellation request is in progress, we would not want to retry the cancellation
                            // after resolving to new primary as it is not valid for the new primary that will take over.
                            //
                            // Have a cancellation token that can used to cancel the cancellation task if needed.
                            var remoteCancellationTaskCts = new CancellationTokenSource();

                            // Cancellation token is not sent in this call that means that cancellation *will* be
                            // delivered.
                            var remoteCancellationTask = this.InvokeWithRetryAsync(
                                client => client.RequestResponseAsync(remotingRequestMessage),
                                remoteCancellationTaskCts.Token);

                            // During failover, the actual remote request task retries to resolve to new primary and
                            // before each retry, checks if the user has requested cancellation.
                            //
                            // To handle both normal and long/stuck failover scenario, wait for either of cancellation
                            // task or actual request task to finish.
                            var finishedTask = await Task.WhenAny(innerTask, remoteCancellationTask);

                            if (finishedTask != innerTask)
                            {
                                ServiceTrace.Source.WriteInfo(
                                    TraceType,
                                    "Cancellation delivered for CallContext : {0}, MethodId : {1}, InterfaceId : {2}",
                                    headers.InvocationId,
                                    headers.MethodId,
                                    headers.InterfaceId);
                            }
                            else
                            {
                                // Actual task finished before cancellation task.
                                // Cancel the cancellation task and observe exception if any.
                                remoteCancellationTaskCts.Cancel();

                                try
                                {
                                    await remoteCancellationTask;
                                }
                                catch (Exception)
                                {
                                    // Ignore.
                                }
                            }
                        }
                    }

                    tcs.TrySetResult(true);
                    returnValue = innerTask;
                }
            }

            IServiceRemotingResponseMessage response;
            try
            {
                response = await returnValue;
            }
            catch (Exception ex)
            {
                ServiceRemotingClientEvents.RaiseExceptionResponse(ex, remotingRequestMessage);
                throw;
            }

            ServiceRemotingClientEvents.RaiseRecieveResponse(response, remotingRequestMessage);

            return response;
        }
    }
}
