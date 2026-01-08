// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Load Information about a Service Fabric application.
    /// </summary>
    public partial class ApplicationLoadInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationLoadInfo class.
        /// </summary>
        /// <param name="id">The identity of the application. This is an encoded representation of the application name. This
        /// is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="minimumNodes">The minimum number of nodes for this application.
        /// It is the number of nodes where Service Fabric will reserve Capacity in the cluster which equals to ReservedLoad *
        /// MinimumNodes for this Application instance.
        /// For applications that do not have application capacity defined this value will be zero.
        /// </param>
        /// <param name="maximumNodes">The maximum number of nodes where this application can be instantiated.
        /// It is the number of nodes this application is allowed to span.
        /// For applications that do not have application capacity defined this value will be zero.
        /// </param>
        /// <param name="nodeCount">The number of nodes on which this application is instantiated.
        /// For applications that do not have application capacity defined this value will be zero.
        /// </param>
        /// <param name="applicationLoadMetricInformation">List of application load metric information.</param>
        public ApplicationLoadInfo(
            string id = default(string),
            long? minimumNodes = default(long?),
            long? maximumNodes = default(long?),
            long? nodeCount = default(long?),
            IEnumerable<ApplicationLoadMetricInformation> applicationLoadMetricInformation = default(IEnumerable<ApplicationLoadMetricInformation>))
        {
            this.Id = id;
            this.MinimumNodes = minimumNodes;
            this.MaximumNodes = maximumNodes;
            this.NodeCount = nodeCount;
            this.ApplicationLoadMetricInformation = applicationLoadMetricInformation;
        }

        /// <summary>
        /// Gets the identity of the application. This is an encoded representation of the application name. This is used in
        /// the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the minimum number of nodes for this application.
        /// It is the number of nodes where Service Fabric will reserve Capacity in the cluster which equals to ReservedLoad *
        /// MinimumNodes for this Application instance.
        /// For applications that do not have application capacity defined this value will be zero.
        /// </summary>
        public long? MinimumNodes { get; }

        /// <summary>
        /// Gets the maximum number of nodes where this application can be instantiated.
        /// It is the number of nodes this application is allowed to span.
        /// For applications that do not have application capacity defined this value will be zero.
        /// </summary>
        public long? MaximumNodes { get; }

        /// <summary>
        /// Gets the number of nodes on which this application is instantiated.
        /// For applications that do not have application capacity defined this value will be zero.
        /// </summary>
        public long? NodeCount { get; }

        /// <summary>
        /// Gets list of application load metric information.
        /// </summary>
        public IEnumerable<ApplicationLoadMetricInformation> ApplicationLoadMetricInformation { get; }
    }
}
