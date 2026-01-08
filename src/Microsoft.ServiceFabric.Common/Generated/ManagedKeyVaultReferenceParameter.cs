// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a mapping from application parameters which are managed key vault references to the application managed
    /// identity which is used to access the vault.
    /// </summary>
    public partial class ManagedKeyVaultReferenceParameter
    {
        /// <summary>
        /// Initializes a new instance of the ManagedKeyVaultReferenceParameter class.
        /// </summary>
        /// <param name="parameterName">The application parameter which is a managed key vault reference.</param>
        /// <param name="applicationIdentityReference">The associated application managed identity used to access the
        /// vault.</param>
        public ManagedKeyVaultReferenceParameter(
            string parameterName = default(string),
            string applicationIdentityReference = default(string))
        {
            this.ParameterName = parameterName;
            this.ApplicationIdentityReference = applicationIdentityReference;
        }

        /// <summary>
        /// Gets the application parameter which is a managed key vault reference.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Gets the associated application managed identity used to access the vault.
        /// </summary>
        public string ApplicationIdentityReference { get; }
    }
}
