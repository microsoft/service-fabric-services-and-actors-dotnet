// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;

[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Wcf" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Services.Wcf.Tests" + PublicKey)]
[assembly: InternalsVisibleTo("Microsoft.ServiceFabric.Actors.Wcf.Tests" + PublicKey)]