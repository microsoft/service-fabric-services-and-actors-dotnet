// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Fabric;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration
{
    internal class ServiceFabricConfigurationSource : IConfigurationSource
    {
        private readonly ServiceFabricConfigurationOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricConfigurationSource"/> class.
        /// </summary>
        /// <param name="activationContext">the code package activation context.</param>
        /// <param name="options">the configuration options.</param>
        public ServiceFabricConfigurationSource(ICodePackageActivationContext activationContext, ServiceFabricConfigurationOptions options)
        {
            this.ActivationContext = activationContext;
            this.options = options;
        }

        public ICodePackageActivationContext ActivationContext { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServiceFabricConfigurationProvider(this.ActivationContext, this.options);
        }
    }
}
