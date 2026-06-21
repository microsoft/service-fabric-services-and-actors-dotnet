// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Client;

/// <summary>
/// Public actor interface used to exercise <see cref="ActorProxyFactory"/>. The dynamic assembly produced
/// by <c>ActorCodeBuilder</c> cannot access <c>internal</c> interfaces defined in this test assembly,
/// so the interface must be <c>public</c>.
/// </summary>
public interface IFactoryTestActor : IActor
{
}
