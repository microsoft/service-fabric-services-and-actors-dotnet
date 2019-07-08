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
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    internal class StatefulServiceReplicaAdapter : IStatefulServiceReplica
    {
        private const string TraceType = "StatefulServiceReplicaAdapter";
        private const int PrimaryStatusCheckRetryIntervalInMillis = 512;

        private readonly string traceId;

        private readonly ServiceHelper serviceHelper;
        private readonly StatefulServiceContext serviceContext;
        private readonly IStatefulUserServiceReplica userServiceReplica;

        private IStateProviderReplica stateProviderReplica;
        private IStatefulServicePartition servicePartition;
        private IEnumerable<ServiceReplicaListener> replicaListeners;
        private IList<CommunicationListenerInfo> communicationListenersInfo;
        private ServiceEndpointCollection endpointCollection;

        private CancellationTokenSource runAsynCancellationTokenSource;

        /// <summary>
        /// This task wraps the actual RunAsync task. All the exceptions
        /// escaping from actual RunAsync are handled inside the task itself.
        /// </summary>
        private Task executeRunAsyncTask;

        internal StatefulServiceReplicaAdapter(
            StatefulServiceContext context,
            IStatefulUserServiceReplica userServiceReplica)
        {
            this.serviceContext = context;
            this.traceId = ServiceTrace.GetTraceIdForReplica(context.PartitionId, context.ReplicaId);
            this.serviceHelper = new ServiceHelper(TraceType, this.traceId);

            this.servicePartition = null;
            this.replicaListeners = null;
            this.communicationListenersInfo = null;
            this.endpointCollection = new ServiceEndpointCollection();

            this.runAsynCancellationTokenSource = null;
            this.executeRunAsyncTask = null;

            this.userServiceReplica = userServiceReplica;
            this.userServiceReplica.Addresses = this.endpointCollection.ToReadOnlyDictionary();

            // The state provider replica should ideally be initialized
            // here (this.stateProviderReplica.Initialize()) with ServiceContext.
            // However the initialize function takes in StatefulServiceInitializationParameter
            // and resides in the DATA layer. DATA layer lies below SERVICES layer
            // and is agnostic of services and ServiceContext lies in SERVICES layer.
            // For now state provider replica is initialized when runtime calls
            // IStatefulServiceReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
            this.stateProviderReplica = this.userServiceReplica.CreateStateProviderReplica();
        }

        /// <summary>
        /// Gets the communication listener that this service is using. This is only for used for testing.
        /// </summary>
        internal IList<CommunicationListenerInfo> Test_CommunicationListeners
        {
            get { return this.communicationListenersInfo; }
        }

        #region Implementation of IStatefulServiceReplica

        void IStatefulServiceReplica.Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
            // See comments regarding state provider replica in c'tor
            this.stateProviderReplica.Initialize(initializationParameters);
        }

        async Task<IReplicator> IStatefulServiceReplica.OpenAsync(
            ReplicaOpenMode openMode, IStatefulServicePartition partition, CancellationToken cancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "OpenAsync");

            this.servicePartition = partition;
            this.userServiceReplica.Partition = partition;

            var replicator = await this.stateProviderReplica.OpenAsync(openMode, partition, cancellationToken);

            Exception userReplicaEx = null;
            try
            {
                await this.userServiceReplica.OnOpenAsync(openMode, cancellationToken);
            }
            catch (Exception ex)
            {
                userReplicaEx = ex;

                ServiceTrace.Source.WriteWarningWithId(
                    TraceType,
                    this.traceId,
                    "Unhandled exception from userServiceReplica.OnOpenAsync() - {0}",
                    ex);
            }

            if (userReplicaEx != null)
            {
                await this.stateProviderReplica.CloseAsync(cancellationToken);
                throw userReplicaEx;
            }

            return replicator;
        }

        async Task<string> IStatefulServiceReplica.ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "ChangeRoleAsync : new role {0}",
                newRole);

            await this.CloseCommunicationListenersAsync(cancellationToken);

            if (newRole == ReplicaRole.Primary)
            {
                this.endpointCollection = await this.OpenCommunicationListenersAsync(newRole, cancellationToken);
                this.userServiceReplica.Addresses = this.endpointCollection.ToReadOnlyDictionary();

                this.runAsynCancellationTokenSource = new CancellationTokenSource();
                this.executeRunAsyncTask = this.ScheduleRunAsync(this.runAsynCancellationTokenSource.Token);
            }
            else
            {
                await this.CancelRunAsync();

                if (newRole == ReplicaRole.ActiveSecondary)
                {
                    this.endpointCollection = await this.OpenCommunicationListenersAsync(newRole, cancellationToken);
                    this.userServiceReplica.Addresses = this.endpointCollection.ToReadOnlyDictionary();
                }
            }

            await this.stateProviderReplica.ChangeRoleAsync(newRole, cancellationToken);

            // ChangeRole (CR) on user service replica should be invoked after CR has been invoked
            //  on StateProvider (SP) to ensure consistent experience for user service replica
            // accross out-of-box SPs (ReliableCollection, KVS etc.) provided by Service Fabric
            // and custom SPs provided by users.
            //
            // SF's out-of-box SPs are based on local state and use SF's replicator to replicate state changes
            // to secondary replicas. A custom state provider may be based on some external store
            // (e.g. custom implementation of IActorStateProvider) and may not use replicator for replication.
            //
            // When a CR is initiated, ReliabilitySubsystem first invokes CR on the replicator and revokes its write status
            // (for a P->S CR) before invoking CR on service replica (i.e IStatefulServiceReplica.ChangeRoleAsync).
            // Hence, even if CR has not been invoked on SP, it will not be able to replicate and effectively has
            // write permission revoked.
            //
            // However, a custom SP which does not uses replication, needs to be notified of CR (P->S) so that
            // it does not allow further writes when CR is invoked for user service replica.
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "ChangeRoleAsync : Begin UserServiceReplica change role to {0}",
                newRole);

            await this.userServiceReplica.OnChangeRoleAsync(newRole, cancellationToken);

            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "ChangeRoleAsync : End UserServiceReplica change role");

            return this.endpointCollection.ToString();
        }

        async Task IStatefulServiceReplica.CloseAsync(CancellationToken cancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "CloseAsync");

            await this.CloseCommunicationListenersAsync(cancellationToken);
            await this.CancelRunAsync();
            await this.userServiceReplica.OnCloseAsync(cancellationToken);

            if (this.stateProviderReplica != null)
            {
                await this.stateProviderReplica.CloseAsync(cancellationToken);
                this.stateProviderReplica = null;
            }
        }

        void IStatefulServiceReplica.Abort()
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Abort");

            this.AbortCommunicationListeners();
            this.CancelRunAsync().ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted);
            this.userServiceReplica.OnAbort();

            if (this.stateProviderReplica != null)
            {
                this.stateProviderReplica.Abort();
                this.stateProviderReplica = null;
            }
        }

        #endregion

        internal bool Test_IsRunAsyncTaskRunning()
        {
            return (!this.executeRunAsyncTask.IsCompleted &&
                    !this.executeRunAsyncTask.IsCanceled &&
                    !this.executeRunAsyncTask.IsFaulted);
        }

        #region RunAsync Management

        private Task ScheduleRunAsync(CancellationToken runAsyncCancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Scheduling RunAsync");

            // Ensure that RunAsync is invoked on a different thread so that calling thread
            // can return and complete the ChangeRoleAsync() call. If we await user's RunAsync
            // directly in current thread, then user can block the current thread and
            // ChangeRoleAsync() call will not complete.
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
            var writeStatusGranted = await this.WaitForWriteStatusAsync(runAsyncCancellationToken);

            // 'writeStatusGranted' will be false only when:
            // 1) This replica is no longer primary.
            // 2) This replica is closing.
            // 3) Checking for partition write status has thrown an unexpected exception
            //    in which fault transient is reported (see method WaitForWriteStatusAsync()).
            if (!writeStatusGranted)
            {
                ServiceTrace.Source.WriteWarningWithId(TraceType, this.traceId, "Unable to acquire write status prior to calling RunAsync.");
                return;
            }

            runAsyncCancellationToken.ThrowIfCancellationRequested();

            ServiceFrameworkEventSource.Writer.StatefulRunAsyncInvocation(this.serviceContext);
            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, "Calling RunAsync");

            try
            {
                await this.userServiceReplica.RunAsync(runAsyncCancellationToken);
            }
            catch (OperationCanceledException e)
            {
                if (!runAsyncCancellationToken.IsCancellationRequested)
                {
                    ServiceFrameworkEventSource.Writer.StatefulRunAsyncFailure(
                        this.serviceContext,
                        runAsyncCancellationToken.IsCancellationRequested,
                        e);

                    this.serviceHelper.HandleRunAsyncUnexpectedException(this.servicePartition, e);
                    return;
                }

                ServiceTrace.Source.WriteInfoWithId(
                        TraceType + ServiceHelper.ApiFinishTraceTypeSuffix,
                        this.traceId,
                        "RunAsync successfully canceled by throwing OperationCanceledException: {0}",
                        e.ToString());
            }
            catch (FabricException e)
            {
                ServiceFrameworkEventSource.Writer.StatefulRunAsyncFailure(
                        this.serviceContext,
                        runAsyncCancellationToken.IsCancellationRequested,
                        e);

                this.serviceHelper.HandleRunAsyncUnexpectedFabricException(this.servicePartition, e);
                return;
            }
            catch (Exception e)
            {
                ServiceFrameworkEventSource.Writer.StatefulRunAsyncFailure(
                        this.serviceContext,
                        runAsyncCancellationToken.IsCancellationRequested,
                        e);

                this.serviceHelper.HandleRunAsyncUnexpectedException(this.servicePartition, e);
                return;
            }

            ServiceFrameworkEventSource.Writer.StatefulRunAsyncCompletion(
                this.serviceContext,
                runAsyncCancellationToken.IsCancellationRequested);
            ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, "RunAsync completed");
        }

        /// <summary>
        /// This gets called in three cases:
        ///
        /// 1) When replica is changing role from primary to secondary.
        /// 2) When replica is being closed.
        /// 3) When replica is being aborted.
        ///
        /// </summary>
        private async Task CancelRunAsync()
        {
            if (this.runAsynCancellationTokenSource != null &&
                this.runAsynCancellationTokenSource.IsCancellationRequested == false)
            {
                ServiceFrameworkEventSource.Writer.StatefulRunAsyncCancellation(
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
                        // inside this task (see method ExecuteRunAsync()). The only exception expected
                        // on awaiting it is OperationCanceledException (whose cancellation token matches
                        // 'this.runAsynCancellationTokenSource') since we check for cancellation requested
                        // during and right after write status is acquired and RunAsync is actually invoked.
                        // Any other exception is unexpected and should be re-thrown.
                        //
                        // When CancelRunAsync() is invoked as part of replica role change from primary
                        // to secondary (see method IStatefulServiceReplica.ChangeRoleAsync()) or when
                        // replica is closing (see method IStatefulServiceReplica.CloseAsync) it is awaited
                        // by the caller and re-thrown exception will then propagate to RA which will take
                        // appropriate actions.
                        //
                        // When CancelRunAsync() is invoked as part of replica aborting, the caller does
                        // not await it and exception is ignored as replica is anyway aborting.
                        await this.serviceHelper.AwaitRunAsyncWithHealthReporting(this.servicePartition, this.executeRunAsyncTask);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    if (ex.CancellationToken != this.runAsynCancellationTokenSource.Token)
                    {
                        ServiceTrace.Source.WriteErrorWithId(
                            TraceType + ServiceHelper.ApiErrorTraceTypeSuffix,
                            this.traceId,
                            "executeRunAsyncTask threw a non-matching OperationCanceledException: {0}",
                            ex.ToString());
                        throw;
                    }

                    ServiceTrace.Source.WriteNoiseWithId(TraceType, this.traceId, "executeRunAsyncTask canceled cooperatively");
                }
                catch (Exception ex)
                {
                    ServiceTrace.Source.WriteErrorWithId(
                        TraceType + ServiceHelper.ApiErrorTraceTypeSuffix,
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
                    ServiceFrameworkEventSource.Writer.StatefulRunAsyncSlowCancellation(
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

        private async Task<bool> WaitForWriteStatusAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                PartitionAccessStatus result;
                try
                {
                    result = this.servicePartition.WriteStatus;
                }
                catch (FabricObjectClosedException)
                {
                    // Normal case, replica is closing, so no need to do anything further
                    return false;
                }
                catch (Exception e)
                {
                    ServiceTrace.Source.WriteErrorWithId(
                        TraceType,
                        this.traceId,
                        "ServicePartition.WriteStatus threw an unexpected exception: {0}",
                        e.ToString());
                    this.servicePartition.ReportFault(FaultType.Transient);
                    return false;
                }

                if (result == PartitionAccessStatus.Granted)
                {
                    return true;
                }
                else if (result == PartitionAccessStatus.NotPrimary)
                {
                    return false;
                }

                await Task.Delay(PrimaryStatusCheckRetryIntervalInMillis, cancellationToken);
            }
        }

        #endregion

        #region Communication Listeners Management

        private void AddCommunicationListener(CommunicationListenerInfo communicationListenerInfo)
        {
            if (this.communicationListenersInfo == null)
            {
                this.communicationListenersInfo = new List<CommunicationListenerInfo>();
            }

            this.communicationListenersInfo.Add(communicationListenerInfo);
        }

        private async Task<ServiceEndpointCollection> OpenCommunicationListenersAsync(
            ReplicaRole replicaRole,
            CancellationToken cancellationToken)
        {
            ServiceTrace.Source.WriteInfoWithId(
                TraceType,
                this.traceId,
                "Opening communication listeners - New role : {0}",
                replicaRole);

            if (this.replicaListeners == null)
            {
                this.replicaListeners = this.userServiceReplica.CreateServiceReplicaListeners();
            }

            var endpointsCollection = new ServiceEndpointCollection();
            var listenerOpenedCount = 0;

            foreach (var entry in this.replicaListeners)
            {
                string traceMsg;

                if (entry is null)
                {
                    traceMsg = "Skipped (<null>) replica listener.";
                    ServiceTrace.Source.WriteInfoWithId(TraceType, this.traceId, traceMsg);

                    continue;
                }

                if (replicaRole == ReplicaRole.Primary ||
                    (replicaRole == ReplicaRole.ActiveSecondary && entry.ListenOnSecondary))
                {
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
                "Closed communication listeners..");
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
