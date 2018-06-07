// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Common
{
    using System;
    using System.Threading;

    internal class RwLock
    {
        private readonly ReaderWriterLockSlim rwLock;

        public RwLock()
        {
            this.rwLock = new ReaderWriterLockSlim();
        }

        public IDisposable AcquireWriteLock()
        {
            return new DisposableWriteLock(this.rwLock);
        }

        public IDisposable AcquireReadLock()
        {
            return new DisposableReadLock(this.rwLock);
        }

        private abstract class DisposableLockBase : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;
            private bool isDisposed;

            protected DisposableLockBase(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                this.isDisposed = false;
            }

            ~DisposableLockBase()
            {
                this.Dispose(false);
            }

            protected ReaderWriterLockSlim Lock
            {
                get { return this.rwLock; }
            }

            void IDisposable.Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed)
                {
                    this.OnDispose();

                    // ignore disposing flag - no native resources
                }

                this.isDisposed = true;
            }

            protected abstract void OnDispose();
        }

        private class DisposableReadLock : DisposableLockBase
        {
            public DisposableReadLock(ReaderWriterLockSlim rwLock)
                : base(rwLock)
            {
                this.Lock.EnterReadLock();
            }

            protected override void OnDispose()
            {
                this.Lock.ExitReadLock();
            }
        }

        private class DisposableWriteLock : DisposableLockBase
        {
            public DisposableWriteLock(ReaderWriterLockSlim rwLock)
                : base(rwLock)
            {
                this.Lock.EnterWriteLock();
            }

            protected override void OnDispose()
            {
                this.Lock.ExitWriteLock();
            }
        }
    }
}
