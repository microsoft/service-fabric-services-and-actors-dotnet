// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Data.Notifications;

    internal class MockReliableDictionary<TKey, TValue> : IReliableDictionary2<TKey, TValue>
        where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        private Dictionary<TKey, TValue> kvPairs;

        public MockReliableDictionary()
        {
            this.kvPairs = new Dictionary<TKey, TValue>();
        }

        event EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>> IReliableDictionary<TKey, TValue>.DictionaryChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        long IReliableDictionary2<TKey, TValue>.Count { get => this.kvPairs.Count; }

        Func<IReliableDictionary<TKey, TValue>, NotifyDictionaryRebuildEventArgs<TKey, TValue>, Task> IReliableDictionary<TKey, TValue>.RebuildNotificationAsyncCallback { set => throw new NotImplementedException(); }

        Uri IReliableState.Name { get => throw new NotImplementedException(); }

        Task IReliableDictionary<TKey, TValue>.AddAsync(ITransaction tx, TKey key, TValue value)
        {
            return Task.Run(() => this.kvPairs.Add(key, value));
        }

        Task IReliableDictionary<TKey, TValue>.AddAsync(ITransaction tx, TKey key, TValue value, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.Run(() => this.kvPairs.Add(key, value));
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return Task.Run(() =>
            {
                if (this.kvPairs.ContainsKey(key))
                {
                    this.kvPairs[key] = updateValueFactory.Invoke(key, this.kvPairs[key]);
                }
                else
                {
                    this.kvPairs.Add(key, addValueFactory.Invoke(key));
                }

                return this.kvPairs[key];
            });
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return Task.Run(() =>
            {
                if (this.kvPairs.ContainsKey(key))
                {
                    this.kvPairs[key] = updateValueFactory.Invoke(key, this.kvPairs[key]);
                }
                else
                {
                    this.kvPairs.Add(key, addValue);
                }

                return this.kvPairs[key];
            });
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (this.kvPairs.ContainsKey(key))
                {
                    this.kvPairs[key] = updateValueFactory.Invoke(key, this.kvPairs[key]);
                }
                else
                {
                    this.kvPairs.Add(key, addValueFactory.Invoke(key));
                }

                return this.kvPairs[key];
            });
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.AddOrUpdateAsync(ITransaction tx, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (this.kvPairs.ContainsKey(key))
                {
                    this.kvPairs[key] = updateValueFactory.Invoke(key, this.kvPairs[key]);
                }
                else
                {
                    this.kvPairs.Add(key, addValue);
                }

                return this.kvPairs[key];
            });
        }

        Task IReliableDictionary<TKey, TValue>.ClearAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            this.kvPairs.Clear();
            return Task.CompletedTask;
        }

        Task IReliableCollection<KeyValuePair<TKey, TValue>>.ClearAsync()
        {
            this.kvPairs.Clear();
            return Task.CompletedTask;
        }

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key)
        {
            return Task.FromResult(this.kvPairs.ContainsKey(key));
        }

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key, LockMode lockMode)
        {
            return Task.FromResult(this.kvPairs.ContainsKey(key));
        }

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.kvPairs.ContainsKey(key));
        }

        Task<bool> IReliableDictionary<TKey, TValue>.ContainsKeyAsync(ITransaction tx, TKey key, LockMode lockMode, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.kvPairs.ContainsKey(key));
        }

        Task<Data.IAsyncEnumerable<KeyValuePair<TKey, TValue>>> IReliableDictionary<TKey, TValue>.CreateEnumerableAsync(ITransaction txn)
        {
            throw new NotImplementedException();
        }

        Task<Data.IAsyncEnumerable<KeyValuePair<TKey, TValue>>> IReliableDictionary<TKey, TValue>.CreateEnumerableAsync(ITransaction txn, EnumerationMode enumerationMode)
        {
            throw new NotImplementedException();
        }

        Task<Data.IAsyncEnumerable<KeyValuePair<TKey, TValue>>> IReliableDictionary<TKey, TValue>.CreateEnumerableAsync(ITransaction txn, Func<TKey, bool> filter, EnumerationMode enumerationMode)
        {
            throw new NotImplementedException();
        }

        Task<Data.IAsyncEnumerable<TKey>> IReliableDictionary2<TKey, TValue>.CreateKeyEnumerableAsync(ITransaction txn)
        {
            throw new NotImplementedException();
        }

        Task<Data.IAsyncEnumerable<TKey>> IReliableDictionary2<TKey, TValue>.CreateKeyEnumerableAsync(ITransaction txn, EnumerationMode enumerationMode)
        {
            throw new NotImplementedException();
        }

        Task<Data.IAsyncEnumerable<TKey>> IReliableDictionary2<TKey, TValue>.CreateKeyEnumerableAsync(ITransaction txn, EnumerationMode enumerationMode, TimeSpan timeout, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<long> IReliableCollection<KeyValuePair<TKey, TValue>>.GetCountAsync(ITransaction tx)
        {
           return Task.FromResult((long)this.kvPairs.Count);
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!this.kvPairs.ContainsKey(key))
            {
                this.kvPairs.Add(key, valueFactory.Invoke(key));
            }

            return Task.FromResult(this.kvPairs[key]);
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key, TValue value)
        {
            if (!this.kvPairs.ContainsKey(key))
            {
                this.kvPairs.Add(key, value);
            }

            return Task.FromResult(this.kvPairs[key]);
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key, Func<TKey, TValue> valueFactory, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (!this.kvPairs.ContainsKey(key))
            {
                this.kvPairs.Add(key, valueFactory.Invoke(key));
            }

            return Task.FromResult(this.kvPairs[key]);
        }

        Task<TValue> IReliableDictionary<TKey, TValue>.GetOrAddAsync(ITransaction tx, TKey key, TValue value, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (!this.kvPairs.ContainsKey(key))
            {
                this.kvPairs.Add(key, value);
            }

            return Task.FromResult(this.kvPairs[key]);
        }

        Task IReliableDictionary<TKey, TValue>.SetAsync(ITransaction tx, TKey key, TValue value)
        {
            this.kvPairs.Add(key, value);
            return Task.CompletedTask;
        }

        Task IReliableDictionary<TKey, TValue>.SetAsync(ITransaction tx, TKey key, TValue value, TimeSpan timeout, CancellationToken cancellationToken)
        {
            this.kvPairs.Add(key, value);
            return Task.CompletedTask;
        }

        Task<bool> IReliableDictionary<TKey, TValue>.TryAddAsync(ITransaction tx, TKey key, TValue value)
        {
            return Task.FromResult(this.kvPairs.TryAdd(key, value));
        }

        Task<bool> IReliableDictionary<TKey, TValue>.TryAddAsync(ITransaction tx, TKey key, TValue value, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.kvPairs.TryAdd(key, value));
        }

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key)
        {
            if (this.kvPairs.TryGetValue(key, out var value))
            {
                return Task.FromResult(new ConditionalValue<TValue>(true, value));
            }

            return Task.FromResult(new ConditionalValue<TValue>(false, default(TValue)));
        }

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key, LockMode lockMode)
        {
            if (this.kvPairs.TryGetValue(key, out var value))
            {
                return Task.FromResult(new ConditionalValue<TValue>(true, value));
            }

            return Task.FromResult(new ConditionalValue<TValue>(false, default(TValue)));
        }

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (this.kvPairs.TryGetValue(key, out var value))
            {
                return Task.FromResult(new ConditionalValue<TValue>(true, value));
            }

            return Task.FromResult(new ConditionalValue<TValue>(false, default(TValue)));
        }

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryGetValueAsync(ITransaction tx, TKey key, LockMode lockMode, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (this.kvPairs.TryGetValue(key, out var value))
            {
                return Task.FromResult(new ConditionalValue<TValue>(true, value));
            }

            return Task.FromResult(new ConditionalValue<TValue>(false, default(TValue)));
        }

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryRemoveAsync(ITransaction tx, TKey key)
        {
            if (this.kvPairs.Remove(key, out var value))
            {
                return Task.FromResult(new ConditionalValue<TValue>(true, value));
            }

            return Task.FromResult(new ConditionalValue<TValue>(false, default(TValue)));
        }

        Task<ConditionalValue<TValue>> IReliableDictionary<TKey, TValue>.TryRemoveAsync(ITransaction tx, TKey key, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (this.kvPairs.Remove(key, out var value))
            {
                return Task.FromResult(new ConditionalValue<TValue>(true, value));
            }

            return Task.FromResult(new ConditionalValue<TValue>(false, default(TValue)));
        }

        Task<bool> IReliableDictionary<TKey, TValue>.TryUpdateAsync(ITransaction tx, TKey key, TValue newValue, TValue comparisonValue)
        {
            if (this.kvPairs.TryGetValue(key, out var value))
            {
                if (comparisonValue.Equals(value))
                {
                    this.kvPairs[key] = newValue;

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        Task<bool> IReliableDictionary<TKey, TValue>.TryUpdateAsync(ITransaction tx, TKey key, TValue newValue, TValue comparisonValue, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (this.kvPairs.TryGetValue(key, out var value))
            {
                if (comparisonValue.Equals(value))
                {
                    this.kvPairs[key] = newValue;

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
