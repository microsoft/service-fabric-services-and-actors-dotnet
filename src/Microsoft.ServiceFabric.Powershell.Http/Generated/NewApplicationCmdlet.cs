// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Creates a Service Fabric application.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFApplication")]
    public partial class NewApplicationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Name. The name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ApplicationName Name { get; set; }

        /// <summary>
        /// Gets or sets TypeName. The application type name as defined in the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets TypeVersion. The version of the application type as defined in the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string TypeVersion { get; set; }

        /// <summary>
        /// Gets or sets Parameters. List of application parameters with overridden values from their default values specified
        /// in the application manifest.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public System.Collections.Hashtable Parameters { get; set; }

        /// <summary>
        /// Gets or sets MinimumNodes. The minimum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. If this property is set to zero, no capacity will be reserved. The value of this property cannot be more
        /// than the value of the MaximumNodes property.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? MinimumNodes { get; set; }

        /// <summary>
        /// Gets or sets MaximumNodes. The maximum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. By default, the value of this property is zero and it means that the services can be placed on any node.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? MaximumNodes { get; set; } = 0;

        /// <summary>
        /// Gets or sets ApplicationMetrics. List of application capacity metric description.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public IEnumerable<ApplicationMetricDescription> ApplicationMetrics { get; set; }

        /// <summary>
        /// Gets or sets TokenServiceEndpoint. Token service endpoint.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public string TokenServiceEndpoint { get; set; }

        /// <summary>
        /// Gets or sets ManagedIdentities. A list of managed application identity objects.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public IEnumerable<ManagedApplicationIdentity> ManagedIdentities { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var applicationCapacityDescription = new ApplicationCapacityDescription(
            minimumNodes: this.MinimumNodes,
            maximumNodes: this.MaximumNodes,
            applicationMetrics: this.ApplicationMetrics);

            var managedApplicationIdentityDescription = new ManagedApplicationIdentityDescription(
            tokenServiceEndpoint: this.TokenServiceEndpoint,
            managedIdentities: this.ManagedIdentities);

            var applicationDescription = new ApplicationDescription(
            name: this.Name,
            typeName: this.TypeName,
            typeVersion: this.TypeVersion,
            parameters: this.Parameters?.ToDictionary<string, string>(),
            applicationCapacity: applicationCapacityDescription,
            managedApplicationIdentity: managedApplicationIdentityDescription);

            this.ServiceFabricClient.Applications.CreateApplicationAsync(
                applicationDescription: applicationDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
