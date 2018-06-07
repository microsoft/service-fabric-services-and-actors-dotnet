// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Maintains cancellation tokens associated with the method calls being dispatched.
    /// </summary>
    internal class ServiceRemotingCancellationTracker
    {
        /// <summary>
        /// Maintains the information about the method calls that are in-flight.
        /// </summary>
        private ConcurrentDictionary<int, MethodCallTracker> methodCallTrackerDictionary;

        public ServiceRemotingCancellationTracker()
        {
            this.methodCallTrackerDictionary = new ConcurrentDictionary<int, MethodCallTracker>();
        }

        /// <summary>
        /// Returns the cancellation token associated with the specified callId. A new token one is created if the
        /// callId is not tracked already.
        /// </summary>
        /// <param name="methodId">The method ID.</param>
        /// <param name="callId">The call ID.</param>
        /// <returns>The cancellation token.</returns>
        public Task<CancellationTokenSource> GetOrAddCancellationTokenSource(int methodId, string callId)
        {
            var methodCallTracker = this.methodCallTrackerDictionary.GetOrAdd(methodId, obj => new MethodCallTracker(methodId) { });

            return methodCallTracker.GetOrAddCancellationTokenSourceAsync(callId);
        }

        /// <summary>
        /// Gets a cancellation token if it exists for the specified callId.
        /// </summary>
        /// <param name="methodId">The method ID.</param>
        /// <param name="callId">The call ID.</param>
        /// <returns>The cancellation token.</returns>
        public Task<CancellationTokenResult> TryGetCancellationTokenSource(int methodId, string callId)
        {
            var methodCallTracker = this.methodCallTrackerDictionary.GetOrAdd(methodId, obj => new MethodCallTracker(methodId) { });

            return methodCallTracker.TryGetCancellationTokenSource(callId);
        }

        /// <summary>
        /// Removes the cancellation token associated with the callId specified.
        /// </summary>
        /// <param name="methodId">The method ID.</param>
        /// <param name="callId">The call ID.</param>
        public Task TryRemoveCancellationTokenSource(int methodId, string callId)
        {
            var methodCallTracker = this.methodCallTrackerDictionary.GetOrAdd(methodId, obj => new MethodCallTracker(methodId) { });

            return methodCallTracker.TryRemoveCancellationToken(callId);
        }

        /// <summary>
        /// Maintains the information related to the currently inflight calls for a service Method - identified by the methodId property.
        /// </summary>
        private sealed class MethodCallTracker
        {
            private int methodId;
            private Dictionary<string, MethodCallTrackerEntry> callTracker;
            private SemaphoreSlim callTrackerLock;

            public MethodCallTracker(int methodId)
            {
                this.methodId = methodId;
                this.callTracker = new Dictionary<string, MethodCallTrackerEntry>();
                this.callTrackerLock = new SemaphoreSlim(1, 1);
            }

            public async Task<CancellationTokenSource> GetOrAddCancellationTokenSourceAsync(string callId)
            {
                CancellationTokenSource cancellationTokenSource = null;
                {
                    await this.callTrackerLock.WaitAsync();

                    if (this.callTracker.TryGetValue(callId, out var callTrackerEntry))
                    {
                        ++callTrackerEntry.NumberOfInflightCalls;
                        cancellationTokenSource = callTrackerEntry.CancellationTknSource;
                    }
                    else
                    {
                        callTrackerEntry = new MethodCallTrackerEntry(callId);
                        this.callTracker.Add(callId, callTrackerEntry);
                        cancellationTokenSource = callTrackerEntry.CancellationTknSource;
                    }

                    this.callTrackerLock.Release();
                }

                return cancellationTokenSource;
            }

            public async Task<CancellationTokenResult> TryGetCancellationTokenSource(string callId)
            {
                var ret = false;
                CancellationTokenSource cancellationToken = null;
                {
                    await this.callTrackerLock.WaitAsync();

                    ret = this.callTracker.TryGetValue(callId, out var callTrackerEntry);
                    if (ret)
                    {
                        cancellationToken = callTrackerEntry.CancellationTknSource;
                    }

                    this.callTrackerLock.Release();
                }

                return new CancellationTokenResult()
                {
                    CancellationTokenValid = ret,
                    CancellationTknSource = cancellationToken,
                };
            }

            /// <summary>
            /// Removes the cancellation token with a specified callID..
            /// </summary>
            /// <param name="callId">The method call identifier.</param>
            /// <returns>The cancellation token.</returns>
            public async Task TryRemoveCancellationToken(string callId)
            {
                await this.callTrackerLock.WaitAsync();

                if (this.callTracker.TryGetValue(callId, out var callTrackerEntry))
                {
                    if (--callTrackerEntry.NumberOfInflightCalls == 0)
                    {
                        this.callTracker.Remove(callId);
                    }
                }

                this.callTrackerLock.Release();
            }
        }

        private sealed class MethodCallTrackerEntry
        {
           public MethodCallTrackerEntry(string callId)
            {
                this.CancellationTknSource = new CancellationTokenSource();
                this.NumberOfInflightCalls = 1;
                this.CallId = callId;
            }

           public CancellationTokenSource CancellationTknSource { get; set; }

           public int NumberOfInflightCalls { get; set; }

           private string CallId { get; set; }
        }
    }
}
