// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for SettingType.
    /// </summary>
    public enum SettingType
    {
        /// <summary>
        /// The setting in clear text, will not be processed in any way and passed in as is.
        /// </summary>
        ClearText,

        /// <summary>
        /// The URI to a KeyVault secret version, will be resolved using the application's managed identity (this type is only
        /// valid if the app was assigned a managed identity) before getting passed in.
        /// </summary>
        KeyVaultReference,

        /// <summary>
        /// The reference to a SecretValue resource, will be resolved before getting passed in.
        /// </summary>
        SecretValueReference,
    }
}
