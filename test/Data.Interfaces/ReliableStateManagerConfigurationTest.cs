// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Threading.Tasks;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class ReliableStateManagerConfigurationTest
{
    readonly ReliableStateManagerConfiguration sut;

    // Constructor parameters
    readonly string configPackageName = fuzzy.String();
    readonly string replicatorSecuritySectionName = fuzzy.String();
    readonly string replicatorSettingsSectionName = fuzzy.String();
    readonly Func<Task> onInitializeStateSerializersEvent = static () => Task.CompletedTask;

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ReliableStateManagerConfigurationTest() =>
        sut = new ReliableStateManagerConfiguration(
            configPackageName,
            replicatorSecuritySectionName,
            replicatorSettingsSectionName,
            onInitializeStateSerializersEvent);

    public sealed class Constructor_ReliableStateManagerReplicatorSettings_FuncOfTask : ReliableStateManagerConfigurationTest
    {
        readonly ReliableStateManagerReplicatorSettings replicatorSettings = new();

        [Fact]
        public void SetsAllProperties()
        {
            var sut = new ReliableStateManagerConfiguration(replicatorSettings, onInitializeStateSerializersEvent);

            Assert.Same(replicatorSettings, sut.ReplicatorSettings);
            Assert.Null(sut.ConfigPackageName);
            Assert.Null(sut.ReplicatorSecuritySectionName);
            Assert.Null(sut.ReplicatorSettingsSectionName);
            Assert.Same(onInitializeStateSerializersEvent, sut.OnInitializeStateSerializersEvent);
        }
    }

    public sealed class Constructor_String_String_String_FuncOfTask : ReliableStateManagerConfigurationTest
    {
        [Fact]
        public async Task SetsAllPropertiesToDefaultsWhenCalledWithoutArguments()
        {
            var sut = new ReliableStateManagerConfiguration();

            Assert.Null(sut.ReplicatorSettings);
            Assert.Equal("Config", sut.ConfigPackageName);
            Assert.Equal("ReplicatorSecurityConfig", sut.ReplicatorSecuritySectionName);
            Assert.Equal("ReplicatorConfig", sut.ReplicatorSettingsSectionName);
            await sut.OnInitializeStateSerializersEvent();
        }

        [Fact]
        public void SetsAllProperties()
        {
            Assert.Null(sut.ReplicatorSettings);
            Assert.Same(configPackageName, sut.ConfigPackageName);
            Assert.Same(replicatorSecuritySectionName, sut.ReplicatorSecuritySectionName);
            Assert.Same(replicatorSettingsSectionName, sut.ReplicatorSettingsSectionName);
            Assert.Same(onInitializeStateSerializersEvent, sut.OnInitializeStateSerializersEvent);
        }
    }
}
