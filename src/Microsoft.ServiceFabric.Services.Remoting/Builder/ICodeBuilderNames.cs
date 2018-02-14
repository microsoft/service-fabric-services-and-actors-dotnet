// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;

    internal interface ICodeBuilderNames
    {
        string InterfaceId { get; }

        string MethodId { get; }

        string RetVal { get; }

        string RequestBody { get; }

        string GetMethodBodyTypesAssemblyName(Type interfaceType);

        string GetMethodBodyTypesAssemblyNamespace(Type interfaceType);

        string GetRequestBodyTypeName(string methodName);

        string GetResponseBodyTypeName(string methodName);

        string GetDataContractNamespace();

        string GetMethodDispatcherAssemblyName(Type interfaceType);

        string GetMethodDispatcherAssemblyNamespace(Type interfaceType);

        string GetMethodDispatcherClassName(Type interfaceType);

        string GetProxyAssemblyName(Type interfaceType);

        string GetProxyAssemblyNamespace(Type interfaceType);

        string GetProxyClassName(Type interfaceType);

        string GetProxyActivatorClassName(Type interfaceType);
    }
}
