// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Remoting" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.FabricTransport.V2" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.FabricTransport.Test" + PublicKey)]
