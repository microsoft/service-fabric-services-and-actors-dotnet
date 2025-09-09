// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services
{
    public abstract partial class TraceTest
    {
        readonly ITrace sut;

        // Constructor parameters
        readonly Type type = fuzzy.Type();
        readonly ServiceContext context = fuzzy.ServiceContext();
        readonly IServiceEventSource events = Mock.Of<IServiceEventSource>();

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected TraceTest() =>
            sut = new Trace(type, context, events);

        public sealed class Constructor : TraceTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenTypeIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new Trace(null, context, events));
                Assert.Equal(nameof(type), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenContextIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new Trace(type, null, events));
                Assert.Equal(nameof(context), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenEventsIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new Trace(type, context, null));
                Assert.Equal(nameof(events), exception.ParamName);
            }
        }

        public abstract class MethodTest : TraceTest
        {
            // Parameters
            protected readonly string message = fuzzy.String();

            protected readonly string traceId;

            protected MethodTest() =>
                traceId = ServiceTrace.GetTraceIdForReplica(context.PartitionId, context.ReplicaOrInstanceId);
        }

        public sealed class Error : MethodTest
        {
            [Fact]
            public void EmitsErrorTextEvent()
            {
                sut.Error(message);
                Mock.Get(events).Verify(_ => _.ErrorText(traceId, type.Name, message));
            }
        }

        public sealed class Info : MethodTest
        {
            [Fact]
            public void EmitsInfoTextEvent()
            {
                sut.Info(message);
                Mock.Get(events).Verify(_ => _.InfoText(traceId, type.Name, message));
            }
        }

        public sealed class Warning : MethodTest
        {
            [Fact]
            public void EmitsWarningTextEvent()
            {
                sut.Warning(message);
                Mock.Get(events).Verify(_ => _.WarningText(traceId, type.Name, message));
            }
        }

        public sealed class EqualsTest : TraceTest
        {
            new readonly IEquatable<Trace> sut;

            public EqualsTest() =>
                sut = (IEquatable<Trace>)base.sut;

            [Fact]
            public void ReturnsTrueWhenTypeContextAndEventsAreSame() =>
                Assert.True(sut.Equals(new Trace(type, context, events)));

            [Fact]
            public void ReturnsFalseWhenTypesAreDifferent() =>
                Assert.False(sut.Equals(new Trace(fuzzy.Type(), context, events)));

            [Fact]
            public void ReturnsFalseWhenContextsAreDifferent() =>
                Assert.False(sut.Equals(new Trace(type, fuzzy.ServiceContext(), events)));

            [Fact]
            public void ReturnsFalseWhenEventsAreDifferent() =>
                Assert.False(sut.Equals(new Trace(type, context, Mock.Of<IServiceEventSource>())));

            [Fact]
            public void ReturnsFalseWhenOtherIsNull() =>
                Assert.False(sut.Equals(null));
        }
    }
}
