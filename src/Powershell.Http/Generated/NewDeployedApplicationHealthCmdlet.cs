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
    /// Sends a health report on the Service Fabric application deployed on a Service Fabric node.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFDeployedApplicationHealth")]
    public partial class NewDeployedApplicationHealthCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets SourceId. The source name that identifies the client/watchdog/system component that generated the
        /// health information.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string SourceId { get; set; }

        /// <summary>
        /// Gets or sets Property. The property of the health information. An entity can have health reports for different
        /// properties.
        /// The property is a string and not a fixed enumeration to allow the reporter flexibility to categorize the state
        /// condition that triggers the report.
        /// For example, a reporter with SourceId "LocalWatchdog" can monitor the state of the available disk on a node,
        /// so it can report "AvailableDisk" property on that node.
        /// The same reporter can monitor the node connectivity, so it can report a property "Connectivity" on the same node.
        /// In the health store, these reports are treated as separate health events for the specified node.
        /// 
        /// Together with the SourceId, the property uniquely identifies the health information.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets HealthState. The health state of a Service Fabric entity such as Cluster, Node, Application, Service,
        /// Partition, Replica etc. Possible values include: 'Invalid', 'Ok', 'Warning', 'Error', 'Unknown'
        /// </summary>
        [Parameter(Mandatory = true, Position = 4)]
        public HealthState? HealthState { get; set; }

        /// <summary>
        /// Gets or sets TimeToLiveInMilliSeconds. The duration for which this health report is valid. This field uses ISO8601
        /// format for specifying the duration.
        /// When clients report periodically, they should send reports with higher frequency than time to live.
        /// If clients report on transition, they can set the time to live to infinite.
        /// When time to live expires, the health event that contains the health information
        /// is either removed from health store, if RemoveWhenExpired is true, or evaluated at error, if RemoveWhenExpired
        /// false.
        /// 
        /// If not specified, time to live defaults to infinite value.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public TimeSpan? TimeToLiveInMilliSeconds { get; set; }

        /// <summary>
        /// Gets or sets Description. The description of the health information. It represents free text used to add human
        /// readable information about the report.
        /// The maximum string length for the description is 4096 characters.
        /// If the provided string is longer, it will be automatically truncated.
        /// When truncated, the last characters of the description contain a marker "[Truncated]", and total string size is
        /// 4096 characters.
        /// The presence of the marker indicates to users that truncation occurred.
        /// Note that when truncated, the description has less than 4096 characters from the original string.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets SequenceNumber. The sequence number for this health report as a numeric string.
        /// The report sequence number is used by the health store to detect stale reports.
        /// If not specified, a sequence number is auto-generated by the health client when a report is added.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets RemoveWhenExpired. Value that indicates whether the report is removed from health store when it
        /// expires.
        /// If set to true, the report is removed from the health store after it expires.
        /// If set to false, the report is treated as an error when expired. The value of this property is false by default.
        /// When clients report periodically, they should set RemoveWhenExpired false (default).
        /// This way, if the reporter has issues (e.g. deadlock) and can't report, the entity is evaluated at error when the
        /// health report expires.
        /// This flags the entity as being in Error health state.
        /// </summary>
        [Parameter(Mandatory = false, Position = 8)]
        public bool? RemoveWhenExpired { get; set; }

        /// <summary>
        /// Gets or sets HealthReportId. A health report ID which identifies the health report and can be used to find more
        /// detailed information about a specific health event at
        /// aka.ms/sfhealthid
        /// </summary>
        [Parameter(Mandatory = false, Position = 9)]
        public string HealthReportId { get; set; }

        /// <summary>
        /// Gets or sets Immediate. A flag that indicates whether the report should be sent immediately.
        /// A health report is sent to a Service Fabric gateway Application, which forwards to the health store.
        /// If Immediate is set to true, the report is sent immediately from HTTP Gateway to the health store, regardless of
        /// the fabric client settings that the HTTP Gateway Application is using.
        /// This is useful for critical reports that should be sent as soon as possible.
        /// Depending on timing and other conditions, sending the report may still fail, for example if the HTTP Gateway is
        /// closed or the message doesn't reach the Gateway.
        /// If Immediate is set to false, the report is sent based on the health client settings from the HTTP Gateway.
        /// Therefore, it will be batched according to the HealthReportSendInterval configuration.
        /// This is the recommended setting because it allows the health client to optimize health reporting messages to health
        /// store as well as health report processing.
        /// By default, reports are not sent immediately.
        /// </summary>
        [Parameter(Mandatory = false, Position = 10)]
        public bool? Immediate { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 11)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var healthInformation = new HealthInformation(
            sourceId: this.SourceId,
            property: this.Property,
            healthState: this.HealthState,
            timeToLiveInMilliSeconds: this.TimeToLiveInMilliSeconds,
            description: this.Description,
            sequenceNumber: this.SequenceNumber,
            removeWhenExpired: this.RemoveWhenExpired,
            healthReportId: this.HealthReportId);

            this.ServiceFabricClient.Applications.ReportDeployedApplicationHealthAsync(
                nodeName: this.NodeName,
                applicationId: this.ApplicationId,
                healthInformation: healthInformation,
                immediate: this.Immediate,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
