// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    public abstract class NullMeterProviderTest
    {
        public class CreateMeter : NullMeterProviderTest
        {
            [Fact]
            public void ReturnsNullMeterForNoDimension()
            {
                NullMeterProvider<long> meterProvider = new NullMeterProvider<long>();
                Assert.IsType<NullMeter<long>>(meterProvider.CreateMeter("namespace", "meterName"));
            }

            [Fact]
            public void ReturnsNullMeterForOneDimension()
            {
                NullMeterProvider<long> meterProvider = new NullMeterProvider<long>();
                Assert.IsType<NullMeter1D<long>>(meterProvider.CreateMeter("namespace", "meterName", "stringDimension1"));
            }

            [Fact]
            public void ReturnsNullMeterForTwoDimensions()
            {
                NullMeterProvider<long> meterProvider = new NullMeterProvider<long>();
                Assert.IsType<NullMeter2D<long>>(meterProvider.CreateMeter("namespace", "meterName", "stringDimension1", "stringDimension2"));
            }

            [Fact]
            public void ReturnsNullMeterForThreeDimensions()
            {
                NullMeterProvider<long> meterProvider = new NullMeterProvider<long>();
                Assert.IsType<NullMeter3D<long>>(meterProvider.CreateMeter("namespace", "meterName", "stringDimension1", "stringDimension2", "stringDimension3"));
            }
        }
    }
}
