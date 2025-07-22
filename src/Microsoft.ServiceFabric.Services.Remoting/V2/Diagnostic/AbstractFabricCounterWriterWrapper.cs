// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Fabric.Common;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{
    abstract internal class AbstractFabricCounterWriterWrapper
    {
        protected FabricBaselessPerformanceCounterWriter performanceCounterWriter;

        protected AbstractFabricCounterWriterWrapper(FabricBaselessPerformanceCounterWriter fabricBaselessPerformanceCounterWriter)
        {
            this.performanceCounterWriter = fabricBaselessPerformanceCounterWriter;
        }

        abstract internal void UpdateCounterValue(long delta);
    }
}
