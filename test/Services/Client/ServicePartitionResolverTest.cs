// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Client;

public abstract class ServicePartitionResolverTest
{
    readonly ServicePartitionResolver sut;

    // Constructor parameters
    readonly CreateFabricClientDelegate createFabricClient = Mock.Of<CreateFabricClientDelegate>();
    readonly CreateFabricClientDelegate recreateFabricClient = Mock.Of<CreateFabricClientDelegate>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServicePartitionResolverTest() =>
        sut = new ServicePartitionResolver(createFabricClient, recreateFabricClient);

    public sealed class Constructor_CreateFabricClientDelegate : ServicePartitionResolverTest
    {
        [Fact]
        public void StoresGivenDelegateAsCreateAndRecreate()
        {
            var sut = new ServicePartitionResolver(createFabricClient);

            Assert.Same(createFabricClient, sut.Field<CreateFabricClientDelegate>("createFabricClient").Value);
            Assert.Same(createFabricClient, sut.Field<CreateFabricClientDelegate>("recreateFabricClient").Value);
        }

        // TODO: SUT should throw ArgumentNullException when createFabricClient is null; null delegate is stored and
        // later dereferenced by GetClient, surfacing as a deferred NullReferenceException.
        [Fact]
        public void DoesNotValidateCreateFabricClient() =>
            new ServicePartitionResolver((CreateFabricClientDelegate)null);
    }

    public sealed class Constructor_CreateFabricClientDelegate_CreateFabricClientDelegate : ServicePartitionResolverTest
    {
        [Fact]
        public void StoresGivenDelegates()
        {
            Assert.Same(createFabricClient, sut.Field<CreateFabricClientDelegate>("createFabricClient").Value);
            Assert.Same(recreateFabricClient, sut.Field<CreateFabricClientDelegate>("recreateFabricClient").Value);
        }

        [Fact]
        public void UsesCreateFabricClientAsRecreateWhenRecreateFabricClientIsNull()
        {
            var sut = new ServicePartitionResolver(createFabricClient, null);

            Assert.Same(createFabricClient, sut.Field<CreateFabricClientDelegate>("createFabricClient").Value);
            Assert.Same(createFabricClient, sut.Field<CreateFabricClientDelegate>("recreateFabricClient").Value);
        }

        // TODO: SUT should throw ArgumentNullException when createFabricClient is null; null delegate is stored and
        // later dereferenced by GetClient, surfacing as a deferred NullReferenceException.
        [Fact]
        public void DoesNotValidateCreateFabricClient() =>
            new ServicePartitionResolver(null, recreateFabricClient);

        [Fact]
        public void SetsUseNotificationToTrue() =>
            Assert.True(sut.UseNotification);
    }

    public sealed class Constructor_FabricClientSettings_StringArray : ServicePartitionResolverTest
    {
        readonly FabricClientSettings settings = null;
        readonly string[] connectionEndpoints = fuzzy.Array(fuzzy.String);

        [Fact]
        public void SetsBothDelegatesToSameNonNullDelegate() =>
            AssertCreateAndRecreateAreSameNonNullDelegate(new ServicePartitionResolver(settings, connectionEndpoints));

        // connectionEndpoints is captured into a lambda invoked later by FabricClient, which owns validation.
        [Fact]
        public void DoesNotValidateConnectionEndpoints() =>
            new ServicePartitionResolver(settings, (string[])null);
    }

    public sealed class Constructor_SecurityCredentials_FabricClientSettings_StringArray : ServicePartitionResolverTest
    {
        readonly SecurityCredentials credential = null;
        readonly FabricClientSettings settings = null;
        readonly string[] connectionEndpoints = fuzzy.Array(fuzzy.String);

        [Fact]
        public void SetsBothDelegatesToSameNonNullDelegate() =>
            AssertCreateAndRecreateAreSameNonNullDelegate(new ServicePartitionResolver(credential, settings, connectionEndpoints));

        // connectionEndpoints is captured into a lambda invoked later by FabricClient, which owns validation.
        [Fact]
        public void DoesNotValidateConnectionEndpoints() =>
            new ServicePartitionResolver(credential, settings, (string[])null);
    }

    public sealed class Constructor_SecurityCredentials_StringArray : ServicePartitionResolverTest
    {
        readonly SecurityCredentials credential = null;
        readonly string[] connectionEndpoints = fuzzy.Array(fuzzy.String);

        [Fact]
        public void SetsBothDelegatesToSameNonNullDelegate() =>
            AssertCreateAndRecreateAreSameNonNullDelegate(new ServicePartitionResolver(credential, connectionEndpoints));

        // connectionEndpoints is captured into a lambda invoked later by FabricClient, which owns validation.
        [Fact]
        public void DoesNotValidateConnectionEndpoints() =>
            new ServicePartitionResolver(credential, (string[])null);
    }

    public sealed class Constructor_StringArray : ServicePartitionResolverTest
    {
        readonly string[] connectionEndpoints = fuzzy.Array(fuzzy.String);

