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
    /// Gets the health of a Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFClusterHealth")]
    public partial class GetClusterHealthCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodesHealthStateFilter. Allows filtering of the node health state objects returned in the result of
        /// cluster health query
        /// based on their health state. The possible values for this parameter include integer value of one of the
        /// following health states. Only nodes that match the filter are returned. All nodes are used to evaluate the
        /// aggregated health state.
        /// If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of nodes with HealthState value of OK (2) and Warning (4)
        /// are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public int? NodesHealthStateFilter { get; set; }

        /// <summary>
        /// Gets or sets ApplicationsHealthStateFilter. Allows filtering of the application health state objects returned in
        /// the result of cluster health
        /// query based on their health state.
        /// The possible values for this parameter include integer value obtained from members or bitwise operations
        /// on members of HealthStateFilter enumeration. Only applications that match the filter are returned.
        /// All applications are used to evaluate the aggregated health state. If not specified, all entries are returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6 then health state of applications with HealthState value of OK (2) and
        /// Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public int? ApplicationsHealthStateFilter { get; set; }

        /// <summary>
        /// Gets or sets EventsHealthStateFilter. Allows filtering the collection of HealthEvent objects returned based on
        /// health state.
        /// The possible values for this parameter include integer value of one of the following health states.
        /// Only events that match the filter are returned. All events are used to evaluate the aggregated health state.
        /// If not specified, all entries are returned. The state values are flag-based enumeration, so the value could be a
        /// combination of these values, obtained using the bitwise 'OR' operator. For example, If the provided value is 6 then
        /// all of the events with HealthState value of OK (2) and Warning (4) are returned.
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public int? EventsHealthStateFilter { get; set; }

        /// <summary>
        /// Gets or sets ExcludeHealthStatistics. Indicates whether the health statistics should be returned as part of the
        /// query result. False by default.
        /// The statistics show the number of children entities in health state Ok, Warning, and Error.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public bool? ExcludeHealthStatistics { get; set; }

        /// <summary>
        /// Gets or sets IncludeSystemApplicationHealthStatistics. Indicates whether the health statistics should include the
        /// fabric:/System application health statistics. False by default.
        /// If IncludeSystemApplicationHealthStatistics is set to true, the health statistics include the entities that belong
        /// to the fabric:/System application.
        /// Otherwise, the query result includes health statistics only for user applications.
        /// The health statistics must be included in the query result for this parameter to be applied.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public bool? IncludeSystemApplicationHealthStatistics { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var result = this.ServiceFabricClient.Cluster.GetClusterHealthAsync(
                nodesHealthStateFilter: this.NodesHealthStateFilter,
                applicationsHealthStateFilter: this.ApplicationsHealthStateFilter,
                eventsHealthStateFilter: this.EventsHealthStateFilter,
                excludeHealthStatistics: this.ExcludeHealthStatistics,
                includeSystemApplicationHealthStatistics: this.IncludeSystemApplicationHealthStatistics,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            var outputResult = output as ClusterHealth;

            var nodeHealthStatesObj = new PSObject(outputResult.NodeHealthStates);
            nodeHealthStatesObj.Members.Add(new PSCodeMethod("ToString", typeof(OutputFormatter).GetMethod("FormatObject")));

            var applicationHealthStatesObj = new PSObject(outputResult.ApplicationHealthStates);
            applicationHealthStatesObj.Members.Add(new PSCodeMethod("ToString", typeof(OutputFormatter).GetMethod("FormatObject")));

            var healthEventsObj = new PSObject(outputResult.HealthEvents);
            healthEventsObj.Members.Add(new PSCodeMethod("ToString", typeof(OutputFormatter).GetMethod("FormatObject")));

            var healthStatisticsObj = new PSObject(outputResult.HealthStatistics);
            healthStatisticsObj.Members.Add(new PSCodeMethod("ToString", typeof(OutputFormatter).GetMethod("FormatObject")));

            var result = new PSObject(outputResult);

            result.Properties.Add(new PSNoteProperty("AggregatedHealthState", outputResult.AggregatedHealthState));

            return result;
        }
    }
}
