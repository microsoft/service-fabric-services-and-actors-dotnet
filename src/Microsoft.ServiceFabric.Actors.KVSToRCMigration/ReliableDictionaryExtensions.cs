// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data.Collections;

    internal static class ReliableDictionaryExtensions
    {
        public static async Task<string> GetAsync(
            this IReliableDictionary2<string, string> dict,
            Data.ITransaction tx,
            string key,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var result = await dict.TryGetValueAsync(tx, key, timeout, cancellationToken);
            if (result.HasValue)
            {
                return result.Value;
            }

            throw new KeyNotFoundException(key.ToString()); // TODO consider throwing SF exception.
        }

        public static async Task<string> GetValueOrDefaultAsync(
            this IReliableDictionary2<string, string> dict,
            Data.ITransaction tx,
            string key,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var result = await dict.TryGetValueAsync(tx, key, timeout, cancellationToken);
            if (result.HasValue)
            {
                return result.Value;
            }

            return null;
        }
    }
}
