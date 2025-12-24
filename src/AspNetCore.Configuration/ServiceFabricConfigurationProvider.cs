// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration
{
    internal class ServiceFabricConfigurationProvider : ConfigurationProvider
    {
        private readonly ICodePackageActivationContext context;
        private readonly ServiceFabricConfigurationOptions options;

        public ServiceFabricConfigurationProvider(ICodePackageActivationContext activationContext, ServiceFabricConfigurationOptions options)
        {
            this.context = activationContext;

            this.options = options ?? throw new ArgumentNullException(nameof(options));

            this.context.ConfigurationPackageModifiedEvent += (sender, e) =>
            {
                this.HandleNewPackage(e.NewPackage);
            };

            this.context.ConfigurationPackageAddedEvent += (sender, e) =>
            {
                this.HandleNewPackage(e.Package);
            };
        }

        /// <summary>
        /// Load the configuration.
        /// </summary>
        public override void Load()
        {
            var config = this.context.GetConfigurationPackageObject(this.options.PackageName);
            this.LoadPackage(config);
        }

        /// <summary>
        /// Handle loading of a new package provided by a ConfigurationPackageModifiedEvent or a ConfigurationPackageAddedEvent.
        /// </summary>
        /// <param name="package">The new package to load from.</param>
        private void HandleNewPackage(ConfigurationPackage package)
        {
            if (package.Description is null)
            {
                throw new ArgumentNullException($"{nameof(package)}.Description", $"A valid Description must be provided with {nameof(package)}.");
            }

            // Load configuration from new package only if it is the ConfigPackage mapped to this provider
            if (package.Description.Name == this.options.PackageName)
            {
                this.LoadPackage(package, reload: true);
                this.OnReload(); // Notify the change
            }
        }

        /// <summary>
        /// Load and populate data from <paramref name="package"/>.
        /// </summary>
        /// <param name="package">The package to load from.</param>
        /// <param name="reload">Whether or not to completely refresh <see cref="Data"/>.</param>
        private void LoadPackage(ConfigurationPackage package, bool reload = false)
        {
            if (reload)
            {
                this.Data.Clear();  // Remove the old keys on re-load
            }

            // call the delegate action to populate the Data
            this.options.ConfigAction(package, this.Data);
        }
    }
}
