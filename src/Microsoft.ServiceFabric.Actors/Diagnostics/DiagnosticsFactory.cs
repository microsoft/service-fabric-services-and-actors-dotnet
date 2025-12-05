// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Diagnostics;
using Microsoft.ServiceFabric.Diagnostics.Metrics;


namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    class DiagnosticsFactory : IDisposable
    {
        readonly ServiceContext serviceContext;
        readonly ActorTypeInformation typeInformation;
        readonly ActorMethodFriendlyNameBuilder friendlyNameBuilder;

        readonly PerformanceCounterProviderV2 performanceCounterProvider;
        readonly IMeterProvider<TimeSpan> timeSpanMeterProvider;
        readonly IMeterProvider<long> longMeterProvider;

        public DiagnosticsFactory(ServiceContext serviceContext, ActorTypeInformation typeInformation, ActorMethodFriendlyNameBuilder friendlyNameBuilder)
        {
            this.serviceContext = serviceContext ?? throw new ArgumentNullException(nameof(serviceContext));
            this.typeInformation = typeInformation ?? throw new ArgumentNullException(nameof(typeInformation));
            this.friendlyNameBuilder = friendlyNameBuilder ?? throw new ArgumentNullException(nameof(friendlyNameBuilder));

            performanceCounterProvider = new PerformanceCounterProviderV2(serviceContext.PartitionId, typeInformation);
            performanceCounterProvider.InitializeActorMethodInfo(this.friendlyNameBuilder);

            // TODO: Stop using NullMeters when native metrics are integrated
            timeSpanMeterProvider = new NullMeterProvider<TimeSpan>();
            longMeterProvider = new NullMeterProvider<long>();
        }

        public virtual IDiagnostics CreateDiagnostics(IClock clock)
        {
            var performanceCounterDiagnostics = new PerformanceCounterDiagnostics(performanceCounterProvider, clock);
            var eventSourceDiagnostics = new EventSourceDiagnostics(ActorFrameworkEventSource.Writer, clock, serviceContext, friendlyNameBuilder, typeInformation);
            var metricDiagnostics = new MetricDiagnostics(longMeterProvider, timeSpanMeterProvider, clock, friendlyNameBuilder, typeInformation);
            var registeredDiagnostics = new IDiagnostics[] { performanceCounterDiagnostics, eventSourceDiagnostics, metricDiagnostics };

            return new AggregatedDiagnostics(registeredDiagnostics);
        }

        public virtual void Dispose()
        {
            performanceCounterProvider.Dispose();
        }
    }
}
