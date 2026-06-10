// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyStateManagerChangedEventArgsTest
{
    public sealed class Action : NotifyStateManagerChangedEventArgsTest
    {
        [Theory]
        [InlineData(NotifyStateManagerChangedAction.Add)]
        [InlineData(NotifyStateManagerChangedAction.Remove)]
        [InlineData(NotifyStateManagerChangedAction.Rebuild)]
        public void ReturnsActionPassedToConstructor(NotifyStateManagerChangedAction action) =>
            Assert.Equal(action, new TestArgs(action).Action);

        sealed class TestArgs(NotifyStateManagerChangedAction action) : NotifyStateManagerChangedEventArgs(action);
    }
}
