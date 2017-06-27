// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    internal class IdleObjectGcHandle
    {
        private readonly long maxIdleCount;
        private long idleCount;
        private long useCount;
        private long timerCount; // tracks calls from timers.
        private readonly object locker = new object();
        private bool collected;

        /// <summary>
        /// Signals that object can be collected early rather than waiting until idleCount == maxIdleCount
        /// </summary>
        private bool collectEarly;        

        public IdleObjectGcHandle(long maxIdleCount)
        {
            if (maxIdleCount <= 0)
            {
                throw new ArgumentOutOfRangeException("maxIdleCount");
            }

            this.maxIdleCount = maxIdleCount;
            this.idleCount = 0;
            this.useCount = 0;
            this.timerCount = 0;
        }

        public bool TryUse(bool timerCall)
        {
            lock (this.locker)
            {
                if (this.collected)
                {
                    return false;
                }

                // for timer call increment timerCount
                if (timerCall)
                {
                    this.timerCount++;
                }
                else
                {
                    this.useCount++;
                    this.idleCount = 0;
                }

                return true;
            }
        }

        public void Unuse(bool timerCall)
        {
            lock (this.locker)
            {
                if (timerCall)
                {
                    this.timerCount--;
                }
                else
                {
                    this.useCount--;
                }
            }
        }

        public bool TryCollect()
        {
            lock (this.locker)
            {
                // Object cannot be collected when its in use.
                if (this.useCount > 0 || this.timerCount > 0)
                {
                    return false;
                }

                // Object can be collected when its not in Use and is Marked For Early Collection
                if (this.collectEarly)
                {
                    this.collected = true;
                    return true;
                }

                // Object can be collected when its not in Use and idleCount == maxIdleCount
                if (this.idleCount == this.maxIdleCount)
                {
                    this.collected = true;
                    return true;
                }

                this.idleCount++;
                return false;
            }
        }

        public void MarkForEarlyCollection()
        {
            // Mark for early collection, so that TryCollect can return true for early collection rather than waiting until idleCount == maxIdleCount.
            this.collectEarly = true;
        }

        public bool IsGarbageCollected
        {
            get { return this.collected; }
        }
    }
}
