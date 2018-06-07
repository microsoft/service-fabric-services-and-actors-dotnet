// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class DiagnosticsManagerActorContext
    {
#pragma warning disable SA1401 // Fields should be private. Used in Interlocked increment and decrement.
        internal long PendingActorMethodCalls;
#pragma warning restore SA1401 // Fields should be private

        private const int PreallocatedStopwatchCount = 2;
        private readonly ActorMethodDiagnosticData methodData;
        private readonly Stopwatch stateStopwatch;
        private readonly ActorStateDiagnosticData stateData;
        private readonly ActivationDiagnosticData activationDiagnosticData;
        private readonly Stopwatch onActivateAsyncStopwatch;
        private readonly PendingActorMethodDiagnosticData pendingMethodDiagnosticData;

        // Preallocated stopwatches
        private readonly Stack<Stopwatch> preallocatedStopwatches;

        // Stack of active stopwatches to handle reentrancy. We push a new stopwatch
        // to the stack for each level of reentrancy.
        private readonly Stack<Stopwatch> stopwatchStack;

        internal DiagnosticsManagerActorContext()
        {
            this.preallocatedStopwatches = new Stack<Stopwatch>(PreallocatedStopwatchCount);
            for (var i = 0; i < PreallocatedStopwatchCount; i++)
            {
                this.preallocatedStopwatches.Push(new Stopwatch());
            }

            this.stopwatchStack = new Stack<Stopwatch>(PreallocatedStopwatchCount);

            this.stateStopwatch = new Stopwatch();

            this.onActivateAsyncStopwatch = new Stopwatch();

            this.methodData = default(ActorMethodDiagnosticData);
            this.stateData = default(ActorStateDiagnosticData);
            this.activationDiagnosticData = default(ActivationDiagnosticData);
            this.pendingMethodDiagnosticData = default(PendingActorMethodDiagnosticData);
        }

        internal ActorMethodDiagnosticData MethodData => this.methodData;

        internal Stopwatch StateStopwatch => this.stateStopwatch;

        internal ActorStateDiagnosticData StateData => this.stateData;

        internal ActivationDiagnosticData ActivationDiagnosticData => this.activationDiagnosticData;

        internal PendingActorMethodDiagnosticData PendingMethodDiagnosticData => this.pendingMethodDiagnosticData;

        internal long LastReportedPendingActorMethodCalls { get; set; }

        internal Stopwatch OnActivateAsyncStopwatch => this.onActivateAsyncStopwatch;

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
    }
}
