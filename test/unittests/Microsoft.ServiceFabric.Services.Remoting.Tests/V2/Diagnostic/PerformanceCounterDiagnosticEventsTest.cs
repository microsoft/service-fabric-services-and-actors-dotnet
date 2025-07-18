// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class PerformanceCounterDiagnosticEventsTest
    {
        public class Class : PerformanceCounterDiagnosticEventsTest
        {
            [Fact]
            public void ImplementsIDiagnosticsEvents()
            {
                var performanceCounterDiagnosticsSourceType = typeof(PerformanceCounterDiagnosticEvents);
                var iDiagnosticsSourceType = typeof(IDiagnosticEvents);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(performanceCounterDiagnosticsSourceType));
            }
        }
    }
}
