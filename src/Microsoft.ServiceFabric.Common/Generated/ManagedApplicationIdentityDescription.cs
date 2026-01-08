// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Managed application identity description.
    /// </summary>
    public partial class ManagedApplicationIdentityDescription
    {
        /// <summary>
        /// Initializes a new instance of the ManagedApplicationIdentityDescription class.
        /// </summary>
        /// <param name="tokenServiceEndpoint">Token service endpoint.</param>
        /// <param name="managedIdentities">A list of managed application identity objects.</param>
        public ManagedApplicationIdentityDescription(
            string tokenServiceEndpoint = default(string),
            IEnumerable<ManagedApplicationIdentity> managedIdentities = default(IEnumerable<ManagedApplicationIdentity>))
        {
            this.TokenServiceEndpoint = tokenServiceEndpoint;
            this.ManagedIdentities = managedIdentities;
        }

        /// <summary>
        /// Gets token service endpoint.
        /// </summary>
        public string TokenServiceEndpoint { get; }

        /// <summary>
        /// Gets a list of managed application identity objects.
        /// </summary>
        public IEnumerable<ManagedApplicationIdentity> ManagedIdentities { get; }
    }
}
