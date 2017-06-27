// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Specifies the class used by the ServiceRemoting to lookup the interfaces implemented
    /// by the service.
    /// </summary>
    public sealed class ServiceTypeInformation
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeInformation class.
        /// </summary>
        private ServiceTypeInformation()
        {
        }

        /// <summary>
        /// Gets or sets the interface types implemented.
        /// </summary>
        /// <value>List of interface types</value>
        public IEnumerable<Type> InterfaceTypes { get; private set; }

        /// <summary>
        /// Gets or sets the type of the class implementing the service interface.
        /// </summary>
        /// <value><see cref="System.Type"/> of the class implementing the service interface.</value>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Gets or sets if class implementing service interface is abstract.
        /// </summary>
        /// <value>true if class implementing service interface is abstract, otherwise false.</value>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets the Factory method that constructs a ServiceTypeInformation object from the given type.
        /// </summary>
        /// <param name="serviceType">Type to examine</param>
        /// <param name="serviceTypeInformation">The constructed ServiceTypeInformation</param>
        /// <returns>true if the specified serviceType is a service, false otherwise</returns>
        public static bool TryGet(Type serviceType, out ServiceTypeInformation serviceTypeInformation)
        {
            try
            {
                serviceTypeInformation = Get(serviceType);
                return true;
            }
            catch (ArgumentException)
            {
                serviceTypeInformation = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the Factory method that constructs a ServiceTypeInformation object from the given type.
        /// </summary>
        /// <param name="serviceType">Type to examine</param>
        /// <returns>ServiceTypeInformation</returns>
        public static ServiceTypeInformation Get(Type serviceType)
        {
            var serviceInterfaces = serviceType.GetServiceInterfaces();
            if ((serviceInterfaces.Length == 0) && (!serviceType.GetTypeInfo().IsAbstract))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorNoServiceInterfaceFound,
                        serviceType.FullName,
                        typeof(IService).FullName),
                    "serviceType");
            }

            return new ServiceTypeInformation()
            {
                InterfaceTypes = serviceInterfaces,
                ImplementationType = serviceType,
                IsAbstract = serviceType.GetTypeInfo().IsAbstract,
            };
        }
    }
}
