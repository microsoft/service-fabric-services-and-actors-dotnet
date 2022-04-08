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

        public static async Task<string> GetAsync(
            this IReliableDictionary2<string, string> dict,
            Func<Data.ITransaction> txFactory,
            string key,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            using (var tx = txFactory.Invoke())
            {
                return await GetAsync(dict, tx, key, timeout, cancellationToken);
            }
        }

        public static async Task UpdateAsync(
            this IReliableDictionary2<string, string> dict,
            Data.ITransaction tx,
            string key,
            string value,
            string comparisionValue,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            var result = await dict.TryUpdateAsync(tx, key, value, comparisionValue, timeout, cancellationToken);
            if (!result)
            {
                throw new Exception($"failed to update  Key: {key}, NewValue: {value}, ComparisonValue: {comparisionValue}"); // TODO: Fabric ex
            }
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

        public static async Task<string> GetValueOrDefaultAsync(
            this IReliableDictionary2<string, string> dict,
            Func<Data.ITransaction> txFactory,
            string key,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            using (var tx = txFactory.Invoke())
            {
                return await GetValueOrDefaultAsync(dict, tx, key, timeout, cancellationToken);
            }
        }
    }
}
