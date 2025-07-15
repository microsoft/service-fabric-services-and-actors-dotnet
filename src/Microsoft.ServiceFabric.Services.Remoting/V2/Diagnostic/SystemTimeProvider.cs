using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}
