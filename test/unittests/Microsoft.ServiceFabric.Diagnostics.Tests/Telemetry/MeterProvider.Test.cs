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
            public void CreateMeter_WithNoDimensionMetricsConfigDisabled_ReturnsNullMeter()
            {
                NullMeterProvider meterProvider = new NullMeterProvider();
                Assert.True(meterProvider.CreateMeter<long>("myLong") is NullMeter<long>);
            }

            [Fact]
            public void CreateMeter_With1DimensionMetricsConfigDisabled_ReturnsNullMeter()
            {
                NullMeterProvider meterProvider = new NullMeterProvider();
                Assert.True(meterProvider.CreateMeter<long>("myLong", "stringDimension1") is NullMeter1D<long>);
                Assert.True(meterProvider.CreateMeter<double>("myDouble", "stringDimension1") is NullMeter1D<double>);
            }

            [Fact]
            public void CreateMeter_With2DimensionMetricsConfigDisabled_ReturnsNullMeter()
            {
                NullMeterProvider meterProvider = new NullMeterProvider();
                Assert.True(meterProvider.CreateMeter<long>("myLong", "stringDimension1", "stringDimension2") is NullMeter2D<long>);
                Assert.True(meterProvider.CreateMeter<double>("myDouble", "stringDimension1", "stringDimension2") is NullMeter2D<double>);
            }

            [Fact]
            public void CreateMeter_With3DimensionMetricsConfigDisabled_ReturnsNullMeter()
            {
                NullMeterProvider meterProvider = new NullMeterProvider();
                Assert.True(meterProvider.CreateMeter<long>("myLong", "stringDimension1", "stringDimension2", "stringDimension3") is NullMeter3D<long>);
                Assert.True(meterProvider.CreateMeter<double>("myDouble", "stringDimension1", "stringDimension2", "stringDimension3") is NullMeter3D<double>);
            }
        }
    }
}
