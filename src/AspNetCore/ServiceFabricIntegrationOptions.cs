// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Communication.AspNetCore
{
    /// <summary>
    /// Integration options for <c>UseServiceFabricIntegration</c> methods when used with Microsoft.AspNetCore.Hosting.IWebHostBuilder
    /// or Microsoft.Extensions.Hosting.IHostBuilder.
    /// </summary>
    [Flags]
    public enum ServiceFabricIntegrationOptions
    {
        /// <summary>
        /// This option will not configure the <see cref="AspNetCoreCommunicationListener"/> to add any suffix to url when providing the url to Service Fabric Runtime from its <see cref="Microsoft.ServiceFabric.Services.Communication.Runtime.ICommunicationListener.OpenAsync"/> method.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// This option will configure the <see cref="AspNetCoreCommunicationListener"/> to add a url suffix containing <see cref="System.Fabric.ServiceContext.PartitionId"/> and <see cref="System.Fabric.ServiceContext.ReplicaOrInstanceId"/>
        /// to url when providing the url to Service Fabric Runtime from its <see cref="Microsoft.ServiceFabric.Services.Communication.Runtime.ICommunicationListener.OpenAsync"/> method.
        /// </summary>
        UseUniqueServiceUrl = 0x01,

        /// <summary>
        /// This option will enable the Service Fabric Reverse Proxy integration middleware.
        /// </summary>
        UseReverseProxyIntegration = 0x02,
    }
}
