using Xunit;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    public class DiagnosticsManagerActorContextTest
    {
        readonly DiagnosticsManagerActorContext sut = new DiagnosticsManagerActorContext();

        [Fact]
        public void IncrementsPendingCalls()
        {
            long expectedNewPendingCalls = sut.PendingActorMethodCalls + 1;

            sut.IncremenetPendingActorMethodCalls();

            Assert.Equal(expectedNewPendingCalls, sut.PendingActorMethodCalls);
        }

        [Fact]
        public void DecrementsPendingCalls()
        {
            long expectedNewPendingCalls = sut.PendingActorMethodCalls - 1;

            sut.DecremenetPendingActorMethodCalls();

            Assert.Equal(expectedNewPendingCalls, sut.PendingActorMethodCalls);
        }

        [Fact]
        public void UpdatesLastReportedCallsAndReturnsDelta()
        {
            long expectedNewLastReportedPendingCalls = sut.LastReportedPendingActorMethodCalls + 1;
            sut.IncremenetPendingActorMethodCalls();

            var result = sut.UpdateLastReportedActorMethodCalls();

            Assert.Equal(expectedNewLastReportedPendingCalls, sut.LastReportedPendingActorMethodCalls);
            Assert.Equal(1, result);
        }
    }
}
