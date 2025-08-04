using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal interface IMeterProvider
    {
        IMeter<ValueType> CreateMeter<ValueType>(string name);

        IMeter1D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name);

        IMeter2D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name, string dimension2Name);

        IMeter3D<ValueType> CreateMeter<ValueType>(string name, string dimension1Name, string dimension2Name, string dimension3Name);
    }
}
