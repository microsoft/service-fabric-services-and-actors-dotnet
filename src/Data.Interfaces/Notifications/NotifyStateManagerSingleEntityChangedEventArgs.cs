// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Notifications
{
    using Microsoft.ServiceFabric.Data;

    /// <summary>
    /// Provides data for the <see cref="IReliableStateManager.StateManagerChanged"/> event caused by a transactional single entity operation.
    /// </summary>
    /// <remarks>
    /// Raised when a single <see cref="IReliableState"/> provider is added to or removed from the State Manager within a transaction.
    /// </remarks>
    public class NotifyStateManagerSingleEntityChangedEventArgs : NotifyStateManagerChangedEventArgs
    {
        private readonly ITransaction transaction;
        private readonly IReliableState reliableState;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyStateManagerSingleEntityChangedEventArgs"/> class.
        /// </summary>
        /// <param name="transaction">The transaction that the change is related to.</param>
        /// <param name="reliableState">The reliable state that was changed.</param>
        /// <param name="action">One of the enumeration values that specifies the action that caused the event.</param>
        public NotifyStateManagerSingleEntityChangedEventArgs(
            ITransaction transaction,
            IReliableState reliableState,
            NotifyStateManagerChangedAction action) : base(action)
        {
            this.transaction = transaction;
            this.reliableState = reliableState;
        }

        /// <summary>
        /// Gets the <see cref="ITransaction"/> within which the change occurred.
        /// </summary>
        public ITransaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }

        /// <summary>
        /// Gets the <see cref="IReliableState"/> that was added or removed.
        /// </summary>
        public IReliableState ReliableState
        {
            get
            {
                return this.reliableState;
            }
        }
    }
}
