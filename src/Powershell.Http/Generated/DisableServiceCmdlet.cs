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
    /// Disables a Service Fabric service.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "SFService")]
    public partial class DisableServiceCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ServiceId. The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets DisableServiceFlag. Specifies the behavior when disabling a service. The only supported value is
        /// 'RemoveData', which removes the service replicas and data when the service is disabled. Possible values include:
        /// 'RemoveData'
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public DisableServiceFlag? DisableServiceFlag { get; set; }

        /// <summary>
        /// Gets or sets ForceDisable. Indicates whether the service should be force-disabled, bypassing graceful replica
        /// shutdown. Force-disabling a stateful service can leave persisted state on disk that is not properly cleaned up,
        /// because replicas are terminated without a graceful shutdown.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? ForceDisable { get; set; }

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
            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.Services.DisableServiceAsync(
                    serviceId: this.ServiceId,
                    disableServiceFlag: this.DisableServiceFlag,
                    forceDisable: this.ForceDisable,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
