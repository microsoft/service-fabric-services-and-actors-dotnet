// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Impl" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Interfaces.Tests" + PublicKey)]

// fix by bug : 16505120 : only needed for internal settings that we will move to Data.Interfaces.V2.
#if NETFRAMEWORK
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Interfaces.V2" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Data.Interfaces.V2.Tests" + PublicKey)]
#endif
