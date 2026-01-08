// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information describing the identities associated with this application.
    /// </summary>
    public partial class IdentityDescription
    {
        /// <summary>
        /// Initializes a new instance of the IdentityDescription class.
        /// </summary>
        /// <param name="type">the types of identities associated with this resource; currently restricted to 'SystemAssigned
        /// and UserAssigned'</param>
        /// <param name="tokenServiceEndpoint">the endpoint for the token service managing this identity</param>
        /// <param name="tenantId">the identifier of the tenant containing the application's identity.</param>
        /// <param name="principalId">the object identifier of the Service Principal of the identity associated with this
        /// resource.</param>
        /// <param name="userAssignedIdentities">represents user assigned identities map.</param>
        public IdentityDescription(
            string type,
            string tokenServiceEndpoint = default(string),
            string tenantId = default(string),
            string principalId = default(string),
            IReadOnlyDictionary<string, IdentityItemDescription> userAssignedIdentities = default(IReadOnlyDictionary<string, IdentityItemDescription>))
        {
            type.ThrowIfNull(nameof(type));
            this.Type = type;
            this.TokenServiceEndpoint = tokenServiceEndpoint;
            this.TenantId = tenantId;
            this.PrincipalId = principalId;
            this.UserAssignedIdentities = userAssignedIdentities;
        }

        /// <summary>
        /// Gets the endpoint for the token service managing this identity
        /// </summary>
        public string TokenServiceEndpoint { get; }

        /// <summary>
        /// Gets the types of identities associated with this resource; currently restricted to 'SystemAssigned and
        /// UserAssigned'
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the identifier of the tenant containing the application's identity.
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// Gets the object identifier of the Service Principal of the identity associated with this resource.
        /// </summary>
        public string PrincipalId { get; }

        /// <summary>
        /// Gets represents user assigned identities map.
        /// </summary>
        public IReadOnlyDictionary<string, IdentityItemDescription> UserAssignedIdentities { get; }
    }
}
