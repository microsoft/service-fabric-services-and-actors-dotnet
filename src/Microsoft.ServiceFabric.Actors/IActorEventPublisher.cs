// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// Represents publisher of actor events. Publishers of actor events must implement this interface
    /// </summary>
    public interface IActorEventPublisher
    {
    }

    /// <summary>
    /// Represents publisher of actor events. Publishers of actor events must implement this interface
    /// </summary>
    /// <typeparam name="TIActorEvents">Actor event interface type derived from <see cref="IActorEvents"/>.</typeparam>
    public interface IActorEventPublisher<TIActorEvents> : IActorEventPublisher where TIActorEvents : IActorEvents
    {
    }
}
