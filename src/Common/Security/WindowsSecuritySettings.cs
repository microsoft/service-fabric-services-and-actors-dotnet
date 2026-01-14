// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    /// <summary>
    /// Specifies the security settings for Windows credentials.
    /// </summary>
    public sealed class WindowsSecuritySettings : SecuritySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsSecuritySettings" /> class.
        /// </summary>
        public WindowsSecuritySettings() 
            : base(SecurityType.Windows)
        {
        }
    }
}
