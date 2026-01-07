// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class NullMeter1D<TValueType> : IMeter1D<TValueType>
    {
        public void Record(TValueType value, string dimension1) { }
    }
}
