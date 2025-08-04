// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    public abstract class NullMeterTest
    {
        public class Record : NullMeterTest
        {
            [Fact]
            public void DoesNotProduceMetricFor_0DIntegralValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter<long> meter = meterProvider.CreateMeter<long>("testLongMetric");

                var recorder = new MetricsRecorder();

                meter.Record(1);

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_1DIntegralValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter1D<long> meter = meterProvider.CreateMeter<long>("testLongMetric", "dimension1Name");

                var recorder = new MetricsRecorder();

                meter.Record(1, "dimension1Value");

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_2DIntegralValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter2D<long> meter = meterProvider.CreateMeter<long>("testLongMetric", "dimension1Name", "dimension2Name");

                var recorder = new MetricsRecorder();

                meter.Record(1, "dimension1Value", "dimension2Value");

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_3DIntegralValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter3D<long> meter = meterProvider.CreateMeter<long>("testLongMetric", "dimension1Name", "dimension2Name", "dimension3Name");

                var recorder = new MetricsRecorder();

                meter.Record(1, "dimension1Value", "dimension2Value", "dimension3Value");

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_0DDoubleValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric");

                var recorder = new MetricsRecorder();

                meter.Record(1.0);

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_1DDoubleValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter1D<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric", "dimension1Name");

                var recorder = new MetricsRecorder();

                meter.Record(1.0, "dimension1Value");

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_2DDoubleValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter2D<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric", "dimension1Name", "dimension2Name");

                var recorder = new MetricsRecorder();

                meter.Record(1.0, "dimension1Value", "dimension2Value");

                Assert.Empty(recorder.GetMeasurements());
            }

            [Fact]
            public void DoesNotProduceMetricFor_3DDoubleValue()
            {

                NullMeterProvider meterProvider = new NullMeterProvider();
                IMeter3D<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric", "dimension1Name", "dimension2Name", "dimension3Name");

                var recorder = new MetricsRecorder();

                meter.Record(1.0, "dimension1Value", "dimension2Value", "dimension3Value");

                Assert.Empty(recorder.GetMeasurements());
            }
        }
    }
}
