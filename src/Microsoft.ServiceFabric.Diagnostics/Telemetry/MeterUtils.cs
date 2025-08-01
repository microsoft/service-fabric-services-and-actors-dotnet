// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
    
namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{

    public static class MeterUtils
    {
        public static bool isIntegral(object value)
        {
            return (value is SByte || value is Int16 || value is Int32
              || value is Int64 || value is Byte || value is UInt16
              || value is UInt32 || value is UInt64);
        }
        public static bool isFloatingPoint(object value)
        {
            return (value is float | value is double | value is Decimal);
        }
    }
}