        [Fact]
        public void SetsBothDelegatesToSameNonNullDelegate() =>
            AssertCreateAndRecreateAreSameNonNullDelegate(new ServicePartitionResolver(connectionEndpoints));

        // connectionEndpoints is captured into a lambda invoked later by FabricClient, which owns validation.
        [Fact]
        public void DoesNotValidateConnectionEndpoints() =>
            new ServicePartitionResolver((string[])null);
    }

    public sealed class DefaultMaxRetryBackoffInterval : ServicePartitionResolverTest
    {
        [Fact]
        public void IsFiveSeconds() =>
            Assert.Equal(TimeSpan.FromSeconds(5), ServicePartitionResolver.DefaultMaxRetryBackoffInterval);
    }

    public sealed class DefaultResolveTimeout : ServicePartitionResolverTest
    {
        [Fact]
        public void IsThirtySeconds() =>
            Assert.Equal(TimeSpan.FromSeconds(30), ServicePartitionResolver.DefaultResolveTimeout);
    }

    public sealed class GetDefault : ServicePartitionResolverTest, IDisposable
    {
        readonly ServicePartitionResolver original = ServicePartitionResolver.GetDefault();

        void IDisposable.Dispose() => ServicePartitionResolver.SetDefault(original);

        [Fact]
        public void ReturnsSameInstanceOnRepeatedCalls()
        {
            var first = ServicePartitionResolver.GetDefault();
            Assert.Same(first, ServicePartitionResolver.GetDefault());
        }

        [Fact]
        public void CreatesNewDefaultWhenNoneIsSet()
        {
            ServicePartitionResolver.SetDefault(null);
            Assert.NotSame(original, ServicePartitionResolver.GetDefault());
        }
    }

    public sealed class ResolveAsync_Uri_ServicePartitionKey_CancellationToken : ServicePartitionResolverTest
    {
        // Method parameters
        readonly Uri serviceUri = fuzzy.Uri();
        readonly ServicePartitionKey partitionKey = ServicePartitionKey.Singleton;

        [Fact]
        public async Task ThrowsOperationCanceledExceptionWhenCancellationIsAlreadyRequested()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => sut.ResolveAsync(serviceUri, partitionKey, cts.Token));
        }
    }

    public sealed class ResolveAsync_Uri_ServicePartitionKey_TimeSpan_TimeSpan_CancellationToken : ServicePartitionResolverTest
    {
        // Method parameters
        readonly Uri serviceUri = fuzzy.Uri();
        readonly TimeSpan resolveTimeoutPerTry = fuzzy.TimeSpan();
        readonly TimeSpan maxRetryBackoffInterval = fuzzy.TimeSpan();

        [Theory]
        [InlineData(ServicePartitionKind.Singleton)]
        [InlineData(ServicePartitionKind.Named)]
        [InlineData(ServicePartitionKind.Int64Range)]
        public async Task ThrowsOperationCanceledExceptionWhenCancellationIsAlreadyRequested(ServicePartitionKind kind)
        {
            ServicePartitionKey key = kind switch
            {
                ServicePartitionKind.Singleton => ServicePartitionKey.Singleton,
                ServicePartitionKind.Named => new ServicePartitionKey(fuzzy.String()),
                ServicePartitionKind.Int64Range => new ServicePartitionKey(fuzzy.Int64()),
                _ => throw new InvalidOperationException(),
            };
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => sut.ResolveAsync(serviceUri, key, resolveTimeoutPerTry, maxRetryBackoffInterval, cts.Token));
        }

        [Fact]
        public async Task AcceptsNullPartitionKey()
        {
            // Null partitionKey is replaced with one of the valid kinds before dispatch, so it reaches
            // ResolveHelperAsync instead of the switch's default ArgumentOutOfRangeException branch.
            // A cancelled token is used to short-circuit ResolveHelperAsync without contacting Service Fabric.
            // This test does not verify which kind null maps to; observing that requires intercepting
            // the per-kind delegate, which ServicePartitionResolver does not currently expose.
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => sut.ResolveAsync(serviceUri, null, resolveTimeoutPerTry, maxRetryBackoffInterval, cts.Token));
        }
    }

    public sealed class SetDefault : ServicePartitionResolverTest, IDisposable
    {
        readonly ServicePartitionResolver original = ServicePartitionResolver.GetDefault();

        void IDisposable.Dispose() => ServicePartitionResolver.SetDefault(original);

        [Fact]
        public void UpdatesValueReturnedByGetDefault()
        {
            ServicePartitionResolver.SetDefault(sut);
            Assert.Same(sut, ServicePartitionResolver.GetDefault());
        }
    }

    static void AssertCreateAndRecreateAreSameNonNullDelegate(ServicePartitionResolver sut)
    {
        var create = sut.Field<CreateFabricClientDelegate>("createFabricClient").Value;
        var recreate = sut.Field<CreateFabricClientDelegate>("recreateFabricClient").Value;
        Assert.NotNull(create);
        Assert.Same(create, recreate);
    }
}
