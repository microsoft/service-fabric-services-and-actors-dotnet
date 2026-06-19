// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Threading;
using Fuzzy;
using Inspector;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class CommunicationClientCacheTest : IDisposable
{
    readonly CommunicationClientCache<ICommunicationClient> sut;

    // Constructor parameters
    readonly string traceId = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    CommunicationClientCacheTest() =>
        sut = new CommunicationClientCache<ICommunicationClient>(traceId);

    void IDisposable.Dispose() =>
        sut.Dispose();

    public sealed class CacheCleanupTimerCallback : CommunicationClientCacheTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Cleanup is driven by a private Timer with a non-configurable interval.
        public void RemovesEntriesWhoseClientFailedValidation()
        {
            // The cleanup pipeline (CacheCleanupTimerCallback -> CleanupCacheEntries) flips IsInCache,
            // removes invalid entries, and prunes empty partition caches. It is wired to a private Timer
            // constructed in the SUT with a non-configurable interval and cannot be triggered through any
            // public API. Exercising this behavior would require either exposing the callback for testing
            // or injecting the Timer, both of which are SUT testability changes out of scope for the
            // initial test suite. Documented here so the limitation is discoverable from the test class.
            throw new NotImplementedException();
        }
    }

    public sealed class ClearClientCacheEntries : CommunicationClientCacheTest
    {
        // Method parameters
        readonly Guid partitionId = fuzzy.Guid();

        [Fact]
        public void DoesNotRemoveCachedEntriesForPartition() // Currently a no-op
        {
            ResolvedServiceEndpoint endpoint = MakeEndpoint();
            string listenerName = fuzzy.String();
            CommunicationClientCacheEntry<ICommunicationClient> added =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, null);

            sut.ClearClientCacheEntries(partitionId);

            Assert.True(sut.TryGetClientCacheEntry(partitionId, endpoint, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Same(added, cacheEntry);
        }

        [Fact]
        public void DoesNotThrowWhenPartitionDoesNotExist() =>
            sut.ClearClientCacheEntries(partitionId);
    }

    public sealed class Constructor : CommunicationClientCacheTest
    {
        [Fact]
        public void InitializesCacheWithoutEntries()
        {
            // No size/enumeration API. Verify emptiness via TryGetClientCacheEntry.
            Assert.False(sut.TryGetClientCacheEntry(fuzzy.Guid(), MakeEndpoint(), fuzzy.String(), out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Null(cacheEntry);
        }

        [Fact]
        public void DoesNotRestoreExecutionContextFlowWhenAlreadySuppressed()
        {
            using AsyncFlowControl flow = ExecutionContext.SuppressFlow();
            using var other = new CommunicationClientCache<ICommunicationClient>(traceId);
            Assert.True(ExecutionContext.IsFlowSuppressed());
        }

        [Fact]
        public void RestoresExecutionContextFlowAfterSuppressingIt()
        {
            Assert.False(ExecutionContext.IsFlowSuppressed());
            using var other = new CommunicationClientCache<ICommunicationClient>(traceId);
            Assert.False(ExecutionContext.IsFlowSuppressed());
        }
    }

    public sealed class Dispose : CommunicationClientCacheTest
    {
        [Fact]
        public void RemovesCachedEntries()
        {
            Guid partitionId = fuzzy.Guid();
            ResolvedServiceEndpoint endpoint = MakeEndpoint();
            string listenerName = fuzzy.String();
            _ = sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, null);

            sut.Dispose();

            Assert.False(sut.TryGetClientCacheEntry(partitionId, endpoint, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Null(cacheEntry);
        }

        [Fact]
        public void IsIdempotent()
        {
            sut.Dispose();
            sut.Dispose();
        }
    }

    public sealed class Dispose_Boolean : CommunicationClientCacheTest
    {
        readonly Action<bool> dispose;

        public Dispose_Boolean() => dispose = sut.Protected().Method<Action<bool>>();

        [Fact]
        public void DoesNotRemoveCachedEntriesWhenDisposingIsFalse()
        {
            Guid partitionId = fuzzy.Guid();
            ResolvedServiceEndpoint endpoint = MakeEndpoint();
            string listenerName = fuzzy.String();
            _ = sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, null);

            dispose(false);

            Assert.True(sut.TryGetClientCacheEntry(partitionId, endpoint, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.NotNull(cacheEntry);
        }

        [Fact]
        public void IsIdempotentWhenDisposingIsTrue()
        {
            dispose(true);
            dispose(true);
        }
    }

    public sealed class GetOrAddClientCacheEntry : CommunicationClientCacheTest
    {
        // Method parameters
        readonly Guid partitionId = fuzzy.Guid();
        readonly ResolvedServiceEndpoint endpoint = MakeEndpoint();
        readonly string listenerName = fuzzy.String();
        readonly ResolvedServicePartition rsp = Type<ResolvedServicePartition>.Uninitialized(); // matches SUT parameter name

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for endpoint.
        public void ThrowsNullReferenceExceptionWhenEndpointIsNull() =>
            _ = Assert.Throws<NullReferenceException>(() =>
                sut.GetOrAddClientCacheEntry(partitionId, null, listenerName, rsp));

        [Fact]
        public void ReturnsNewEntryInitializedWithGivenArguments()
        {
            CommunicationClientCacheEntry<ICommunicationClient> entry =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, rsp);

            Assert.NotNull(entry);
            Assert.Same(endpoint, entry.Endpoint);
            Assert.Same(listenerName, entry.ListenerName);
            Assert.Same(rsp, entry.Rsp);
        }

        [Fact]
        public void ReturnsSameEntryWhenCalledAgainWithSameKey()
        {
            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, rsp);

            Assert.Same(first, second);
        }

        [Fact]
        public void ReturnsSameEntryWhenCalledAgainWithNullListenerName()
        {
            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, null, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, null, rsp);

            Assert.Same(first, second);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. PartitionClientCacheKey.GetHashCode is case-sensitive while Equals is case-insensitive.
        public void ReturnsSameEntryWhenListenerNameDiffersOnlyInCase()
        {
            string upper = (listenerName + "abc").ToUpperInvariant();
            string lower = upper.ToLowerInvariant();

            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, upper, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, lower, rsp);

            Assert.Same(first, second);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. PartitionClientCacheKey.GetHashCode is case-sensitive while Equals is case-insensitive.
        public void ReturnsSameEntryWhenEndpointAddressDiffersOnlyInCase()
        {
            string address = (fuzzy.String() + "abc").ToUpperInvariant();
            ResolvedServiceEndpoint upper = MakeEndpoint(address, ServiceEndpointRole.Stateless);
            ResolvedServiceEndpoint lower = MakeEndpoint(address.ToLowerInvariant(), ServiceEndpointRole.Stateless);

            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, upper, listenerName, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, lower, listenerName, rsp);

            Assert.Same(first, second);
        }

        [Fact]
        public void ReturnsDifferentEntryWhenListenerNameDiffers()
        {
            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName + fuzzy.String(), rsp);

            Assert.NotSame(first, second);
        }

        [Fact]
        public void ReturnsDifferentEntryWhenEndpointAddressDiffers()
        {
            ResolvedServiceEndpoint different = MakeEndpoint(endpoint.Address + fuzzy.String(), endpoint.Role);

            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, different, listenerName, rsp);

            Assert.NotSame(first, second);
        }

        [Fact]
        public void ReturnsDifferentEntryWhenEndpointRoleDiffers()
        {
            ResolvedServiceEndpoint stateless = MakeEndpoint(endpoint.Address, ServiceEndpointRole.Stateless);
            ResolvedServiceEndpoint primary = MakeEndpoint(endpoint.Address, ServiceEndpointRole.StatefulPrimary);

            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, stateless, listenerName, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(partitionId, primary, listenerName, rsp);

            Assert.NotSame(first, second);
        }

        [Fact]
        public void ReturnsDifferentEntryWhenPartitionIdDiffers()
        {
            CommunicationClientCacheEntry<ICommunicationClient> first =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, rsp);
            CommunicationClientCacheEntry<ICommunicationClient> second =
                sut.GetOrAddClientCacheEntry(fuzzy.Guid(), endpoint, listenerName, rsp);

            Assert.NotSame(first, second);
        }
    }

    public sealed class TryGetClientCacheEntry : CommunicationClientCacheTest
    {
        // Method parameters
        readonly Guid partitionId = fuzzy.Guid();
        readonly ResolvedServiceEndpoint endpoint = MakeEndpoint();
        readonly string listenerName = fuzzy.String();

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for endpoint.
        public void ThrowsNullReferenceExceptionWhenEndpointIsNull() =>
            _ = Assert.Throws<NullReferenceException>(() =>
                sut.TryGetClientCacheEntry(partitionId, null, listenerName, out _));

        [Fact]
        public void ReturnsFalseAndNullWhenPartitionDoesNotExist()
        {
            Assert.False(sut.TryGetClientCacheEntry(partitionId, endpoint, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ReturnsFalseAndNullWhenEndpointNotFoundInExistingPartition()
        {
            _ = sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, null);

            Assert.False(sut.TryGetClientCacheEntry(partitionId, MakeEndpoint(endpoint.Address + fuzzy.String(), endpoint.Role), listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ReturnsFalseAndNullWhenListenerNameNotFoundInExistingPartition()
        {
            _ = sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, null);

            Assert.False(sut.TryGetClientCacheEntry(partitionId, endpoint, listenerName + fuzzy.String(), out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ReturnsFalseAndNullWhenEndpointRoleNotFoundInExistingPartition()
        {
            ResolvedServiceEndpoint stateless = MakeEndpoint(endpoint.Address, ServiceEndpointRole.Stateless);
            ResolvedServiceEndpoint primary = MakeEndpoint(endpoint.Address, ServiceEndpointRole.StatefulPrimary);
            _ = sut.GetOrAddClientCacheEntry(partitionId, stateless, listenerName, null);

            Assert.False(sut.TryGetClientCacheEntry(partitionId, primary, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Null(cacheEntry);
        }

        [Fact]
        public void ReturnsTrueAndPreviouslyAddedEntry()
        {
            CommunicationClientCacheEntry<ICommunicationClient> added =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, listenerName, null);

            Assert.True(sut.TryGetClientCacheEntry(partitionId, endpoint, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Same(added, cacheEntry);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. PartitionClientCacheKey.GetHashCode is case-sensitive while Equals is case-insensitive.
        public void ReturnsTrueAndPreviouslyAddedEntryWhenListenerNameDiffersOnlyInCase()
        {
            string upper = (listenerName + "abc").ToUpperInvariant();
            string lower = upper.ToLowerInvariant();
            CommunicationClientCacheEntry<ICommunicationClient> added =
                sut.GetOrAddClientCacheEntry(partitionId, endpoint, upper, null);

            Assert.True(sut.TryGetClientCacheEntry(partitionId, endpoint, lower, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Same(added, cacheEntry);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. PartitionClientCacheKey.GetHashCode is case-sensitive while Equals is case-insensitive.
        public void ReturnsTrueAndPreviouslyAddedEntryWhenEndpointAddressDiffersOnlyInCase()
        {
            string address = (fuzzy.String() + "abc").ToUpperInvariant();
            ResolvedServiceEndpoint upper = MakeEndpoint(address, ServiceEndpointRole.Stateless);
            ResolvedServiceEndpoint lower = MakeEndpoint(address.ToLowerInvariant(), ServiceEndpointRole.Stateless);
            CommunicationClientCacheEntry<ICommunicationClient> added =
                sut.GetOrAddClientCacheEntry(partitionId, upper, listenerName, null);

            Assert.True(sut.TryGetClientCacheEntry(partitionId, lower, listenerName, out CommunicationClientCacheEntry<ICommunicationClient> cacheEntry));
            Assert.Same(added, cacheEntry);
        }
    }

    static ResolvedServiceEndpoint MakeEndpoint() =>
        MakeEndpoint(fuzzy.String(), fuzzy.Enum<ServiceEndpointRole>());

    static ResolvedServiceEndpoint MakeEndpoint(string address, ServiceEndpointRole role)
    {
        var endpoint = new ResolvedServiceEndpoint();
        endpoint.Property<string>().Set(address);
        endpoint.Property<ServiceEndpointRole>().Set(role);
        return endpoint;
    }
}
