// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ServiceHelper
    {
        internal const string ApiStartTraceTypeSuffix = ".Api.Start";
        internal const string ApiFinishTraceTypeSuffix = ".Api.Finish";
        internal const string ApiErrorTraceTypeSuffix = ".Api.Error";
        internal const string ApiSlowTraceTypeSuffix = ".Api.Slow";

        internal static readonly TimeSpan RunAsyncExpectedCancellationTimeSpan = TimeSpan.FromSeconds(15);
        internal static readonly TimeSpan CommunicationListenerExpectedCloseTimeSpan = TimeSpan.FromSeconds(15);

        private const string RunAsyncHealthSourceId = "RunAsync";
        private const string CommunicationListenerHealthSourceId = "CommunicationListener";
        private const string RunAsyncHealthUnhandledExceptionProperty = "RunAsyncUnhandledException";
        private const string RunAsyncHealthSlowCanecellationProperty = "RunAsyncSlowCancellation";
        private const string CommunicationListenerSlowCloseProperty = "CommunicationListenerSlowClose";
        private const int MaxHealthDescriptionLength = (4 * 1024) - 1;

        private static readonly TimeSpan RunAsyncUnexpectedExceptionHealthInfoTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan RunAsyncSlowCancellationHealthInfoTtl = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan CommunicationListenerSlowCloseHealthInfoTtl = TimeSpan.FromMinutes(1);

        private readonly string traceType;
        private readonly string traceId;

        public ServiceHelper(string traceType, string traceId)
        {
            this.traceType = traceType;
            this.traceId = traceId;
        }

        internal static void ObserveExceptionIfAny(Task tsk)
        {
            Task.Run(async () =>
            {
                try
                {
                    await tsk.ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }
            });
        }

        internal void HandleRunAsyncUnexpectedFabricException(IServicePartition partition, FabricException fex)
        {
            ServiceTrace.Source.WriteErrorWithId(
                this.traceType + ApiErrorTraceTypeSuffix,
                this.traceId,
                "RunAsync failed due to an unhandled FabricException causing replica to fault: {0}",
                fex.ToString());

            this.ReportRunAsyncUnexpectedExceptionHealth(partition, fex);
            partition.ReportFault(FaultType.Transient);
        }

        internal void HandleRunAsyncUnexpectedException(IServicePartition partition, Exception ex)
        {
            // ReSharper disable once UseStringInterpolation
            var msg = $"RunAsync failed due to an unhandled exception causing the host process to crash: {ex}";

            ServiceTrace.Source.WriteErrorWithId(this.traceType + ApiErrorTraceTypeSuffix, this.traceId, msg);

            try
            {
                // Seen some instances when service is running with ResourceGovernance placing limit of 1 GB on the service process.
                // So before  we can FailFast on exception from user's RunAsync code, it can hit OOM while reporting health. Handle exception here from Health Reporting
                // so that FailFast can happen below.
                this.ReportRunAsyncUnexpectedExceptionHealth(partition, ex);
            }
            catch (Exception exception)
            {
                ServiceTrace.Source.WriteWarningWithId(this.traceType, this.traceId, $"Exception while reporting Health for Partition. {exception.ToString()}");
            }

            // In LRC test we have observed that sometimes FailFast takes time to write error
            // details to WER and bring down the service host. This causes delays in failover
            // and availibility loss to service.
            //
            // Report fault transient and post FailFast on another thread to unblock ChangeRole.
            // Service host will come down once FailFast completes.
            partition.ReportFault(FaultType.Transient);
            Task.Run(() => Environment.FailFast(msg));
        }

        internal async Task AwaitAsyncTaskWithHealthReporting(IServicePartition partition, Task taskToAwait, TimeSpan expectedCancellationTime, Action reportHealthFunc)
        {
            while (true)
            {
                var delayTaskCts = new CancellationTokenSource();
                var delayTask = Task.Delay(expectedCancellationTime, delayTaskCts.Token);

                var finishedTask = await Task.WhenAny(taskToAwait, delayTask);

                if (finishedTask == taskToAwait)
                {
                    delayTaskCts.Cancel();
                    ObserveExceptionIfAny(delayTask);

                    await taskToAwait;
                    break;
                }

                reportHealthFunc.Invoke();
            }
        }

        internal async Task AwaitCloseCommunicationListerWithHealthReporting(IServicePartition partition, Task closeCommunicationListenerTask, string communicationListenerName)
        {
            var expectedCloseTime = CommunicationListenerExpectedCloseTimeSpan;

            void HealthReportFunc()
            {
                var msg = $"Closing of {communicationListenerName} communication listener is taking longer than expected time ({expectedCloseTime.TotalSeconds}s).";
                ServiceTrace.Source.WriteWarningWithId(this.traceType + ApiSlowTraceTypeSuffix, this.traceId, msg);
                this.ReportCommunicationListenerSlowCloseHealth(partition, msg);
            }

            await this.AwaitAsyncTaskWithHealthReporting(partition, closeCommunicationListenerTask, expectedCloseTime, HealthReportFunc);
        }

        internal async Task AwaitRunAsyncWithHealthReporting(IServicePartition partition, Task runAsyncTask)
        {
            var expectedCancellationTime = RunAsyncExpectedCancellationTimeSpan;

            void HealthReportFunc()
            {
                var msg = $"RunAsync is taking longer than expected time ({expectedCancellationTime.TotalSeconds}s) to cancel.";
                ServiceTrace.Source.WriteWarningWithId(this.traceType + ApiSlowTraceTypeSuffix, this.traceId, msg);
                this.ReportRunAsyncSlowCancellationHealth(partition, msg);
            }

            await this.AwaitAsyncTaskWithHealthReporting(partition, runAsyncTask, expectedCancellationTime, HealthReportFunc);
        }

        private static string TrimToLength(string str, int length)
        {
            return (str.Length <= length) ? str : str.Substring(0, length);
        }

        private static HealthInformation GetRunAsyncUnexpectedExceptionHealthInformation(Exception e)
        {
            var healthDescription = e.ToString();

            // Trim the health description to maximum allowed size.
            healthDescription = TrimToLength(healthDescription, MaxHealthDescriptionLength);

            var healthInfo = new HealthInformation(RunAsyncHealthSourceId, RunAsyncHealthUnhandledExceptionProperty, HealthState.Warning)
            {
                TimeToLive = RunAsyncUnexpectedExceptionHealthInfoTtl,
                RemoveWhenExpired = true,
                Description = healthDescription,
            };

            return healthInfo;
        }

        private static HealthInformation GetRunAsyncSlowCancellationHealthInformation(string description)
        {
            var healthInfo = new HealthInformation(RunAsyncHealthSourceId, RunAsyncHealthSlowCanecellationProperty, HealthState.Warning)
            {
                TimeToLive = RunAsyncSlowCancellationHealthInfoTtl,
                RemoveWhenExpired = true,
                Description = TrimToLength(description, MaxHealthDescriptionLength),
            };

            return healthInfo;
        }

        private static HealthInformation GetCommunicationListenerSlowCloseHealthInformation(string description)
        {
            var healthInfo = new HealthInformation(CommunicationListenerHealthSourceId, CommunicationListenerSlowCloseProperty, HealthState.Warning)
            {
                TimeToLive = CommunicationListenerSlowCloseHealthInfoTtl,
                RemoveWhenExpired = true,
                Description = TrimToLength(description, MaxHealthDescriptionLength),
            };

            return healthInfo;
        }

        private void ReportPartitionHealth(IServicePartition partition, HealthInformation healthInformation)
        {
            try
            {
                partition.ReportPartitionHealth(healthInformation);
            }
            catch (Exception ex)
            {
                ServiceTrace.Source.WriteWarningWithId(
                    this.traceType,
                    this.traceId,
                    "ReportPartitionHealth() failed with: {0} while reporting health information: {1}.",
                    ex.ToString(),
                    healthInformation.ToString());
            }
        }

        private void ReportRunAsyncSlowCancellationHealth(IServicePartition partition, string description)
        {
            var healthInfo = GetRunAsyncSlowCancellationHealthInformation(description);
            this.ReportPartitionHealth(partition, healthInfo);
        }

        private void ReportRunAsyncUnexpectedExceptionHealth(IServicePartition partition, Exception unexpectedException)
        {
            var healthInfo = GetRunAsyncUnexpectedExceptionHealthInformation(unexpectedException);
            this.ReportPartitionHealth(partition, healthInfo);
        }

        private void ReportCommunicationListenerSlowCloseHealth(IServicePartition partition, string description)
        {
            var healthInfo = GetCommunicationListenerSlowCloseHealthInformation(description);
            this.ReportPartitionHealth(partition, healthInfo);
        }
    }
}
