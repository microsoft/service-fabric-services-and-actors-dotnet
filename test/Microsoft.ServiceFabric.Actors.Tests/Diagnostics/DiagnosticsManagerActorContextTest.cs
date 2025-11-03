using System;
using Fuzzy;
using Inspector;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public class DiagnosticsManagerActorContextTest
    {
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        readonly DiagnosticsManagerActorContext sut = new DiagnosticsManagerActorContext();

        long pendingActorMethodCalls = fuzzy.Int64();
        long lastReportedPendingActorMethodCalls = fuzzy.Int32();

        public DiagnosticsManagerActorContextTest()
        {
            sut.Field<long>("pendingActorMethodCalls").Set(pendingActorMethodCalls);
            sut.Field<long>("lastReportedPendingActorMethodCalls").Set(lastReportedPendingActorMethodCalls);
        }

        [Fact]
        public void IncrementsPendingCalls()
        {
            sut.IncremenetPendingActorMethodCalls();

            Assert.Equal(pendingActorMethodCalls + 1, sut.PendingActorMethodCalls);
        }

        [Fact]
        public void DecrementsPendingCalls()
        {
            sut.DecremenetPendingActorMethodCalls();

            Assert.Equal(pendingActorMethodCalls - 1, sut.PendingActorMethodCalls);
        }

        [Fact]
        public void UpdatesLastReportedCallsAndReturnsDelta()
        {
            var result = sut.UpdateLastReportedActorMethodCalls();

            Assert.Equal(pendingActorMethodCalls - 1, sut.LastReportedPendingActorMethodCalls);
            Assert.Equal(pendingActorMethodCalls - 1, sut.PendingActorMethodCalls);
            Assert.Equal(pendingActorMethodCalls - lastReportedPendingActorMethodCalls - 1, result);
        }
    }
}
