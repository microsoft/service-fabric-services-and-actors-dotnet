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
    /// Sets the capacity release level for the cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "SFCapacityReleaseLevel", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public partial class SetCapacityReleaseLevelCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Level. The capacity release level to set for the cluster.
        /// 
        /// - None - Restores the original target replica counts for primary, secondary, and auxiliary replicas.
        /// - Minor - Releases capacity without making services unavailable. Services configured to drop replicas to zero
        /// retain their minimum replica set, services configured to drop replicas to their minimum retain their current
        /// primary and secondary targets, and auxiliary replica targets are reduced to zero.
        /// - Major - Releases additional capacity and can make services unavailable. Services configured to drop replicas to
        /// zero have their target replica count reduced to zero, services configured to drop replicas to their minimum have
        /// their target reduced to the minimum replica set size, and auxiliary replica targets are reduced to zero. Services
        /// reduced to zero become unavailable, and stateful services reduced to zero permanently lose their state.
        /// . Possible values include: 'None', 'Minor', 'Major'
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public CapacityReleaseLevel? Level { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            string action = this.Level switch
            {
                CapacityReleaseLevel.None =>
                    "Set capacity release level to None: restore configured replica, instance, minimum, and auxiliary targets",
                CapacityReleaseLevel.Minor =>
                    "Set capacity release level to Minor: reduce DropToZero services to configured minimum and auxiliary targets " +
                    "to zero; this increases quorum-loss exposure",
                CapacityReleaseLevel.Major =>
                    "Set capacity release level to Major: reduce DropToMin services to configured minimum, DropToZero services " +
                    "to zero, and auxiliary targets to zero; services at zero become unavailable and stateful services at zero " +
                    "permanently lose data",
                _ => throw new ArgumentOutOfRangeException(nameof(this.Level), this.Level, null),
            };

            if (!this.ShouldProcess("Service Fabric cluster", action))
                return;

            this.ServiceFabricClient.Cluster.SetCapacityReleaseLevelAsync(
                level: this.Level,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
