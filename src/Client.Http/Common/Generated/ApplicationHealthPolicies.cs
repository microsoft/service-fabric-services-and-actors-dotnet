// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the application health policy map used to evaluate the health of an application or one of its children
    /// entities.
    /// </summary>
    public partial class ApplicationHealthPolicies
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthPolicies class.
        /// </summary>
        /// <param name="applicationHealthPolicyMap">The wrapper that contains the map with application health policies used to
        /// evaluate specific applications in the cluster.</param>
        public ApplicationHealthPolicies(
            IEnumerable<ApplicationHealthPolicyMapItem> applicationHealthPolicyMap = default(IEnumerable<ApplicationHealthPolicyMapItem>))
        {
            this.ApplicationHealthPolicyMap = applicationHealthPolicyMap;
        }

        /// <summary>
        /// Gets the wrapper that contains the map with application health policies used to evaluate specific applications in
        /// the cluster.
        /// </summary>
        public IEnumerable<ApplicationHealthPolicyMapItem> ApplicationHealthPolicyMap { get; }
    }
}
