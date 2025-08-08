// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    internal class TimespanMeter : IMeter<TimeSpan>
    {
        public void Record(TimeSpan value)
        {
            throw new NotImplementedException();
        }
    }
}
