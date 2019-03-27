// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System;
    using System.Globalization;

    /// <summary>
    /// This class provides the logic for deriving the names of various items within the manifest from the code.
    /// It is used by framework components when names are not specified in the API and the framework types have
    /// to default it to a meaningful name derived from the service type.
    /// </summary>
    public static class ServiceNameFormat
    {
        /// <summary>
        /// Gets the default endpoint resource name for the given service type
        /// </summary>
        /// <param name="serviceInterfaceType">Service interface type name.</param>
        /// <returns>The name of the endpoint resource.</returns>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>
        ///         If the type name is <code>IMyService</code>, this method returns <code>MyServiceEndpoint</code> as the name of the endpoint resource.
        ///     </item>
        ///     <item>
        ///         If the type name is <code>Foo</code>, this method returns <code>FooServiceEndpoint</code> as the name of the endpoint resource.
        ///     </item>
        /// </list>
        /// </remarks>
        public static string GetEndpointName(Type serviceInterfaceType)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Endpoint", GetName(serviceInterfaceType));
        }

        internal static string GetName(Type serviceInterfaceType)
        {
            return GetName(serviceInterfaceType.Name);
        }

        internal static string GetName(string serviceInterfaceTypeName)
        {
            var serviceName = serviceInterfaceTypeName;
            if (!serviceName.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase))
            {
                serviceName = string.Format(CultureInfo.InvariantCulture, "{0}Service", serviceName);
            }

            if ((serviceName[0] == 'I') && !char.IsLower(serviceName[1]))
            {
                return serviceName.Substring(1);
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, serviceName);
            }
        }
    }
}
