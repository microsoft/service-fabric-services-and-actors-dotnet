// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime.Volatile
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Xunit;

    public class TimeManagerTest : VolatileStateProviderTestBase
    {
        private const double UpperBoundBuffer = 2;
        private const double LowerBoundBuffer = 0.5;

        [Fact]
        public void CurrentLogicalTimeTest()
        {
            TestCase("### CurrentLogicalTimeTest ###");

            var snapshotHandler = new SnapshotHandler();
            var snapshotInterval = UpperBoundBuffer;
            var timeManager = new VolatileLogicalTimeManager(snapshotHandler, GetTimestamp(snapshotInterval));

            VerifyCurrentTime(timeManager, 0);
            VerifySnapshotTime(timeManager, 0);

            Thread.Sleep(GetTimestamp(2));

            VerifyCurrentTime(timeManager, 2);
            VerifySnapshotTime(timeManager, 0);

            Thread.Sleep(GetTimestamp(3));

            VerifyCurrentTime(timeManager, 5);
            VerifySnapshotTime(timeManager, 0);

            Thread.Sleep(GetTimestamp(3));

            VerifyCurrentTime(timeManager, 8);
            VerifySnapshotTime(timeManager, 0);

            timeManager.CurrentLogicalTime = GetTimestamp(1);

            VerifyCurrentTime(timeManager, 1);
            VerifySnapshotTime(timeManager, 1);

            timeManager.CurrentLogicalTime = GetTimestamp(5);

            VerifyCurrentTime(timeManager, 5);
            VerifySnapshotTime(timeManager, 5);

            timeManager.CurrentLogicalTime = GetTimestamp(15);

            Thread.Sleep(GetTimestamp(1));

            VerifyCurrentTime(timeManager, 16);
            VerifySnapshotTime(timeManager, 15);
        }

        [Fact]
        public void SnapshotTest()
        {
            TestCase("### SnapshotTest ###");

            var snapshotHandler = new SnapshotHandler();
            var snapshotInterval = UpperBoundBuffer * 2;
            var timeManager = new VolatileLogicalTimeManager(snapshotHandler, GetTimestamp(snapshotInterval));

            VerifySnapshotTime(timeManager, 0);
            VerifySnapshotCount(snapshotHandler, 0);

            timeManager.Start();

            WaitForSnapshotCount(snapshotHandler, 1, true, GetTimestamp(snapshotInterval + UpperBoundBuffer));

            VerifyCurrentTime(timeManager, snapshotInterval);
            VerifySnapshotTime(timeManager, snapshotInterval);

            WaitForSnapshotCount(snapshotHandler, 2, true, GetTimestamp(snapshotInterval + UpperBoundBuffer));

            VerifyCurrentTime(timeManager, snapshotInterval * 2);
            VerifySnapshotTime(timeManager, snapshotInterval * 2);

            timeManager.Stop();

            WaitForSnapshotCount(snapshotHandler, 2, false, GetTimestamp(snapshotInterval));

            VerifyCurrentTime(timeManager, snapshotInterval * 3);
            VerifySnapshotTime(timeManager, snapshotInterval * 2);

            timeManager.Start();

            WaitForSnapshotCount(snapshotHandler, 3, true, GetTimestamp(snapshotInterval + UpperBoundBuffer));

            VerifyCurrentTime(timeManager, snapshotInterval * 3);
            VerifySnapshotTime(timeManager, snapshotInterval * 3);

            timeManager.Stop();

            WaitForSnapshotCount(snapshotHandler, 3, false, GetTimestamp(snapshotInterval));

            VerifyCurrentTime(timeManager, snapshotInterval * 4);
            VerifySnapshotTime(timeManager, snapshotInterval * 3);
        }

        private static void VerifyCurrentTime(
            VolatileLogicalTimeManager timeManager,
            double lowerBound)
        {
            var current = timeManager.CurrentLogicalTime;
            var lower = GetTimestamp(lowerBound - LowerBoundBuffer);
            var upper = GetTimestamp(lowerBound + UpperBoundBuffer);

            FailTestIf(current < lower, "current={0} < lower={1}", current, lower);
            FailTestIf(current > upper, "current={0} > upper={1}", current, upper);
        }

        private static void VerifySnapshotTime(
            VolatileLogicalTimeManager timeManager,
            double lowerBound)
        {
            var snapshot = timeManager.Test_GetCurrentSnapshot();
            var lower = GetTimestamp(lowerBound - LowerBoundBuffer);
            var upper = GetTimestamp(lowerBound + UpperBoundBuffer);

            FailTestIf(snapshot < lower, "snapshot={0} < lower={1}", snapshot, lower);
            FailTestIf(snapshot > upper, "snapshot={0} > upper={1}", snapshot, upper);
        }

        private static void WaitForSnapshotCount(
            SnapshotHandler handler,
            double expectedCount,
            bool breakOnMatch,
            TimeSpan timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            double count = 0;

            while (stopwatch.Elapsed < timeout)
            {
                count = handler.SnapshotCount;

                if (breakOnMatch && count == expectedCount)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }

            stopwatch.Stop();
            
            FailTestIf(
                count != expectedCount, 
                "WaitForSnapshotCount(): count={0} expected={1} break={2} timeout={3}", 
                count, 
                expectedCount, 
                breakOnMatch,
                timeout);
        }

        private static void VerifySnapshotCount(
            SnapshotHandler handler,
            double expectedCont)
        {
            var count = handler.SnapshotCount;

            FailTestIf(count != expectedCont, "count={0} expected={1}", count, expectedCont);
        }

        private static TimeSpan GetTimestamp(double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        private class SnapshotHandler : VolatileLogicalTimeManager.ISnapshotHandler
        {
            public int SnapshotCount { get; private set; }

            public SnapshotHandler()
            {
                this.SnapshotCount = 0;
            }

            async Task VolatileLogicalTimeManager.ISnapshotHandler.OnSnapshotAsync(TimeSpan currentLogicalTime)
            {
                TimeManagerTest.TestLog("OnSnapshotAsync({0})", currentLogicalTime);

                this.SnapshotCount++;

                await TimeManagerTest.CreateCompletedTask();
            }
        }

        private static Task CreateCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}
