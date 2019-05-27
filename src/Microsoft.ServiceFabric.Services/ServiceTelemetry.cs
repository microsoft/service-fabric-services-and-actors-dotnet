// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System.Fabric;

    /// <summary>
    /// ServiceTelemetry contains the telemetry methods for ServiceFramework.
    /// </summary>
    internal static class ServiceTelemetry
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
        /// WCFCommunicationListenerEvent captures the telemetry event of the usage of
        /// WCFCommunicationListener as a communication listener.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        internal static void WCFCommunicationListenerEvent(ServiceContext context)
        {
            CommunicationListenerUsageEvent(context, TelemetryConstants.WCFCommunicationListener);
        }

        /// <summary>
        /// ASPNETCoreCommunicationListenerEvent captures the telemetry event of the usage of
        /// ASPNETCoreCommunicationListener as a communication listener.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        internal static void ASPNETCoreCommunicationListenerEvent(ServiceContext context)
        {
            CommunicationListenerUsageEvent(context, TelemetryConstants.ASPNetCoreCommunicationListener);
        }

        /// <summary>
        /// FabricTransportServiceRemotingV1Event captures the telemetry of the usage of V1 FabricTransport
        /// remoting.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        /// <param name="isSecure">Captures if the remoting is secure or unsecure.</param>
        internal static void FabricTransportServiceRemotingV1Event(ServiceContext context, bool isSecure)
        {
            FabricTransportServiceRemotingEvent(context, TelemetryConstants.RemotingVersionV1, isSecure);
        }

        /// <summary>
        /// FabricTransportServiceRemotingV1Event captures the telemetry of the usage of V2 FabricTransport
        /// remoting.
        /// </summary>
        /// <param name="context"><see cref="ServiceContext"/> is the service context.</param>
        /// <param name="isSecure">Captures if the remoting is secure or unsecure.</param>
        internal static void FabricTransportServiceRemotingV2Event(ServiceContext context, bool isSecure)
        {
            FabricTransportServiceRemotingEvent(context, TelemetryConstants.RemotingVersionV2, isSecure);
        }

        /// <summary>
        /// CustomCommunicationClientUsageEvent captures the telemetry event of the usage of a
        /// custom CommunicationClient by clients.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="customCommunicationClientFactoryType">The custom CommunicationClientFactory type.</param>
        /// <param name="partitionKey">PartitionKey of the service partition.</param>
        internal static void CustomCommunicationClientUsageEvent(
            string serviceName,
            string customCommunicationClientFactoryType,
            string partitionKey)
        {
            if (!customCommunicationClientFactoryType.Contains(TelemetryConstants.ServicesRemotingType))
            {
                ServiceEventSource.Instance.CustomCommunicationClientUsageEventWrapper(
                    TelemetryConstants.CustomCommunicationClientUsageEventName,
                    TelemetryConstants.OsType,
                    TelemetryConstants.RuntimePlatform,
                    serviceName,
                    customCommunicationClientFactoryType,
                    partitionKey);
            }
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

        private static void CommunicationListenerUsageEvent(ServiceContext context, string communicationListenerType)
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

        private static void FabricTransportServiceRemotingEvent(ServiceContext context, string remotingVersion, bool isSecure)
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
                TelemetryConstants.FabricTransportCommunicationListener);
        }
    }
}
