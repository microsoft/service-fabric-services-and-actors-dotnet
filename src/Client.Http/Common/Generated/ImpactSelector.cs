// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Filter criteria for selecting which impact objects to clear.
    /// All fields are optional. Use them to narrow down which impact objects
    /// to remove. If all fields use their default values, all impact objects are cleared.
    /// </summary>
    public partial class ImpactSelector
    {
        /// <summary>
        /// Initializes a new instance of the ImpactSelector class.
        /// </summary>
        /// <param name="idFilter">Only clear the impact object with this specific ID.
        /// Use an empty GUID (00000000-0000-0000-0000-000000000000) to match all impact objects.
        /// </param>
        /// <param name="typeFilter">Only clear impact objects of this type (e.g., NodeDeactivation, ApplicationUpgrade).
        /// Possible values include: 'Unknown', 'NodeDeactivation', 'ApplicationUpgrade', 'FabricUpgrade', 'Partition'
        /// 
        /// The category of operation that created the impact approval object.
        /// </param>
        /// <param name="statusFilter">Only clear impact objects with this approval status (e.g., WaitingForApproval,
        /// Approved). Possible values include: 'None', 'Nominal', 'WaitingForApproval', 'Approved'
        /// 
        /// The current approval state of an impact object in the Failover Manager.
        /// </param>
        /// <param name="operationFilter">Only clear impact objects of this operation kind (e.g., Restart, Remove, Add).
        /// Possible values include: 'Unknown', 'Restart', 'Remove', 'Add'
        /// 
        /// The type of operation associated with the impacted instances (e.g., restart, removal, or addition of an impacted
        /// instance).
        /// </param>
        public ImpactSelector(
            Guid? idFilter = default(Guid?),
            ImpactType? typeFilter = default(ImpactType?),
            ImpactApprovalStatus? statusFilter = default(ImpactApprovalStatus?),
            ImpactOperationKind? operationFilter = default(ImpactOperationKind?))
        {
            this.IdFilter = idFilter;
            this.TypeFilter = typeFilter;
            this.StatusFilter = statusFilter;
            this.OperationFilter = operationFilter;
        }

        /// <summary>
        /// Gets only clear the impact object with this specific ID.
        /// Use an empty GUID (00000000-0000-0000-0000-000000000000) to match all impact objects.
        /// </summary>
        public Guid? IdFilter { get; }

        /// <summary>
        /// Gets only clear impact objects of this type (e.g., NodeDeactivation, ApplicationUpgrade). Possible values include:
        /// 'Unknown', 'NodeDeactivation', 'ApplicationUpgrade', 'FabricUpgrade', 'Partition'
        /// 
        /// The category of operation that created the impact approval object.
        /// </summary>
        public ImpactType? TypeFilter { get; }

        /// <summary>
        /// Gets only clear impact objects with this approval status (e.g., WaitingForApproval, Approved). Possible values
        /// include: 'None', 'Nominal', 'WaitingForApproval', 'Approved'
        /// 
        /// The current approval state of an impact object in the Failover Manager.
        /// </summary>
        public ImpactApprovalStatus? StatusFilter { get; }

        /// <summary>
        /// Gets only clear impact objects of this operation kind (e.g., Restart, Remove, Add). Possible values include:
        /// 'Unknown', 'Restart', 'Remove', 'Add'
        /// 
        /// The type of operation associated with the impacted instances (e.g., restart, removal, or addition of an impacted
        /// instance).
        /// </summary>
        public ImpactOperationKind? OperationFilter { get; }
    }
}
