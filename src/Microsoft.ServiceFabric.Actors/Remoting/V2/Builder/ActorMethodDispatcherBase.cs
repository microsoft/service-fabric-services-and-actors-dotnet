// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    using Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder;

    /// <summary>
    /// The class is used by actor remoting code generator to generate a type that dispatches requests to actor
    /// object by invoking right method on it.
    /// </summary>
    public abstract class ActorMethodDispatcherBase : MethodDispatcherBase
    {
    }
}
