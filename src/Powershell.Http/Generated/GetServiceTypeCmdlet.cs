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
    /// Gets the list containing the information about service types that are supported by a provisioned application type
    /// in a Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFServiceType", DefaultParameterSetName = "GetServiceTypeInfoList")]
    public partial class GetServiceTypeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationTypeName. The name of the application type.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetServiceTypeInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetServiceTypeInfoByName")]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeVersion. The version of the application type.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetServiceTypeInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetServiceTypeInfoByName")]
        public string ApplicationTypeVersion { get; set; }

        /// <summary>
        /// Gets or sets ServiceTypeName. Specifies the name of a Service Fabric service type.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "GetServiceTypeInfoByName")]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetServiceTypeInfoList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetServiceTypeInfoByName")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetServiceTypeInfoList"))
            {
                var result = this.ServiceFabricClient.ServiceTypes.GetServiceTypeInfoListAsync(
                    applicationTypeName: this.ApplicationTypeName,
                    applicationTypeVersion: this.ApplicationTypeVersion,
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
            else if (this.ParameterSetName.Equals("GetServiceTypeInfoByName"))
            {
                var result = this.ServiceFabricClient.ServiceTypes.GetServiceTypeInfoByNameAsync(
                    applicationTypeName: this.ApplicationTypeName,
                    applicationTypeVersion: this.ApplicationTypeVersion,
                    serviceTypeName: this.ServiceTypeName,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    this.WriteObject(this.FormatOutput(result));
                }
            }
        }
    }
}
