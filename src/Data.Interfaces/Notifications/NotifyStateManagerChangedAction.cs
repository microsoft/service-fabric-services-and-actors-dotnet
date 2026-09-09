// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Notifications
{
    /// <summary>
    /// Describes the action that caused the <see cref="IReliableStateManager.StateManagerChanged"/> event.
    /// </summary>
    public enum NotifyStateManagerChangedAction : int
    {
        /// <summary>
        /// Indicates that an <see cref="IReliableState"/> has been added to the state manager.
        /// </summary>
        Add = 0,

        /// <summary>
        /// Indicates that an <see cref="IReliableState"/> has been removed from the state manager.
        /// </summary>
        Remove = 1,

        /// <summary>
        /// Indicates that the entire state manager has been rebuilt, typically during recovery, restore, or end of copy.
        /// </summary>
        Rebuild = 2,
    }
}
