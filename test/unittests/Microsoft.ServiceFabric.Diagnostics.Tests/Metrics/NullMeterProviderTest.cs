// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    public abstract class NullMeterProviderTest
    {
        readonly NullMeterProvider<long> sut = new NullMeterProvider<long>();

        public class CreateMeter : NullMeterProviderTest
        {
            [Fact]
            public void ReturnsNullMeterForNoDimension()
            {
                Assert.IsType<NullMeter<long>>(sut.CreateMeter("namespace", "meterName"));
            }

            [Fact]
            public void ReturnsNullMeterForOneDimension()
            {
                Assert.IsType<NullMeter1D<long>>(sut.CreateMeter("namespace", "meterName", "stringDimension1"));
            }

            [Fact]
            public void ReturnsNullMeterForTwoDimensions()
            {
                Assert.IsType<NullMeter2D<long>>(sut.CreateMeter("namespace", "meterName", "stringDimension1", "stringDimension2"));
            }

            [Fact]
            public void ReturnsNullMeterForThreeDimensions()
            {
                Assert.IsType<NullMeter3D<long>>(sut.CreateMeter("namespace", "meterName", "stringDimension1", "stringDimension2", "stringDimension3"));
            }
        }
    }
}
