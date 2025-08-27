// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter
    {
        protected readonly IEnumerable<string> systemDimensionValues;
        protected readonly IFabricMeter fabricMeter;

        internal Meter(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues)
        {
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues ?? throw new ArgumentNullException(nameof(systemDimensionValues));
        }
    }
}
