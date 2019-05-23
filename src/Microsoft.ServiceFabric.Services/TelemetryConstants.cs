// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    internal static class TelemetryConstants
    {
        internal static readonly string Undefined = "undefined";
        internal static readonly string ClusterTypeStandalone = "standalone";
        internal static readonly string ClusterTypeSfrp = "SFRP";
        internal static readonly string ClusterTypePaasV1 = "PaasV1";
        internal static readonly string ClusterOSWindows = "Windows";
        internal static readonly string ClusterOSLinux = "Linux";
        internal static readonly string DotNetStandard = "DotNetStandard";
        internal static readonly string DotNetFramework = "DotNetFramework";
        internal static readonly string LifecycleEventOpened = "Opened";
        internal static readonly string LifecycleEventClosed = "Closed";
        internal static readonly string ActorServiceType = "Microsoft.ServiceFabric.Actors.Runtime.ActorService";
        internal static readonly string ServicesRemotingType = "Microsoft.ServiceFabric.Services.Remoting";
        internal static readonly string StatefulServiceKind = "StatefulService";
        internal static readonly string StatelessServiceKind = "StatelessService";
        internal static readonly string ActorServiceKind = "ActorService";
        internal static readonly string NullActorStateProvider = "NullActorStateProvider";
        internal static readonly string KvsActorStateProvider = "KvsActorStateProvider";
        internal static readonly string ReliableCollectionsActorStateProvider = "ReliableCollectionsActorStateProvider";
        internal static readonly string VolatileActorStateProvider = "VolatileActorStateProvider";
        internal static readonly string SecureRemoting = "Secure";
        internal static readonly string UnsecureRemoting = "Unsecure";
        internal static readonly string RemotingVersionV1 = "V1";
        internal static readonly string RemotingVersionV2 = "V2";
        internal static readonly string FabricTransportCommunicationListener = "FabricTransport";
        internal static readonly string WCFCommunicationListener = "WCF";
        internal static readonly string ASPNetCoreCommunicationListener = "ASP.NET Core";
        internal static readonly string ServiceLifecycleEventName = "TelemetryEvents.ServiceLifecycleEvent";
        internal static readonly string ActorStateProviderUsageEventName = "TelemetryEvents.ActorStateProviderUsageEvent";
        internal static readonly string ServiceRemotingUsageEventName = "TelemetryEvents.ServiceRemotingUsageEvent";
        internal static readonly string CommunicationListenerUsageEventName = "TelemetryEvents.CommunicationListenerUsageEvent";
        internal static readonly string CustomCommunicationClientUsageEventName = "TelemetryEvents.CustomCommunicationClientUsageEvent";
        internal static readonly string CustomActorServiceUsageEventName = "TelemetryEvents.CustomActorServiceUsageEvent";
        internal static readonly string ActorReminderRegisterationEventName = "TelemetryEvents.ActorReminderRegisterationEvent";
        internal static readonly bool RemotingIsUnsecure = false;

        internal static readonly string OsType;
        internal static readonly string RuntimePlatform;

        static TelemetryConstants()
        {
#if DotNetCoreClr
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                OsType = TelemetryConstants.ClusterOSWindows;
            }
            else
            {
                OsType = TelemetryConstants.ClusterOSLinux;
            }

            RuntimePlatform = TelemetryConstants.DotNetStandard;
#else
            OsType = TelemetryConstants.ClusterOSWindows;
            RuntimePlatform = TelemetryConstants.DotNetFramework;
#endif
        }
    }
}
