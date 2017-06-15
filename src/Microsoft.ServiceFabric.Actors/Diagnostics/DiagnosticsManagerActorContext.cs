// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class DiagnosticsManagerActorContext
    {
        private const int PreallocatedStopwatchCount = 2;

        internal DiagnosticsManagerActorContext()
        {
            this.preallocatedStopwatches = new Stack<Stopwatch>(PreallocatedStopwatchCount);
            for (int i = 0; i < PreallocatedStopwatchCount; i++)
            {
                this.preallocatedStopwatches.Push(new Stopwatch());
            }
            this.stopwatchStack = new Stack<Stopwatch>(PreallocatedStopwatchCount);

            this.StateStopwatch = new Stopwatch();

            this.OnActivateAsyncStopwatch = new Stopwatch();
        }

        internal Stopwatch GetOrCreateActorMethodStopwatch()
        {
            // If we have a preallocated stopwatch available, use it. Otherwise
            // allocate a new one.
            return (this.preallocatedStopwatches.Count == 0) ?
                new Stopwatch() :
                this.preallocatedStopwatches.Pop();
        }

        internal void PushActorMethodStopwatch(Stopwatch stopwatch)
        {
            this.stopwatchStack.Push(stopwatch);
        }

        internal Stopwatch PopActorMethodStopwatch()
        {
            var stopwatch = this.stopwatchStack.Pop();

            // If we are below the target count for preallocated stopwatches then
            // maintain a reference to the stopwatch that we just popped. We'll
            // reuse it later.
            if (this.preallocatedStopwatches.Count < PreallocatedStopwatchCount)
            {
                this.preallocatedStopwatches.Push(stopwatch);
            }
            return stopwatch;
        }

        internal readonly ActorMethodDiagnosticData MethodData;
        internal readonly Stopwatch StateStopwatch;
        internal readonly ActorStateDiagnosticData StateData;
        internal readonly ActivationDiagnosticData ActivationDiagnosticData;
        internal readonly Stopwatch OnActivateAsyncStopwatch;

        internal long PendingActorMethodCalls;
        internal long LastReportedPendingActorMethodCalls;
        internal readonly PendingActorMethodDiagnosticData PendingMethodDiagnosticData;

        // Specifies Preallocated stopwatches
        private readonly Stack<Stopwatch> preallocatedStopwatches;

        // Provides stack of active stopwatches to handle reentrancy. We push a new stopwatch
        // to the stack for each level of reentrancy.
        private readonly Stack<Stopwatch> stopwatchStack;
    }
}
