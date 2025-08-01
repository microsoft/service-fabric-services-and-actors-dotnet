// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    using System.Runtime.InteropServices;

    internal static class NativeMetricSource
    {
        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void LongMetric(string name, long value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void LongMetric1D(string name, long value, string dimension1Name, string dimension1Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void LongMetric2D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void LongMetric3D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value);
        
        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void LongMetric4D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void LongMetric5D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value, string dimension5Name, string dimension5Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void DoubleMetric(string name, double value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void DoubleMetric1D(string name, double value, string dimension1Name, string dimension1Value);
        
        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void DoubleMetric2D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void DoubleMetric3D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void DoubleMetric4D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value);

        [DllImport("Microsoft.ServiceFabric.Telemetry.EventSource.dll")]
        public static extern void DoubleMetric5D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value, string dimension5Name, string dimension5Value);
        
        public static void WriteNativeLongMetric(string name, long value){
            LongMetric(name, value);
        }

        public static void WriteNativeLongMetric1D(string name, long value, string dimension1Name, string dimension1Value){
            LongMetric1D(name, value, dimension1Name, dimension1Value);
        }

        public static void WriteNativeLongMetric2D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value){
            LongMetric2D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value);
        }

        public static void WriteNativeLongMetric3D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value){
            LongMetric3D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value, dimension3Name, dimension3Value);
        }

        public static void WriteNativeLongMetric4D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value){
            LongMetric4D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value, dimension3Name, dimension3Value, dimension4Name, dimension4Value);
        }

        public static void WriteNativeLongMetric5D(string name, long value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value, string dimension5Name, string dimension5Value){
            LongMetric5D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value, dimension3Name, dimension3Value, dimension4Name, dimension4Value, dimension5Name, dimension5Value);
        }

        public static void WriteNativeDoubleMetric(string name, double value){
            DoubleMetric(name, value);
        }

        public static void WriteNativeDoubleMetric1D(string name, double value, string dimension1Name, string dimension1Value){
            DoubleMetric1D(name, value, dimension1Name, dimension1Value);
        }

        public static void WriteNativeDoubleMetric2D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value){
            DoubleMetric2D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value);
        }

        public static void WriteNativeDoubleMetric3D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value){
            DoubleMetric3D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value, dimension3Name, dimension3Value);
        }

        public static void WriteNativeDoubleMetric4D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value){
            DoubleMetric4D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value, dimension3Name, dimension3Value, dimension4Name, dimension4Value);
        }

        public static void WriteNativeDoubleMetric5D(string name, double value, string dimension1Name, string dimension1Value, string dimension2Name, string dimension2Value, string dimension3Name, string dimension3Value, string dimension4Name, string dimension4Value, string dimension5Name, string dimension5Value){
            DoubleMetric5D(name, value, dimension1Name, dimension1Value, dimension2Name, dimension2Value, dimension3Name, dimension3Value, dimension4Name, dimension4Value, dimension5Name, dimension5Value);
        }

    }
}
