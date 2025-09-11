// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    public abstract class ServiceInstanceListenerTest
    {
        readonly ServiceInstanceListener sut;

        // Constructor parameters
        readonly Func<StatelessServiceContext, ICommunicationListener> createCommunicationListener = Mock.Of<Func<StatelessServiceContext, ICommunicationListener>>();
        readonly string name = fuzzy.String();

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected ServiceInstanceListenerTest() =>
            sut = new ServiceInstanceListener(createCommunicationListener, name);

        public sealed class Constructor : ServiceInstanceListenerTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenCreateCommunicationListenerIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new ServiceInstanceListener(null, name));
                Assert.Equal(nameof(createCommunicationListener), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenNameIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new ServiceInstanceListener(createCommunicationListener, null));
                Assert.Equal(nameof(name), exception.ParamName);
            }

            [Fact]
            public void InitializesPropertiesWithGivenArguments()
            {
                Assert.Same(createCommunicationListener, sut.CreateCommunicationListener);
                Assert.Same(name, sut.Name);
            }

            [Fact]
            public void InitializesPropertiesWithDefaultArgumentValues()
            {
                var sut = new ServiceInstanceListener(createCommunicationListener);
                Assert.Same(ServiceInstanceListener.DefaultName, sut.Name);
            }
        }

        public sealed class Instantiate : ServiceInstanceListenerTest
        {
            // Parameters
            readonly StatelessServiceContext context = fuzzy.StatelessServiceContext();

            // Fixture
            readonly ICommunicationListener listener = Mock.Of<ICommunicationListener>();

            public Instantiate() =>
                Mock.Get(createCommunicationListener).Setup(_ => _.Invoke(context)).Returns(listener);

            [Fact]
            public void ReturnsTracingListener()
            {
                CommunicationListenerInfo actual = ServiceInstanceListener.Instantiate(sut, context);

                Assert.Same(name, actual.Name);
                var tracer = Assert.IsType<TracingCommunicationListener>(actual.Listener);
                var original = tracer.Field<CommunicationListenerInfo>().Value;
                Assert.Equal(new CommunicationListenerInfo(name, listener), original);
                var trace = Assert.IsType<Trace>(tracer.Field<ITrace>().Value);
                Assert.Equal(new Trace(typeof(ServiceInstanceListener), context, ServiceEventSource.Instance), trace);
            }

            [Fact]
            public void ReturnsTracingListenerWithDefaultName()
            {
                var sut = new ServiceInstanceListener(createCommunicationListener);

                CommunicationListenerInfo actual = ServiceInstanceListener.Instantiate(sut, context);

                string expectedName = "default";
                Assert.Same(expectedName, actual.Name);
                var tracer = Assert.IsType<TracingCommunicationListener>(actual.Listener);
                var original = tracer.Field<CommunicationListenerInfo>().Value;
                Assert.Equal(new CommunicationListenerInfo(expectedName, listener), original);
                var trace = Assert.IsType<Trace>(tracer.Field<ITrace>().Value);
                Assert.Equal(new Trace(typeof(ServiceInstanceListener), context, ServiceEventSource.Instance), trace);
            }

            [Fact]
            public void ReturnsNullWhenCreateCommunicationListenerReturnsNull()
            {
                Mock.Get(createCommunicationListener).Setup(_ => _.Invoke(context)).Returns(default(ICommunicationListener));
                Assert.Null(ServiceInstanceListener.Instantiate(sut, context));
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenListenerIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ServiceInstanceListener.Instantiate(null, context));
                Assert.Equal("listener", exception.ParamName);
            }
        }
    }
}
