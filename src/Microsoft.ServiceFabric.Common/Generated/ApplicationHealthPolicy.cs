// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a health policy used to evaluate the health of an application or one of its children entities.
    /// </summary>
    public partial class ApplicationHealthPolicy
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationHealthPolicy class.
        /// </summary>
        /// <param name="considerWarningAsError">Indicates whether warnings are treated with the same severity as
        /// errors.</param>
        /// <param name="maxPercentUnhealthyDeployedApplications">The maximum allowed percentage of unhealthy deployed
        /// applications. Allowed values are Byte values from zero to 100.
        /// The percentage represents the maximum tolerated percentage of deployed applications that can be unhealthy before
        /// the application is considered in error.
        /// This is calculated by dividing the number of unhealthy deployed applications over the number of nodes where the
        /// application is currently deployed on in the cluster.
        /// The computation rounds up to tolerate one failure on small numbers of nodes. Default percentage is zero.
        /// </param>
        /// <param name="defaultServiceTypeHealthPolicy">The health policy used by default to evaluate the health of a service
        /// type.</param>
        /// <param name="serviceTypeHealthPolicyMap">The map with service type health policy per service type name. The map is
        /// empty by default.</param>
        public ApplicationHealthPolicy(
            bool? considerWarningAsError = false,
            int? maxPercentUnhealthyDeployedApplications = 0,
            ServiceTypeHealthPolicy defaultServiceTypeHealthPolicy = default(ServiceTypeHealthPolicy),
            IEnumerable<ServiceTypeHealthPolicyMapItem> serviceTypeHealthPolicyMap = default(IEnumerable<ServiceTypeHealthPolicyMapItem>))
        {
            this.ConsiderWarningAsError = considerWarningAsError;
            this.MaxPercentUnhealthyDeployedApplications = maxPercentUnhealthyDeployedApplications;
            this.DefaultServiceTypeHealthPolicy = defaultServiceTypeHealthPolicy;
            this.ServiceTypeHealthPolicyMap = serviceTypeHealthPolicyMap;
        }

        /// <summary>
        /// Gets indicates whether warnings are treated with the same severity as errors.
        /// </summary>
        public bool? ConsiderWarningAsError { get; }

        /// <summary>
        /// Gets the maximum allowed percentage of unhealthy deployed applications. Allowed values are Byte values from zero to
        /// 100.
        /// The percentage represents the maximum tolerated percentage of deployed applications that can be unhealthy before
        /// the application is considered in error.
        /// This is calculated by dividing the number of unhealthy deployed applications over the number of nodes where the
        /// application is currently deployed on in the cluster.
        /// The computation rounds up to tolerate one failure on small numbers of nodes. Default percentage is zero.
        /// </summary>
        public int? MaxPercentUnhealthyDeployedApplications { get; }

        /// <summary>
        /// Gets the health policy used by default to evaluate the health of a service type.
        /// </summary>
        public ServiceTypeHealthPolicy DefaultServiceTypeHealthPolicy { get; }

        /// <summary>
        /// Gets the map with service type health policy per service type name. The map is empty by default.
        /// </summary>
        public IEnumerable<ServiceTypeHealthPolicyMapItem> ServiceTypeHealthPolicyMap { get; }
    }
}
