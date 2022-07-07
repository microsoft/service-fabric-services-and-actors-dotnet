// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    /// <summary>
    /// Interface to resolve actorId from storage key incase there is any ambiguity.
    /// The implementation should have a default constructor.
    /// </summary>
    public interface IActorIdResolver
    {
        /// <summary>
        /// Resolves actor id from storage key.
        /// </summary>
        /// <param name="key">Underscore delimited actor id and state name(Ex, MyActorId_MyStateName)</param>
        /// <param name="actorId">Resolved Actor id.</param>
        /// <returns>True if resolved, false otherwise.</returns>
        bool TryResolveActorIdAndStateName(string key, out string actorId);
    }
}
