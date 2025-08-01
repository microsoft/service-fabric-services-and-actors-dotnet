// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal class Meter<ValueType> : IMeter0D<ValueType>
    {
        readonly string name;
        readonly string nodeName;
        readonly string runtimeVersion;

        public Meter(string name, string nodeName, string runtimeVersion)
        {
            this.name = name;
            this.nodeName = nodeName;
            this.runtimeVersion = runtimeVersion;
        }

        public void Record(ValueType value)
        {
            if (MeterUtils.isIntegral(value))
            {
                NativeMetricSource.WriteNativeLongMetric2D(this.name, Convert.ToInt64(value), "NodeName", nodeName, "RuntimeVersion", runtimeVersion);
            }
            else if (MeterUtils.isFloatingPoint(value))
            {
                NativeMetricSource.WriteNativeDoubleMetric2D(this.name, Convert.ToDouble(value), "NodeName", nodeName, "RuntimeVersion", runtimeVersion);
            }
            else
            {
                throw new ArgumentException("Value type is not integral/floating");
            }
        }
    }

    internal class Meter1D<ValueType> : IMeter1D<ValueType>
    {
        readonly string name;
        readonly string dimension1Name;
        readonly string nodeName;
        readonly string runtimeVersion;

        public Meter1D(string name, string dimension1Name, string nodeName, string runtimeVersion)
        {
            this.name = name;
            this.dimension1Name = dimension1Name;
            this.nodeName = nodeName;
            this.runtimeVersion = runtimeVersion;
        }

        public void Record(ValueType value, string dimension1Value)
        {
            if (MeterUtils.isIntegral(value))
            {
                NativeMetricSource.WriteNativeLongMetric3D(this.name, Convert.ToInt64(value), this.dimension1Name, dimension1Value, "NodeName", nodeName, "RuntimeVersion", runtimeVersion);
            }
            else if (MeterUtils.isFloatingPoint(value))
            {
                NativeMetricSource.WriteNativeDoubleMetric3D(this.name, Convert.ToDouble(value), this.dimension1Name, dimension1Value, "NodeName", nodeName, "RuntimeVersion", runtimeVersion);
            }
            else
            {
                throw new ArgumentException("Value type is not integral/floating");
            }

        }
    }

    internal class Meter2D<ValueType> : IMeter2D<ValueType>
    {
        readonly string name;
        readonly string dimension1Name;
        readonly string dimension2Name;
        readonly string nodeName;
        readonly string runtimeVersion;

        public Meter2D(string name, string dimension1Name, string dimension2Name, string nodeName, string runtimeVersion)
        {
            this.name = name;
            this.dimension1Name = dimension1Name;
            this.dimension2Name = dimension2Name;
            this.nodeName = nodeName;
            this.runtimeVersion = runtimeVersion;
        }

        public void Record(ValueType value, string dimension1Value, string dimension2Value)
        {
            if (MeterUtils.isIntegral(value))
            {
                NativeMetricSource.WriteNativeLongMetric4D(this.name, Convert.ToInt64(value), this.dimension1Name, dimension1Value, this.dimension2Name, dimension2Value, "NodeName", nodeName, "RuntimeVersion", runtimeVersion);

            }
            else if (MeterUtils.isFloatingPoint(value))
            {
                NativeMetricSource.WriteNativeDoubleMetric4D(this.name, Convert.ToDouble(value), this.dimension1Name, dimension1Value, this.dimension2Name, dimension2Value, "NodeName", nodeName, "RuntimeVersion", runtimeVersion);
            }
            else
            {
                throw new ArgumentException("Value type is not integral/floating");
            }
        }
    }

    internal class Meter3D<ValueType> : IMeter3D<ValueType>
    {
        readonly string name;
        readonly string dimension1Name;
        readonly string dimension2Name;
        readonly string dimension3Name;
        readonly string nodeName;
        readonly string runtimeVersion;

        public Meter3D(string name, string dimension1Name, string dimension2Name, string dimension3Name, string nodeName, string runtimeVersion)
        {
            this.name = name;
            this.dimension1Name = dimension1Name;
            this.dimension2Name = dimension2Name;
            this.dimension3Name = dimension3Name;
            this.nodeName = nodeName;
            this.runtimeVersion = runtimeVersion;
        }

        public void Record(ValueType value, string dimension1Value, string dimension2Value, string dimension3Value)
        {
            if (MeterUtils.isIntegral(value))
            {
                NativeMetricSource.WriteNativeLongMetric5D(this.name, Convert.ToInt64(value), this.dimension1Name, dimension1Value, this.dimension2Name, dimension2Value, this.dimension3Name, dimension3Value, "NodeName", nodeName, "RuntimeVersion", runtimeVersion);

            }
            else if (MeterUtils.isFloatingPoint(value))
            {
                NativeMetricSource.WriteNativeLongMetric5D(this.name, Convert.ToInt64(value), this.dimension1Name, dimension1Value, this.dimension2Name, dimension2Value, this.dimension3Name, dimension3Value, "NodeName", nodeName, "RuntimeVersion", runtimeVersion);

            }
            else
            {
                throw new ArgumentException("Value type is not integral/floating");
            }
        }
    }
}
