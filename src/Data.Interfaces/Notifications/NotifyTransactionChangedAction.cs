// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data.Notifications
{
    /// <summary>
    /// Describes the action that caused the <see cref="IReliableStateManager.TransactionChanged"/> event.
    /// </summary>
    public enum NotifyTransactionChangedAction : int
    {
        /// <summary>
        /// Indicates that the transaction has been committed.
        /// </summary>
        Commit = 0,
    }
}