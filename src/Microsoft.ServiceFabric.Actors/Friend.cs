// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("FabActUtil" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Wcf" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Tests" + TestKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.IntegrationTests" + PublicKey)]
[assembly: InternalsVisibleTo("FabActTest.ClientWorkload" + TestKey)]
[assembly: InternalsVisibleTo("FabActTest.ClientWorkload" + PublicKey)]
[assembly: InternalsVisibleTo("BackupRestoreActorService" + TestKey)]
[assembly: InternalsVisibleTo("PresenceLoadDriverLib" + TestKey)]
[assembly: InternalsVisibleTo("EventsValidationTest" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.KVSToRCMigration" + PublicKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestKvsActorService" + TestKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestRcActorService" + TestKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestKvsActorService" + PublicKey)]
[assembly: InternalsVisibleTo("ActorMigrationTestRcActorService" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.StateMigration.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.StateMigration.Tests" + TestKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Wcf.Tests" + PublicKey)]
