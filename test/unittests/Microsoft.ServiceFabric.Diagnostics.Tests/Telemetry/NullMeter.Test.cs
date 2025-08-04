// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Diagnostics.Telemetry;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Tests.Telemetry
{
    public class NullMeterTest
    {
        [Fact]
        public void Record_NullMeter_IntegralValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter<long> meter = meterProvider.CreateMeter<long>("testLongMetric");

            var recorder = new MetricsRecorder();

            meter.Record(1);

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter1D_IntegralValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter1D<long> meter = meterProvider.CreateMeter<long>("testLongMetric", "dimension1Name");

            var recorder = new MetricsRecorder();

            meter.Record(1, "dimension1Value");

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter2D_IntegralValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter2D<long> meter = meterProvider.CreateMeter<long>("testLongMetric", "dimension1Name", "dimension2Name");

            var recorder = new MetricsRecorder();

            meter.Record(1, "dimension1Value", "dimension2Value");

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter3D_IntegralValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter3D<long> meter = meterProvider.CreateMeter<long>("testLongMetric", "dimension1Name", "dimension2Name", "dimension3Name");

            var recorder = new MetricsRecorder();

            meter.Record(1, "dimension1Value", "dimension2Value", "dimension3Value");

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter_FloatingPointValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric");

            var recorder = new MetricsRecorder();

            meter.Record(1.0);

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter1D_FloatingPointValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter1D<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric", "dimension1Name");

            var recorder = new MetricsRecorder();

            meter.Record(1.0, "dimension1Value");

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter2D_FloatingPointValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter2D<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric", "dimension1Name", "dimension2Name");

            var recorder = new MetricsRecorder();

            meter.Record(1.0, "dimension1Value", "dimension2Value");

            Assert.Empty(recorder.GetMeasurements());
        }

        [Fact]
        public void Record_NullMeter3D_FloatingPointValue_DoesNotProduceMetric(){

            MeterProvider meterProvider = new MeterProvider(new MockConfigStore("Node_0", "10.1", false));
            IMeter3D<double> meter = meterProvider.CreateMeter<double>("testDoubleMetric", "dimension1Name", "dimension2Name", "dimension3Name");

            var recorder = new MetricsRecorder();

            meter.Record(1.0, "dimension1Value", "dimension2Value", "dimension3Value");

            Assert.Empty(recorder.GetMeasurements());
        }
    }
}
