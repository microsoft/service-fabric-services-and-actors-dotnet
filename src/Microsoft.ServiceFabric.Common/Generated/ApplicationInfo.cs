// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a Service Fabric application.
    /// </summary>
    public partial class ApplicationInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationInfo class.
        /// </summary>
        /// <param name="id">The identity of the application. This is an encoded representation of the application name. This
        /// is used in the REST APIs to identify the application resource.
        /// Starting in version 6.0, hierarchical names are delimited with the "\~" character. For example, if the application
        /// name is "fabric:/myapp/app1",
        /// the application identity would be "myapp\~app1" in 6.0+ and "myapp/app1" in previous versions.
        /// </param>
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="typeName">The application type name as defined in the application manifest.</param>
        /// <param name="typeVersion">The version of the application type as defined in the application manifest.</param>
        /// <param name="status">The status of the application.
        /// . Possible values include: 'Invalid', 'Ready', 'Upgrading', 'Creating', 'Deleting', 'Failed'</param>
        /// <param name="parameters">List of application parameters with overridden values from their default values specified
        /// in the application manifest.</param>
        /// <param name="healthState">The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'</param>
        /// <param name="applicationDefinitionKind">The mechanism used to define a Service Fabric application.
        /// . Possible values include: 'Invalid', 'ServiceFabricApplicationDescription', 'Compose'</param>
        /// <param name="managedApplicationIdentity">Managed application identity description.</param>
        /// <param name="applicationMetadata">Metadata associated with a specific application.</param>
        public ApplicationInfo(
            string id = default(string),
            ApplicationName name = default(ApplicationName),
            string typeName = default(string),
            string typeVersion = default(string),
            ApplicationStatus? status = default(ApplicationStatus?),
            IReadOnlyDictionary<string, string> parameters = default(IReadOnlyDictionary<string, string>),
            HealthState? healthState = default(HealthState?),
            ApplicationDefinitionKind? applicationDefinitionKind = default(ApplicationDefinitionKind?),
            ManagedApplicationIdentityDescription managedApplicationIdentity = default(ManagedApplicationIdentityDescription),
            ApplicationMetadata applicationMetadata = default(ApplicationMetadata))
        {
            this.Id = id;
            this.Name = name;
            this.TypeName = typeName;
            this.TypeVersion = typeVersion;
            this.Status = status;
            this.Parameters = parameters;
            this.HealthState = healthState;
            this.ApplicationDefinitionKind = applicationDefinitionKind;
            this.ManagedApplicationIdentity = managedApplicationIdentity;
            this.ApplicationMetadata = applicationMetadata;
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
        /// Gets the version of the application type as defined in the application manifest.
        /// </summary>
        public string TypeVersion { get; }

        /// <summary>
        /// Gets the status of the application.
        /// . Possible values include: 'Invalid', 'Ready', 'Upgrading', 'Creating', 'Deleting', 'Failed'
        /// </summary>
        public ApplicationStatus? Status { get; }

        /// <summary>
        /// Gets list of application parameters with overridden values from their default values specified in the application
        /// manifest.
        /// </summary>
        public IReadOnlyDictionary<string, string> Parameters { get; }

        /// <summary>
        /// Gets the health state of a Service Fabric entity such as Cluster, Node, Application, Service, Partition, Replica
        /// etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        public HealthState? HealthState { get; }

        /// <summary>
        /// Gets the mechanism used to define a Service Fabric application.
        /// . Possible values include: 'Invalid', 'ServiceFabricApplicationDescription', 'Compose'
        /// </summary>
        public ApplicationDefinitionKind? ApplicationDefinitionKind { get; }

        /// <summary>
        /// Gets managed application identity description.
        /// </summary>
        public ManagedApplicationIdentityDescription ManagedApplicationIdentity { get; }

        /// <summary>
        /// Gets metadata associated with a specific application.
        /// </summary>
        public ApplicationMetadata ApplicationMetadata { get; }
    }
}
