// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Notifications
{
    using System;

    /// <summary>
    /// Provides data for the <see cref="IReliableStateManager.StateManagerChanged"/> event.
    /// </summary>
    public abstract class NotifyStateManagerChangedEventArgs : EventArgs
    {
        private readonly NotifyStateManagerChangedAction action;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyStateManagerChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">One of the enumeration values that specifies the action that caused the event.</param>
        public NotifyStateManagerChangedEventArgs(NotifyStateManagerChangedAction action)
        {
            this.action = action;
        }

        /// <summary>
        /// Gets the action that caused the event.
        /// </summary>
        public NotifyStateManagerChangedAction Action
        {
            get
            {
                return this.action;
            }
        }
    }
}
