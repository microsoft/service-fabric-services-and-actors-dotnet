// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
[assembly: CaptureConsole]

#if NET
[assembly: AssemblyFixture(typeof(Microsoft.ServiceFabric.TestFramework.FabricTraceDllFixture))]
#endif
