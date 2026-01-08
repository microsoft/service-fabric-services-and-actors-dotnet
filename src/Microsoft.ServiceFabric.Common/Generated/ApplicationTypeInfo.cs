// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about an application type.
    /// </summary>
    public partial class ApplicationTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeInfo class.
        /// </summary>
        /// <param name="name">The application type name as defined in the application manifest.</param>
        /// <param name="version">The version of the application type as defined in the application manifest.</param>
        /// <param name="defaultParameterList">List of application type parameters that can be overridden when creating or
        /// updating the application.</param>
        /// <param name="status">The status of the application type.
        /// . Possible values include: 'Invalid', 'Provisioning', 'Available', 'Unprovisioning', 'Failed'</param>
        /// <param name="statusDetails">Additional detailed information about the status of the application type.</param>
        /// <param name="applicationTypeDefinitionKind">The mechanism used to define a Service Fabric application type.
        /// . Possible values include: 'Invalid', 'ServiceFabricApplicationPackage', 'Compose'</param>
        /// <param name="applicationTypeMetadata">Metadata associated with a specific application type.</param>
        /// <param name="managedKeyVaultReferenceParameters">A list of ManagedKeyVaultReferenceParameter objects.</param>
        public ApplicationTypeInfo(
            string name = default(string),
            string version = default(string),
            IReadOnlyDictionary<string, string> defaultParameterList = default(IReadOnlyDictionary<string, string>),
            ApplicationTypeStatus? status = default(ApplicationTypeStatus?),
            string statusDetails = default(string),
            ApplicationTypeDefinitionKind? applicationTypeDefinitionKind = default(ApplicationTypeDefinitionKind?),
            ApplicationTypeMetadata applicationTypeMetadata = default(ApplicationTypeMetadata),
            IEnumerable<ManagedKeyVaultReferenceParameter> managedKeyVaultReferenceParameters = default(IEnumerable<ManagedKeyVaultReferenceParameter>))
        {
            this.Name = name;
            this.Version = version;
            this.DefaultParameterList = defaultParameterList;
            this.Status = status;
            this.StatusDetails = statusDetails;
            this.ApplicationTypeDefinitionKind = applicationTypeDefinitionKind;
            this.ApplicationTypeMetadata = applicationTypeMetadata;
            this.ManagedKeyVaultReferenceParameters = managedKeyVaultReferenceParameters;
        }

        /// <summary>
        /// Gets the application type name as defined in the application manifest.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the version of the application type as defined in the application manifest.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets list of application type parameters that can be overridden when creating or updating the application.
        /// </summary>
        public IReadOnlyDictionary<string, string> DefaultParameterList { get; }

        /// <summary>
        /// Gets the status of the application type.
        /// . Possible values include: 'Invalid', 'Provisioning', 'Available', 'Unprovisioning', 'Failed'
        /// </summary>
        public ApplicationTypeStatus? Status { get; }

        /// <summary>
        /// Gets additional detailed information about the status of the application type.
        /// </summary>
        public string StatusDetails { get; }

        /// <summary>
        /// Gets the mechanism used to define a Service Fabric application type.
        /// . Possible values include: 'Invalid', 'ServiceFabricApplicationPackage', 'Compose'
        /// </summary>
        public ApplicationTypeDefinitionKind? ApplicationTypeDefinitionKind { get; }

        /// <summary>
        /// Gets metadata associated with a specific application type.
        /// </summary>
        public ApplicationTypeMetadata ApplicationTypeMetadata { get; }

        /// <summary>
        /// Gets a list of ManagedKeyVaultReferenceParameter objects.
        /// </summary>
        public IEnumerable<ManagedKeyVaultReferenceParameter> ManagedKeyVaultReferenceParameters { get; }
    }
}
