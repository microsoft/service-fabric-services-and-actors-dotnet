// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    /// <summary>
    /// Enumerates how communication is protected.
    /// </summary>
    public enum ProtectionLevel
    {
        /// <summary>
        /// Not protected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only integrity is protected.
        /// </summary>
        Sign = 1,

        /// <summary>
        /// Both confidentiality and integrity are protected.
        /// </summary>
        EncryptAndSign = 2,
    }
}
