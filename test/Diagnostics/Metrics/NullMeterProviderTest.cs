// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class NullMeterProviderTest
    {
        private struct TestValueType
        {
            public long Value { get; set; }
        }

        readonly NullMeterProvider<TestValueType> sut = new NullMeterProvider<TestValueType>();

        public class CreateMeter : NullMeterProviderTest
        {
            [Fact]
            public void ReturnsNullMeterForNoDimension()
            {
                Assert.IsType<NullMeter<TestValueType>>(sut.CreateMeter("namespace", "meterName"));
            }

            [Fact]
            public void ReturnsNullMeterForOneDimension()
            {
                Assert.IsType<NullMeter1D<TestValueType>>(sut.CreateMeter("namespace", "meterName", "stringDimension1"));
            }

            [Fact]
            public void ReturnsNullMeterForTwoDimensions()
            {
                Assert.IsType<NullMeter2D<TestValueType>>(sut.CreateMeter("namespace", "meterName", "stringDimension1", "stringDimension2"));
            }

            [Fact]
            public void ReturnsNullMeterForThreeDimensions()
            {
                Assert.IsType<NullMeter3D<TestValueType>>(sut.CreateMeter("namespace", "meterName", "stringDimension1", "stringDimension2", "stringDimension3"));
            }
        }
    }
}
