// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters for updating an application instance.
    /// </summary>
    public partial class ApplicationUpdateDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationUpdateDescription class.
        /// </summary>
        /// <param name="flags">Flags indicating whether other properties are set. Each of the associated properties
        /// corresponds to a flag, specified below, which, if set, indicate that the property is specified.
        /// If flags are not specified for a certain property, the property will not be updated even if the new value is
        /// provided.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator. Exception is
        /// RemoveApplicationCapacity which cannot be specified along with other parameters.
        /// For example, if the provided value is 3 then the flags for MinimumNodes (1) and MaximumNodes (2) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is 0.
        /// - MinimumNodes - Indicates whether the MinimumNodes property is set. The value is 1.
        /// - MaximumNodes - Indicates whether the MinimumNodes property is set. The value is  2.
        /// - ApplicationMetrics - Indicates whether the ApplicationMetrics property is set. The value is 4.
        /// </param>
        /// <param name="removeApplicationCapacity">Used to clear all parameters related to Application Capacity for this
        /// application. |
        /// It is not possible to specify this parameter together with other Application Capacity parameters.
        /// </param>
        /// <param name="minimumNodes">The minimum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. If this property is set to zero, no capacity will be reserved. The value of this property cannot be more
        /// than the value of the MaximumNodes property.</param>
        /// <param name="maximumNodes">The maximum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. By default, the value of this property is zero and it means that the services can be placed on any
        /// node.</param>
        /// <param name="applicationMetrics">List of application capacity metric description.</param>
        public ApplicationUpdateDescription(
            string flags = default(string),
            bool? removeApplicationCapacity = false,
            long? minimumNodes = default(long?),
            long? maximumNodes = 0,
            IEnumerable<ApplicationMetricDescription> applicationMetrics = default(IEnumerable<ApplicationMetricDescription>))
        {
            minimumNodes?.ThrowIfLessThan("minimumNodes", 0);
            maximumNodes?.ThrowIfLessThan("maximumNodes", 0);
            this.Flags = flags;
            this.RemoveApplicationCapacity = removeApplicationCapacity;
            this.MinimumNodes = minimumNodes;
            this.MaximumNodes = maximumNodes;
            this.ApplicationMetrics = applicationMetrics;
        }

        /// <summary>
        /// Gets flags indicating whether other properties are set. Each of the associated properties corresponds to a flag,
        /// specified below, which, if set, indicate that the property is specified.
        /// If flags are not specified for a certain property, the property will not be updated even if the new value is
        /// provided.
        /// This property can be a combination of those flags obtained using bitwise 'OR' operator. Exception is
        /// RemoveApplicationCapacity which cannot be specified along with other parameters.
        /// For example, if the provided value is 3 then the flags for MinimumNodes (1) and MaximumNodes (2) are set.
        /// 
        /// - None - Does not indicate any other properties are set. The value is 0.
        /// - MinimumNodes - Indicates whether the MinimumNodes property is set. The value is 1.
        /// - MaximumNodes - Indicates whether the MinimumNodes property is set. The value is  2.
        /// - ApplicationMetrics - Indicates whether the ApplicationMetrics property is set. The value is 4.
        /// </summary>
        public string Flags { get; }

        /// <summary>
        /// Gets used to clear all parameters related to Application Capacity for this application. |
        /// It is not possible to specify this parameter together with other Application Capacity parameters.
        /// </summary>
        public bool? RemoveApplicationCapacity { get; }

        /// <summary>
        /// Gets the minimum number of nodes where Service Fabric will reserve capacity for this application. Note that this
        /// does not mean that the services of this application will be placed on all of those nodes. If this property is set
        /// to zero, no capacity will be reserved. The value of this property cannot be more than the value of the MaximumNodes
        /// property.
        /// </summary>
        public long? MinimumNodes { get; }

        /// <summary>
        /// Gets the maximum number of nodes where Service Fabric will reserve capacity for this application. Note that this
        /// does not mean that the services of this application will be placed on all of those nodes. By default, the value of
        /// this property is zero and it means that the services can be placed on any node.
        /// </summary>
        public long? MaximumNodes { get; }

        /// <summary>
        /// Gets list of application capacity metric description.
        /// </summary>
        public IEnumerable<ApplicationMetricDescription> ApplicationMetrics { get; }
    }
}
