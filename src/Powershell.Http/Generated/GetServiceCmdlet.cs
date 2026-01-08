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
    /// Gets the information about all services belonging to the application specified by the application ID.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFService", DefaultParameterSetName = "GetServiceInfoList")]
    public partial class GetServiceCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetServiceInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetServiceInfo")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServiceId. The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetServiceInfo")]
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets ServiceTypeName. The service type name used to filter the services to query for.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetServiceInfoList")]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetServiceInfoList")]
        public long? MaxResults { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetServiceInfoList")]
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetServiceInfo")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetServiceInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.Services.GetServiceInfoListAsync(
                        applicationId: this.ApplicationId,
                        serviceTypeName: this.ServiceTypeName,
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
            else if (this.ParameterSetName.Equals("GetServiceInfo"))
            {
                var result = this.ServiceFabricClient.Services.GetServiceInfoAsync(
                    applicationId: this.ApplicationId,
                    serviceId: this.ServiceId,
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
            var outputResult = output as ServiceInfo;

            var result = new PSObject();

            result.Properties.Add(new PSNoteProperty("ServiceId", outputResult.Id));
            result.Properties.Add(new PSNoteProperty("ServiceKind", outputResult.ServiceKind));
            result.Properties.Add(new PSNoteProperty("ServiceName", outputResult.Name));
            result.Properties.Add(new PSNoteProperty("ServiceTypeName", outputResult.TypeName));
            result.Properties.Add(new PSNoteProperty("ManifestVersion", outputResult.ManifestVersion));
            result.Properties.Add(new PSNoteProperty("HealthState", outputResult.HealthState));
            result.Properties.Add(new PSNoteProperty("ServiceStatus", outputResult.ServiceStatus));
            result.Properties.Add(new PSNoteProperty("IsServiceGroup", outputResult.IsServiceGroup));
            result.Properties.Add(new PSNoteProperty("ServiceMetadata", outputResult.ServiceMetadata));

            if (output is StatefulServiceInfo statefulServiceInfo)
            {
                result.Properties.Add(new PSNoteProperty("HasPersistedState", statefulServiceInfo.HasPersistedState));
            }

            return result;
        }
    }
}
