// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    sealed class NullMeter<TValueType> : IMeter<TValueType>
    {
        public void Record(TValueType value) { }

        public void Dispose() { }
    }
}
