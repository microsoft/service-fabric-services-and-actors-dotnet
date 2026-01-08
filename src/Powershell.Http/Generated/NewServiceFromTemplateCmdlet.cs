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
    /// Creates a Service Fabric service from the service template.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFServiceFromTemplate")]
    public partial class NewServiceFromTemplateCmdlet : CommonCmdletBase
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
        /// Gets or sets ApplicationName. The name of the application, including the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public ApplicationName ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets ServiceName. The full name of the service with 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public ServiceName ServiceName { get; set; }

        /// <summary>
        /// Gets or sets ServiceTypeName. Name of the service type as specified in the service manifest.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// Gets or sets InitializationData. The initialization data for the newly created service instance.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public byte[] InitializationData { get; set; }

        /// <summary>
        /// Gets or sets ServicePackageActivationMode. The activation mode of service package to be used for a service.
        /// Possible values include: 'SharedProcess', 'ExclusiveProcess'
        /// 
        /// The activation mode of service package to be used for a Service Fabric service. This is specified at the time of
        /// creating the Service.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public ServicePackageActivationMode? ServicePackageActivationMode { get; set; }

        /// <summary>
        /// Gets or sets ServiceDnsName. The DNS name of the service. It requires the DNS system service to be enabled in
        /// Service Fabric cluster.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6)]
        public string ServiceDnsName { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var serviceFromTemplateDescription = new ServiceFromTemplateDescription(
            applicationName: this.ApplicationName,
            serviceName: this.ServiceName,
            serviceTypeName: this.ServiceTypeName,
            initializationData: this.InitializationData,
            servicePackageActivationMode: this.ServicePackageActivationMode,
            serviceDnsName: this.ServiceDnsName);

            this.ServiceFabricClient.Services.CreateServiceFromTemplateAsync(
                applicationId: this.ApplicationId,
                serviceFromTemplateDescription: serviceFromTemplateDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
