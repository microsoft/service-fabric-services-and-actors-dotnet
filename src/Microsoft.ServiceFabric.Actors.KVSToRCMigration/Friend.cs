// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("FabActUtil" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors" + PublicKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestKvsActorService" + TestKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestRcActorService" + TestKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestKvsActorService" + PublicKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestRcActorService" + PublicKey)]
[assembly: InternalsVisibleTo("FabActTest.ClientWorkload" + TestKey)]
[assembly: InternalsVisibleTo("FabActTest.ClientWorkload" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.StateMigration.Tests" + PublicKey)]
[assembly: InternalsVisibleTo(DynamicProxyGenAssembly2)]
