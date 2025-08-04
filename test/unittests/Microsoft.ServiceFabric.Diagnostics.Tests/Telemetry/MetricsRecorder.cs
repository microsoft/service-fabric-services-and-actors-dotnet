// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Tests.Telemetry
{
    public class MetricsRecorder
    {
        public class RecordedMeasurement
        {
            public string Name { get; }
            public object Value { get; }
            public IReadOnlyList<KeyValuePair<string, object>> Dimensions { get; }

            public RecordedMeasurement(string name, object value, IEnumerable<KeyValuePair<string, object>> dimensions)
            {
                Name = name;
                Value = value;
                Dimensions = dimensions.ToList();
            }
        }

        private readonly MeterListener _listener;
        private readonly List<RecordedMeasurement> _measurements = new List<RecordedMeasurement>();

        public MetricsRecorder()
        {
            _listener = new MeterListener
            {
                InstrumentPublished = (instrument, listener) =>
                {
                    listener.EnableMeasurementEvents(instrument);
                }
            };

            _listener.SetMeasurementEventCallback<int>(RecordMeasurement);
            _listener.SetMeasurementEventCallback<long>(RecordMeasurement);
            _listener.SetMeasurementEventCallback<float>(RecordMeasurement);
            _listener.SetMeasurementEventCallback<double>(RecordMeasurement);
            _listener.SetMeasurementEventCallback<decimal>(RecordMeasurement);

            _listener.Start();
        }

        private void RecordMeasurement<T>(Instrument instrument, T value, ReadOnlySpan<KeyValuePair<string, object>> dimensions, object state)
            where T : struct
        {
            _measurements.Add(new RecordedMeasurement(instrument.Name, value, dimensions.ToArray()));
        }

        public IReadOnlyList<RecordedMeasurement> GetMeasurements() => _measurements.AsReadOnly();
    }
}