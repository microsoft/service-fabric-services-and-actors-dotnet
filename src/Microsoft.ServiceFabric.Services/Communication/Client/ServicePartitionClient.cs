// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;

    /// <summary>
    /// Specifies an instance of the communication client that can communicate with the replicas of a particular partition.
    /// </summary>
    /// <typeparam name="TCommunicationClient">type of Communication client</typeparam>
    public class ServicePartitionClient<TCommunicationClient> : IServicePartitionClient<TCommunicationClient>
        where TCommunicationClient : ICommunicationClient
    {
        private const string TraceType = "ServicePartitionClient";

        private readonly ICommunicationClientFactory<TCommunicationClient> communicationClientFactory;
        private readonly SemaphoreSlim communicationClientLock;

        private readonly string traceId;
        private readonly Uri serviceUri;
        private readonly ServicePartitionKey partitionKey;
        private readonly TargetReplicaSelector targetReplicaSelector;
        private readonly string listenerName;
        private readonly OperationRetrySettings retrySettings;

        private TCommunicationClient communicationClient;
        private volatile ResolvedServicePartition lastRsp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionClient{TCommunicationClient}"/> class that uses the specified communication client factory to create
        /// a client to talk to the service endpoint identified by the service uri, partitionkey, replica and listener
        /// arguments.
        /// </summary>
        /// <param name="communicationClientFactory">Communication client factory</param>
        /// <param name="serviceUri">Name of the service</param>
        /// <param name="partitionKey">The partition key used to identify the partition within the service.</param>
        /// <param name="targetReplicaSelector">Target replica information</param>
        /// <param name="listenerName">Listener in the replica to which the client should connect to</param>
        /// <param name="retrySettings">Retry policy for exceptions seen during communication</param>
        public ServicePartitionClient(
            ICommunicationClientFactory<TCommunicationClient> communicationClientFactory,
            Uri serviceUri,
            ServicePartitionKey partitionKey = null,
            TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
            string listenerName = null,
            OperationRetrySettings retrySettings = null)
        {
            this.communicationClientFactory = communicationClientFactory;
            this.communicationClientLock = new SemaphoreSlim(1);
            this.communicationClient = default(TCommunicationClient);
            this.traceId = Guid.NewGuid().ToString();
            this.serviceUri = serviceUri;
            this.partitionKey = partitionKey ?? ServicePartitionKey.Singleton;
            this.listenerName = listenerName;
            this.targetReplicaSelector = targetReplicaSelector;
            this.lastRsp = null;
            this.retrySettings = retrySettings ?? new OperationRetrySettings();
        }

        /// <summary>
        /// Gets the communication client factory
        /// </summary>
        /// <value>Communication client factory</value>
        public ICommunicationClientFactory<TCommunicationClient> Factory
        {
            get { return this.communicationClientFactory; }
        }

        /// <summary>
        /// Gets the name of the service
        /// </summary>
        /// <value>Name of the service</value>
        public Uri ServiceUri
        {
            get { return this.serviceUri; }
        }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        /// <value>Partition key</value>
        public ServicePartitionKey PartitionKey
        {
            get { return this.partitionKey; }
        }

        /// <summary>
        /// Gets the information about which replica in the partition the client should connect to.
        /// </summary>
        /// <value>A <see cref="Microsoft.ServiceFabric.Services.Communication.Client.TargetReplicaSelector"/></value>
        public TargetReplicaSelector TargetReplicaSelector
        {
            get { return this.targetReplicaSelector; }
        }

        /// <summary>
        /// Gets the name of the listener in the replica to which the client should connect to.
        /// </summary>
        /// <value>Listener name</value>
        public string ListenerName
        {
            get { return this.listenerName; }
        }

        /// <summary>
        /// Gets the resolved service partition that was set on the client.
        /// </summary>
        /// <param name="resolvedServicePartition">previous ResolvedServicePartition</param>
        /// <returns>true if a ResolvedServicePartition was set</returns>
        public bool TryGetLastResolvedServicePartition(out ResolvedServicePartition resolvedServicePartition)
        {
            resolvedServicePartition = this.lastRsp;
            return (resolvedServicePartition != null);
        }

        /// <summary>
        /// Invokes the given Function, retrying for exceptions thrown other than the exceptions in the doNotRetryExceptionTypes.
        /// For exceptions that are not in doNotRetryExceptionTypes, CommunicationClientFactory's ReportOperationExceptionAsync() method
        /// controls if the exception should be retried or not.
        /// If you are invoking this method in Asp.Net / UI thread, these are recommendations to avoid deadlock:
        ///  1 if your calling Api is Async , use <see cref="Task.ConfigureAwait(bool)"/> to not to resume in orignal context by setting it to false.
        ///  2 Or To  invoke this Api in a threadpool thread using Task.Run.
        /// </summary>
        /// <typeparam name="TResult">Result from the function being invoked</typeparam>
        /// <param name="func">Function being invoked</param>
        /// <param name="doNotRetryExceptionTypes">Exceptions for which the service partition client should not retry</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the result from the function given in the argument.
        /// </returns>
        public Task<TResult> InvokeWithRetryAsync<TResult>(
            Func<TCommunicationClient, Task<TResult>> func,
            params Type[] doNotRetryExceptionTypes)
        {
            return this.InvokeWithRetryAsync(func, CancellationToken.None, doNotRetryExceptionTypes);
        }

        /// <summary>
        /// Invokes the given Function, retrying for exceptions thrown other than the exceptions in the doNotRetryExceptionTypes.
        /// For exceptions that are not in doNotRetryExceptionTypes, CommunicationClientFactory's ReportOperationExceptionAsync() method
        /// controls if the exception should be retried or not.
        ///  If you are invoking this method in Asp.Net / UI thread, these are recommendations to avoid deadlock:
        ///  1 if your calling Api is Async , use <see cref="Task.ConfigureAwait(bool)"/> to not to resume in orignal context by setting it to false.
        ///  2 Or To  invoke this Api  in a threadpool thread using Task.Run.
        /// </summary>
        /// <typeparam name="TResult">Result from the function being invoked</typeparam>
        /// <param name="func">Function being invoked</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="doNotRetryExceptionTypes">Exceptions for which the service partition client should not retry</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result of the Task is
        /// the result from the function given in the argument.
        /// </returns>
        public async Task<TResult> InvokeWithRetryAsync<TResult>(
            Func<TCommunicationClient, Task<TResult>> func,
            CancellationToken cancellationToken,
            params Type[] doNotRetryExceptionTypes)
        {
            var currentRetryCount = 0;
            string currentExceptionId = null;

            while (true)
            {
                Exception exception;
                var client = await this.GetCommunicationClientAsync(cancellationToken);

                try
                {
                    // throw if cancellation has been requested.
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = await func.Invoke(client);
                    return result;
                }
                catch (AggregateException ae)
                {
                    ServiceTrace.Source.WriteNoiseWithId(
                        TraceType,
                        this.traceId,
                        "AggregateException While Invoking API {0}",
                        ae);

                    ae.Handle(x => !doNotRetryExceptionTypes.Contains(x.GetType()));
                    exception = ae;
                }
                catch (Exception e)
                {
                    ServiceTrace.Source.WriteNoiseWithId(
                        TraceType,
                        this.traceId,
                        "Exception While Invoking API {0}",
                        e);

                    if (doNotRetryExceptionTypes.Contains(e.GetType()))
                    {
                        throw;
                    }

                    exception = e;
                }

                // The exception that is being processed by the factory could be because of the cancellation
                // requested to the remote call, so not passing the same cancellation token to the api below
                // to not let the regular exception processing be interrupted.
                var exceptionReportResult = await this.communicationClientFactory.ReportOperationExceptionAsync(
                        client,
                        new ExceptionInformation(exception, this.targetReplicaSelector),
                        this.retrySettings,
                        CancellationToken.None);

                if (!exceptionReportResult.ShouldRetry ||
                    !Utility.ShouldRetryOperation(
                        exceptionReportResult.ExceptionId,
                        exceptionReportResult.MaxRetryCount,
                        ref currentExceptionId,
                        ref currentRetryCount))
                {
                    throw exceptionReportResult.Exception ?? exception;
                }

                ServiceTrace.Source.WriteInfoWithId(
                    TraceType,
                    this.traceId,
                    "Exception report result Id: {0}  IsTransient : {1} Delay : {2}",
                    exceptionReportResult.ExceptionId,
                    exceptionReportResult.IsTransient,
                    exceptionReportResult.RetryDelay);

                if (!exceptionReportResult.IsTransient)
                {
                    await this.ResetCommunicationClientAsync();
                }

                await Task.Delay(exceptionReportResult.RetryDelay, cancellationToken);
            }
        }

        /// <summary>
        /// Invokes the given Function, retrying for exceptions thrown other than the exceptions in the doNotRetryExceptionTypes.
        /// For exceptions that are not in doNotRetryExceptionTypes, CommunicationClientFactory's ReportOperationExceptionAsync() method
        /// controls if the exception should be retried or not.
        /// If you are invoking this method in Asp.Net / UI thread, these are recommendations to avoid deadlock:
        ///  1 if your calling Api is Async , use <see cref="Task.ConfigureAwait(bool)"/> to not to resume in orignal context by setting it to false.
        ///  2 Or To  invoke this Api in a threadpool thread using Task.Run.
        /// </summary>
        /// <param name="func">Function being invoked</param>
        /// <param name="doNotRetryExceptionTypes">Exceptions for which the service partition client should not retry</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        public Task InvokeWithRetryAsync(Func<TCommunicationClient, Task> func, params Type[] doNotRetryExceptionTypes)
        {
            return this.InvokeWithRetryAsync(func, CancellationToken.None, doNotRetryExceptionTypes);
        }

        /// <summary>
        /// Invokes the given Function, retrying for exceptions thrown other than the exceptions in the doNotRetryExceptionTypes.
        /// For exceptions that are not in doNotRetryExceptionTypes, CommunicationClientFactory's ReportOperationExceptionAsync() method
        /// controls if the exception should be retried or not.
        /// If you are invoking this method in Asp.Net / UI thread, these are recommendations to avoid deadlock:
        ///  1 if your calling Api is Async , use <see cref="Task.ConfigureAwait(bool)"/> to not to resume in orignal context by setting it to false.
        ///  2 Or To  invoke this Api in a threadpool thread using Task.Run.
        /// </summary>
        /// <param name="func">Function being invoked</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="doNotRetryExceptionTypes">Exceptions for which the service partition client should not retry</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        public async Task InvokeWithRetryAsync(
            Func<TCommunicationClient, Task> func,
            CancellationToken cancellationToken,
            params Type[] doNotRetryExceptionTypes)
        {
            await this.InvokeWithRetryAsync(
                async client =>
                {
                    await func.Invoke(client);
                    return new object();
                },
                cancellationToken,
                doNotRetryExceptionTypes);
        }

        /// <summary>
        /// Invokes the given Function, retrying for exceptions thrown other than the exceptions in the doNotRetryExceptionTypes.
        /// For exceptions that are not in doNotRetryExceptionTypes, CommunicationClientFactory's ReportOperationExceptionAsync() method
        /// controls if the exception should be retried or not.
        /// </summary>
        /// <typeparam name="TResult">Result from the function being invoked</typeparam>
        /// <param name="func">Function being invoked</param>
        /// <param name="doNotRetryExceptionTypes">Exceptions for which the service partition client should not retry</param>
        /// <returns>Result from the function given in the argument</returns>
        [Obsolete("Use InvokeWithRetryAsync Api instead ")]
        public TResult InvokeWithRetry<TResult>(
            Func<TCommunicationClient, TResult> func,
            params Type[] doNotRetryExceptionTypes)
        {
            var task = this.InvokeWithRetryAsync(
                client =>
                {
                    var tcs = new TaskCompletionSource<TResult>();
                    tcs.SetResult(func.Invoke(client));
                    return tcs.Task;
                },
                CancellationToken.None,
                doNotRetryExceptionTypes);
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Invokes the given Function, retrying for exceptions thrown other than the exceptions in the doNotRetryExceptionTypes.
        /// For exceptions that are not in doNotRetryExceptionTypes, CommunicationClientFactory's ReportOperationExceptionAsync() method
        /// controls if the exception should be retried or not.
        /// </summary>
        /// <param name="func">Function being invoked</param>
        /// <param name="doNotRetryExceptionTypes">Exceptions for which the service partition client should not retry</param>
        [Obsolete("Use InvokeWithRetryAsync Api instead ")]
        public void InvokeWithRetry(
            Action<TCommunicationClient> func,
            params Type[] doNotRetryExceptionTypes)
        {
            this.InvokeWithRetry<object>(
                client =>
                {
                    func.Invoke(client);
                    return null;
                },
                doNotRetryExceptionTypes);
        }

        private async Task<TCommunicationClient> GetCommunicationClientAsync(CancellationToken cancellationToken)
        {
            TCommunicationClient client;
            await this.communicationClientLock.WaitAsync(cancellationToken);
            try
            {
                if (this.lastRsp == null)
                {
                    this.communicationClient = await this.communicationClientFactory.GetClientAsync(
                        this.ServiceUri,
                        this.partitionKey,
                        this.targetReplicaSelector,
                        this.listenerName,
                        this.retrySettings,
                        cancellationToken);

                    this.lastRsp = this.communicationClient.ResolvedServicePartition;
                }
                else if (this.communicationClient == null)
                {
                    this.communicationClient = await this.communicationClientFactory.GetClientAsync(
                        this.lastRsp,
                        this.targetReplicaSelector,
                        this.listenerName,
                        this.retrySettings,
                        cancellationToken);

                    this.lastRsp = this.communicationClient.ResolvedServicePartition;
                }

                client = this.communicationClient;
            }
            finally
            {
                // Release the lock incase of exceptions from the GetClientAsync method, which can
                // happen if there are non retriable exceptions in that method. Eg: There can be
                // ServiceNotFoundException if the GetClientAsync client is called before the
                // service creation completes.
                this.communicationClientLock.Release();
            }

            return client;
        }

        private async Task ResetCommunicationClientAsync()
        {
            await this.communicationClientLock.WaitAsync();

            this.communicationClient = default(TCommunicationClient);

            this.communicationClientLock.Release();
        }
    }
}
