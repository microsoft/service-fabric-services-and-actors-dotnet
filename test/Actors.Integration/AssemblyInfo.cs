// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric;
using Xunit;

// Run tests sequentially to prevent failures in tests that depend on global state.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
[assembly: CaptureConsole]

[assembly: AssemblyFixture(typeof(FabricTelemetryDllFixture))]
[assembly: AssemblyFixture(typeof(EventSourceFixture))]
