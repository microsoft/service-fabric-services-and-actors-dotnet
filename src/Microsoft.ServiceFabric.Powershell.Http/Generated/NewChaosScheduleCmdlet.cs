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
    /// Set the schedule used by Chaos.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFChaosSchedule")]
    public partial class NewChaosScheduleCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Version. The version number of the Schedule.
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public int? Version { get; set; }

        /// <summary>
        /// Gets or sets StartDate. The date and time Chaos will start using this schedule.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public DateTime? StartDate { get; set; } = System.Xml.XmlConvert.ToDateTime("1601-01-01T00:00:00.000Z", System.Xml.XmlDateTimeSerializationMode.Utc);

        /// <summary>
        /// Gets or sets ExpiryDate. The date and time Chaos will continue to use this schedule until.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public DateTime? ExpiryDate { get; set; } = System.Xml.XmlConvert.ToDateTime("9999-12-31T23:59:59.999Z", System.Xml.XmlDateTimeSerializationMode.Utc);

        /// <summary>
        /// Gets or sets ChaosParametersDictionary. A mapping of string names to Chaos Parameters to be referenced by Chaos
        /// Schedule Jobs.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public IEnumerable<ChaosParametersDictionaryItem> ChaosParametersDictionary { get; set; }

        /// <summary>
        /// Gets or sets Jobs. A list of all Chaos Schedule Jobs that will be automated by the schedule.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public IEnumerable<ChaosScheduleJob> Jobs { get; set; }

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
            var chaosSchedule = new ChaosSchedule(
            startDate: this.StartDate,
            expiryDate: this.ExpiryDate,
            chaosParametersDictionary: this.ChaosParametersDictionary,
            jobs: this.Jobs);

            var chaosScheduleDescription = new ChaosScheduleDescription(
            version: this.Version,
            schedule: chaosSchedule);

            this.ServiceFabricClient.ChaosClient.PostChaosScheduleAsync(
                chaosSchedule: chaosScheduleDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
