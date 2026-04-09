// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Runtime.CompilerServices;
using Microsoft.ServiceFabric;
using Xunit;
using static Microsoft.ServiceFabric.Constants.AssemblyInfo;


// This attribute is used to run tests in the same assembly in sequence.
// It is necessary for ActorRemotingProviderAttributeTest suit to run properly,
// because ActorRemotingProviderAtrribut has static state and cannot be teste in parallel.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
[assembly: CaptureConsole]

[assembly: AssemblyFixture(typeof(FabricTelemetryDllFixture))]
[assembly: AssemblyFixture(typeof(EventSourceFixture))]

[assembly: InternalsVisibleTo(DynamicProxyGenAssembly2)]
