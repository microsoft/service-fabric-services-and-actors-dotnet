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
    /// Gets the list of applications created in the Service Fabric cluster that match the specified filters.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFApplication", DefaultParameterSetName = "GetApplicationInfoList")]
    public partial class GetApplicationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetApplicationInfo")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ApplicationDefinitionKindFilter. Used to filter on ApplicationDefinitionKind, which is the mechanism
        /// used to define a Service Fabric application.
        /// - Default - Default value, which performs the same function as selecting "All". The value is 0.
        /// - All - Filter that matches input with any ApplicationDefinitionKind value. The value is 65535.
        /// - ServiceFabricApplicationDescription - Filter that matches input with ApplicationDefinitionKind value
        /// ServiceFabricApplicationDescription. The value is 1.
        /// - Compose - Filter that matches input with ApplicationDefinitionKind value Compose. The value is 2.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "GetApplicationInfoList")]
        public int? ApplicationDefinitionKindFilter { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeName. The application type name used to filter the applications to query for. This
        /// value should not contain the application type version.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetApplicationInfoList")]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ExcludeApplicationParameters. The flag that specifies whether application parameters will be excluded
        /// from the result.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetApplicationInfoList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetApplicationInfo")]
        public bool? ExcludeApplicationParameters { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetApplicationInfoList")]
        public long? MaxResults { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetApplicationInfoList")]
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetApplicationInfo")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetApplicationInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.Applications.GetApplicationInfoListAsync(
                        applicationDefinitionKindFilter: this.ApplicationDefinitionKindFilter,
                        applicationTypeName: this.ApplicationTypeName,
                        excludeApplicationParameters: this.ExcludeApplicationParameters,
                        continuationToken: continuationToken,
                        maxResults: this.MaxResults,
                        serverTimeout: this.ServerTimeout,
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
            else if (this.ParameterSetName.Equals("GetApplicationInfo"))
            {
                var result = this.ServiceFabricClient.Applications.GetApplicationInfoAsync(
                    applicationId: this.ApplicationId,
                    excludeApplicationParameters: this.ExcludeApplicationParameters,
                    serverTimeout: this.ServerTimeout,
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
            var outputResult = output as Common.ApplicationInfo;

            var result = new PSObject();

            result.Properties.Add(new PSNoteProperty("ApplicationId", outputResult.Id));
            result.Properties.Add(new PSNoteProperty("ApplicationName", outputResult.Name));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeName", outputResult.TypeName));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeVersion", outputResult.TypeVersion));
            result.Properties.Add(new PSNoteProperty("ApplicationStatus", outputResult.Status));
            result.Properties.Add(new PSNoteProperty("ApplicationParameterList", outputResult.Parameters));
            result.Properties.Add(new PSNoteProperty("ApplicationDefinitionKind", outputResult.ApplicationDefinitionKind));
            result.Properties.Add(new PSNoteProperty("ManagedApplicationIdentityDescription", outputResult.ManagedApplicationIdentity));
            result.Properties.Add(new PSNoteProperty("ApplicationMetadata", outputResult.ApplicationMetadata));

            return result;
        }
    }
}
