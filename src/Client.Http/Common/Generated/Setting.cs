// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a setting for the container. The setting file path can be fetched from environment variable
    /// "Fabric_SettingPath". The path for Windows container is "C:\\secrets". The path for Linux container is
    /// "/var/secrets".
    /// </summary>
    public partial class Setting
    {
        /// <summary>
        /// Initializes a new instance of the Setting class.
        /// </summary>
        /// <param name="type">The type of the setting being given in value. Possible values include: 'ClearText',
        /// 'KeyVaultReference', 'SecretValueReference'</param>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting, will be processed based on the type provided.</param>
        public Setting(
            SettingType? type = Common.SettingType.ClearText,
            string name = default(string),
            string value = default(string))
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the type of the setting being given in value. Possible values include: 'ClearText', 'KeyVaultReference',
        /// 'SecretValueReference'
        /// </summary>
        public SettingType? Type { get; }

        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the setting, will be processed based on the type provided.
        /// </summary>
        public string Value { get; }
    }
}
