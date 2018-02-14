// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Description
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal sealed class ServiceInterfaceDescription : InterfaceDescription
    {

        private ServiceInterfaceDescription(
            Type serviceInterfaceType,
            bool useCRCIdGeneration) :
            base("service", serviceInterfaceType, useCRCIdGeneration)
        {
        }

        public static ServiceInterfaceDescription Create(Type serviceInterfaceType)
        {
            EnsureServiceInterface(serviceInterfaceType);
            return new ServiceInterfaceDescription(serviceInterfaceType, false);
        }

        public static ServiceInterfaceDescription CreateUsingCRCId(Type serviceInterfaceType, bool checkForServiceInterface)
        {
            if (checkForServiceInterface)
            {
                EnsureServiceInterface(serviceInterfaceType);
            }

            return new ServiceInterfaceDescription(serviceInterfaceType, true);
        }

        private static void EnsureServiceInterface(Type serviceInterfaceType)
        {
            if (!serviceInterfaceType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        SR.ErrorNotAServiceInterface_InterfaceCheck,
                        serviceInterfaceType.FullName,
                        typeof(IService).FullName),
                    "serviceInterfaceType");
            }

            var nonActorParentInterface = serviceInterfaceType.GetNonServiceParentInterface();
            if (nonActorParentInterface != null)
            {
                if (nonActorParentInterface == serviceInterfaceType)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture,
                            SR.ErrorNotAServiceInterface_DerivationCheck1,
                            serviceInterfaceType.FullName,
                            typeof(IService).FullName),
                        "serviceInterfaceType");
                }
                else
                {
                    throw new ArgumentException(
                       string.Format(CultureInfo.CurrentCulture,
                           SR.ErrorNotAServiceInterface_DerivationCheck1,
                           serviceInterfaceType.FullName,
                           nonActorParentInterface.FullName,
                           typeof(IService).FullName),
                       "serviceInterfaceType");
                }
            }
        }
    }
}
