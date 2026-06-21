// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using Inspector;
using Microsoft.Extensions.Configuration;
using ConfigurationSection = System.Fabric.Description.ConfigurationSection;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration;

static class MockConfigurationPackage
{
    internal static ConfigurationPackage CreateDefaultPackage(IConfiguration config, string packageName)
    {
        string basePath = Environment.CurrentDirectory;

        var settings = Type<ConfigurationSettings>.New();
        settings.Property<KeyedCollection<string, ConfigurationSection>>().Set(ConfigurationSections(config));

        var desc = Type<ConfigurationPackageDescription>.New();
        desc.Property<string>(nameof(desc.Name)).Set(packageName);
        desc.Property<string>(nameof(desc.Version)).Set("1.0");
        desc.Property<string>(nameof(desc.Path)).Set($"{basePath}\\{packageName}\\PackageRoot\\Config\\");
        desc.Property<ConfigurationSettings>().Set(settings);

        return Type<ConfigurationPackage>.New(desc);
    }

    static KeyedCollection<string, ConfigurationSection> ConfigurationSections(IConfiguration config)
    {
        StubKeyedCollection<string, ConfigurationSection> sections = new(_ => _.Name);

        foreach (IConfigurationSection item in config.GetChildren())
        {
            var section = Type<ConfigurationSection>.New();
            section.Property<string>().Set(item.Key);
            section.Property<KeyedCollection<string, ConfigurationProperty>>().Set(ConfigurationProperties(item));
            sections.Add(section);
        }

        return sections;
    }

    static KeyedCollection<string, ConfigurationProperty> ConfigurationProperties(IConfigurationSection section)
    {
        StubKeyedCollection<string, ConfigurationProperty> parameters = new(_ => _.Name);

        foreach (IConfigurationSection item in section.GetChildren())
        {
            var parameter = Type<ConfigurationProperty>.New();
            parameter.Property<string>(nameof(parameter.Name)).Set(item.Key);
            parameter.Property<string>(nameof(parameter.Value)).Set(item.Value);
            parameter.Property<bool>(nameof(parameter.IsEncrypted)).Set(item.Key.Contains("Security") || item.Value.Contains("Security"));
            parameters.Add(parameter);
        }

        return parameters;
    }
}
