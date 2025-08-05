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
                Assert.True(meterProvider.CreateMeter("myLong") is NullMeter<long>);
            }

            [Fact]
            public void ReturnsNullMeterForOneDimensions()
            {
                NullMeterProvider<long> longNullMeterProvider = new NullMeterProvider<long>();
                Assert.True(longNullMeterProvider.CreateMeter("myLong", "stringDimension1") is NullMeter1D<long>);

                NullMeterProvider<double> doubleNullMeterProvider = new NullMeterProvider<double>();
                Assert.True(doubleNullMeterProvider.CreateMeter("myDouble", "stringDimension1") is NullMeter1D<double>);
            }

            [Fact]
            public void ReturnsNullMeterForTwoDimensions()
            {
                NullMeterProvider<long> longNullMeterProvider = new NullMeterProvider<long>();
                Assert.True(longNullMeterProvider.CreateMeter("myLong", "stringDimension1", "stringDimension2") is NullMeter2D<long>);

                NullMeterProvider<double> doubleNullMeterProvider = new NullMeterProvider<double>();
                Assert.True(doubleNullMeterProvider.CreateMeter("myDouble", "stringDimension1", "stringDimension2") is NullMeter2D<double>);
            }

            [Fact]
            public void ReturnsNullMeterForThreeDimensions()
            {
                NullMeterProvider<long> longNullMeterProvider = new NullMeterProvider<long>();
                Assert.True(longNullMeterProvider.CreateMeter("myLong", "stringDimension1", "stringDimension2", "stringDimension3") is NullMeter3D<long>);

                NullMeterProvider<double> doubleNullMeterProvider = new NullMeterProvider<double>();
                Assert.True(doubleNullMeterProvider.CreateMeter("myDouble", "stringDimension1", "stringDimension2", "stringDimension3") is NullMeter3D<double>);
            }
        }
    }
}
