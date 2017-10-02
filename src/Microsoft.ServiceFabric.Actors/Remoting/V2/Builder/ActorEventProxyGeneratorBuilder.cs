// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
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

    internal class ActorEventProxyGeneratorBuilder :ProxyGeneratorBuilder<ActorEventProxyGenerator, ActorEventProxy>
    {
#if !DotNetCoreClr
        private readonly MethodInfo invokeMethodInfoV1;
        private readonly V1.Builder.ActorEventProxyGeneratorBuilder proxyGeneratorBuilderV1;
#endif
        public ActorEventProxyGeneratorBuilder(ICodeBuilder codeBuilder) : base(codeBuilder)
        {
#if !DotNetCoreClr

            this.invokeMethodInfoV1 = this.proxyBaseType.GetMethod(
                "Invoke",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(object) },
                null);
            this.proxyGeneratorBuilderV1 = new V1.Builder.ActorEventProxyGeneratorBuilder(codeBuilder);
#endif
        }

        public override ProxyGeneratorBuildResult Build(
            Type proxyInterfaceType,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            
            var result =  base.Build(proxyInterfaceType, interfaceDescriptions);

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

            var v1ProxyGenerator =  new Remoting.V1.Builder.ActorEventProxyGeneratorWith(
                proxyInterfaceType,
                null,
                requestBodyTypes,
                responseBodyTypes);

            ((ActorEventProxyGenerator)result.ProxyGenerator).InitializeV1ProxyGenerator(v1ProxyGenerator);
        #endif
            return result;
        }


        protected override ActorEventProxyGenerator CreateProxyGenerator(Type proxyInterfaceType, Type proxyActivatorType)
        {
            return new ActorEventProxyGenerator(
                proxyInterfaceType,
                (IProxyActivator) Activator.CreateInstance(proxyActivatorType)
                );
        }

#if !DotNetCoreClr
        private void AddVoidMethodImplementationV1(
            ILGenerator ilGen,
            int interfaceIdV1,
            MethodDescription methodDescription,
            MethodBodyTypes methodBodyTypes)
        {
                var interfaceMethod = methodDescription.MethodInfo;
                var parameters = interfaceMethod.GetParameters();
            
                LocalBuilder requestBody = null;
                if (methodBodyTypes.RequestBodyType != null)
                {
                    // create requestBody and assign the values to its field from the arguments
                    requestBody = ilGen.DeclareLocal(methodBodyTypes.RequestBodyType);
                    var requestBodyCtor = methodBodyTypes.RequestBodyType.GetConstructor(Type.EmptyTypes);

                    if (requestBodyCtor != null)
                    {
                        ilGen.Emit(OpCodes.Newobj, requestBodyCtor);
                        ilGen.Emit(OpCodes.Stloc, requestBody);

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            ilGen.Emit(OpCodes.Ldloc, requestBody);
                            ilGen.Emit(OpCodes.Ldarg, i + 1);
                            ilGen.Emit(OpCodes.Stfld, methodBodyTypes.RequestBodyType.GetField(parameters[i].Name));
                        }
                    }
                }

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
        
        internal override void AddInterfaceImplementations(
            TypeBuilder classBuilder,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            // ensure that method data types are built for each of the remote interfaces
            var methodBodyTypesResultsMap = interfaceDescriptions.ToDictionary(
                d => d,
                d => this.CodeBuilder.GetOrBuildMethodBodyTypes(d.InterfaceType));


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

                        base.AddVoidMethodImplementation2(ilGen, interfaceDescription.Id, methodDescription,
                            interfaceDescription.InterfaceType.FullName);
#if !DotNetCoreClr

                        this.AddVoidMethodImplementationV1(ilGen,
                                                            interfaceDescription.V1Id,
                                                            methodDescription,
                                                            methodBodyTypes);
#endif
                        ilGen.Emit(OpCodes.Ret);

                    }
                }
            }
        }

    }
}