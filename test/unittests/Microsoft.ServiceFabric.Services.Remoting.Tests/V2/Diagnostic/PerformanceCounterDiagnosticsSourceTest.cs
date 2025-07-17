// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
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

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(performanceCounterDiagnosticsSourceType));
            }
        }

        public class RegisterDiagnosticsSource : PerformanceCounterDiagnosticsSourceTest
        {
            [Fact]
            public void ShouldThrow_NotImplementedException()
            {
                var performanceCounterDiagnosticsSource = new PerformanceCounterDiagnosticsSource();
                var mockDiagnosticsSource = Mock.Of<IDiagnosticsSource>();

                Assert.Throws<NotSupportedException>(() => 
                    performanceCounterDiagnosticsSource.RegisterDiagnosticsSource(mockDiagnosticsSource));
            }
        }
    }
}
