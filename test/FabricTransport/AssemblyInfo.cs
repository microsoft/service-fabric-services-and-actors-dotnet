// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using Xunit;

// FabricServiceConfigTest mutates the process-wide static FabricServiceConfig.instance, so its nested test classes
// must not run in parallel with each other. CollectionPerAssembly serializes all tests in this assembly.
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
