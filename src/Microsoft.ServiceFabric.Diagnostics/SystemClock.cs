// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Diagnostics
{
    internal class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
