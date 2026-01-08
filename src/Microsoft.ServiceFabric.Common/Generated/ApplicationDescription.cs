// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Service Fabric application.
    /// </summary>
    public partial class ApplicationDescription
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDescription class.
        /// </summary>
        /// <param name="name">The name of the application, including the 'fabric:' URI scheme.</param>
        /// <param name="typeName">The application type name as defined in the application manifest.</param>
        /// <param name="typeVersion">The version of the application type as defined in the application manifest.</param>
        /// <param name="parameters">List of application parameters with overridden values from their default values specified
        /// in the application manifest.</param>
        /// <param name="applicationCapacity">Describes capacity information for services of this application. This description
        /// can be used for describing the following.
        /// - Reserving the capacity for the services on the nodes
        /// - Limiting the total number of nodes that services of this application can run on
        /// - Limiting the custom capacity metrics to limit the total consumption of this metric by the services of this
        /// application
        /// </param>
        /// <param name="managedApplicationIdentity">Managed application identity description.</param>
        public ApplicationDescription(
            ApplicationName name,
            string typeName,
            string typeVersion,
            IReadOnlyDictionary<string, string> parameters = default(IReadOnlyDictionary<string, string>),
            ApplicationCapacityDescription applicationCapacity = default(ApplicationCapacityDescription),
            ManagedApplicationIdentityDescription managedApplicationIdentity = default(ManagedApplicationIdentityDescription))
        {
            name.ThrowIfNull(nameof(name));
            typeName.ThrowIfNull(nameof(typeName));
            typeVersion.ThrowIfNull(nameof(typeVersion));
            this.Name = name;
            this.TypeName = typeName;
            this.TypeVersion = typeVersion;
            this.Parameters = parameters;
            this.ApplicationCapacity = applicationCapacity;
            this.ManagedApplicationIdentity = managedApplicationIdentity;
        }

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
        /// Gets list of application parameters with overridden values from their default values specified in the application
        /// manifest.
        /// </summary>
        public IReadOnlyDictionary<string, string> Parameters { get; }

        /// <summary>
        /// Gets capacity information for services of this application. This description can be used for describing the
        /// following.
        /// - Reserving the capacity for the services on the nodes
        /// - Limiting the total number of nodes that services of this application can run on
        /// - Limiting the custom capacity metrics to limit the total consumption of this metric by the services of this
        /// application
        /// </summary>
        public ApplicationCapacityDescription ApplicationCapacity { get; }

        /// <summary>
        /// Gets managed application identity description.
        /// </summary>
        public ManagedApplicationIdentityDescription ManagedApplicationIdentity { get; }
    }
}
