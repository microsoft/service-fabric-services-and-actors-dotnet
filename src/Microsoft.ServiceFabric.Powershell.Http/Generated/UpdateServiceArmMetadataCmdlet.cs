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
    /// Updates the Arm Metadata for a specific service.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFServiceArmMetadata")]
    public partial class UpdateServiceArmMetadataCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ServiceId. The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets ArmResourceId. A string containing the ArmResourceId.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string ArmResourceId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets Force. Force parameter used to prevent accidental Arm metadata update.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public bool? Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var armMetadata = new ArmMetadata(
            armResourceId: this.ArmResourceId);

            var serviceArmMetadataUpdateDescription = new ServiceArmMetadataUpdateDescription(
            armMetadata: armMetadata);

            this.ServiceFabricClient.Services.UpdateServiceArmMetadataAsync(
                serviceId: this.ServiceId,
                serviceArmMetadataUpdateDescription: serviceArmMetadataUpdateDescription,
                serverTimeout: this.ServerTimeout,
                force: this.Force,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
