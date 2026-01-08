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
    /// Gets all Containers-related events.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFContainersEventList")]
    public partial class GetContainersEventListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets StartTimeUtc. The start time of a lookup query in ISO UTC yyyy-MM-ddTHH:mm:ssZ.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string StartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets EndTimeUtc. The end time of a lookup query in ISO UTC yyyy-MM-ddTHH:mm:ssZ.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string EndTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets EventsTypesFilter. This is a comma separated string specifying the types of FabricEvents that should
        /// only be included in the response.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public string EventsTypesFilter { get; set; }

        /// <summary>
        /// Gets or sets ExcludeAnalysisEvents. This param disables the retrieval of AnalysisEvents if true is passed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public bool? ExcludeAnalysisEvents { get; set; }

        /// <summary>
        /// Gets or sets SkipCorrelationLookup. This param disables the search of CorrelatedEvents information if true is
        /// passed. otherwise the CorrelationEvents get processed and HasCorrelatedEvents field in every FabricEvent gets
        /// populated.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public bool? SkipCorrelationLookup { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.EventsStore.GetContainersEventListAsync(
                startTimeUtc: this.StartTimeUtc,
                endTimeUtc: this.EndTimeUtc,
                serverTimeout: this.ServerTimeout,
                eventsTypesFilter: this.EventsTypesFilter,
                excludeAnalysisEvents: this.ExcludeAnalysisEvents,
                skipCorrelationLookup: this.SkipCorrelationLookup,
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
