// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Migration
{
#if DotNetCoreClr
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;

    internal class ServiceFabricSetupFilter : IStartupFilter
    {
        private readonly string urlSuffix;
        private readonly ServiceFabricIntegrationOptions options;

        internal ServiceFabricSetupFilter(string urlSuffix, ServiceFabricIntegrationOptions options)
        {
            this.urlSuffix = urlSuffix;
            this.options = options;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                if (!string.IsNullOrEmpty(this.urlSuffix))
                {
                    app.UseServiceFabricMiddleware(this.urlSuffix);
                }

                next(app);
            };
        }
    }
#endif
}
