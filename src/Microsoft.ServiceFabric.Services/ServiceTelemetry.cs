// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System.Fabric;

    /// <summary>
    /// ServiceTelemetry contains the telemetry methods for ServiceFramework.
    /// This class is to be used by other Service Fabric components only and
    /// is not meant for external use.
    /// </summary>
    public static class ServiceTelemetry
    {
        /// <summary>
        /// StatefulServiceInitalizeEvent captures the telemetry event of the initialization
        /// of a StatefulService replica.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/> is the service context.</param>
        internal static void StatefulServiceInitializeEvent(StatefulServiceContext context)
        {
            StatefulServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventOpened);
        }

        /// <summary>
        /// StatefulServiceReplicaCloseEvent captures the telemetry event of the closing of
        /// a StatefulService replica.
        /// </summary>
        /// <param name="context"><see cref="StatefulServiceContext"/> is the service context.</param>
        internal static void StatefulServiceReplicaCloseEvent(StatefulServiceContext context)
        {
            StatefulServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventClosed);
        }

        /// <summary>
        /// StatelessServiceInitializeEvent captures the telemetry event of the initialization
        /// of a StatelessService instance.
        /// </summary>
        /// <param name="context"><see cref="StatelessServiceContext"/> is the service context.</param>
        internal static void StatelessServiceInitializeEvent(StatelessServiceContext context)
        {
            StatelessServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventOpened);
        }

        /// <summary>
        /// StatelessServiceInstanceCloseEvent captures the telemetry event of the closing of
        /// a StatelessService instance.
        /// </summary>
        /// <param name="context"><see cref="StatelessServiceContext"/> is the service context.</param>
        internal static void StatelessServiceInstanceCloseEvent(StatelessServiceContext context)
        {
            StatelessServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventClosed);
        }

        /// <summary>
        /// CommunicationListenerUsageEvent captures the telemetry event of the usage of
        /// ICommunicationListener.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        /// <param name="communicationListenerType">The name of the type which implements ICommunicationListener.</param>
        internal static void CommunicationListenerUsageEvent(ServiceContext context, string communicationListenerType)
        {
            ServiceEventSource.Instance.CommunicationListenerUsageEventWrapper(
                TelemetryConstants.CommunicationListenerUsageEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaOrInstanceId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName.ToString(),
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                communicationListenerType);
        }

        /// <summary>
        /// FabricTransportServiceRemotingV1Event captures the telemetry of the usage of V1 FabricTransport
        /// remoting.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        /// <param name="isSecure">Captures if the remoting is secure or unsecure.</param>
        /// <param name="exceptionSerializationTechnique">Exception serailization techinique used.</param>
        internal static void FabricTransportServiceRemotingV1Event(ServiceContext context, bool isSecure, string exceptionSerializationTechnique)
        {
            FabricTransportServiceRemotingEvent(context, TelemetryConstants.RemotingVersionV1, isSecure, exceptionSerializationTechnique);
        }

        /// <summary>
        /// FabricTransportServiceRemotingV1Event captures the telemetry of the usage of V2 FabricTransport
        /// remoting.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        /// <param name="isSecure">Captures if the remoting is secure or unsecure.</param>
        /// <param name="exceptionSerializationTechnique">Exception serailization techinique used.</param>
        internal static void FabricTransportServiceRemotingV2Event(ServiceContext context, bool isSecure, string exceptionSerializationTechnique)
        {
            FabricTransportServiceRemotingEvent(context, TelemetryConstants.RemotingVersionV2, isSecure, exceptionSerializationTechnique);
        }

        private static void StatefulServiceLifecycleEvent(StatefulServiceContext context, string lifecycleEvent)
        {
            ServiceEventSource.Instance.ServiceLifecycleEventWrapper(
                TelemetryConstants.ServiceLifecycleEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName.ToString(),
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                lifecycleEvent,
                TelemetryConstants.StatefulServiceKind);
        }

        private static void StatelessServiceLifecycleEvent(StatelessServiceContext context, string lifecycleEvent)
        {
            ServiceEventSource.Instance.ServiceLifecycleEventWrapper(
                TelemetryConstants.ServiceLifecycleEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.InstanceId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName.ToString(),
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                lifecycleEvent,
                TelemetryConstants.StatelessServiceKind);
        }

        private static void FabricTransportServiceRemotingEvent(ServiceContext context, string remotingVersion, bool isSecure, string exceptionSerializationTechnique)
        {
            ServiceEventSource.Instance.ServiceRemotingUsageEventWrapper(
                TelemetryConstants.ServiceRemotingUsageEventName,
                TelemetryConstants.OsType,
                TelemetryConstants.RuntimePlatform,
                context.PartitionId.ToString(),
                context.ReplicaOrInstanceId.ToString(),
                context.ServiceName.OriginalString,
                context.ServiceTypeName,
                context.CodePackageActivationContext.ApplicationName,
                context.CodePackageActivationContext.ApplicationTypeName,
                isSecure,
                remotingVersion,
                TelemetryConstants.FabricTransportCommunicationListener,
                exceptionSerializationTechnique);
        }
    }
}
