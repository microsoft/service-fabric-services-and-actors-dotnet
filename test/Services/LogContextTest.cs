// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services;

public abstract class LogContextTest : IDisposable
{
    readonly LogContext sut = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    LogContextTest() =>
        LogContext.Clear();

    void IDisposable.Dispose() =>
        LogContext.Clear();

    public sealed class Clear : LogContextTest
    {
        [Fact]
        public void RemovesCurrentLogContext()
        {
            LogContext.Set(sut);

            LogContext.Clear();

            Assert.False(LogContext.IsPresent());
            Assert.False(LogContext.TryGet(out LogContext actual));
            Assert.Null(actual);
        }
    }

    public sealed class GetRequestIdOrDefault : LogContextTest
    {
        [Fact]
        public void ReturnsRequestIdOfCurrentLogContext()
        {
            Guid expected = fuzzy.Guid();
            sut.RequestId = expected;
            LogContext.Set(sut);

            Assert.Equal(expected, LogContext.GetRequestIdOrDefault());
        }

        [Fact]
        public void ReturnsEmptyGuidWhenNoLogContextIsSet() =>
            Assert.Equal(Guid.Empty, LogContext.GetRequestIdOrDefault());
    }

    public sealed class IsPresent : LogContextTest
    {
        [Fact]
        public void ReturnsTrueWhenLogContextIsSet()
        {
            LogContext.Set(sut);
            Assert.True(LogContext.IsPresent());
        }

        [Fact]
        public void ReturnsFalseWhenNoLogContextIsSet() =>
            Assert.False(LogContext.IsPresent());
    }

    public sealed class RequestId : LogContextTest
    {
        [Fact]
        public void IsSetToGivenValue()
        {
            Guid expected = fuzzy.Guid();
            sut.RequestId = expected;
            Assert.Equal(expected, sut.RequestId);
        }
    }

    public sealed class Set : LogContextTest
    {
        readonly LogContext logContext = new() { RequestId = fuzzy.Guid() };

        [Fact]
        public void StoresGivenLogContextRetrievableViaTryGet()
        {
            LogContext.Set(logContext);

            Assert.True(LogContext.TryGet(out LogContext actual));
            Assert.Same(logContext, actual);
        }

        [Fact]
        public void ReplacesPreviouslySetLogContext()
        {
            LogContext.Set(sut);

            LogContext.Set(logContext);

            Assert.True(LogContext.TryGet(out LogContext actual));
            Assert.Same(logContext, actual);
        }

        [Fact]
        public async Task FlowsToAsyncContinuation()
        {
            LogContext.Set(logContext);

            await Task.Yield();

            Assert.True(LogContext.TryGet(out LogContext actual));
            Assert.Same(logContext, actual);
        }

        [Fact]
        public async Task IsolatesValuesAcrossConcurrentAsyncFlows()
        {
            LogContext a = new() { RequestId = fuzzy.Guid() };
            LogContext b = new() { RequestId = fuzzy.Guid() };
            TaskCompletionSource<bool> readyA = new();
            TaskCompletionSource<bool> readyB = new();
            TaskCompletionSource<bool> released = new();

            async Task<LogContext> SetAndRead(LogContext value, TaskCompletionSource<bool> ready)
            {
                LogContext.Set(value);
                ready.SetResult(true);
                await released.Task;
                LogContext.TryGet(out LogContext actual);
                return actual;
            }

            Task<LogContext> ta = Task.Run(() => SetAndRead(a, readyA));
            Task<LogContext> tb = Task.Run(() => SetAndRead(b, readyB));
            await Task.WhenAll(readyA.Task, readyB.Task);
            released.SetResult(true);

            Assert.Same(a, await ta);
            Assert.Same(b, await tb);
        }
    }

    public sealed class TryGet : LogContextTest
    {
        [Fact]
        public void ReturnsTrueAndOutputsStoredLogContextWhenSet()
        {
            LogContext.Set(sut);

            bool result = LogContext.TryGet(out LogContext logContext);

            Assert.True(result);
            Assert.Same(sut, logContext);
        }

        [Fact]
        public void ReturnsFalseAndOutputsNullWhenNoLogContextIsSet()
        {
            bool result = LogContext.TryGet(out LogContext logContext);

            Assert.False(result);
            Assert.Null(logContext);
        }
    }
}
