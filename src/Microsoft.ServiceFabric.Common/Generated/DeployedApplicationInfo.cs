// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about application deployed on the node.
    /// </summary>
    public partial class DeployedApplicationInfo
    {
        /// <summary>
        /// Initializes a new instance of the DeployedApplicationInfo class.
        /// </summary>
        /// <param name="id">The identity of the application. This is an encoded representation of the application name. This
        /// is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="typeName">The application type name as defined in the application manifest.</param>
        /// <param name="status">The status of the application deployed on the node. Following are the possible values.
        /// . Possible values include: 'Invalid', 'Downloading', 'Activating', 'Active', 'Upgrading', 'Deactivating'</param>
        /// <param name="workDirectory">The work directory of the application on the node. The work directory can be used to
        /// store application data.</param>
        /// <param name="logDirectory">The log directory of the application on the node. The log directory can be used to store
        /// application logs.</param>
        /// <param name="tempDirectory">The temp directory of the application on the node. The code packages belonging to the
        /// application are forked with this directory set as their temporary directory.</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        public DeployedApplicationInfo(
            string id = default(string),
            ApplicationName name = default(ApplicationName),
            string typeName = default(string),
            DeployedApplicationStatus? status = default(DeployedApplicationStatus?),
            string workDirectory = default(string),
            string logDirectory = default(string),
            string tempDirectory = default(string),
            HealthState? healthState = default(HealthState?))
        {
            this.Id = id;
            this.Name = name;
            this.TypeName = typeName;
            this.Status = status;
            this.WorkDirectory = workDirectory;
            this.LogDirectory = logDirectory;
            this.TempDirectory = tempDirectory;
            this.HealthState = healthState;
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
        /// Gets the name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        public ApplicationName Name { get; }

        /// <summary>
        /// Gets the application type name as defined in the application manifest.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Gets the status of the application deployed on the node. Following are the possible values.
        /// . Possible values include: 'Invalid', 'Downloading', 'Activating', 'Active', 'Upgrading', 'Deactivating'
        /// </summary>
        public DeployedApplicationStatus? Status { get; }

        /// <summary>
        /// Gets the work directory of the application on the node. The work directory can be used to store application data.
        /// </summary>
        public string WorkDirectory { get; }

        /// <summary>
        /// Gets the log directory of the application on the node. The log directory can be used to store application logs.
        /// </summary>
        public string LogDirectory { get; }

        /// <summary>
        /// Gets the temp directory of the application on the node. The code packages belonging to the application are forked
        /// with this directory set as their temporary directory.
        /// </summary>
        public string TempDirectory { get; }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; }
    }
}
