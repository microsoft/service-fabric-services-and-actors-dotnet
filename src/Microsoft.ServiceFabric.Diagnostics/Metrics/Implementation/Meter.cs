// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    abstract class Meter
    {
        protected readonly string[] systemDimensionValues;
        protected readonly IFabricMeter fabricMeter;

        internal Meter(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues)
        {
            if (systemDimensionValues == null)
            {
                throw new ArgumentNullException(nameof(systemDimensionValues));
            }
            this.fabricMeter = fabricMeter ?? throw new ArgumentNullException(nameof(fabricMeter));
            this.systemDimensionValues = systemDimensionValues.ToArray();
        }
    }
}
