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
    /// Removes or unregisters a Service Fabric application type from the cluster.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Unregister, "SFApplicationType")]
    public partial class UnregisterApplicationTypeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationTypeName. The name of the application type.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeVersion. The version of the application type as defined in the application manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string ApplicationTypeVersion { get; set; }

        /// <summary>
        /// Gets or sets Async. The flag indicating whether or not unprovision should occur asynchronously. When set to true,
        /// the unprovision operation returns when the request is accepted by the system, and the unprovision operation
        /// continues without any timeout limit. The default value is false. However, we recommend setting it to true for large
        /// application packages that were provisioned.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? Async { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the force flag. If provided, then the destructive action will be performed without asking for
        /// confirmation prompt.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public SwitchParameter Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var unprovisionApplicationTypeDescriptionInfo = new UnprovisionApplicationTypeDescriptionInfo(
            applicationTypeVersion: this.ApplicationTypeVersion,
            async: this.Async);

            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.ApplicationTypes.UnprovisionApplicationTypeAsync(
                    applicationTypeName: this.ApplicationTypeName,
                    unprovisionApplicationTypeDescriptionInfo: unprovisionApplicationTypeDescriptionInfo,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
