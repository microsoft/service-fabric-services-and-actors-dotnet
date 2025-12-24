// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using System.Fabric.Description;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Mock implementation of the package.
    /// </summary>
    public static class MockConfigurationPackage
    {
        internal static ConfigurationPackage CreateDefaultPackage(IConfiguration config, string packageName)
        {
            var basePath = Environment.CurrentDirectory;

            var settings = TestHelper.CreateInstanced<ConfigurationSettings>();
            var section = TestHelper.CreateInstanced<System.Fabric.Description.ConfigurationSection>();
            settings.Set(nameof(ConfigurationSettings.Sections), MockConfigurationSections.CreateDefault(config));

            var desc = TestHelper.CreateInstanced<ConfigurationPackageDescription>();
            desc.Set("Name", packageName);
            desc.Set("Version", "1.0");
            desc.Set("Path", $"{basePath}\\{packageName}\\PackageRoot\\Config\\");
            desc.Set("Settings", settings);

            var package = TestHelper.CreateInstanced<ConfigurationPackage>();
            package.Set("Description", desc);

            return package;
        }
    }
}
