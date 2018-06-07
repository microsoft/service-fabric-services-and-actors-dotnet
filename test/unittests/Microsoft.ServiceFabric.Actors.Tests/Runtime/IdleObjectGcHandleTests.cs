// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Xunit;

    /// <summary>
    /// Tests for IdleObjectGcHandle.
    /// </summary>
    public class IdleObjectGcHandleTests
    {
        /// <summary>
        /// Verifies basic usage of IdleObjectGcHandle.
        /// </summary>
        [Fact]
        public void VerifyBasicUsage()
        {
            var gchandle = new IdleObjectGcHandle(1);
            gchandle.TryUse(false).Should().BeTrue();
            gchandle.TryUse(true).Should().BeTrue();
            gchandle.TryCollect().Should().BeFalse();
            gchandle.Unuse(false);
            gchandle.TryCollect().Should().BeFalse();
            gchandle.Unuse(true);
            gchandle.TryCollect().Should().BeFalse();
            gchandle.TryUse(false).Should().BeTrue();
            gchandle.Unuse(false);
            gchandle.TryCollect().Should().BeFalse();
            gchandle.TryCollect().Should().BeTrue();
        }

        /// <summary>
        /// Verify UseUnuseCollect
        /// </summary>
        /// <param name="n">MaxIdleCount for IdleObjectGcHandle.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]

        public void VerifyUseUnuseCollect(int n)
        {
            // 1. Set MaxIdleCount for IdleObjectGcHandle to N.
            // 2. Call TryUse Once.
            // 3. Then perform interleaved TryUse and TryCollect N times, which should always result in true for TryUse and false for TryCollect.
            // 4. Then perform interleaved Unuse and TryCollect N times, which should always result in false for TryCollect.
            // 5. Call Unuse to un-use the object in step 1.
            // 6. Then perform interleaved TryCollect N times, which should always result in false for TryCollect as its counting to maxIdleTicks.
            // 7. Final call to TryCollect must return true.
            var gchandle = new IdleObjectGcHandle(n);
            gchandle.TryUse(false).Should().BeTrue();
            var tasks = new List<Task>();

            for (var i = 0; i < n; i++)
            {
                tasks.Add(Task.Run(() => Assert.True(gchandle.TryUse(false))));
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            for (var i = 0; i < n; i++)
            {
                tasks.Add(Task.Run(() => gchandle.Unuse(false)));
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            gchandle.Unuse(false);
            for (var i = 0; i < n; i++)
            {
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            gchandle.TryCollect().Should().BeTrue();
        }

        /// <summary>
        /// Tests UseUnuseCollectWithTimerCalls
        /// </summary>
        /// <param name="n">MaxIdleCount for IdleObjectGcHandle.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void VerifyUseUnuseCollectWithTimerCalls(int n)
        {
            // 1. Set MaxIdleCount for IdleObjectGcHandle to N.
            // 2. Call TryUse Once.
            // 3. Then perform interleaved TryUse, TryUse(forTimer) and TryCollect N times, which should always result in true for TryUse and false for TryCollect.
            // 4. Then perform interleaved Unuse, UnUse(forTimer) and TryCollect N times, which should always result in false for TryCollect.
            // 5. Call Unuse to un-use the object in step 1.
            // 6. Then perform interleaved TryCollect N-1 times, which should always result in false for TryCollect as its counting to maxIdleTicks.
            // 7. Then perform TryUse(for Timer) result in true for TryUse and false for TryUse
            // 8. Then perform interleaved TryCollect N times, which should always result in false for TryCollect as its not counting towards GC because of use by a timer call.
            // 9. Call UnUse to unuse object in step8.
            // 10. Then perform TryCollect 1 times, which should result in false for TryCollect as its counting towards GC.
            // 11. Final call to TryCollect must return true.
            var gchandle = new IdleObjectGcHandle(n);
            gchandle.TryUse(false).Should().BeTrue();
            var tasks = new List<Task>();

            for (var i = 0; i < n; i++)
            {
                tasks.Add(Task.Run(() => Assert.True(gchandle.TryUse(false))));
                tasks.Add(Task.Run(() => Assert.True(gchandle.TryUse(true))));
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            for (var i = 0; i < n; i++)
            {
                tasks.Add(Task.Run(() => gchandle.Unuse(false)));
                tasks.Add(Task.Run(() => gchandle.Unuse(true)));
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            gchandle.Unuse(false);

            for (var i = 0; i < n - 1; i++)
            {
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            gchandle.TryUse(true).Should().BeTrue();

            for (var i = 0; i < n; i++)
            {
                tasks.Add(Task.Run(() => Assert.False(gchandle.TryCollect())));
            }

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            gchandle.Unuse(true);
            gchandle.TryCollect().Should().BeFalse();

            Task.WaitAll(tasks.ToArray());
            tasks.Clear();

            gchandle.TryCollect().Should().BeTrue();
            gchandle.IsGarbageCollected.Should().BeTrue();
        }
    }
}
