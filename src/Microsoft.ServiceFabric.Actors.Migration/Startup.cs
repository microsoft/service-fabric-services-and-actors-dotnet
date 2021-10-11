// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System.Diagnostics;
    using System.Linq;
#if DotNetCoreClr
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_0);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseMvc();

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                var infos = RouteInformation.GetAllRouteInformations(actionDescriptorCollectionProvider);
                Debug.WriteLine("======== ALL ROUTE INFORMATION ========");
                ActorTrace.Source.WriteInfo("Startup", "======== ALL ROUTE INFORMATION ========");
                ActorTrace.Source.WriteInfo("Startup", $"infos count: {infos.ToList().Count}");

                foreach (var info in infos)
                {
                    Debug.WriteLine(info.ToString());
                    ActorTrace.Source.WriteInfo("Startup", info.ToString());
                }

                Debug.WriteLine(string.Empty);
                Debug.WriteLine(string.Empty);
            });
        }
    }
#else
#endif
}
