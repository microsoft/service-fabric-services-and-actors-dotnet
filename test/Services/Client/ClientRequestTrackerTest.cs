using System;
using System.Threading.Tasks;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Client;

public abstract class ClientRequestTrackerTest : IDisposable
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ClientRequestTrackerTest() { }

    void IDisposable.Dispose() { }

    public sealed class IsPresent : ClientRequestTrackerTest
    {
        [Fact]
        public void ReturnsTrueWhenValueIsSet()
        {
            ClientRequestTracker.Set(fuzzy.String());
            Assert.True(ClientRequestTracker.IsPresent());
        }

        [Fact]
        public void ReturnsFalseWhenNoValueIsSet() =>
            Assert.False(ClientRequestTracker.IsPresent());

        [Fact]
        public void ReturnsFalseAfterValueIsClearedWithNull()
        {
            ClientRequestTracker.Set(fuzzy.String());
            ClientRequestTracker.Set(null);
            Assert.False(ClientRequestTracker.IsPresent());
        }
    }

    public sealed class Set : ClientRequestTrackerTest
    {
        readonly string callContextValue = fuzzy.String();

        [Fact]
        public void StoresGivenValueRetrievableViaTryGet()
        {
            ClientRequestTracker.Set(callContextValue);

            Assert.True(ClientRequestTracker.TryGet(out string actual));
            Assert.Same(callContextValue, actual);
        }

        [Fact]
        public void ReplacesPreviouslySetValue()
        {
            string replacement = callContextValue + fuzzy.String();
            ClientRequestTracker.Set(callContextValue);

            ClientRequestTracker.Set(replacement);

            Assert.True(ClientRequestTracker.TryGet(out string actual));
            Assert.Same(replacement, actual);
        }

        [Fact]
        public async Task FlowsToAsyncContinuation()
        {
            ClientRequestTracker.Set(callContextValue);

            await Task.Yield();

            Assert.True(ClientRequestTracker.TryGet(out string actual));
            Assert.Same(callContextValue, actual);
        }

        [Fact]
        public async Task IsolatesValuesAcrossConcurrentAsyncFlows()
        {
            string a = fuzzy.String();
            string b = fuzzy.String();
            TaskCompletionSource<bool> readyA = new();
            TaskCompletionSource<bool> readyB = new();
            TaskCompletionSource<bool> released = new();

            async Task<string> SetAndRead(string value, TaskCompletionSource<bool> ready)
            {
                ClientRequestTracker.Set(value);
                ready.SetResult(true);
                await released.Task;
                ClientRequestTracker.TryGet(out string actual);
                return actual;
            }

            Task<string> ta = Task.Run(() => SetAndRead(a, readyA));
            Task<string> tb = Task.Run(() => SetAndRead(b, readyB));
            await Task.WhenAll(readyA.Task, readyB.Task);
            released.SetResult(true);

            Assert.Same(a, await ta);
            Assert.Same(b, await tb);
        }
    }

    public sealed class TryGet : ClientRequestTrackerTest
    {
        [Fact]
        public void ReturnsTrueAndOutputsValueWhenValueIsSet()
        {
            string expected = fuzzy.String();
            ClientRequestTracker.Set(expected);

            bool result = ClientRequestTracker.TryGet(out string actual);

            Assert.True(result);
            Assert.Same(expected, actual);
        }

        [Fact]
        public void ReturnsFalseAndOutputsNullWhenNoValueIsSet()
        {
            bool result = ClientRequestTracker.TryGet(out string actual);

            Assert.False(result);
            Assert.Null(actual);
        }
    }
}
