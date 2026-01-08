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
    /// Updates a Service Fabric application.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFApplication")]
    public partial class UpdateApplicationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets Flags. Flags indicating whether other properties are set. Each of the associated properties
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
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string Flags { get; set; }

        /// <summary>
        /// Gets or sets RemoveApplicationCapacity. Used to clear all parameters related to Application Capacity for this
        /// application. |
        /// It is not possible to specify this parameter together with other Application Capacity parameters.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? RemoveApplicationCapacity { get; set; } = false;

        /// <summary>
        /// Gets or sets MinimumNodes. The minimum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. If this property is set to zero, no capacity will be reserved. The value of this property cannot be more
        /// than the value of the MaximumNodes property.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public long? MinimumNodes { get; set; }

        /// <summary>
        /// Gets or sets MaximumNodes. The maximum number of nodes where Service Fabric will reserve capacity for this
        /// application. Note that this does not mean that the services of this application will be placed on all of those
        /// nodes. By default, the value of this property is zero and it means that the services can be placed on any node.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? MaximumNodes { get; set; } = 0;

        /// <summary>
        /// Gets or sets ApplicationMetrics. List of application capacity metric description.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public IEnumerable<ApplicationMetricDescription> ApplicationMetrics { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var applicationUpdateDescription = new ApplicationUpdateDescription(
            flags: this.Flags,
            removeApplicationCapacity: this.RemoveApplicationCapacity,
            minimumNodes: this.MinimumNodes,
            maximumNodes: this.MaximumNodes,
            applicationMetrics: this.ApplicationMetrics);

            this.ServiceFabricClient.Applications.UpdateApplicationAsync(
                applicationId: this.ApplicationId,
                applicationUpdateDescription: applicationUpdateDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
