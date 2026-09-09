// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Inspector;

namespace Microsoft.ServiceFabric.FabricTransport;

/// <summary>
/// Base class for tests that need to set or reset the <see cref="FabricServiceConfig.GetConfig"/> singleton.
/// </summary>
public abstract class FabricServiceConfigAccessor: IDisposable
{
    internal FabricServiceConfigAccessor() => SetSingleton(null);
    public virtual void Dispose() => SetSingleton(null);
    internal void SetSingleton(FabricServiceConfig instance) => typeof(FabricServiceConfig).Field<FabricServiceConfig>().Set(instance);
}
