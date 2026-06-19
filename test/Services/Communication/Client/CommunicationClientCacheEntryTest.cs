// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Threading;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class CommunicationClientCacheEntryTest
{
    readonly CommunicationClientCacheEntry<ICommunicationClient> sut = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Client : CommunicationClientCacheEntryTest
    {
        readonly ICommunicationClient client = Mock.Of<ICommunicationClient>();

        [Fact]
        public void ReturnsValuePreviouslySet()
        {
            sut.Client = client;
            Assert.Same(client, sut.Client);
        }

        [Fact]
        public void IsNullAfterSettingToNull()
        {
            sut.Client = client;
            sut.Client = null;
            Assert.Null(sut.Client);
        }

        [Fact]
        public void ReturnsValueViaWeakReferenceAfterStrongReferenceCleared()
        {
            sut.Client = client;
            sut.Field<ICommunicationClient>().Set(null);

            Assert.Same(client, sut.Client);
        }

        [Fact]
        public void ReturnsNullAfterWeakReferenceTargetWasCollected()
        {
            sut.Client = client;
            sut.Field<ICommunicationClient>().Set(null);
            sut.Field<WeakReference>().Set(new WeakReference(null)); // Simulates GC collecting the target.

            Assert.Null(sut.Client);
        }
    }

    public sealed class Constructor : CommunicationClientCacheEntryTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Null(sut.Endpoint);
            Assert.Null(sut.ListenerName);
            Assert.Null(sut.Rsp);
            Assert.Null(sut.Client);
            Assert.True(sut.IsInCache);
            Assert.NotNull(sut.Semaphore);
            Assert.Equal(1, sut.Semaphore.CurrentCount);
        }
    }

    public sealed class GetEndpoint : CommunicationClientCacheEntryTest
    {
        public GetEndpoint() =>
            sut.Rsp = MakeRsp();

        [Fact]
        public void ReturnsEndpointAddressWhenAddressIsNotJsonAndListenerNameIsNull()
        {
            string address = fuzzy.String();
            sut.Endpoint = MakeEndpoint(address);

            Assert.Same(address, sut.GetEndpoint());
        }

        [Fact]
        public void ReturnsParsedEndpointAddressWhenAddressIsJsonAndListenerNameIsNull()
        {
            string address = fuzzy.String().LettersOrDigits();
            string listener = fuzzy.String().LettersOrDigits();
            sut.Endpoint = MakeEndpoint(EndpointsJson((listener, address)));

            Assert.Equal(address, sut.GetEndpoint());
        }

        [Fact]
        public void ReturnsNamedEndpointAddressWhenAddressIsJsonAndListenerNameMatches()
        {
            string listener = fuzzy.String().LettersOrDigits();
            string expected = fuzzy.String().LettersOrDigits();
            string otherListener = listener + fuzzy.String().LettersOrDigits();
            string otherAddress = expected + fuzzy.String().LettersOrDigits();
            sut.Endpoint = MakeEndpoint(EndpointsJson(
                (otherListener, otherAddress),
                (listener, expected)));
            sut.ListenerName = listener;

            Assert.Equal(expected, sut.GetEndpoint());
        }

        [Fact]
        public void ReturnsCachedAddressOnSubsequentCalls()
        {
            string address = fuzzy.String();
            sut.Endpoint = MakeEndpoint(address);
            string first = sut.GetEndpoint();

            sut.Endpoint = MakeEndpoint(address + fuzzy.String()); // Endpoint setter does not invalidate cache.

            Assert.Same(first, sut.GetEndpoint());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsFabricExceptionWhenEndpointAddressIsNullOrEmpty(string address)
        {
            sut.Endpoint = MakeEndpoint(address);
            _ = Assert.Throws<FabricException>(() => sut.GetEndpoint());
        }

        [Fact]
        public void ThrowsFabricInvalidAddressExceptionWhenAddressIsNotJsonAndListenerNameIsSpecified()
        {
            sut.Endpoint = MakeEndpoint(fuzzy.String());
            sut.ListenerName = fuzzy.String();

            _ = Assert.Throws<FabricInvalidAddressException>(() => sut.GetEndpoint());
        }

        [Fact]
        public void ThrowsFabricInvalidAddressExceptionWhenAddressIsJsonWithoutEndpointsAndListenerNameIsNull()
        {
            sut.Endpoint = MakeEndpoint(EndpointsJson());

            _ = Assert.Throws<FabricInvalidAddressException>(() => sut.GetEndpoint());
        }

        [Fact]
        public void ThrowsFabricInvalidAddressExceptionWhenListenerNameIsNotFoundInJson()
        {
            string listener = fuzzy.String().LettersOrDigits();
            sut.Endpoint = MakeEndpoint(EndpointsJson(
                (listener, fuzzy.String().LettersOrDigits())));
            sut.ListenerName = listener + fuzzy.String().LettersOrDigits();

            _ = Assert.Throws<FabricInvalidAddressException>(() => sut.GetEndpoint());
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for Endpoint.
        public void ThrowsNullReferenceExceptionWhenEndpointIsNull() =>
            _ = Assert.Throws<NullReferenceException>(() => sut.GetEndpoint());

        static string EndpointsJson(params (string Listener, string Address)[] endpoints)
        {
            var sb = new System.Text.StringBuilder("{\"Endpoints\":{");
            for (int i = 0; i < endpoints.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append('"').Append(endpoints[i].Listener).Append("\":\"").Append(endpoints[i].Address).Append('"');
            }
            sb.Append("}}");
            return sb.ToString();
        }
    }

    public sealed class IsCommunicationClientValid : CommunicationClientCacheEntryTest
    {
        readonly ICommunicationClient client = Mock.Of<ICommunicationClient>();

        [Fact]
        public void ReturnsFalseWhenClientWasNeverSet() =>
            Assert.False(sut.IsCommunicationClientValid());

        [Fact]
        public void ReturnsTrueOnFirstCallAfterClientWasSet()
        {
            sut.Client = client;
            Assert.True(sut.IsCommunicationClientValid());
        }

        [Fact]
        public void ReturnsTrueOnSubsequentCallWhileWeakReferenceTargetIsAlive()
        {
            sut.Client = client;
            _ = sut.IsCommunicationClientValid();

            Assert.True(sut.IsCommunicationClientValid());
        }

        [Fact]
        public void ClearsStrongReferenceOnFirstCall()
        {
            sut.Client = client;
            _ = sut.IsCommunicationClientValid();

            Assert.Null(sut.Field<ICommunicationClient>().Value);
        }

        [Fact]
        public void ReturnsFalseAfterClientWasSetToNull()
        {
            sut.Client = client;
            sut.Client = null;

            Assert.False(sut.IsCommunicationClientValid());
        }

        [Fact]
        public void ReturnsFalseAfterWeakReferenceTargetWasCollected()
        {
            sut.Client = client;
            _ = sut.IsCommunicationClientValid(); // Clears the strong reference.
            sut.Field<WeakReference>().Set(new WeakReference(null)); // Simulates GC collecting the target.

            Assert.False(sut.IsCommunicationClientValid());
        }
    }

    public sealed class IsInCache : CommunicationClientCacheEntryTest
    {
        [Fact]
        public void ReturnsValuePreviouslySet()
        {
            sut.IsInCache = false;
            Assert.False(sut.IsInCache);
        }
    }

    public sealed class Rsp : CommunicationClientCacheEntryTest
    {
        [Fact]
        public void ReturnsValuePreviouslySet()
        {
            ResolvedServicePartition rsp = MakeRsp();
            sut.Rsp = rsp;
            Assert.Same(rsp, sut.Rsp);
        }

        [Fact]
        public void InvalidatesCachedEndpointAddress()
        {
            sut.Rsp = MakeRsp();
            sut.Endpoint = MakeEndpoint(fuzzy.String());
            _ = sut.GetEndpoint(); // Caches address.

            sut.Rsp = MakeRsp();

            Assert.Null(sut.Field<string>("address").Value);
        }
    }

    static ResolvedServiceEndpoint MakeEndpoint(string address)
    {
        var endpoint = new ResolvedServiceEndpoint();
        endpoint.Property<string>().Set(address);
        return endpoint;
    }

    static ResolvedServicePartition MakeRsp()
    {
        var rsp = Type<ResolvedServicePartition>.Uninitialized();
        rsp.Property<ServicePartitionInformation>().Set(Type<SingletonPartitionInformation>.Uninitialized());
        return rsp;
    }
}
