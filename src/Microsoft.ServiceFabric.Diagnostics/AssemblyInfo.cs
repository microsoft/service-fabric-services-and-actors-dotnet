// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("ServiceFabricTracingStandaloneTest" + TestKey)]
[assembly: InternalsVisibleTo("ServiceFabricTracingTest" + TestKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.KVSToRCMigration" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Remoting" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Wcf" + PublicKey)]
[assembly: InternalsVisibleTo("CoordinatorService" + PublicKey)]
[assembly: InternalsVisibleTo("NodeAgentSFUtilityDll" + PublicKey)]
[assembly: InternalsVisibleTo("NodeAgentSFUtility" + PublicKey)]
[assembly: InternalsVisibleTo("NodeAgentWorkerLib" + PublicKey)]
[assembly: InternalsVisibleTo("NodeAgentService" + PublicKey)]
[assembly: InternalsVisibleTo("CoordinatorServiceTest" + TestKey)]
[assembly: InternalsVisibleTo("NodeAgentSFUtilityTest" + TestKey)]
[assembly: InternalsVisibleTo("TelemetryLibTest" + TestKey)]
[assembly: InternalsVisibleTo("EventsValidationTest" + PublicKey)]
[assembly: InternalsVisibleTo("AzureFilesVolumePlugin" + PublicKey)]
[assembly: InternalsVisibleTo("AzureFilesVolumePluginSetup" + PublicKey)]
[assembly: InternalsVisibleTo("SFVolumeDiskDebugAgentCommon" + PublicKey)]
[assembly: InternalsVisibleTo("NodeAgentServiceSetup" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Diagnostics.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.TestFramework" + PublicKey)]
[assembly: InternalsVisibleTo(DynamicProxyGenAssembly2)]

// Making internals visible for assemblies using Metrics
[assembly: InternalsVisibleTo("System.Fabric.BackupRestore" + PublicKey)]
[assembly: InternalsVisibleTo("System.Fabric.BackupRestore" + TestKey)]
[assembly: InternalsVisibleTo("FabricBRS" + PublicKey)]
[assembly: InternalsVisibleTo("FabricBRS" + TestKey)]
[assembly: InternalsVisibleTo("BackupCopier" + PublicKey)]
[assembly: InternalsVisibleTo("BackupCopier" + TestKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Impl" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Impl" + TestKey)]

[assembly: InternalsVisibleTo("FabricIS.parallel" + PublicKey)]
[assembly: InternalsVisibleTo("FabricIS.parallel" + TestKey)]
[assembly: InternalsVisibleTo("FabricInfrastructureManualControl" + PublicKey)]
[assembly: InternalsVisibleTo("FabricInfrastructureManualControl" + TestKey)]
[assembly: InternalsVisibleTo("FabricIS.parallel.Test" + PublicKey)]
[assembly: InternalsVisibleTo("FabricIS.parallel.Test" + TestKey)]
