// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal class StatelessServiceInstanceAdapter : IStatelessServiceInstance
    {
        private const string TraceType = "StatelessServiceInstanceAdapter";
        private readonly string traceId;

        private readonly ServiceHelper serviceHelper;
        private readonly StatelessServiceContext serviceContext;
        private readonly IStatelessUserServiceInstance userServiceInstance;

        private IStatelessServicePartition servicePartition;
        private IEnumerable<ServiceInstanceListener> instanceListeners;
        private IList<CommunicationListenerInfo> communicationListenersInfo;
        private ServiceEndpointCollection endpointCollection;

        private CancellationTokenSource runAsynCancellationTokenSource;

        /// <summary>
        /// This task wraps the actual RunAsync task. All the exceptions
        /// escaping from actual RunAsync are handled inside the task itself.
        /// </summary>
        private Task executeRunAsyncTask;

        internal StatelessServiceInstanceAdapter(
            StatelessServiceContext context,
            IStatelessUserServiceInstance userServiceInstance)
        {
            this.serviceContext = context;
            this.traceId = ServiceTrace.GetTraceIdForReplica(context.PartitionId, context.InstanceId);
            this.serviceHelper = new ServiceHelper(TraceType, this.traceId);

            this.servicePartition = null;
            this.instanceListeners = null;
            this.communicationListenersInfo = null;
            this.endpointCollection = new ServiceEndpointCollection();

            this.runAsynCancellationTokenSource = null;
            this.executeRunAsyncTask = null;

            this.userServiceInstance = userServiceInstance;
            this.userServiceInstance.Addresses = this.endpointCollection.ToReadOnlyDictionary();
        }

        #region Implementation of IStatelessServiceInstance

        void IStatelessServiceInstance.Initialize(StatelessServiceInitializationParameters initializationParameters)
        {
            // no-op - the initialization is done in the constructor
        }

        async Task<string> IStatelessServiceInstance.OpenAsync(
            IStatelessServicePartition partition,
            CancellationToken cancellationToken)
        {
            this.servicePartition = partition;

            // Set partition for user service instance before opening communication
            // listeners to enable users to report health using partition during
            // listener open process.
            this.userServiceInstance.Partition = partition;

            try
            {
                this.endpointCollection = await this.OpenCommunicationListenersAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                ServiceTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "Got exception when opening communication listeners - {0}",
                    exception);

                this.AbortCommunicationListeners();

                throw;
            }

            this.userServiceInstance.Addresses = this.endpointCollection.ToReadOnlyDictionary();

            this.runAsynCancellationTokenSource = new CancellationTokenSource();
            this.executeRunAsyncTask = this.ScheduleRunAsync(this.runAsynCancellationTokenSource.Token);

            await this.userServiceInstance.OnOpenAsync(cancellationToken);

            return this.endpointCollection.ToString();
        }

        async Task IStatelessServiceInstance.CloseAsync(CancellationToken cancellationToken)
        {
            await this.CloseCommunicationListenersAsync(cancellationToken);
            await this.CancelRunAsync();

            await this.userServiceInstance.OnCloseAsync(cancellationToken);
        }

        void IStatelessServiceInstance.Abort()
        {
            this.CancelRunAsync().ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted);

            this.AbortCommunicationListeners();

            this.userServiceInstance.OnAbort();
        }

        #endregion

        #region Test Hooks

        internal bool Test_IsRunAsyncTaskRunning()
        {
            return (!this.executeRunAsyncTask.IsCompleted &&
                    !this.executeRunAsyncTask.IsCanceled &&
                    !this.executeRunAsyncTask.IsFaulted);
        }

        #endregion

        #region RunAsync Management

        private Task ScheduleRunAsync(CancellationToken runAsyncCancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Scheduling RunAsync");

            // Ensure that RunAsync is invoked on a different thread so that calling thread
            // can return and complete the OpenAsync() call. If we await RunAsync directly in
            // current thread, then user can block the current thread and OpenAsync() call will
            // not complete.
            //
            // Explicitly passing CancellationToken.None to Task.Run() to ensure that user's
            // RunAsync() does get invoked. Passing 'runAsyncCancellationToken' to Task.Run()
            // means that if 'runAsyncCancellationToken' is signaled before RunAsync() is actually
            // scheduled, awaiting 'executeRunAsyncTask' in CancelRunAsync() will throw
            // TaskCanceledException and we will not know if it came from user's RunAsync or not.
            return Task.Run(
                () => this.ExecuteRunAsync(runAsyncCancellationToken),
                CancellationToken.None);
        }

        /// <summary>
        /// We handle all the exceptions coming from actual RunAsync here.
        /// </summary>
        private async Task ExecuteRunAsync(CancellationToken runAsyncCancellationToken)
        {
            ServiceFrameworkEventSource.Writer.StatelessRunAsyncInvocation(this.serviceContext);

            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Calling RunAsync");

            try
            {
                await this.userServiceInstance.RunAsync(runAsyncCancellationToken);
            }
            catch (OperationCanceledException e)
            {
                if (!runAsyncCancellationToken.IsCancellationRequested)
                {
                    ServiceFrameworkEventSource.Writer.StatelessRunAsyncFailure(
                        this.serviceContext,
                        runAsyncCancellationToken.IsCancellationRequested,
                        e);

                    this.serviceHelper.HandleRunAsyncUnexpectedException(this.servicePartition, e);
                    return;
                }

                ServiceTrace.Source.WriteInfoWithId(
                        TraceType,
                        this.traceId,
                        "RunAsync successfully canceled by throwing OperationCanceledException: {0}",
                        e.ToString());
            }
            catch (FabricException e)
            {
                ServiceFrameworkEventSource.Writer.StatelessRunAsyncFailure(
                        this.serviceContext,
                        runAsyncCancellationToken.IsCancellationRequested,
                        e);

                this.serviceHelper.HandleRunAsyncUnexpectedFabricException(this.servicePartition, e);
                return;
            }
            catch (Exception e)
            {
                ServiceFrameworkEventSource.Writer.StatelessRunAsyncFailure(
                        this.serviceContext,
                        runAsyncCancellationToken.IsCancellationRequested,
                        e);

                this.serviceHelper.HandleRunAsyncUnexpectedException(this.servicePartition, e);
                return;
            }

            ServiceFrameworkEventSource.Writer.StatelessRunAsyncCompletion(
                this.serviceContext,
                runAsyncCancellationToken.IsCancellationRequested);

            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, "RunAsync completed");
        }

        /// <summary>
        /// This gets called in two cases:
        ///
        /// 1) When replica is being closed.
        /// 2) When replica is being aborted.
        ///
        /// </summary>
        private async Task CancelRunAsync()
        {
            if (this.runAsynCancellationTokenSource != null &&
                this.runAsynCancellationTokenSource.IsCancellationRequested == false)
            {
                ServiceFrameworkEventSource.Writer.StatelessRunAsyncCancellation(
                    this.serviceContext,
                    ServiceHelper.RunAsyncExpectedCancellationTimeSpan);

                ServiceTrace.Source.WriteInfoWithId(
                    TraceType + ServiceHelper.ApiStartTraceTypeSuffix,
                    this.traceId,
                    "Canceling RunAsync");

                var cancellationStopwatch = new Stopwatch();
                cancellationStopwatch.Start();

                this.runAsynCancellationTokenSource.Cancel();

                try
                {
                    if (this.executeRunAsyncTask != null)
                    {
                        // All exception escaping from actual RunAsync are already taken care of
                        // inside this task (see method ExecuteRunAsync()). No exception is expected
                        // on awaiting this task and should be re-thrown.
                        //
                        // When CancelRunAsync() is invoked as part of replica closing (see method
                        // IStatelessServiceInstance.CloseAsync) it is awaited by the caller
                        // and re-thrown exception will then propagate to RA which will take
                        // appropriate actions.
                        //
                        // When CancelRunAsync() is invoked as part of replica aborting, the caller does
                        // not await it and exception is ignored as replica is anyway aborting.
                        await this.serviceHelper.AwaitRunAsyncWithHealthReporting(this.servicePartition, this.executeRunAsyncTask);
                    }
                }
                catch (Exception ex)
                {
                    ServiceTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.traceId,
                        "executeRunAsyncTask threw an unexpected exception: {0}",
                        ex.ToString());
                    throw;
                }
                finally
                {
                    this.runAsynCancellationTokenSource = null;
                    this.executeRunAsyncTask = null;
                    cancellationStopwatch.Stop();
                }

                if (cancellationStopwatch.Elapsed > ServiceHelper.RunAsyncExpectedCancellationTimeSpan)
                {
                    ServiceFrameworkEventSource.Writer.StatelessRunAsyncSlowCancellation(
                        this.serviceContext,
                        cancellationStopwatch.Elapsed,
                        ServiceHelper.RunAsyncExpectedCancellationTimeSpan);

                    ServiceTrace.Source.WriteWarningWithId(
                        TraceType + ServiceHelper.ApiSlowTraceTypeSuffix,
                        this.traceId,
                        "RunAsync slow cancellation: Time: {0}s",
                        cancellationStopwatch.Elapsed.TotalSeconds);
                }
            }
        }

        #endregion

        #region Communication Listeners Management

        private async Task<ServiceEndpointCollection> OpenCommunicationListenersAsync(CancellationToken cancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Opening communication listeners");

            if (this.instanceListeners == null)
            {
                this.instanceListeners = this.userServiceInstance.CreateServiceInstanceListeners();
            }

            var endpointsCollection = new ServiceEndpointCollection();
            var listenerOpenedCount = 0;

            foreach (var entry in this.instanceListeners)
            {
                string traceMsg;

                if (entry is null)
                {
                    traceMsg = "Skipped (<null>) instance listener.";
                    ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);

                    continue;
                }

                var communicationListener = entry.CreateCommunicationListener(this.serviceContext);
                if (communicationListener is null)
                {
                    traceMsg = $"Skipped '{entry.Name}' (<null>) communication listener.";
                    ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);

                    continue;
                }

                var communicationListenerInfo = new CommunicationListenerInfo
                {
                    Name = entry.Name.Equals(ServiceInstanceListener.DefaultName) ? "default" : entry.Name,
                    Listener = communicationListener,
                };

                this.AddCommunicationListener(communicationListenerInfo);

                traceMsg = $"Opening {communicationListenerInfo.Name} communication listener.";
                ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);

                var endpointAddress = await communicationListener.OpenAsync(cancellationToken);
                endpointsCollection.AddEndpoint(entry.Name, endpointAddress);
                listenerOpenedCount++;

                traceMsg = $"Opened {communicationListenerInfo.Name} communication listener.";
                ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);
            }

            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, $"Opened {listenerOpenedCount} communication listeners.");
            return endpointsCollection;
        }

        private async Task CloseCommunicationListenersAsync(CancellationToken cancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Closing {0} communication listeners..",
                (this.communicationListenersInfo != null) ? this.communicationListenersInfo.Count : 0);

            if (this.communicationListenersInfo != null)
            {
                try
                {
                    foreach (var entry in this.communicationListenersInfo)
                    {
                        var traceMsg = $"Closing {entry.Name} communication listener.";
                        ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);

                        var closeCommunicationListenerTask = entry.Listener.CloseAsync(cancellationToken);
                        await this.serviceHelper.AwaitCloseCommunicationListerWithHealthReporting(this.servicePartition, closeCommunicationListenerTask, entry.Name);

                        traceMsg = $"Closed {entry.Name} communication listener.";
                        ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);
                    }
                }
                catch (Exception exception)
                {
                    ServiceTrace.Source.WriteWarningWithId(
                        TraceType,
                        this.traceId,
                        "Got exception when closing communication listeners : {0}",
                        exception);

                    this.AbortCommunicationListeners();
                }

                this.communicationListenersInfo = null;
            }

            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Closed all communication listeners.");
        }

        private void AddCommunicationListener(CommunicationListenerInfo communicationListenerInfo)
        {
            if (this.communicationListenersInfo == null)
            {
                this.communicationListenersInfo = new List<CommunicationListenerInfo>();
            }

            this.communicationListenersInfo.Add(communicationListenerInfo);
        }

        private void AbortCommunicationListeners()
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Aborting communication listeners..");

            if (this.communicationListenersInfo != null)
            {
                List<Exception> exceptions = null;
                foreach (var entry in this.communicationListenersInfo)
                {
                    try
                    {
                        entry.Listener.Abort();
                    }
                    catch (Exception e)
                    {
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }

                        exceptions.Add(e);
                    }
                }

                this.communicationListenersInfo = null;
                if (exceptions != null)
                {
                    // Trace the exception and continue. Do not bubble up exception as abort path
                    // should do best effort cleanup and continue. This allows other component in
                    // abort path to perform their best effort cleanup.
                    var aggregateException = new AggregateException(exceptions);

                    ServiceTrace.Source.WriteWarningWithId(
                        TraceType,
                        this.traceId,
                        "Got exception when aborting communication listeners : {0}",
                        aggregateException);
                }
            }
        }

        #endregion

    }
}
