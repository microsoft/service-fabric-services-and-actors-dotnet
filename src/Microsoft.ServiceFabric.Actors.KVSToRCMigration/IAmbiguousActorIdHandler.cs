// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// ActorId Handler.
    /// </summary>
    internal interface IAmbiguousActorIdHandler
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
        public class ConditionalValue
        {
            public string Value { get; set; }

            public bool HasValue { get; set; }
        }
    }
}
