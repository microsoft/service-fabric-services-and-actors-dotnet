// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a managed application identity.
    /// </summary>
    public partial class ManagedApplicationIdentity
    {
        /// <summary>
        /// Initializes a new instance of the ManagedApplicationIdentity class.
        /// </summary>
        /// <param name="name">The name of the identity.</param>
        /// <param name="principalId">The identity's PrincipalId.</param>
        public ManagedApplicationIdentity(
            string name,
            string principalId = default(string))
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;
            this.PrincipalId = principalId;
        }

        /// <summary>
        /// Gets the name of the identity.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the identity's PrincipalId.
        /// </summary>
        public string PrincipalId { get; }
    }
}
