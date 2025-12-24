// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.AspNetCore.Configuration;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test for ServiceFabricConfigurationProvider.
    /// </summary>
    public class ServiceFabricConfigurationProviderTest
    {
        private int valueCount = 0;
        private int sectionCount = 0;

        /// <summary>
        /// Verify that the basic types could be loaded.
        /// </summary>
        [Fact]
        public void TestHappyCase()
        {
            var contextConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Section1:Name", "Xiaoxiao" },
                { "Section1:Age", "6" },
                { "Section1:Gender", "M" },
                { "Section2:Gender", "F" },
            }).Build();

            var context = new TestCodePackageActivationContext(contextConfig);
            var names = context.GetCodePackageNames();
            names.Count.Should().Be(1, "Only 1 config package");

            var builder = new ConfigurationBuilder();
            builder.AddServiceFabricConfiguration(context);
            var config = builder.Build();

            config["Config:Section1:Name"].Should().Be("Xiaoxiao");
            config["Section1:Name"].Should().BeNull("Default behavior shall include the package name in key.");
            config["Config:Section1:Age"].Should().Be("6");
            config["Config:Gender"].Should().Be(null);
            config["Config:Section1:Gender"].Should().Be("M");
            config["Config:Section2:Gender"].Should().Be("F");

            // basic validate to bind to a class directly
            // Note, in asp.net core 2.1 you could use the more simple ConfigurationBinder.Get<T> binds and returns the specified type instance directly.
            // Get<T> is more convenient than using Bind but will require .net core version higher than 1.0
            var person = new Person();
            config.GetSection("Config:Section1").Bind(person);

            person.Name.Should().Be("Xiaoxiao");
            person.Age.Should().Be(6);
            person.Gender.Should().Be("M");
        }

        /// <summary>
        /// Verify the configuration updates.
        /// </summary>
        [Fact]
        public void TestConfigUpdate()
        {
            var contextConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Section1:Name", "Xiaoxiao" },
                { "Section1:Age", "6" },
                { "Section1:Gender", "M" },
            }).Build();

            var context = new TestCodePackageActivationContext(contextConfig);

            var builder = new ConfigurationBuilder();
            builder.AddServiceFabricConfiguration(context);
            var config = builder.Build();

            config["Config:Section1:Name"].Should().Be("Xiaoxiao");
            config["Config:Section1:Age"].Should().Be("6");
            config["Config:Section1:Gender"].Should().Be("M");

            // trigger config update
            context.TriggerConfigurationPackageModifiedEvent(
                new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Section1:Name", "Lele" },
                { "Section1:Age", "3" },
                { "Section1:Gender", "M" },
            }).Build(),
                "Config");

            config["Config:Section1:Name"].Should().Be("Lele");
            config["Config:Section1:Age"].Should().Be("3");
            config["Config:Section1:Gender"].Should().Be("M");
        }

        /// <summary>
        /// Tests the empty configuration.
        /// </summary>
        [Fact]
        public void TestEmptyConfig()
        {
            var contextConfig = new ConfigurationBuilder().Build();
            var context = new TestCodePackageActivationContext(contextConfig);

            var builder = new ConfigurationBuilder();
            builder.AddServiceFabricConfiguration(context);
            var config = builder.Build();

            config.GetChildren().Should().BeEmpty();
        }

        /// <summary>
        /// Tests the multi configs.
        /// </summary>
        [Fact]
        public void TestMultiConfigsWithUpdate()
        {
            // Case 1: Configuration is loaded correctly from multiple providers
            var contextConfig1 = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "SameSection:Name", "Xiaoxiao" },
                { "Section1:Age", "6" },
                { "Section1:Gender", "M" },
            }).Build();

            var contextConfig2 = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "SameSection:Name", "Lele" },
                { "Section2:Age", "3" },
                { "Section2:Gender", "M" },
            }).Build();

            var context = new TestCodePackageActivationContext(new Dictionary<string, IConfiguration>() { { "Config1", contextConfig1 }, { "Config2", contextConfig2 } });

            var builder = new ConfigurationBuilder();
            builder.AddServiceFabricConfiguration(context);
            var config = builder.Build() as ConfigurationRoot;

            config["Config1:SameSection:Name"].Should().Be("Xiaoxiao");
            config["Config1:Section1:Age"].Should().Be("6");
            config["Config1:Section1:Gender"].Should().Be("M");

            config["Config2:SameSection:Name"].Should().Be("Lele");
            config["Config2:Section2:Age"].Should().Be("3");
            config["Config2:Section2:Gender"].Should().Be("M");

            // Case 2: ServiceFabricConfigurationProvider only loads configuration from the ConfigPackage it is mapped to
            //  (and does not load from other ConfigPackages) when a config update event is triggered
            context.TriggerConfigurationPackageModifiedEvent(
                new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "SameSection:Name", "Jill" },
                { "Section1:Age", "30" },
                { "Section1:Gender", "F" },
            }).Build(),
                "Config1");

            config["Config1:SameSection:Name"].Should().Be("Jill");
            config["Config1:Section1:Age"].Should().Be("30");
            config["Config1:Section1:Gender"].Should().Be("F");

            config["Config2:SameSection:Name"].Should().Be("Lele");
            config["Config2:Section2:Age"].Should().Be("3");
            config["Config2:Section2:Gender"].Should().Be("M");
        }

        /// <summary>
        /// Tests the security configuration.
        /// </summary>
        [Fact]
        public void TestEncryptedConfig()
        {
            var contextConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                // in the MockConfigurationProperties this section is special handled to turn IsEncrypted to true as follow
                // parameter.Set(nameof(ConfigurationProperty.IsEncrypted), item.Key.Contains("Security") || item.Value.Contains("Security"));
                { "SecuritySection:SecuritySSN", "EncryptedValue" },
            }).Build();

            var context = new TestCodePackageActivationContext(contextConfig);

            var builder = new ConfigurationBuilder();
            builder.AddServiceFabricConfiguration(context);
            var config = builder.Build();

            config["Config:SecuritySection:SecuritySSN"].Should().Be("EncryptedValue");

            var builder2 = new ConfigurationBuilder();

            // set flag to decrypt the value
            builder2.AddServiceFabricConfiguration(context, (options) => options.DecryptValue = true);

            Action config2 = () => builder2.Build();
            config2.Should().Throw<Exception>("Exception expected here because DecryptValue will fail here with invalid values.");
        }

        /// <summary>
        /// Tests the configuration action.
        /// </summary>
        [Fact]
        public void TestConfigAction()
        {
            var contextConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Section1:Name", "Xiaoxiao" },
                { "Section1:Age", "6" },
            }).Build();

            // initial load
            var context = new TestCodePackageActivationContext(contextConfig);
            this.valueCount = 0;
            this.sectionCount = 0;
            var builder = new ConfigurationBuilder();
            builder.AddServiceFabricConfiguration(context, (options) =>
                {
                    options.ConfigAction = (package, configData) =>
                    {
                        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                        ILogger logger = loggerFactory.CreateLogger("test");
                        logger.LogInformation($"Config Update for package {package.Path} started");

                        foreach (var section in package.Settings.Sections)
                        {
                            this.sectionCount++;

                            foreach (var param in section.Parameters)
                            {
                                configData[options.ExtractKeyFunc(section, param)] = options.ExtractValueFunc(section, param);
                                this.valueCount++;
                            }
                        }

                        logger.LogInformation($"Config Update for package {package.Path} finished");
                    };

                    options.IncludePackageName = false;
                });

            var config = builder.Build();
            config["Section1:Name"].Should().Be("Xiaoxiao");
            config["Section1:Age"].Should().Be("6");
            this.sectionCount.Should().Be(1);
            this.valueCount.Should().Be(2);

            this.valueCount = 0;
            this.sectionCount = 0;

            // trigger config update
            context.TriggerConfigurationPackageModifiedEvent(
                new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Section1:Name", "Lele" },
            }).Build(), "Config");

            config["Section1:Name"].Should().Be("Lele");
            this.sectionCount.Should().Be(1);
            this.valueCount.Should().Be(1);
        }

        internal class Person
        {
            public string Name { get; set; }

            public string Gender { get; set; }

            public int Age { get; set; }
        }
    }
}
