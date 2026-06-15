// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using Xunit;

namespace Microsoft.ServiceFabric.Data.Notifications;

public abstract class NotifyDictionaryChangedEventArgsTest
{
    public sealed class Action : NotifyDictionaryChangedEventArgsTest
    {
        [Theory]
        [InlineData(NotifyDictionaryChangedAction.Add)]
        [InlineData(NotifyDictionaryChangedAction.Update)]
        [InlineData(NotifyDictionaryChangedAction.Remove)]
        [InlineData(NotifyDictionaryChangedAction.Clear)]
        [InlineData(NotifyDictionaryChangedAction.Rebuild)]
        public void ReturnsActionPassedToConstructor(NotifyDictionaryChangedAction action) =>
            Assert.Equal(action, new TestArgs(action).Action);

        sealed class TestArgs(NotifyDictionaryChangedAction action) : NotifyDictionaryChangedEventArgs<string, int>(action);
    }
}
