// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public class PerformanceCounterDiagnosticsSourceTest
    {
        public class Signature : PerformanceCounterDiagnosticsSourceTest
        {
            [Fact]
            public void ShouldImplement_IDiagnosticsSource()
            {
                var performanceCounterDiagnosticsSourceType = typeof(PerformanceCounterDiagnosticsSource);
                var iDiagnosticsSourceType = typeof(IDiagnosticsSource);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(performanceCounterDiagnosticsSourceType));            }
        }
    }
}
