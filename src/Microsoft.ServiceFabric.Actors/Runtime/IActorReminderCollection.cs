// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Collections.Generic;

    /// <summary>
    /// Captures the ActorReminderState for Actors.
    /// </summary>
    public interface IActorReminderCollection : IReadOnlyDictionary<ActorId, IReadOnlyCollection<IActorReminderState>>
    {
    }
}
