// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
