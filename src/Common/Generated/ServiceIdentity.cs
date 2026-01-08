// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Map service identity friendly name to an application identity.
    /// </summary>
    public partial class ServiceIdentity
    {
        /// <summary>
        /// Initializes a new instance of the ServiceIdentity class.
        /// </summary>
        /// <param name="name">The identity friendly name.</param>
        /// <param name="identityRef">The application identity name.</param>
        public ServiceIdentity(
            string name = default(string),
            string identityRef = default(string))
        {
            this.Name = name;
            this.IdentityRef = identityRef;
        }

        /// <summary>
        /// Gets the identity friendly name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the application identity name.
        /// </summary>
        public string IdentityRef { get; }
    }
}
