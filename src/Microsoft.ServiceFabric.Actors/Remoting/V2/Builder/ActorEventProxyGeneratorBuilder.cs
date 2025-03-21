// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Builder;
using Microsoft.ServiceFabric.Services.Remoting.Description;

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    internal class ActorEventProxyGeneratorBuilder : Microsoft.ServiceFabric.Services.Remoting.V2.Builder.ProxyGeneratorBuilder<ActorEventProxyGenerator, ActorEventProxy>
    {
        public ActorEventProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
        }

        protected override void AddInterfaceImplementations(
            TypeBuilder classBuilder,
            IDictionary<InterfaceDescription, MethodBodyTypesBuildResult> methodBodyTypesResultsMap)
        {
            foreach (var item in methodBodyTypesResultsMap)
            {
                var interfaceDescription = item.Key;
                var methodBodyTypesMap = item.Value.MethodBodyTypesMap;

                foreach (var methodDescription in interfaceDescription.Methods)
                {
                    var methodBodyTypes = methodBodyTypesMap[methodDescription.Name];

                    if (TypeUtility.IsVoidType(methodDescription.ReturnType))
                    {
                        var interfaceMethod = methodDescription.MethodInfo;

                        var methodBuilder = CodeBuilderUtils.CreateExplitInterfaceMethodBuilder(
                            classBuilder,
                            interfaceMethod);

                        var ilGen = methodBuilder.GetILGenerator();

                        // Create Wrapped Request
                        LocalBuilder wrappedRequestBody =
                            CreateWrappedRequestBody(methodDescription, methodBodyTypes, ilGen, methodDescription.MethodInfo.GetParameters());

                        this.AddVoidMethodImplementation2(
                            ilGen,
                            interfaceDescription.Id,
                            methodDescription,
                            wrappedRequestBody,
                            interfaceDescription.InterfaceType.FullName);

                        ilGen.Emit(OpCodes.Ret);
                    }
                }
            }
        }

        protected override ActorEventProxyGenerator CreateProxyGenerator(Type proxyInterfaceType, Type proxyActivatorType)
        {
            return new ActorEventProxyGenerator(
                proxyInterfaceType,
                (IProxyActivator)Activator.CreateInstance(proxyActivatorType));
        }
    }
}
