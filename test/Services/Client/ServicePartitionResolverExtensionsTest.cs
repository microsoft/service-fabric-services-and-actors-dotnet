// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Client;

public abstract class ServicePartitionResolverExtensionsTest
{
    public sealed class DisableNotification : ServicePartitionResolverExtensionsTest
    {
        readonly ServicePartitionResolver partitionResolver = new(Mock.Of<CreateFabricClientDelegate>());

        [Fact]
        public void SetsUseNotificationToFalse()
        {
            partitionResolver.DisableNotification();
            Assert.False(partitionResolver.UseNotification);
        }

        [Fact]
        public void ReturnsGivenPartitionResolver() =>
            Assert.Same(partitionResolver, partitionResolver.DisableNotification());

        // TODO: SUT bug. DisableNotification dereferences the null receiver instead of throwing ArgumentNullException.
        [Fact(Explicit = true)]
        public void ThrowsArgumentNullExceptionWhenPartitionResolverIsNull()
        {
            var e = Assert.Throws<ArgumentNullException>(() => ServicePartitionResolverExtensions.DisableNotification(null));
            Assert.Equal(nameof(partitionResolver), e.ParamName);
        }
    }
}
