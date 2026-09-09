// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Notifications
{
    using System;

    using Microsoft.ServiceFabric.Data;

    /// <summary>
    /// Provides data for the <see cref="IReliableStateManager.TransactionChanged"/> event.
    /// </summary>
    public class NotifyTransactionChangedEventArgs : EventArgs
    {
        private readonly NotifyTransactionChangedAction action;
        private readonly ITransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyTransactionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="transaction">The transaction that the change is related to.</param>
        /// <param name="action">One of the enumeration values that specifies the action that caused the event.</param>
        public NotifyTransactionChangedEventArgs(ITransaction transaction, NotifyTransactionChangedAction action)
        {
            this.action = action;
            this.transaction = transaction;
        }

        /// <summary>
        /// Gets the action that caused the event.
        /// </summary>
        public NotifyTransactionChangedAction Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITransaction"/> whose state change raised the event.
        /// </summary>
        public ITransaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }
    }
}
