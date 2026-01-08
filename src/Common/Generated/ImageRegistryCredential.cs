// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Image registry credential.
    /// </summary>
    public partial class ImageRegistryCredential
    {
        /// <summary>
        /// Initializes a new instance of the ImageRegistryCredential class.
        /// </summary>
        /// <param name="server">Docker image registry server, without protocol such as `http` and `https`.</param>
        /// <param name="username">The username for the private registry.</param>
        /// <param name="passwordType">The type of the image registry password being given in password. Possible values
        /// include: 'ClearText', 'KeyVaultReference', 'SecretValueReference'</param>
        /// <param name="password">The password for the private registry. The password is required for create or update
        /// operations, however it is not returned in the get or list operations. Will be processed based on the type
        /// provided.</param>
        public ImageRegistryCredential(
            string server,
            string username,
            ImageRegistryPasswordType? passwordType = Common.ImageRegistryPasswordType.ClearText,
            string password = default(string))
        {
            server.ThrowIfNull(nameof(server));
            username.ThrowIfNull(nameof(username));
            this.Server = server;
            this.Username = username;
            this.PasswordType = passwordType;
            this.Password = password;
        }

        /// <summary>
        /// Gets docker image registry server, without protocol such as `http` and `https`.
        /// </summary>
        public string Server { get; }

        /// <summary>
        /// Gets the username for the private registry.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the type of the image registry password being given in password. Possible values include: 'ClearText',
        /// 'KeyVaultReference', 'SecretValueReference'
        /// </summary>
        public ImageRegistryPasswordType? PasswordType { get; }

        /// <summary>
        /// Gets the password for the private registry. The password is required for create or update operations, however it is
        /// not returned in the get or list operations. Will be processed based on the type provided.
        /// </summary>
        public string Password { get; }
    }
}
