// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

// Run tests sequentially to prevent failures in tests that depend on global state.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
[assembly: CaptureConsole]

// Mock calls to FabricTelemetry.dll to prevent failures relating to this
[assembly: AssemblyFixture(typeof(Microsoft.ServiceFabric.TestFramework.FabricTelemetryDllFixture))]
