// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Credential information to connect to container registry.
    /// </summary>
    public partial class RegistryCredential
    {
        /// <summary>
        /// Initializes a new instance of the RegistryCredential class.
        /// </summary>
        /// <param name="registryUserName">The user name to connect to container registry.</param>
        /// <param name="registryPassword">The password for supplied username to connect to container registry.</param>
        /// <param name="passwordEncrypted">Indicates that supplied container registry password is encrypted.</param>
        public RegistryCredential(
            string registryUserName = default(string),
            string registryPassword = default(string),
            bool? passwordEncrypted = default(bool?))
        {
            this.RegistryUserName = registryUserName;
            this.RegistryPassword = registryPassword;
            this.PasswordEncrypted = passwordEncrypted;
        }

        /// <summary>
        /// Gets the user name to connect to container registry.
        /// </summary>
        public string RegistryUserName { get; }

        /// <summary>
        /// Gets the password for supplied username to connect to container registry.
        /// </summary>
        public string RegistryPassword { get; }

        /// <summary>
        /// Gets indicates that supplied container registry password is encrypted.
        /// </summary>
        public bool? PasswordEncrypted { get; }
    }
}
