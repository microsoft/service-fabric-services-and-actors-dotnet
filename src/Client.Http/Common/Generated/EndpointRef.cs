// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a reference to a service endpoint.
    /// </summary>
    public partial class EndpointRef
    {
        /// <summary>
        /// Initializes a new instance of the EndpointRef class.
        /// </summary>
        /// <param name="name">Name of the endpoint.</param>
        public EndpointRef(
            string name = default(string))
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets name of the endpoint.
        /// </summary>
        public string Name { get; }
    }
}
