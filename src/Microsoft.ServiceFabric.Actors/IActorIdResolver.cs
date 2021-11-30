// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors
{
    /// <summary>
    /// Service developer needs to define the resolver by implementing the following interface. Service developer can have multiple implementations of resolvers.
    /// </summary>
    public interface IActorIdResolver
    {
        /// <summary>
        /// Try Resolve Ambigious ActorId And StateName
        /// </summary>
        /// <param name="key">Key is of the form ActorId_StateName</param>
        /// <param name="actorId">Resolved ActorId</param>
        /// <param name="stateName">Resolved StateName</param>
        /// <returns>Ambigious ActorId and StaterName resolustion status</returns>
        bool TryResolveActorIdAndStateName(string key, out string actorId, out string stateName);

        /// <summary>
        /// Try Resolve Ambigious ActorId And ReminderName
        /// </summary>
        /// <param name="key">Key is of the form ActorId_ReminderName</param>
        /// <param name="actorId">Resolved ActorId</param>
        /// <param name="reminderName">Resolved ReminderName</param>
        /// <returns>Ambigious ActorId and ReminderName resolustion status</returns>
        bool TryResolveActorIdAndReminderName(string key, out string actorId, out string reminderName);
    }
}
