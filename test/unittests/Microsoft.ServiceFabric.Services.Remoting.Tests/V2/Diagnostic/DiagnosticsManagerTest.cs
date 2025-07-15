// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Reflection;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{    public class DiagnosticsManagerTest
    {
        public class Signiture : DiagnosticsManagerTest
        {
            [Fact]
            public void DiagnosticsManager_Should_Implement_IDiagnosticsSource()
            {
                var diagnosticsManagerType = typeof(DiagnosticsManager);
                var iDiagnosticsSourceType = typeof(IDiagnosticsSource);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(diagnosticsManagerType), "DiagnosticsManager should implement IDiagnosticsSource interface");
            }
        }

        public class Constructor : DiagnosticsManagerTest
        {
            [Fact]
            public void DiagnosticsManager_ShouldHave_ConstructorWithParameters()
            {
                var diagnosticsManagerType = typeof(DiagnosticsManager);
                var expectedParameterTypes = new[] { typeof(Guid), typeof(long) };

                var constructor = diagnosticsManagerType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    expectedParameterTypes,
                    null);

                Assert.NotNull(constructor);
                Assert.Equal(2, constructor.GetParameters().Length);
                Assert.Equal(typeof(Guid), constructor.GetParameters()[0].ParameterType);
                Assert.Equal(typeof(long), constructor.GetParameters()[1].ParameterType);
            }
        }

    }
}
