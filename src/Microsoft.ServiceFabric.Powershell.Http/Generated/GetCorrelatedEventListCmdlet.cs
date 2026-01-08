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
    /// Gets all correlated events for a given event.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFCorrelatedEventList")]
    public partial class GetCorrelatedEventListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets EventInstanceId. The EventInstanceId.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string EventInstanceId { get; set; }

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
            var result = this.ServiceFabricClient.EventsStore.GetCorrelatedEventListAsync(
                eventInstanceId: this.EventInstanceId,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                foreach (var item in result)
                {
                    this.WriteObject(this.FormatOutput(item));
                }
            }
        }
    }
}
