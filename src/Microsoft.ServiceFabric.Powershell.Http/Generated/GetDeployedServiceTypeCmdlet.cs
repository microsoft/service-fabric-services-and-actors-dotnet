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
    /// Gets the list containing the information about service types from the applications deployed on a node in a Service
    /// Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFDeployedServiceType", DefaultParameterSetName = "GetDeployedServiceTypeInfoList")]
    public partial class GetDeployedServiceTypeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetDeployedServiceTypeInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetDeployedServiceTypeInfoByName")]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationId. The identity of the application. This is typically the full name of the application
        /// without the 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the application name is "fabric:/myapp/app1", the application identity would be "myapp~app1" in
        /// 6.0+ and "myapp/app1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetDeployedServiceTypeInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetDeployedServiceTypeInfoByName")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets ServiceTypeName. Specifies the name of a Service Fabric service type.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "GetDeployedServiceTypeInfoByName")]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Gets or sets ServiceManifestName. The name of the service manifest to filter the list of deployed service type
        /// information. If specified, the response will only contain the information about service types that are defined in
        /// this service manifest.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetDeployedServiceTypeInfoList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetDeployedServiceTypeInfoByName")]
        public string ServiceManifestName { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetDeployedServiceTypeInfoList")]
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetDeployedServiceTypeInfoByName")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetDeployedServiceTypeInfoList"))
            {
                var result = this.ServiceFabricClient.ServiceTypes.GetDeployedServiceTypeInfoListAsync(
                    nodeName: this.NodeName,
                    applicationId: this.ApplicationId,
                    serviceManifestName: this.ServiceManifestName,
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
            else if (this.ParameterSetName.Equals("GetDeployedServiceTypeInfoByName"))
            {
                var result = this.ServiceFabricClient.ServiceTypes.GetDeployedServiceTypeInfoByNameAsync(
                    nodeName: this.NodeName,
                    applicationId: this.ApplicationId,
                    serviceTypeName: this.ServiceTypeName,
                    serviceManifestName: this.ServiceManifestName,
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
        }
    }
}
