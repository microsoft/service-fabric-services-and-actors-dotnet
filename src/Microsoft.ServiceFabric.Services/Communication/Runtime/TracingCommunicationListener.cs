// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    /// <summary>
    /// Emits traces about <see cref="ICommunicationListener"/> instances that can be created by the user code.
    /// </summary>
    sealed class TracingCommunicationListener : ICommunicationListener
    {
        readonly CommunicationListenerInfo original;
        readonly ITrace trace;

        internal TracingCommunicationListener(CommunicationListenerInfo original, ITrace trace)
        {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
            this.trace = trace ?? throw new ArgumentNullException(nameof(trace));

            trace.Info($"Created {original} of type '{original.Listener.GetType().AssemblyQualifiedName}'.");
        }

        void ICommunicationListener.Abort()
        {
            trace.Info($"Aborting {original}...");
            try
            {
                original.Listener.Abort();
                trace.Info($"Aborted {original}.");
            }
            catch (Exception e)
            {
                trace.Error($"Abort of {original} failed: {e}");
                throw;
            }
        }

        async Task ICommunicationListener.CloseAsync(CancellationToken cancellation)
        {
            trace.Info($"Closing {original}...");
            try
            {
                await original.Listener.CloseAsync(cancellation);
                trace.Info($"Closed {original}.");
            }
            catch (Exception e)
            {
                trace.Warning($"Closing of {original} failed: {e}");
                throw;
            }
        }

        async Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellation)
        {
            trace.Info($"Opening {original}...");
            try
            {
                string endpoint = await original.Listener.OpenAsync(cancellation);
                trace.Info($"Opened {original} on endpoint '{endpoint}'.");
                return endpoint;
            }
            catch (Exception e)
            {
                trace.Warning($"Opening of {original} failed: {e}");
                throw;
            }
        }
    }
}
