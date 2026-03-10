// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class NullMeter3D<TValueType> : IMeter3D<TValueType>
    {
        public void Record(TValueType value, string dimension1, string dimension2, string dimension3) { }

        public void Dispose() { }
    }
}
