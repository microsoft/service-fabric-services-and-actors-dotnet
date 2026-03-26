// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter3D : Meter
    {
        internal Meter3D(IFabricMeter fabricMeter) : base(fabricMeter) { }

        protected void Record(long value, string dimension1Value, string dimension2Value, string dimension3Value)
        {
            _ = dimension1Value ?? throw new ArgumentNullException(nameof(dimension1Value));
            _ = dimension2Value ?? throw new ArgumentNullException(nameof(dimension2Value));
            _ = dimension3Value ?? throw new ArgumentNullException(nameof(dimension3Value));
            base.RecordViaNative(value, 3, dimension1Value, dimension2Value, dimension3Value);
        }
    }
}
