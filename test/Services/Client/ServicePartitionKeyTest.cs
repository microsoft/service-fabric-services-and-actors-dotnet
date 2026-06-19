using System;
using System.Fabric;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Client;

public abstract class ServicePartitionKeyTest
{
    readonly ServicePartitionKey sut = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor : ServicePartitionKeyTest
    {
        [Fact]
        public void SetsKindToSingletonAndValueToNull()
        {
            Assert.Equal(ServicePartitionKind.Singleton, sut.Kind);
            Assert.Null(sut.Value);
        }
    }

    public sealed class Constructor_Int64 : ServicePartitionKeyTest
    {
        readonly long partitionKey = fuzzy.Int64();

        [Fact]
        public void SetsKindToInt64RangeAndValueToPartitionKey()
        {
            var sut = new ServicePartitionKey(partitionKey);

            Assert.Equal(ServicePartitionKind.Int64Range, sut.Kind);
            Assert.Equal(partitionKey, sut.Value);
        }
    }

    public sealed class Constructor_String : ServicePartitionKeyTest
    {
        readonly string partitionKey = fuzzy.String();

        [Fact]
        public void SetsKindToNamedAndValueToPartitionKey()
        {
            var sut = new ServicePartitionKey(partitionKey);

            Assert.Equal(ServicePartitionKind.Named, sut.Kind);
            Assert.Same(partitionKey, sut.Value);
        }

        [Fact]
        public void StoresNullPartitionKey()
        {
            var sut = new ServicePartitionKey((string)null);

            Assert.Equal(ServicePartitionKind.Named, sut.Kind);
            Assert.Null(sut.Value);
        }
    }

    public sealed class Singleton : ServicePartitionKeyTest
    {
        [Fact]
        public void IsServicePartitionKeyWithKindSingletonAndNullValue()
        {
            Assert.NotNull(ServicePartitionKey.Singleton);
            Assert.Equal(ServicePartitionKind.Singleton, ServicePartitionKey.Singleton.Kind);
            Assert.Null(ServicePartitionKey.Singleton.Value);
        }
    }
}
