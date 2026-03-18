// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("System.Fabric.Replicator" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Replicator" + PublicKey)]
[assembly: InternalsVisibleTo("System.Fabric.ReplicatedStore" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.ReplicatedStore" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Impl" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Extensions.V2" + PublicKey)]
[assembly: InternalsVisibleTo("System.Fabric.Common.Test" + TestKey)]
[assembly: InternalsVisibleTo("FabSrvStateManager.Test" + TestKey)]
[assembly: InternalsVisibleTo("System.Fabric.Replicator.Test" + TestKey)]
[assembly: InternalsVisibleTo("System.Fabric.ReplicatorStack.Test" + TestKey)]
[assembly: InternalsVisibleTo("System.Fabric.Store.Test" + TestKey)]
[assembly: InternalsVisibleTo("FabricFAS" + PublicKey)]
[assembly: InternalsVisibleTo("FabricUOS" + PublicKey)]
[assembly: InternalsVisibleTo("FabricBRS" + PublicKey)]
[assembly: InternalsVisibleTo("EventStore.Service" + PublicKey)]
