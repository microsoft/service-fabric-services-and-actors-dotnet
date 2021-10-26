// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Migration
{
#if DotNetCoreClr
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Class containing Service Fabric related extension methods for Microsoft.AspNetCore.Hosting.IWebHostBuilder.
    /// </summary>
    internal static class WebHostBuilderServiceFabricExtension
    {
        private static readonly string SettingName = "UseServiceFabricIntegration";

        /// <summary>
        /// Configures the Service to use ServiceFabricMiddleware and tells the listener that middleware is configured for the service so that it can
        /// suffix PartitionId and ReplicaOrInstanceId  to url before providing it to Service Fabric Runtime.
        /// </summary>
        /// <param name="hostBuilder">The Microsoft.AspNetCore.Hosting.IWebHostBuilder to configure.</param>
        /// <param name="listener">The <see cref="AspNetCoreCommunicationListener"/> to configure.</param>
        /// <param name="options">Options to configure ServiceFabricMiddleware and AspNetCoreCommunicationListener.</param>
        /// <returns>The Microsoft.AspNetCore.Hosting.IWebHostBuilder.</returns>
        public static IWebHostBuilder UseServiceFabricIntegration(this IWebHostBuilder hostBuilder, AspNetCoreCommunicationListener listener, ServiceFabricIntegrationOptions options)
        {
            if (hostBuilder == null)
            {
                throw new ArgumentNullException("hostBuilder");
            }

            // Check if 'UseServiceFabricIntegration' has already been called.
            if (hostBuilder.GetSetting(SettingName) == true.ToString())
            {
                return hostBuilder;
            }

            // Set flag to prevent double service configuration
            hostBuilder.UseSetting(SettingName, true.ToString());

            // Configure listener to use PartitionId and ReplicaId as urlSuffix only when specified in options.
            if (options.HasFlag(ServiceFabricIntegrationOptions.UseUniqueServiceUrl))
            {
                // notify listener to use urlSuffix when giving url to Service Fabric Runtime from OpenAsync()
                listener.ConfigureToUseUniqueServiceUrl();
            }

            hostBuilder.ConfigureServices(services =>
            {
                // Configure MiddleWare
                services.AddSingleton<IStartupFilter>(new ServiceFabricSetupFilter(listener.UrlSuffix, options));
            });

            return hostBuilder;
        }
    }
#endif
}
