// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fuzzy;
using Inspector;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Runtime
{
    public abstract class StatelessServiceInstanceAdapterTest
    {
        readonly IStatelessServiceInstance sut;

        // Constructor parameters
        readonly StatelessServiceContext context = fuzzy.StatelessServiceContext();
        readonly IStatelessUserServiceInstance userServiceInstance = new Mock<IStatelessUserServiceInstance> { DefaultValue = DefaultValue.Mock }.Object;

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        protected StatelessServiceInstanceAdapterTest() =>
            sut = new StatelessServiceInstanceAdapter(context, userServiceInstance);

        public sealed class Constructor : StatelessServiceInstanceAdapterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenContextIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new StatelessServiceInstanceAdapter(null, userServiceInstance));
                Assert.Equal(nameof(context), exception.ParamName);
            }

            [Fact]
            public void ThrowsArgumentNullExceptionWhenUserServiceInstanceIsNull()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new StatelessServiceInstanceAdapter(context, null));
                Assert.Equal(nameof(userServiceInstance), exception.ParamName);
            }

            [Fact]
            public void UsesServiceInstanceListenerInstantiateToCreateCommunicationListeners()
            {
                Assert.Equal(ServiceInstanceListener.Instantiate, sut.Field<Func<ServiceInstanceListener, StatelessServiceContext, CommunicationListenerInfo>>().Value);
            }
        }

        public sealed class OpenAsync : StatelessServiceInstanceAdapterTest
        {
            readonly IStatelessServicePartition partition = Mock.Of<IStatelessServicePartition>();
            readonly CancellationToken cancellation = new CancellationToken();

            [Fact]
            public async Task ToPrimaryCreatesAndOpensCommunicationListeners()
            {
                // Arrange
                IEnumerable<ServiceInstanceListener> instanceListeners = fuzzy.Array(fuzzy.ServiceInstanceListener);
                Mock.Get(userServiceInstance).Setup(_ => _.CreateServiceInstanceListeners()).Returns(instanceListeners);

                var createCommunicationListener = new Mock<Func<ServiceInstanceListener, StatelessServiceContext, CommunicationListenerInfo>>();
                IDictionary<ServiceInstanceListener, CommunicationListenerInfo> communicationListeners = instanceListeners.ToDictionary(_ => _, _ => fuzzy.CommunicationListenerInfo());
                createCommunicationListener.Setup(_ => _.Invoke(It.IsAny<ServiceInstanceListener>(), context))
                    .Returns((ServiceInstanceListener InstanceListener, StatelessServiceContext context) => communicationListeners[InstanceListener]);

                sut.Field<Func<ServiceInstanceListener, StatelessServiceContext, CommunicationListenerInfo>>().Set(createCommunicationListener.Object);

                // Act
                await sut.OpenAsync(partition, cancellation);

                // Assert
                IList<CommunicationListenerInfo> expected = communicationListeners.Values.ToList();
                var actual = sut.Field<IList<CommunicationListenerInfo>>().Value;
                Assert.Equal(expected, actual);
            }
        }
    }
}
