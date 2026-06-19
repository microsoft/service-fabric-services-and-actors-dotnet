// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Globalization;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services;

public abstract class ServiceTraceTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class GetTraceIdForReplica : ServiceTraceTest
    {
        readonly Guid partitionId = fuzzy.Guid();
        readonly long replicaId = fuzzy.Int64();

        [Fact]
        public void ReturnsPartitionIdAndReplicaIdSeparatedByColon()
        {
            string expected = partitionId.ToString("B") + ":" + replicaId.ToString(CultureInfo.InvariantCulture);
            Assert.Equal(expected, ServiceTrace.GetTraceIdForReplica(partitionId, replicaId));
        }
    }

    public sealed class Source : ServiceTraceTest
    {
        [Fact]
        public void ReturnsServiceEventSourceInstance() =>
            Assert.Same(ServiceEventSource.Instance, ServiceTrace.Source);
    }
}
