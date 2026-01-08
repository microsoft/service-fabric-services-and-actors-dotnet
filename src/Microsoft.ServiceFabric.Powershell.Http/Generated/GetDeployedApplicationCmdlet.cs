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
    /// Gets the list of applications deployed on a Service Fabric node.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFDeployedApplication", DefaultParameterSetName = "GetDeployedApplicationInfoList")]
    public partial class GetDeployedApplicationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetDeployedApplicationInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetDeployedApplicationInfo")]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetDeployedApplicationInfo")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetDeployedApplicationInfoList")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetDeployedApplicationInfo")]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets IncludeHealthState. Include the health state of an entity.
        /// If this parameter is false or not specified, then the health state returned is "Unknown".
        /// When set to true, the query goes in parallel to the node and the health system service before the results are
        /// merged.
        /// As a result, the query is more expensive and may take a longer time.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetDeployedApplicationInfoList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetDeployedApplicationInfo")]
        public bool? IncludeHealthState { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetDeployedApplicationInfoList")]
        public long? MaxResults { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetDeployedApplicationInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.Applications.GetDeployedApplicationInfoListAsync(
                        nodeName: this.NodeName,
                        serverTimeout: this.ServerTimeout,
                        includeHealthState: this.IncludeHealthState,
                        continuationToken: continuationToken,
                        maxResults: this.MaxResults,
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
            else if (this.ParameterSetName.Equals("GetDeployedApplicationInfo"))
            {
                var result = this.ServiceFabricClient.Applications.GetDeployedApplicationInfoAsync(
                    nodeName: this.NodeName,
                    applicationId: this.ApplicationId,
                    serverTimeout: this.ServerTimeout,
                    includeHealthState: this.IncludeHealthState,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    this.WriteObject(this.FormatOutput(result));
                }
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
