// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Diagnostic;

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class PerformanceCounterProviderV2 : PerformanceCounterProvider
    {
        private readonly string counterInstanceDifferentiatorV2;

        private Dictionary<long, CounterInstanceData> actorMethodCounterInstanceDataV2;

        internal PerformanceCounterProviderV2(Guid partitionId, ActorTypeInformation actorTypeInformation)
            : base(partitionId, actorTypeInformation)
        {
            // The counter instance names end with "_<TickCount>", where <TickCount> is the tick count when
            // the current object is created.
            //
            // If we didn't include the <TickCount> portion, the following problem could arise. Just after
            // a reconfiguration, a new primary creates a performance counter instance with the same name
            // as the one created by the old primary. If the old primary has not yet finished cleaning up
            // the old counter instance before the new primary creates its counter instance, then both
            // the old and the new primary will be referencing the same counter instance. Eventually, the
            // old primary will clean up the counter instance, while the new primary could still be using
            // it. By appending the <TickCount> portion, we ensure that the old and new primaries do not
            // reference the same counter instance. Therefore, when the old primary cleans up its counter
            // instance, the new primary is not affected by it.
            this.counterInstanceDifferentiatorV2 = string.Concat((object)DateTime.UtcNow.Ticks.ToString("D"), "_", "V2");
        }

        public override void Dispose()
        {
            if (this.actorMethodCounterInstanceDataV2 != null)
            {
                foreach (var counterInstanceData in this.actorMethodCounterInstanceDataV2.Values)
                {
                    if (counterInstanceData.CounterWriters != null)
                    {
                        if (counterInstanceData.CounterWriters.ActorMethodCounterSetInstance != null)
                        {
                            // Remove Counter Instance.
                            counterInstanceData.CounterWriters.ActorMethodCounterSetInstance.Dispose();
                        }
                    }
                }
            }

            base.Dispose();
        }

        internal override void InitializeActorMethodInfo(DiagnosticsEventManager diagnosticsEventManager)
        {
            base.InitializeActorMethodInfo(diagnosticsEventManager);

            this.actorMethodCounterInstanceDataV2 = new Dictionary<long, CounterInstanceData>();

            var methodInfoListV2 = new List<KeyValuePair<long, MethodInfo>>();
            foreach (var actorInterfaceType in this.ActorTypeInformation.InterfaceTypes)
            {
                diagnosticsEventManager.ActorMethodFriendlyNameBuilder.GetActorInterfaceMethodDescriptionsV2(
                    actorInterfaceType,
                    out var interfaceIdV2,
                    out var actorInterfaceMethodDescriptions);
                methodInfoListV2.AddRange(this.GetMethodInfo(actorInterfaceMethodDescriptions, interfaceIdV2));
            }

            var percCounterInstanceNameBuilderV2 =
                new PerformanceCounterInstanceNameBuilder(this.PartitionId, this.counterInstanceDifferentiatorV2, PerformanceCounterInstanceNameBuilder.DefaultMaxInstanceNameVariablePartsLen - 3);

            this.actorMethodCounterInstanceDataV2 = this.CreateActorMethodCounterInstanceData(methodInfoListV2, percCounterInstanceNameBuilderV2);
        }

        internal override MethodSpecificCounterWriters GetMethodSpecificCounterWriters(long interfaceMethodKey, RemotingListenerVersion remotingListener)
        {
            return this.actorMethodCounterInstanceDataV2[interfaceMethodKey].CounterWriters;
        }
    }
}
