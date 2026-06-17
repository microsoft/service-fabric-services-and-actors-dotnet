// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter1D : Meter
    {
        internal Meter1D(IFabricMeter fabricMeter) : base(fabricMeter) { }

        protected void Record(long value, string dimension1Value)
        {
            _ = dimension1Value ?? throw new ArgumentNullException(nameof(dimension1Value));
            base.RecordViaNative(value, 1, dimension1Value, null, null);
        }
    }
}
