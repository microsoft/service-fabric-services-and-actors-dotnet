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
    /// Gets all Applications-related events.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFApplicationsEvent", DefaultParameterSetName = "GetApplicationsEventList")]
    public partial class GetApplicationsEventCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets StartTimeUtc. The start time of a lookup query in ISO UTC yyyy-MM-ddTHH:mm:ssZ.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetApplicationsEventList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetApplicationEventList")]
        public string StartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets EndTimeUtc. The end time of a lookup query in ISO UTC yyyy-MM-ddTHH:mm:ssZ.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetApplicationsEventList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetApplicationEventList")]
        public string EndTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "GetApplicationEventList")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetApplicationsEventList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetApplicationEventList")]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets EventsTypesFilter. This is a comma separated string specifying the types of FabricEvents that should
        /// only be included in the response.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetApplicationsEventList")]
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetApplicationEventList")]
        public string EventsTypesFilter { get; set; }

        /// <summary>
        /// Gets or sets ExcludeAnalysisEvents. This param disables the retrieval of AnalysisEvents if true is passed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetApplicationsEventList")]
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetApplicationEventList")]
        public bool? ExcludeAnalysisEvents { get; set; }

        /// <summary>
        /// Gets or sets SkipCorrelationLookup. This param disables the search of CorrelatedEvents information if true is
        /// passed. otherwise the CorrelationEvents get processed and HasCorrelatedEvents field in every FabricEvent gets
        /// populated.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6, ParameterSetName = "GetApplicationsEventList")]
        [Parameter(Mandatory = false, Position = 6, ParameterSetName = "GetApplicationEventList")]
        public bool? SkipCorrelationLookup { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetApplicationsEventList"))
            {
                var result = this.ServiceFabricClient.EventsStore.GetApplicationsEventListAsync(
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
            else if (this.ParameterSetName.Equals("GetApplicationEventList"))
            {
                var result = this.ServiceFabricClient.EventsStore.GetApplicationEventListAsync(
                    applicationId: this.ApplicationId,
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
}
