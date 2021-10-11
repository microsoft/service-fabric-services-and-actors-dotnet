// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
#if DotNetCoreClr
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Routing;

#pragma warning disable SA1402 // File may only contain a single type
    internal class RouteInformation
#pragma warning restore SA1402 // File may only contain a single type
    {
        public string HttpMethod { get; set; } = "GET";

        public string Area { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Invocation { get; set; } = string.Empty;

        public static IEnumerable<RouteInformation> GetAllRouteInformations(IActionDescriptorCollectionProvider m_actionDescriptorCollectionProvider)
        {
            List<RouteInformation> ret = new List<RouteInformation>();

            var routes = m_actionDescriptorCollectionProvider.ActionDescriptors.Items;
            foreach (ActionDescriptor e1 in routes)
            {
                RouteInformation info = new RouteInformation();

                // Area
                if (e1.RouteValues.ContainsKey("area"))
                {
                    info.Area = e1.RouteValues["area"];
                }

                // Path and Invocation of Razor Pages
                if (e1 is PageActionDescriptor)
                {
                    var e = e1 as PageActionDescriptor;
                    info.Path = e.ViewEnginePath;
                    info.Invocation = e.RelativePath;
                }

                // Path of Route Attribute
                if (e1.AttributeRouteInfo != null)
                {
                    var e = e1;
                    info.Path = $"/{e.AttributeRouteInfo.Template}";
                }

                // Path and Invocation of Controller/Action
                if (e1 is ControllerActionDescriptor)
                {
                    var e = e1 as ControllerActionDescriptor;
                    if (info.Path == string.Empty)
                    {
                        info.Path = $"/{e.ControllerName}/{e.ActionName}";
                    }

                    info.Invocation = $"{e.ControllerName}Controller.{e.ActionName}";
                }

                // Extract HTTP Verb
                if (e1.ActionConstraints != null && e1.ActionConstraints.Select(t => t.GetType()).Contains(typeof(HttpMethodActionConstraint)))
                {
                    HttpMethodActionConstraint httpMethodAction =
                        e1.ActionConstraints.FirstOrDefault(a => a.GetType() == typeof(HttpMethodActionConstraint)) as HttpMethodActionConstraint;

                    if (httpMethodAction != null)
                    {
                        info.HttpMethod = string.Join(",", httpMethodAction.HttpMethods);
                    }
                }

                // Special controller path
                if (info.Path == "/RouteAnalyzer_Main/ShowAllRoutes")
                {
                    info.Path = RouteAnalyzerRouteBuilderExtensions.RouteAnalyzerUrlPath;
                }

                // Additional information of invocation
                info.Invocation += $" ({e1.DisplayName})";

                // Generating List
                ret.Add(info);
            }

            // Result
            return ret;
        }

        public override string ToString()
        {
            return $"RouteInformation{{Area:\"{this.Area}\", HttpMethod: \"{this.HttpMethod}\", Path:\"{this.Path}\", Invocation:\"{this.Invocation}\"}}";
        }
    }

#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable SA1402 // File may only contain a single type
    internal static class RouteAnalyzerRouteBuilderExtensions
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore SA1204 // Static elements should appear before instance elements
    {
        public static string RouteAnalyzerUrlPath { get; private set; } = string.Empty;

        public static IRouteBuilder MapRouteAnalyzer(this IRouteBuilder routes, string routeAnalyzerUrlPath)
        {
            RouteAnalyzerUrlPath = routeAnalyzerUrlPath;
            routes.Routes.Add(new Router(routes.DefaultHandler, routeAnalyzerUrlPath));
            return routes;
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    internal class Router : IRouter
#pragma warning restore SA1402 // File may only contain a single type
    {
        private IRouter defaultRouter;
        private string routePath;

        public Router(IRouter defaultRouter, string routePath)
        {
            this.defaultRouter = defaultRouter;
            this.routePath = routePath;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        async Task IRouter.RouteAsync(RouteContext context)
        {
            if (context.HttpContext.Request.Path == this.routePath)
            {
                var routeData = new RouteData(context.RouteData);
                routeData.Routers.Add(this.defaultRouter);
                routeData.Values["controller"] = "RouteAnalyzer_Main";
                routeData.Values["action"] = "ShowAllRoutes";
                context.RouteData = routeData;
                await this.defaultRouter.RouteAsync(context);
            }
        }
    }
#endif
}
