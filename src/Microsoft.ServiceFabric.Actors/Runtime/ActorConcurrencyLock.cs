// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using System.Globalization;

    /// <summary>
    /// Provides a turn based concurrency that supports logical call based reentrancy for actor calls
    /// </summary>
    internal class ActorConcurrencyLock
    {
        // this semaphore provides turn based concurrency for non-reentrant (independent) calls
        private readonly SemaphoreSlim turnLock;

        // this semaphore allows reentrancy for the reentrant calls, it protects the 
        // currentCallCount and currentCallContext variables.
        private readonly SemaphoreSlim reentrantLock;

        // keeps the count of current number of calls in progress
        private int currentCallCount;

        // current logical call context value, that identifies the current logical call chain
        private string currentCallContext;

        // the current call context is initialized with this value at the start to prevent 
        // it matching from incoming call contexts
        private readonly string initialCallContext;

        // the reentrancy mode for this guard
        private readonly ActorReentrancyMode reentrancyMode;

        // timeout for the turn lock
        private readonly TimeSpan turnLockTimeout;
        private readonly Random turnLockTimeoutRandomizer;
        private int turnLockWaitMaxRandomIntervalMillis;

        // the actor for which this guard provides turn based concurrency with reentrancy 
        private readonly ActorBase owner;

        // if the state of the actor was dirty, it needs to be handled before a call is allowed to it
        // this delegate is required on the acquire method of the guard
        public delegate Task ActorDirtyStateHandler(ActorBase actor);

        public ActorConcurrencyLock(ActorBase owner, ActorConcurrencySettings actorConcurrencySettings)
        {
            this.owner = owner;
            this.reentrancyMode = actorConcurrencySettings.ReentrancyMode;
            this.turnLock = new SemaphoreSlim(1, 1);
            this.reentrantLock = new SemaphoreSlim(1, 1);
            this.initialCallContext = Guid.NewGuid().ToString();
            this.currentCallContext = this.initialCallContext;
            this.currentCallCount = 0;
            this.turnLockTimeout = actorConcurrencySettings.LockTimeout;
            this.turnLockTimeoutRandomizer = GetRandomizer(this.turnLockTimeout, out turnLockWaitMaxRandomIntervalMillis);
        }

        internal string Test_CurrentContext
        {
            get { return this.currentCallContext; }
        }

        internal int Test_CurrentCount
        {
            get { return this.currentCallCount; }
        }

        public Task Acquire(
            string incomingCallContext,
            ActorDirtyStateHandler handler,
            CancellationToken cancellationToken)
        {
            return this.Acquire(
                incomingCallContext,
                handler,
                this.reentrancyMode,
                cancellationToken);
        }

        public async Task Acquire(
            string incomingCallContext,
            ActorDirtyStateHandler handler,
            ActorReentrancyMode actorReentrancyMode,
            CancellationToken cancellationToken)
        {
            // acquire the reentrancy lock
            await this.reentrantLock.WaitAsync(cancellationToken);
            try
            {
                // A new logical call context is appended to every outgoing method call.
                // The received callContext is of form callContext1callContext2callContext3... 
                // thus to check if incoming call was made from the current calls in progress
                // we need to check if the incoming call context starts with the currentCallContext
                var startsWith = incomingCallContext.StartsWith(this.currentCallContext);

                if (startsWith)
                {
                    // the incoming call is part of the current call chain

                    // the messaging layer may deliver duplicate messages, therefore if the 
                    // incomingCallContext is same as currentCallContext it is a duplicate message
                    // this is because every outgoing call from actors has a new callContext appended
                    // to the currentCallContext
                    if (incomingCallContext.Length == this.currentCallContext.Length)
                    {
                        throw new DuplicateMessageException(string.Format(CultureInfo.CurrentCulture,
                                                            SR.ErrorDuplicateMessage,this.GetType()));
                    }

                    //
                    // this is a reentrant call
                    //

                    // if the reentrancy is disallowed, throw and exception
                    if (actorReentrancyMode == ActorReentrancyMode.Disallowed)
                    {
                        throw new ReentrancyModeDisallowedException(String.Format(SR.ReentrancyModeDisallowed, this.GetType()));
                    }

                    // if the actor is dirty, do not allow reentrant call to go through
                    // since its not expected that actor state be dirty in a reentrant call
                    // throw exception that flows back to caller.
                    if (this.owner.IsDirty)
                    {
                        throw new ReentrantActorInvalidStateException(
                            string.Format(
                            CultureInfo.CurrentCulture,
                            SR.ReentrantActorDirtyState,
                            this.owner.Id));
                    }

                    // the currentCallCount must not be zero here as the startsWith comparison can only
                    // be true if the incoming call is part of the current call chain
                    // 
                    // we only allow one cycle in the reentrant call chain, so if this is a second reentrant call
                    // then reject it. this also ensures that if multiple calls are made from the actor in parallel
                    // and they reenter the actor only one is allowed
                    //
                    if (this.currentCallCount == 1)
                    {
                        this.currentCallCount++;
                        return;
                    }
                    else
                    {
                        throw new InvalidReentrantCallException(SR.InvalidReentrantCall);
                    }
                }
            }
            finally
            {
                this.reentrantLock.Release();
            }


            //
            // this is not a reentrant call, which means that the caller needs to wait
            // for its turn to execute this call
            // 
            var timeout = this.GetTurnLockWaitTimeout();
            if (!await this.turnLock.WaitAsync(timeout, cancellationToken))
            {
                throw new ActorConcurrencyLockTimeoutException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ConcurrencyLockTimedOut,
                        this.owner.Id,
                        timeout));
            }

            // the caller has the turn lock 
            try
            {
                // check  if the owner is dirty
                if (this.owner.IsDirty && handler != null)
                {
                    // call dirty state handler to handle it
                    await handler(this.owner);
                }

                // get the reentrancy lock and initialize it with the current call information
                // so that if this call were to generate reentrant calls they are allowed
                await this.reentrantLock.WaitAsync(cancellationToken);
                try
                {
                    this.currentCallContext = incomingCallContext;
                    this.currentCallCount = 1;
                }
                finally
                {
                    this.reentrantLock.Release();
                }

            }
            catch
            {
                // dirty handler threw and exception, release the turn lock
                // and throw the exception back
                this.turnLock.Release();
                throw;
            }

            // release the reentrancy lock but continue to hold the turn lock and release 
            // it through ReleaseContext method after this call invocation on the actor is completed

            // indicate that the turn based concurrency lock is acquired and proceed to 
            // call the method on the actor

        }

        public async Task ReleaseContext(string callContext)
        {
            // first acquire the reentrancy lock to reduce the call counts and 
            await this.reentrantLock.WaitAsync();
            try
            {
                // this call must be part of the original call chain
                if (callContext.StartsWith(this.currentCallContext))
                {
                    if (this.currentCallCount > 0)
                    {
                        // reduce the current call count as this call is finishing
                        --this.currentCallCount;
                        if (this.currentCallCount == 0)
                        {
                            // if this is the first call in the call chain, reset the 
                            // current context to initial context and our turn is finished
                            // so release the turn lock as well, so that another logical call
                            // chain can take the turn
                            this.currentCallContext = this.initialCallContext;
                            this.turnLock.Release();
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    throw new InvalidOperationException(SR.InvalidCallContextReleased);
                }
            }
            finally
            {
                this.reentrantLock.Release();
            }
        }

        private TimeSpan GetTurnLockWaitTimeout()
        {
            if (this.turnLockTimeoutRandomizer != null)
            {
                return this.turnLockTimeout.Add(
                    TimeSpan.FromMilliseconds(
                        this.turnLockTimeoutRandomizer.Next(this.turnLockWaitMaxRandomIntervalMillis)));
            }

            return this.turnLockTimeout;
        }


        private static Random GetRandomizer(TimeSpan timeout, out int turnLockWaitMaxRandomIntervalMillis)
        {
            if ((timeout == Timeout.InfiniteTimeSpan) || (timeout == TimeSpan.MaxValue))
            {
                turnLockWaitMaxRandomIntervalMillis = 0;
                return null;
            }

            try
            {
                if (timeout.TotalSeconds < 60)
                {
                    turnLockWaitMaxRandomIntervalMillis = (int)timeout.TotalMilliseconds;
                }
                else
                {
                    var t = timeout.Add(TimeSpan.FromMilliseconds(60000));
                    turnLockWaitMaxRandomIntervalMillis = 60000;
                }

                return new Random();
            }
            catch (OverflowException)
            {
            }

            turnLockWaitMaxRandomIntervalMillis = 0;
            return null;
        }
    }
}