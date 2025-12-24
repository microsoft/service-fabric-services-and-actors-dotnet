// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Fabric.Description;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Mock implementation of ConfigurationProperties.
    /// </summary>
    public class MockConfigurationProperties : KeyedCollection<string, ConfigurationProperty>
    {
        /// <summary>
        /// Creates the default.
        /// </summary>
        /// <returns>the mock configuration properties.</returns>
        internal static MockConfigurationProperties CreateDefault(IConfigurationSection section)
        {
            var parameters = new MockConfigurationProperties();

            foreach (var item in section.GetChildren())
            {
                var parameter = TestHelper.CreateInstanced<ConfigurationProperty>();
                parameter.Set("Name", item.Key);
                parameter.Set("Value", item.Value);
                parameter.Set(nameof(ConfigurationProperty.IsEncrypted), item.Key.Contains("Security") || item.Value.Contains("Security"));
                parameters.Add(parameter);
            }

            return parameters;
        }

        /// <inheritdoc/>
        protected override string GetKeyForItem(ConfigurationProperty item)
        {
            return item.Name;
        }
    }
}
