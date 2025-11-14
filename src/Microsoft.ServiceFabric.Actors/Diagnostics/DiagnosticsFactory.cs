// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Diagnostics;


namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    internal class DiagnosticsFactory : IDisposable
    {
        readonly PerformanceCounterProviderV2 performanceCounterProvider;
        readonly ServiceContext serviceContext;
        readonly ActorTypeInformation typeInformation;
        readonly ActorMethodFriendlyNameBuilder friendlyNameBuilder;

        public DiagnosticsFactory(ServiceContext serviceContext, ActorTypeInformation typeInformation, ActorMethodFriendlyNameBuilder friendlyNameBuilder)
        {
            this.serviceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            this.typeInformation = typeInformation ?? throw new ArgumentNullException(nameof(typeInformation));
            this.friendlyNameBuilder = friendlyNameBuilder ?? throw new ArgumentNullException(nameof(friendlyNameBuilder));

            performanceCounterProvider = new PerformanceCounterProviderV2(serviceContext.PartitionId, typeInformation);
            performanceCounterProvider.InitializeActorMethodInfo(this.friendlyNameBuilder);
        }

        public virtual IDiagnostics CreateDiagnostics(IClock clock)
        {
            var performanceCounterDiagnosticEvents = new PerformanceCounterDiagnosticEvents(performanceCounterProvider, clock);
            var eventSourceDiagnosticEvents = new EventSourceDiagnosticEvents(ActorFrameworkEventSource.Writer, clock, serviceContext, friendlyNameBuilder, typeInformation);
            var registeredDiagnosticsEvents = new List<IDiagnostics> { performanceCounterDiagnosticEvents, eventSourceDiagnosticEvents };

            return new AggregatedDiagnosticEvents(registeredDiagnosticsEvents);
        }

        public virtual void Dispose()
        {
            performanceCounterProvider.Dispose();
        }
    }
}
