// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    /// <summary>
    /// The class is used by actor remoting code generator to generate a type that dispatches requests to actor
    /// object by invoking right method on it.
    /// </summary>
    public abstract class ActorMethodDispatcherBase : Services.Remoting.V2.Builder.MethodDispatcherBase
    {
    }
}
