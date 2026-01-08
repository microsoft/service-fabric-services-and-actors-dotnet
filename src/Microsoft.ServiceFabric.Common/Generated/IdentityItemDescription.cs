// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a single user-assigned identity associated with the application.
    /// </summary>
    public partial class IdentityItemDescription
    {
        /// <summary>
        /// Initializes a new instance of the IdentityItemDescription class.
        /// </summary>
        /// <param name="principalId">the object identifier of the Service Principal which this identity represents.</param>
        /// <param name="clientId">the client identifier of the Service Principal which this identity represents.</param>
        public IdentityItemDescription(
            string principalId = default(string),
            string clientId = default(string))
        {
            this.PrincipalId = principalId;
            this.ClientId = clientId;
        }

        /// <summary>
        /// Gets the object identifier of the Service Principal which this identity represents.
        /// </summary>
        public string PrincipalId { get; }

        /// <summary>
        /// Gets the client identifier of the Service Principal which this identity represents.
        /// </summary>
        public string ClientId { get; }
    }
}
