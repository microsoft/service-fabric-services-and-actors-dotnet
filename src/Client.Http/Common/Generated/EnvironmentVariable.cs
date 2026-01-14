// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes an environment variable for the container.
    /// </summary>
    public partial class EnvironmentVariable
    {
        /// <summary>
        /// Initializes a new instance of the EnvironmentVariable class.
        /// </summary>
        /// <param name="type">The type of the environment variable being given in value. Possible values include: 'ClearText',
        /// 'KeyVaultReference', 'SecretValueReference'</param>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="value">The value of the environment variable, will be processed based on the type provided.</param>
        public EnvironmentVariable(
            EnvironmentVariableType? type = Common.EnvironmentVariableType.ClearText,
            string name = default(string),
            string value = default(string))
        {
            this.Type = type;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the type of the environment variable being given in value. Possible values include: 'ClearText',
        /// 'KeyVaultReference', 'SecretValueReference'
        /// </summary>
        public EnvironmentVariableType? Type { get; }

        /// <summary>
        /// Gets the name of the environment variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the environment variable, will be processed based on the type provided.
        /// </summary>
        public string Value { get; }
    }
}
