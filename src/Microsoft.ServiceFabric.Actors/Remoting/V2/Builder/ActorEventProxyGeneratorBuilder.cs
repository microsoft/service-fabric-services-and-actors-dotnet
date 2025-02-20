// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Description;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Builder;

    internal class ActorEventProxyGeneratorBuilder : Microsoft.ServiceFabric.Services.Remoting.V2.Builder.ProxyGeneratorBuilder<ActorEventProxyGenerator, ActorEventProxy>
    {
#if !DotNetCoreClr
        [Obsolete("This field is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        private readonly MethodInfo invokeMethodInfoV1;
        [Obsolete("This field is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        private readonly V1.Builder.ActorEventProxyGeneratorBuilder proxyGeneratorBuilderV1;
#endif

        public ActorEventProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
#if !DotNetCoreClr
#pragma warning disable 618
            this.invokeMethodInfoV1 = this.ProxyBaseType.GetMethod(
                "Invoke",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(object) },
                null);
            this.proxyGeneratorBuilderV1 = new V1.Builder.ActorEventProxyGeneratorBuilder(codeBuilder);
#pragma warning restore 618
#endif
        }

        public new ProxyGeneratorBuildResult Build(
            Type proxyInterfaceType,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            var result = base.Build(proxyInterfaceType, interfaceDescriptions);

#if !DotNetCoreClr

            // This code is to support V1 stack serialization logic
            var methodBodyTypesResultsMap = interfaceDescriptions.ToDictionary(
                d => d,
                d => this.CodeBuilder.GetOrBuildMethodBodyTypes(d.InterfaceType));

            var requestBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.V1Id,
                item => item.Value.GetRequestBodyTypes());

            var responseBodyTypes = methodBodyTypesResultsMap.ToDictionary(
                item => item.Key.V1Id,
                item => item.Value.GetResponseBodyTypes());

#pragma warning disable 618
            var v1ProxyGenerator = new Remoting.V1.Builder.ActorEventProxyGeneratorWith(
#pragma warning restore 618
                proxyInterfaceType,
                null,
                requestBodyTypes,
                responseBodyTypes);

#pragma warning disable 618
            ((ActorEventProxyGenerator)result.ProxyGenerator).InitializeV1ProxyGenerator(v1ProxyGenerator);
#pragma warning restore 618
#endif
            return result;
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
#if !DotNetCoreClr
#pragma warning disable 618
                        this.AddVoidMethodImplementationV1(
                            ilGen,
                            interfaceDescription.V1Id,
                            methodDescription,
                            wrappedRequestBody);
#pragma warning restore 618
#endif
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

#if !DotNetCoreClr
        [Obsolete("This method is part of the deprecated V1 service remoting stack. To switch to V2 remoting stack, refer to:")]
        private void AddVoidMethodImplementationV1(
            ILGenerator ilGen,
            int interfaceIdV1,
            MethodDescription methodDescription,
            LocalBuilder requestBody)
        {
            // call the base Invoke method
            ilGen.Emit(OpCodes.Ldarg_0); // base
            ilGen.Emit(OpCodes.Ldc_I4, interfaceIdV1); // interfaceId
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.V1Id); // methodId

            if (requestBody != null)
            {
                ilGen.Emit(OpCodes.Ldloc, requestBody);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
            }

            ilGen.EmitCall(OpCodes.Call, this.invokeMethodInfoV1, null);
        }
#endif
    }
}
