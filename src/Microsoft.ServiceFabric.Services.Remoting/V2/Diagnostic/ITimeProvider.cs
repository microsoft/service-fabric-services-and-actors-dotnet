using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    internal interface ITimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}
