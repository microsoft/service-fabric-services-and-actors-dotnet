// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration
{
    /// <summary>
    /// Configuration extensions to add service fabric configuration.
    /// </summary>
    public static class ServiceFabricConfigurationExtensions
    {
        /// <summary>
        /// Add configuration for all the configuration packages declared for current service.
        /// </summary>
        /// <param name="builder">the configuration builder.</param>
        /// <returns>the same configuration builder with service fabric configuration added.</returns>
        public static IConfigurationBuilder AddServiceFabricConfiguration(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            ICodePackageActivationContext activationContext = FabricRuntime.GetActivationContext();

            // DO NOT dispose activation context as it will be saved to DI container and used later.
            // using (activationContext)
            {
                return builder.AddServiceFabricConfiguration(activationContext);
            }
        }

        /// <summary>
        /// Add configuration for all the configuration packages declared for current code package.
        /// </summary>
        /// <param name="builder">the configuration builder.</param>
        /// <param name="context">the activation context with information for configuration packages.</param>
        /// <returns>the same configuration builder with service fabric configuration added.</returns>
        public static IConfigurationBuilder AddServiceFabricConfiguration(this IConfigurationBuilder builder, ICodePackageActivationContext context)
        {
            return builder.AddServiceFabricConfiguration(context, null);
        }

        /// <summary>
        /// Add configuration for given configuration package from packageName parameter as well as customize config action to populate the Data.
        /// </summary>
        /// <param name="builder">the configuration builder.</param>
        /// <param name="context">the activation context with information for configuration packages.</param>
        /// <param name="optionsDelegate">the delegate to change the configuration options including configuration actions.</param>
        /// <returns>the same configuration builder with service fabric configuration added.</returns>
        public static IConfigurationBuilder AddServiceFabricConfiguration(
            this IConfigurationBuilder builder,
            ICodePackageActivationContext context,
            Action<ServiceFabricConfigurationOptions> optionsDelegate)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var packageNames = context.GetConfigurationPackageNames();
            foreach (var packageName in packageNames)
            {
                var options = new ServiceFabricConfigurationOptions(packageName);

                if (optionsDelegate != null)
                {
                    optionsDelegate(options);
                }

                builder.Add(new ServiceFabricConfigurationSource(context, options));
            }

            return builder;
        }
    }
}
