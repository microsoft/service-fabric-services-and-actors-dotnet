// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Endpoint of a resolved service partition.
    /// </summary>
    public partial class ResolvedServiceEndpoint
    {
        /// <summary>
        /// Initializes a new instance of the ResolvedServiceEndpoint class.
        /// </summary>
        /// <param name="kind">The role of the replica where the endpoint is reported. Possible values include: 'Invalid',
        /// 'Stateless', 'StatefulPrimary', 'StatefulSecondary'</param>
        /// <param name="address">The address of the endpoint. If the endpoint has multiple listeners the address is a JSON
        /// object with one property per listener with the value as the address of that listener.</param>
        public ResolvedServiceEndpoint(
            ServiceEndpointRole? kind = default(ServiceEndpointRole?),
            string address = default(string))
        {
            this.Kind = kind;
            this.Address = address;
        }

        /// <summary>
        /// Gets the role of the replica where the endpoint is reported. Possible values include: 'Invalid', 'Stateless',
        /// 'StatefulPrimary', 'StatefulSecondary'
        /// </summary>
        public ServiceEndpointRole? Kind { get; }

        /// <summary>
        /// Gets the address of the endpoint. If the endpoint has multiple listeners the address is a JSON object with one
        /// property per listener with the value as the address of that listener.
        /// </summary>
        public string Address { get; }
    }
}
