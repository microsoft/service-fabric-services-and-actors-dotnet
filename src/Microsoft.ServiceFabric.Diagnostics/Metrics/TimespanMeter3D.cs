// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class TimespanMeter3D : IMeter3D<TimeSpan>
    {
        public void Record(TimeSpan value, string dimension1, string dimension2, string dimension3)
        {
            throw new NotImplementedException();
        }
    }
}
