// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the map of application health policies for a ServiceFabric cluster upgrade
    /// </summary>
    public partial class ApplicationHealthPolicyMapObject
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthPolicyMapObject class.
        /// </summary>
        /// <param name="applicationHealthPolicyMap">Defines a map that contains specific application health policies for
        /// different applications.
        /// Each entry specifies as key the application name and as value an ApplicationHealthPolicy used to evaluate the
        /// application health.
        /// If an application is not specified in the map, the application health evaluation uses the ApplicationHealthPolicy
        /// found in its application manifest or the default application health policy (if no health policy is defined in the
        /// manifest).
        /// The map is empty by default.
        /// </param>
        public ApplicationHealthPolicyMapObject(
            IEnumerable<ApplicationHealthPolicyMapItem> applicationHealthPolicyMap = default(IEnumerable<ApplicationHealthPolicyMapItem>))
        {
            this.ApplicationHealthPolicyMap = applicationHealthPolicyMap;
        }

        /// <summary>
        /// Gets defines a map that contains specific application health policies for different applications.
        /// Each entry specifies as key the application name and as value an ApplicationHealthPolicy used to evaluate the
        /// application health.
        /// If an application is not specified in the map, the application health evaluation uses the ApplicationHealthPolicy
        /// found in its application manifest or the default application health policy (if no health policy is defined in the
        /// manifest).
        /// The map is empty by default.
        /// </summary>
        public IEnumerable<ApplicationHealthPolicyMapItem> ApplicationHealthPolicyMap { get; }
    }
}
