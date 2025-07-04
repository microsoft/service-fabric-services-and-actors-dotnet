// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using Microsoft.ServiceFabric.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    // REMARKS:
    // When you apply EventAttribute attribute to an ETW event method defined on an EventSource-derived class,
    // you must call the WriteEvent method on the base class, passing the event ID, followed by the same
    // arguments as the defined method is passed. Details at:
    // https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventattribute(v=vs.110).aspx
    [EventSource(Name = "Microsoft-ServiceFabric-Services", LocalizationResources = "Microsoft.ServiceFabric.Services.SR", Guid = "13c2a97d-71da-5ab5-47cb-1497aec602e1")]
    sealed class ServiceFrameworkEventSource : ServiceFabricEventSource
    {
        internal static ServiceFrameworkEventSource Writer { get; private set; } = new ServiceFrameworkEventSource();

        [NonEvent]
        internal void StatefulRunAsyncInvocation(
            StatefulServiceContext serviceContext)
        {
            StatefulRunAsyncInvocation(
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.OriginalString,
                    serviceContext.PartitionId.ToString(),
                    serviceContext.ReplicaId);
        }

        [NonEvent]
        internal void StatefulRunAsyncCancellation(
            StatefulServiceContext serviceContext,
            TimeSpan slowCancellationTimeMillis)
        {
            StatefulRunAsyncCancellation(
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.OriginalString,
                    serviceContext.PartitionId.ToString(),
                    serviceContext.ReplicaId,
                    slowCancellationTimeMillis.TotalMilliseconds);
        }

        [NonEvent]
        internal void StatefulRunAsyncCompletion(
            StatefulServiceContext serviceContext,
            bool wasCanceled)
        {
            StatefulRunAsyncCompletion(
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.OriginalString,
                    serviceContext.PartitionId.ToString(),
                    serviceContext.ReplicaId,
                    wasCanceled);
        }

        [NonEvent]
        internal void StatefulRunAsyncSlowCancellation(
            StatefulServiceContext serviceContext,
            TimeSpan actualCancellationTimeMillis,
            TimeSpan slowCancellationTimeMillis)
        {
            StatefulRunAsyncSlowCancellation(
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.ServiceTypeName,
                    serviceContext.ServiceName.OriginalString,
                    serviceContext.PartitionId.ToString(),
                    serviceContext.ReplicaId,
                    actualCancellationTimeMillis.TotalMilliseconds,
                    slowCancellationTimeMillis.TotalMilliseconds);
        }

        [NonEvent]
        internal void StatefulRunAsyncFailure(
            StatefulServiceContext serviceContext,
            bool wasCanceled,
            Exception exception)
        {
            StatefulRunAsyncFailure(
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.ServiceName.OriginalString,
                serviceContext.PartitionId.ToString(),
                serviceContext.ReplicaId,
                wasCanceled,
                exception.ToString());
        }

        [NonEvent]
        internal void StatelessRunAsyncInvocation(
            StatelessServiceContext serviceContext)
        {
            StatelessRunAsyncInvocation(
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.ServiceName.OriginalString,
                serviceContext.PartitionId.ToString(),
                serviceContext.InstanceId);
        }

        [NonEvent]
        internal void StatelessRunAsyncCancellation(
            StatelessServiceContext serviceContext,
            TimeSpan slowCancellationTimeMillis)
        {
            StatelessRunAsyncCancellation(
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.ServiceName.OriginalString,
                serviceContext.PartitionId.ToString(),
                serviceContext.InstanceId,
                slowCancellationTimeMillis.TotalMilliseconds);
        }

        [NonEvent]
        internal void StatelessRunAsyncCompletion(
            StatelessServiceContext serviceContext,
            bool wasCanceled)
        {
            StatelessRunAsyncCompletion(
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.ServiceName.OriginalString,
                serviceContext.PartitionId.ToString(),
                serviceContext.InstanceId,
                wasCanceled);
        }

        [NonEvent]
        internal void StatelessRunAsyncSlowCancellation(
            StatelessServiceContext serviceContext,
            TimeSpan actualCancellationTimeMillis,
            TimeSpan slowCancellationTimeMillis)
        {
            StatelessRunAsyncSlowCancellation(
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.ServiceName.OriginalString,
                serviceContext.PartitionId.ToString(),
                serviceContext.InstanceId,
                actualCancellationTimeMillis.TotalMilliseconds,
                slowCancellationTimeMillis.TotalMilliseconds);
        }

        [NonEvent]
        internal void StatelessRunAsyncFailure(
            StatelessServiceContext serviceContext,
            bool wasCanceled,
            Exception exception)
        {
            StatelessRunAsyncFailure(
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.ServiceTypeName,
                serviceContext.ServiceName.OriginalString,
                serviceContext.PartitionId.ToString(),
                serviceContext.InstanceId,
                wasCanceled,
                exception.ToString());
        }

        [Event(1, Level = EventLevel.Informational)]
        private void StatefulRunAsyncInvocation(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long replicaId)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    1,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaId);
            }
        }

        [Event(2, Level = EventLevel.Informational)]
        private void StatefulRunAsyncCancellation(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long replicaId,
            double slowCancellationTimeMillis)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    2,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaId,
                    slowCancellationTimeMillis);
            }
        }

        [Event(3, Level = EventLevel.Informational)]
        private void StatefulRunAsyncCompletion(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long replicaId,
            bool wasCanceled)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    3,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaId,
                    wasCanceled);
            }
        }

        [Event(4, Level = EventLevel.Warning)]
        private void StatefulRunAsyncSlowCancellation(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long replicaId,
            double actualCancellationTimeMillis,
            double slowCancellationTimeMillis)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    4,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaId,
                    actualCancellationTimeMillis,
                    slowCancellationTimeMillis);
            }
        }

        [Event(5, Level = EventLevel.Error)]
        private void StatefulRunAsyncFailure(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long replicaId,
            bool wasCanceled,
            string exception)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    5,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaId,
                    wasCanceled,
                    exception);
            }
        }

        [Event(6, Level = EventLevel.Informational)]
        private void StatelessRunAsyncInvocation(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long instanceId)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    6,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    instanceId);
            }
        }

        [Event(7, Level = EventLevel.Informational)]
        private void StatelessRunAsyncCancellation(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long instanceId,
            double slowCancellationTimeMillis)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    7,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    instanceId,
                    slowCancellationTimeMillis);
            }
        }

        [Event(8, Level = EventLevel.Informational)]
        private void StatelessRunAsyncCompletion(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long instanceId,
            bool wasCanceled)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    8,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    instanceId,
                    wasCanceled);
            }
        }

        [Event(9, Level = EventLevel.Warning)]
        private void StatelessRunAsyncSlowCancellation(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long instanceId,
            double actualCancellationTimeMillis,
            double slowCancellationTimeMillis)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    9,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    instanceId,
                    actualCancellationTimeMillis,
                    slowCancellationTimeMillis);
            }
        }

        [Event(10, Level = EventLevel.Error)]
        private void StatelessRunAsyncFailure(
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            string partitionId,
            long instanceId,
            bool wasCanceled,
            string exception)
        {
            if (IsEnabled())
            {
                WriteEvent(
                    10,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    instanceId,
                    wasCanceled,
                    exception);
            }
        }
    }
}
