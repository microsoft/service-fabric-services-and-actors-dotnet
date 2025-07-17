// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Reflection;
using Inspector;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{    public class DiagnosticsManagerTest
    {
        public class Signiture : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldImplement_IDiagnosticsSource()
            {
                var diagnosticsManagerType = typeof(DiagnosticsManager);
                var iDiagnosticsSourceType = typeof(IDiagnosticsSource);

                Assert.True(iDiagnosticsSourceType.IsAssignableFrom(diagnosticsManagerType), "DiagnosticsManager should implement IDiagnosticsSource interface");
            }
        }

        public class Constructor : DiagnosticsManagerTest
        {
            [Fact]
            public void ShouldHave_ConstructorWithParameters()
            {
                var diagnosticsManagerType = typeof(DiagnosticsManager);
                var expectedParameterTypes = new[] { typeof(ITimeProvider), typeof(Guid), typeof(long) };

                var constructor = diagnosticsManagerType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    expectedParameterTypes,
                    null);

                Assert.NotNull(constructor);
                Assert.Equal(3, constructor.GetParameters().Length);
                Assert.Equal(typeof(ITimeProvider), constructor.GetParameters()[0].ParameterType);
                Assert.Equal(typeof(Guid), constructor.GetParameters()[1].ParameterType);
                Assert.Equal(typeof(long), constructor.GetParameters()[2].ParameterType);
            }

            [Fact]
            public void Constructor_ShouldAssignParametersToFields()
            {
                var mockTimeProvider = Mock.Of<ITimeProvider>();
                var partitionId = Guid.NewGuid();
                var replicaOrInstanceId = 123L;
                
                var diagnosticsManager = new DiagnosticsManager(mockTimeProvider, partitionId, replicaOrInstanceId);
                
                var partitionField = diagnosticsManager.Field<Guid>("partitionId");
                Assert.NotNull(partitionField);
                Assert.Equal(partitionId, partitionField.Value);

                var timeProviderFiled = diagnosticsManager.Field<ITimeProvider>("timeProvider");
                Assert.NotNull(timeProviderFiled);
                Assert.Equal(mockTimeProvider, timeProviderFiled.Value);

                var replaicaIdField = diagnosticsManager.Field<long>("replicaOrInstanceId");
                Assert.NotNull(replaicaIdField);
                Assert.Equal(replicaOrInstanceId, replaicaIdField.Value);
            }
        }
    }
}
