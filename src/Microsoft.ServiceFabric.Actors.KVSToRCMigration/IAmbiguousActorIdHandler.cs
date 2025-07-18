// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;

    /// <summary>
    /// ActorId Handler.
    /// </summary>
    [Obsolete(Deprecated.KVSToRCMigrationApis)]
    public interface IAmbiguousActorIdHandler
    {
        /// <summary>
        /// Resolves actor id for a give storage key.
        /// </summary>
        /// <param name="stateStorageKey">State storage key.</param>
        /// <param name="cancellationToken">Token to cancel asynchronous operation.</param>
        /// <returns>Conditional value with resolved actor id.</returns>
        public Task<ConditionalValue> TryResolveActorIdAsync(string stateStorageKey, CancellationToken cancellationToken);

        /// <summary>
        /// Condtional Value.
        /// </summary>
        [Obsolete(Deprecated.KVSToRCMigrationApis)]
        public class ConditionalValue
        {
            /// <summary>
            /// Gets or sets the Value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the ConditionalValue.Value exists.
            /// </summary>
            public bool HasValue { get; set; }
        }
    }
}
