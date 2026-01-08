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
    /// Updates the Arm Metadata for a specific Application Type.
    /// </summary>
    [Cmdlet(VerbsData.Update, "SFApplicationTypeArmMetadata")]
    public partial class UpdateApplicationTypeArmMetadataCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationTypeName. The name of the application type.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeVersion. The version of the application type.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string ApplicationTypeVersion { get; set; }

        /// <summary>
        /// Gets or sets ArmResourceId. A string containing the ArmResourceId.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string ArmResourceId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets Force. Force parameter used to prevent accidental Arm metadata update.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public bool? Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var armMetadata = new ArmMetadata(
            armResourceId: this.ArmResourceId);

            var applicationTypeArmMetadataUpdateDescription = new ApplicationTypeArmMetadataUpdateDescription(
            armMetadata: armMetadata);

            this.ServiceFabricClient.ApplicationTypes.UpdateApplicationTypeArmMetadataAsync(
                applicationTypeName: this.ApplicationTypeName,
                applicationTypeVersion: this.ApplicationTypeVersion,
                applicationTypeArmMetadataUpdateDescription: applicationTypeArmMetadataUpdateDescription,
                serverTimeout: this.ServerTimeout,
                force: this.Force,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
