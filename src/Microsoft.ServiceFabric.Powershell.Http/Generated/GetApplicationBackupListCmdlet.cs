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
    /// Gets the list of backups available for every partition in this application.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFApplicationBackupList")]
    public partial class GetApplicationBackupListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets Latest. Specifies whether to get only the most recent backup available for a partition for the
        /// specified time range.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? Latest { get; set; }

        /// <summary>
        /// Gets or sets StartDateTimeFilter. Specify the start date time from which to enumerate backups, in datetime format.
        /// The date time must be specified in ISO8601 format. This is an optional parameter. If not specified, all backups
        /// from the beginning are enumerated.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public DateTime? StartDateTimeFilter { get; set; }

        /// <summary>
        /// Gets or sets EndDateTimeFilter. Specify the end date time till which to enumerate backups, in datetime format. The
        /// date time must be specified in ISO8601 format. This is an optional parameter. If not specified, enumeration is done
        /// till the most recent backup.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public DateTime? EndDateTimeFilter { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? MaxResults { get; set; }

        /// <summary>
        /// Gets or sets MetadataFromBlob. Specifies whether to fetch backup metadata from the backup blob in the response.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public bool? MetadataFromBlob { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var continuationToken = default(ContinuationToken);
            do
            {
                var result = this.ServiceFabricClient.BackupRestore.GetApplicationBackupListAsync(
                    applicationId: this.ApplicationId,
                    serverTimeout: this.ServerTimeout,
                    latest: this.Latest,
                    startDateTimeFilter: this.StartDateTimeFilter,
                    endDateTimeFilter: this.EndDateTimeFilter,
                    continuationToken: continuationToken,
                    maxResults: this.MaxResults,
                    metadataFromBlob: this.MetadataFromBlob,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result == null)
                {
                    break;
                }

                var count = 0;
                foreach (var item in result.Data)
                {
                    count++;
                    this.WriteObject(this.FormatOutput(item));
                }

                continuationToken = result.ContinuationToken;
                this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
            }
            while (continuationToken.Next);
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
