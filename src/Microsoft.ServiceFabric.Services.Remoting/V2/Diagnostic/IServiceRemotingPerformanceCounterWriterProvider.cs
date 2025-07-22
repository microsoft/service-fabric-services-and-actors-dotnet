// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Fabric.Common;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic
{

    internal interface IServiceRemotingPerformanceCounterWriterProvider
    {
        FabricAverageCount64PerformanceCounterWriter ServiceRequestProcessingTimeCounterWriter { get; }

        FabricAverageCount64PerformanceCounterWriter ServiceRequestDeserializationTimeCounterWriter { get; }

        FabricAverageCount64PerformanceCounterWriter ServiceResponseSerializationTimeCounterWriter { get; }

        FabricNumberOfItems64PerformanceCounterWriter ServiceOutstandingRequestsCounterWriter { get; }
    }
}
