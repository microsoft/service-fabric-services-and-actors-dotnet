// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal abstract class ProxyGeneratorBuilder<TProxyGenerator, TProxy> : CodeBuilderModule
        where TProxyGenerator : ProxyGenerator
        where TProxy : ProxyBase
    {
        private readonly Type proxyBaseType;
        private readonly MethodInfo invokeAsyncMethodInfo;
        private readonly MethodInfo invokeMethodInfo;
        private readonly MethodInfo continueWithResultMethodInfo;
        private readonly MethodInfo continueWithMethodInfo;

        protected ProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
            this.proxyBaseType = typeof(TProxy);

            this.invokeAsyncMethodInfo = this.proxyBaseType.GetMethod(
                "InvokeAsync",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(object), typeof(CancellationToken) },
                null);

            this.invokeMethodInfo = this.proxyBaseType.GetMethod(
                "Invoke",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(object) },
                null);

            this.continueWithResultMethodInfo = this.proxyBaseType.GetMethod(
                "ContinueWithResult",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(Task<object>) },
                null);

            this.continueWithMethodInfo = this.proxyBaseType.GetMethod(
                "ContinueWith",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public ProxyGeneratorBuildResult Build(
            Type proxyInterfaceType,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            // create the context to build the proxy
            var context = new CodeBuilderContext(
                assemblyName: this.CodeBuilder.Names.GetProxyAssemblyName(proxyInterfaceType),
                assemblyNamespace: this.CodeBuilder.Names.GetProxyAssemblyNamespace(proxyInterfaceType),
                enableDebugging: CodeBuilderAttribute.IsDebuggingEnabled(proxyInterfaceType));
            var result = new ProxyGeneratorBuildResult(context);

            // ensure that method data types are built for each of the remote interfaces
            var methodBodyTypesResultsMap = interfaceDescriptions.ToDictionary(
                d => d,
                d => this.CodeBuilder.GetOrBuildMethodBodyTypes(d.InterfaceType));

            // build the proxy class that implements all of the interfaces explicitly
            result.ProxyType = this.BuildProxyType(context, proxyInterfaceType, methodBodyTypesResultsMap);

            // build the activator type to create instances of the proxy
            result.ProxyActivatorType = this.BuildProxyActivatorType(context, proxyInterfaceType, result.ProxyType);

            // build the proxy generator
            result.ProxyGenerator = this.CreateProxyGenerator(
                proxyInterfaceType,
                methodBodyTypesResultsMap,
                result.ProxyActivatorType);

            context.Complete();
            return result;
        }

        protected virtual void AddInterfaceImplementations(
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

                    if (TypeUtility.IsTaskType(methodDescription.ReturnType))
                    {
                        this.AddAsyncMethodImplementation(
                            classBuilder,
                            interfaceDescription.Id,
                            methodDescription,
                            methodBodyTypes);
                    }
                    else if (TypeUtility.IsVoidType(methodDescription.ReturnType))
                    {
                        this.AddVoidMethodImplementation(
                            classBuilder,
                            interfaceDescription.Id,
                            methodDescription,
                            methodBodyTypes);
                    }
                }
            }
        }

        protected virtual void AddAsyncMethodImplementation(
            TypeBuilder classBuilder,
            int interfaceId,
            MethodDescription methodDescription,
            MethodBodyTypes methodBodyTypes)
        {
            var interfaceMethod = methodDescription.MethodInfo;
            var parameters = interfaceMethod.GetParameters();

            var methodBuilder = CodeBuilderUtils.CreateExplitInterfaceMethodBuilder(
                classBuilder,
                interfaceMethod);

            var ilGen = methodBuilder.GetILGenerator();

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

                    var argsLength = parameters.Length;
                    if (methodDescription.HasCancellationToken)
                    {
                        // Cancellation token is tracked locally and should not be serialized and sent
                        // as a part of the request body.
                        argsLength = argsLength - 1;
                    }

                    for (var i = 0; i < argsLength; i++)
                    {
                        ilGen.Emit(OpCodes.Ldloc, requestBody);
                        ilGen.Emit(OpCodes.Ldarg, i + 1);
                        ilGen.Emit(OpCodes.Stfld, methodBodyTypes.RequestBodyType.GetField(parameters[i].Name));
                    }
                }
            }

            var objectTask = ilGen.DeclareLocal(typeof(Task<object>));

            // call the base InvokeAsync method
            ilGen.Emit(OpCodes.Ldarg_0); // base
            ilGen.Emit(OpCodes.Ldc_I4, interfaceId); // interfaceId
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId

            if (requestBody != null)
            {
                ilGen.Emit(OpCodes.Ldloc, requestBody);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
            }

            // Cancellation token argument
            if (methodDescription.HasCancellationToken)
            {
                // Last argument should be the cancellation token
                var cancellationTokenArgIndex = parameters.Length;
                ilGen.Emit(OpCodes.Ldarg, cancellationTokenArgIndex);
            }
            else
            {
                var cancellationTokenNone = typeof(CancellationToken).GetMethod("get_None");
                ilGen.EmitCall(OpCodes.Call, cancellationTokenNone, null);
            }

            ilGen.EmitCall(OpCodes.Call, this.invokeAsyncMethodInfo, null);
            ilGen.Emit(OpCodes.Stloc, objectTask);

            // call the base method to get the continuation task and
            // convert the response body to return value when the task is finished
            if (methodBodyTypes.ResponseBodyType != null)
            {
                var retvalType = methodDescription.ReturnType.GetGenericArguments()[0];

                ilGen.Emit(OpCodes.Ldarg_0); // base pointer
                ilGen.Emit(OpCodes.Ldc_I4, interfaceId); // interfaceId
                ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId
                ilGen.Emit(OpCodes.Ldloc, objectTask); // task<object>
                ilGen.Emit(OpCodes.Call, this.continueWithResultMethodInfo.MakeGenericMethod(retvalType));
                ilGen.Emit(OpCodes.Ret); // return base.ContinueWithResult<TResult>(task);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_0); // base pointer
                ilGen.Emit(OpCodes.Ldloc, objectTask); // task<object>
                ilGen.Emit(OpCodes.Call, this.continueWithMethodInfo);
                ilGen.Emit(OpCodes.Ret); // return base.ContinueWith(task);
            }
        }

        protected abstract TProxyGenerator CreateProxyGenerator(
          Type proxyInterfaceType,
          IDictionary<InterfaceDescription, MethodBodyTypesBuildResult> methodBodyTypesResultsMap,
          Type proxyActivatorType);

        private static void AddCreateInstanceMethod(
           TypeBuilder classBuilder,
           Type proxyType)
        {
            var methodBuilder = CodeBuilderUtils.CreatePublicMethodBuilder(
                classBuilder,
                "CreateInstance",
                typeof(Remoting.Builder.ProxyBase));

            var ilGen = methodBuilder.GetILGenerator();
            var proxyCtor = proxyType.GetConstructor(Type.EmptyTypes);
            if (proxyCtor != null)
            {
                ilGen.Emit(OpCodes.Newobj, proxyCtor);
                ilGen.Emit(OpCodes.Ret);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
                ilGen.Emit(OpCodes.Ret);
            }
        }

        private Type BuildProxyActivatorType(
            CodeBuilderContext context,
            Type proxyInterfaceType,
            Type proxyType)
        {
            var classBuilder = CodeBuilderUtils.CreateClassBuilder(
                context.ModuleBuilder,
                ns: context.AssemblyNamespace,
                className: this.CodeBuilder.Names.GetProxyActivatorClassName(proxyInterfaceType),
                interfaces: new[] { typeof(IProxyActivator) });

            AddCreateInstanceMethod(classBuilder, proxyType);
            return classBuilder.CreateTypeInfo().AsType();
        }

        private void AddVoidMethodImplementation(
            TypeBuilder classBuilder,
            int interfaceId,
            MethodDescription methodDescription,
            MethodBodyTypes methodBodyTypes)
        {
            var interfaceMethod = methodDescription.MethodInfo;
            var parameters = interfaceMethod.GetParameters();

            var methodBuilder = CodeBuilderUtils.CreateExplitInterfaceMethodBuilder(
                classBuilder,
                interfaceMethod);

            var ilGen = methodBuilder.GetILGenerator();

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
            ilGen.Emit(OpCodes.Ldc_I4, interfaceId); // interfaceId
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId

            if (requestBody != null)
            {
                ilGen.Emit(OpCodes.Ldloc, requestBody);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
            }

            ilGen.EmitCall(OpCodes.Call, this.invokeMethodInfo, null);
            ilGen.Emit(OpCodes.Ret);
        }

        private Type BuildProxyType(
            CodeBuilderContext context,
            Type proxyInterfaceType,
            IDictionary<InterfaceDescription, MethodBodyTypesBuildResult> methodBodyTypesResultsMap)
        {
            var classBuilder = CodeBuilderUtils.CreateClassBuilder(
                context.ModuleBuilder,
                ns: context.AssemblyNamespace,
                className: this.CodeBuilder.Names.GetProxyClassName(proxyInterfaceType),
                baseType: this.proxyBaseType,
                interfaces: methodBodyTypesResultsMap.Select(item => item.Key.InterfaceType).ToArray());

            this.AddGetReturnValueMethod(classBuilder, methodBodyTypesResultsMap);
            this.AddInterfaceImplementations(classBuilder, methodBodyTypesResultsMap);

            return classBuilder.CreateTypeInfo().AsType();
        }

        private void AddGetReturnValueMethod(
            TypeBuilder classBuilder,
            IDictionary<InterfaceDescription, MethodBodyTypesBuildResult> methodBodyTypesResultsMap)
        {
            var methodBuilder = CodeBuilderUtils.CreateProtectedMethodBuilder(
                classBuilder,
                "GetReturnValue",
                typeof(object), // return value from the reponseBody
                typeof(int), // interfaceId
                typeof(int), // methodId
                typeof(object)); // responseBody

            var ilGen = methodBuilder.GetILGenerator();

            foreach (var item in methodBodyTypesResultsMap)
            {
                var interfaceDescription = item.Key;
                var methodBodyTypesMap = item.Value.MethodBodyTypesMap;

                foreach (var methodDescription in interfaceDescription.Methods)
                {
                    var methodBodyTypes = methodBodyTypesMap[methodDescription.Name];
                    if (methodBodyTypes.ResponseBodyType == null)
                    {
                        continue;
                    }

                    var elseLabel = ilGen.DefineLabel();

                    this.AddIfInterfaceIdAndMethodIdReturnRetvalBlock(
                        ilGen,
                        elseLabel,
                        interfaceDescription.Id,
                        methodDescription.Id,
                        methodBodyTypes.ResponseBodyType);

                    ilGen.MarkLabel(elseLabel);
                }
            }

            // return null; (if method id's and interfaceId do not mGetReturnValueatch)
            ilGen.Emit(OpCodes.Ldnull);
            ilGen.Emit(OpCodes.Ret);
        }

        private void AddIfInterfaceIdAndMethodIdReturnRetvalBlock(
            ILGenerator ilGen,
            Label elseLabel,
            int interfaceId,
            int methodId,
            Type responseBodyType)
        {
            // if (interfaceId == <interfaceId>)
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldc_I4, interfaceId);
            ilGen.Emit(OpCodes.Bne_Un_S, elseLabel);

            // if (methodId == <methodId>)
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldc_I4, methodId);
            ilGen.Emit(OpCodes.Bne_Un_S, elseLabel);

            var castedResponseBody = ilGen.DeclareLocal(responseBodyType);
            ilGen.Emit(OpCodes.Ldarg_3); // load responseBody object
            ilGen.Emit(OpCodes.Castclass, responseBodyType); // cast it to responseBodyType
            ilGen.Emit(OpCodes.Stloc, castedResponseBody); // store casted result to castedResponseBody local variable

            var fieldInfo = responseBodyType.GetField(this.CodeBuilder.Names.RetVal);
            ilGen.Emit(OpCodes.Ldloc, castedResponseBody);
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);
            if (!fieldInfo.FieldType.GetTypeInfo().IsClass)
            {
                ilGen.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            ilGen.Emit(OpCodes.Ret);
        }
    }
}
