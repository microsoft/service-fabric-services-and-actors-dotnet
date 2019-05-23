// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System;
    using System.Fabric;

    internal static class ServiceTelemetry
    {
        internal static void CustomCommunicationClientUsageEvent(
            string serviceName,
            string customCommunicationClientFactoryType,
            string partitionKey)
        {
            // Custom Communication Client Factory Event
            // What about IServiceRemotingClientFactory
            if (!customCommunicationClientFactoryType.Contains(TelemetryConstants.ServicesRemotingType))
            {
                ServiceEventSource.Instance.CustomCommunicationClientUsageEvent(
                    TelemetryConstants.CustomCommunicationClientUsageEventName,
                    TelemetryConstants.OsType,
                    TelemetryConstants.RuntimePlatform,
                    serviceName,
                    customCommunicationClientFactoryType,
                    partitionKey);
            }
        }

        internal static void StatefulServiceReplicaPrimaryEvent(StatefulServiceContext context)
        {
            StatefulServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventOpened);
        }

        internal static void StatefulServiceReplicaCloseEvent(StatefulServiceContext context)
        {
            StatefulServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventClosed);
        }

        internal static void StatelessServiceInstanceOpenEvent(StatelessServiceContext context)
        {
            StatelessServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventOpened);
        }

        internal static void StatelessServiceInstanceCloseEvent(StatelessServiceContext context)
        {
            StatelessServiceLifecycleEvent(context, TelemetryConstants.LifecycleEventClosed);
        }

        internal static void WCFCommunicationListenerEvent(ServiceContext context)
        {
            CommunicationListenerUsageEvent(context, TelemetryConstants.WCFCommunicationListener);
        }

        internal static void ASPNETCoreCommunicationListenerEvent(ServiceContext context)
        {
            CommunicationListenerUsageEvent(context, TelemetryConstants.ASPNetCoreCommunicationListener);
        }

        internal static void FabricTransportServiceRemotingV1Event(ServiceContext context, bool isSecure)
        {
            FabricTransportServiceRemotingEvent(context, TelemetryConstants.RemotingVersionV1, isSecure);
        }

        internal static void FabricTransportServiceRemotingV2Event(ServiceContext context, bool isSecure)
        {
            FabricTransportServiceRemotingEvent(context, TelemetryConstants.RemotingVersionV2, isSecure);
        }

        private static void StatefulServiceLifecycleEvent(StatefulServiceContext context, string lifecycleEvent)
        {
            ServiceEventSource.Instance.ServiceLifecycleEvent(
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
            ServiceEventSource.Instance.ServiceLifecycleEvent(
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
            ServiceEventSource.Instance.CommunicationListenerUsageEvent(
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
            ServiceEventSource.Instance.ServiceRemotingUsageEvent(
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
