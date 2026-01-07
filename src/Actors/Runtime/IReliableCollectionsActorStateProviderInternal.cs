// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    /// <summary>
    /// Internal method declartion for Reliable Collections actor state provider.
    /// </summary>
    internal interface IReliableCollectionsActorStateProviderInternal
    {
        /// <summary>
        /// Get the actor state provider helper utility.
        /// </summary>
        /// <returns>Actor state prvovider helper.</returns>
        ActorStateProviderHelper GetActorStateProviderHelper();

        /// <summary>
        /// Gets the reliable state manager.
        /// </summary>
        /// <returns>Reliable state manager.</returns>
        IReliableStateManagerReplica2 GetStateManager();

        /// <summary>
        /// Gets the logical time dictionary.
        /// </summary>
        /// <returns>Logical time dictionary.</returns>
        IReliableDictionary2<string, byte[]> GetLogicalTimeDictionary();

        /// <summary>
        /// Gets the actor presence dictionary.
        /// </summary>
        /// <returns>Actor presence dictionary.</returns>
        IReliableDictionary2<string, byte[]> GetActorPresenceDictionary();

        /// <summary>
        /// Gets the reminder completed dictionary.
        /// </summary>
        /// <returns>Reminder completed dictionary.</returns>
        IReliableDictionary2<string, byte[]> GetReminderCompletedDictionary();

        /// <summary>
        /// Gets the actor state dictionary for the given actor id.
        /// </summary>
        /// <param name="actorId">Actor Id.</param>
        /// <returns>Actor state dictionary.</returns>
        IReliableDictionary2<string, byte[]> GetActorStateDictionary(ActorId actorId);

        /// <summary>
        /// Gets the reminder dictionary for the given actor id.
        /// </summary>
        /// <param name="actorId">Actor Id.</param>
        /// <returns>Reminder dictionary.</returns>
        IReliableDictionary2<string, byte[]> GetReminderDictionary(ActorId actorId);
    }
}
