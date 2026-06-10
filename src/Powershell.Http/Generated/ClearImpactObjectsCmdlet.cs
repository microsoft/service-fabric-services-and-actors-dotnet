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
    /// Removes stuck impact approval objects from the Failover Manager.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "SFImpactObjects")]
    public partial class ClearImpactObjectsCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets IdFilter. Only clear the impact object with this specific ID.
        /// Use an empty GUID (00000000-0000-0000-0000-000000000000) to match all impact objects.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public Guid? IdFilter { get; set; }

        /// <summary>
        /// Gets or sets TypeFilter. Only clear impact objects of this type (e.g., NodeDeactivation, ApplicationUpgrade).
        /// Possible values include: 'Unknown', 'NodeDeactivation', 'ApplicationUpgrade', 'FabricUpgrade', 'Partition'
        /// 
        /// The category of operation that created the impact approval object.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public ImpactType? TypeFilter { get; set; }

        /// <summary>
        /// Gets or sets StatusFilter. Only clear impact objects with this approval status (e.g., WaitingForApproval,
        /// Approved). Possible values include: 'None', 'Nominal', 'WaitingForApproval', 'Approved'
        /// 
        /// The current approval state of an impact object in the Failover Manager.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public ImpactApprovalStatus? StatusFilter { get; set; }

        /// <summary>
        /// Gets or sets OperationFilter. Only clear impact objects of this operation kind (e.g., Restart, Remove, Add).
        /// Possible values include: 'Unknown', 'Restart', 'Remove', 'Add'
        /// 
        /// The type of operation associated with the impacted instances (e.g., restart, removal, or addition of an impacted
        /// instance).
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public ImpactOperationKind? OperationFilter { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the force flag. If provided, then the destructive action will be performed without asking for
        /// confirmation prompt.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public SwitchParameter Force { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var impactSelector = new ImpactSelector(
            idFilter: this.IdFilter,
            typeFilter: this.TypeFilter,
            statusFilter: this.StatusFilter,
            operationFilter: this.OperationFilter);

            if (((this.Force != null) && this.Force) || this.ShouldContinue(string.Empty, string.Empty))
            {
                this.ServiceFabricClient.Cluster.ClearImpactObjectsAsync(
                    impactSelector: impactSelector,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                Console.WriteLine("Success!");
            }
        }
    }
}
