// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("FabActUtil" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Wcf" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Wcf" + PublicKey)]
[assembly: InternalsVisibleTo("HeaderTestActorService" + PublicKey)]
[assembly: InternalsVisibleTo("HeaderTestActorService" + TestKey)]
[assembly: InternalsVisibleTo("FabActTest.ClientWorkload" + TestKey)]
[assembly: InternalsVisibleTo("FabActTest.ClientWorkload" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Remoting.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.KVSToRCMigration" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Wcf.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Wcf.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Tests" + PublicKey)]
[assembly: InternalsVisibleTo(DynamicProxyGenAssembly2)]
