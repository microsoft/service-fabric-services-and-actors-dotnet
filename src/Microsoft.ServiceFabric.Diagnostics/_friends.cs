// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("ServiceFabricTracingStandaloneTest" + TestKey)]
[assembly: InternalsVisibleTo("ServiceFabricTracingTest" + TestKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services" + PublicKey)]
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
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]

// Making internals visible for assemblies using Metrics
[assembly: InternalsVisibleTo("System.Fabric.BackupRestore" + PublicKey)]
[assembly: InternalsVisibleTo("System.Fabric.BackupRestore" + TestKey)]
[assembly: InternalsVisibleTo("FabricBRS" + PublicKey)]
[assembly: InternalsVisibleTo("FabricBRS" + TestKey)]
[assembly: InternalsVisibleTo("BackupCopier" + PublicKey)]
[assembly: InternalsVisibleTo("BackupCopier" + TestKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Impl" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Impl" + TestKey)]
// Making internals visible to assemblies for OpenSource
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors,PublicKey=0024000004800000940000000602000000240000525341310004000001000100410770985914c6dc72122c5c3b04ff2bd713f6f3b6457b864c23e5f5d79f2d36037ee37e3af1d23fd3c5284c28f4f946fb25e2a6b4f2764efa4f7864a145fca655f5fdb3f78c9e0d851c38dc2b9bfa58e364b48a611fe11c2c9d51b15dc2344fd2d927079d27398939932024fe956923904a0b2cf79ca4242246787ca509cbed")]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services,PublicKey=0024000004800000940000000602000000240000525341310004000001000100410770985914c6dc72122c5c3b04ff2bd713f6f3b6457b864c23e5f5d79f2d36037ee37e3af1d23fd3c5284c28f4f946fb25e2a6b4f2764efa4f7864a145fca655f5fdb3f78c9e0d851c38dc2b9bfa58e364b48a611fe11c2c9d51b15dc2344fd2d927079d27398939932024fe956923904a0b2cf79ca4242246787ca509cbed")]
