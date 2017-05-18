// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class for maintaining cancellation tokens associated with the method calls being dispatched.
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
        /// <param name="methodId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public Task<CancellationTokenSource> GetOrAddCancellationTokenSource(int methodId, string callId)
        {
            var methodCallTracker = this.methodCallTrackerDictionary.GetOrAdd(methodId, obj => new MethodCallTracker(methodId) {});

            return methodCallTracker.GetOrAddCancellationTokenSourceAsync(callId);
        }

        /// <summary>
        /// Gets a cancellation token if it exists for the specified callId
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public Task<CancellationTokenResult> TryGetCancellationTokenSource(int methodId, string callId)
        {
            var methodCallTracker = this.methodCallTrackerDictionary.GetOrAdd(methodId, obj => new MethodCallTracker(methodId) {});

            return methodCallTracker.TryGetCancellationTokenSourceAsync(callId);
        }

        /// <summary>
        /// Removes the cancellation token associated with the callId specified
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="callId"></param>
        public Task TryRemoveCancellationTokenSource(int methodId, string callId)
        {
            var methodCallTracker = this.methodCallTrackerDictionary.GetOrAdd(methodId, obj => new MethodCallTracker(methodId) {});

            return methodCallTracker.TryRemoveCancellationTokenAsync(callId);
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

                    MethodCallTrackerEntry callTrackerEntry;
                    if (this.callTracker.TryGetValue(callId, out callTrackerEntry))
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

            public async Task<CancellationTokenResult> TryGetCancellationTokenSourceAsync(string callId)
            {
                var ret = false;
                CancellationTokenSource cancellationToken = null;

                {
                    await this.callTrackerLock.WaitAsync();

                    MethodCallTrackerEntry callTrackerEntry;
                    ret = this.callTracker.TryGetValue(callId, out callTrackerEntry);
                    if (ret)
                    {
                        cancellationToken = callTrackerEntry.CancellationTknSource;
                    }

                    this.callTrackerLock.Release();
                }

                return new CancellationTokenResult()
                {
                    CancellationTokenValid = ret,
                    CancellationTknSource = cancellationToken
                };
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="callId">method call identifier, for </param>
            /// <returns></returns>
            public async Task TryRemoveCancellationTokenAsync(string callId)
            {
                await this.callTrackerLock.WaitAsync();

                MethodCallTrackerEntry callTrackerEntry;
                if (this.callTracker.TryGetValue(callId, out callTrackerEntry))
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
            public CancellationTokenSource CancellationTknSource { get; set; }

            public int NumberOfInflightCalls { get; set; }

            public string CallId { get; set; }

            public MethodCallTrackerEntry(string callId)
            {
                this.CancellationTknSource = new CancellationTokenSource();
                this.NumberOfInflightCalls = 1;
                this.CallId = callId;
            }
        }
    }

    /// <summary>
    /// This wrapper object is used to get the result of querying the cancellation token for a
    /// particular method call via Async api's
    /// </summary>
    internal class CancellationTokenResult
    {
        public CancellationTokenSource CancellationTknSource;

        public bool CancellationTokenValid;
    }
}