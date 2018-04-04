// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal abstract class ProxyGeneratorBuilder<TProxyGenerator, TProxy> : CodeBuilderModule
        where TProxyGenerator : ProxyGenerator
        where TProxy : ProxyBase
    {
        private readonly Type proxyBaseType;
        private readonly MethodInfo createMessage;
        private readonly MethodInfo invokeAsyncMethodInfo;
        private readonly MethodInfo invokeMethodInfo;
        private readonly MethodInfo continueWithResultMethodInfo;
        private readonly MethodInfo continueWithMethodInfo;

        public ProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
            this.proxyBaseType = typeof(TProxy);

            this.invokeAsyncMethodInfo = this.proxyBaseType.GetMethod(
                "InvokeAsyncV2",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(IServiceRemotingRequestMessageBody), typeof(CancellationToken) },
                null);

            this.createMessage = this.proxyBaseType.GetMethod(
                "CreateRequestMessageBodyV2",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(string), typeof(string), typeof(int) },
                null);

            this.invokeMethodInfo = this.proxyBaseType.GetMethod(
                "InvokeV2",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(IServiceRemotingRequestMessageBody) },
                null);

            this.continueWithResultMethodInfo = this.proxyBaseType.GetMethod(
                "ContinueWithResultV2",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(Task<IServiceRemotingResponseMessageBody>) },
                null);

            this.continueWithMethodInfo = this.proxyBaseType.GetMethod(
                "ContinueWith",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected Type ProxyBaseType => this.proxyBaseType;

        public virtual ProxyGeneratorBuildResult Build(
            Type proxyInterfaceType,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            // create the context to build the proxy
            var context = new CodeBuilderContext(
                assemblyName: this.CodeBuilder.Names.GetProxyAssemblyName(proxyInterfaceType),
                assemblyNamespace: this.CodeBuilder.Names.GetProxyAssemblyNamespace(proxyInterfaceType),
                enableDebugging: CodeBuilderAttribute.IsDebuggingEnabled(proxyInterfaceType));

            var result = new ProxyGeneratorBuildResult(context)
            {
                // build the proxy class that implements all of the interfaces explicitly
                ProxyType = this.BuildProxyType(context, proxyInterfaceType, interfaceDescriptions),
            };

            // build the activator type to create instances of the proxy
            result.ProxyActivatorType = this.BuildProxyActivatorType(context, proxyInterfaceType, result.ProxyType);

            // build the proxy generator
            result.ProxyGenerator = this.CreateProxyGenerator(
                proxyInterfaceType,
                result.ProxyActivatorType);

            context.Complete();
            return result;
        }

        internal void AddVoidMethodImplementation2(
            ILGenerator ilGen,
            int interfaceDescriptionId,
            MethodDescription methodDescription,
            string interfaceName)
        {
            var interfaceMethod = methodDescription.MethodInfo;

            var parameters = interfaceMethod.GetParameters();

            LocalBuilder requestBody = null;

            if (parameters.Length > 0)
            {
                ilGen.Emit(OpCodes.Ldarg_0); // base
                requestBody = ilGen.DeclareLocal(typeof(IServiceRemotingRequestMessageBody));
                ilGen.Emit(OpCodes.Ldstr, interfaceName);
                ilGen.Emit(OpCodes.Ldstr, methodDescription.Name);
                ilGen.Emit(OpCodes.Ldc_I4, parameters.Length);

                ilGen.EmitCall(OpCodes.Call, this.createMessage, null);
                ilGen.Emit(OpCodes.Stloc, requestBody);

                var setMethod = typeof(IServiceRemotingRequestMessageBody).GetMethod("SetParameter");

                // Add to Dictionary
                for (var i = 0; i < parameters.Length; i++)
                {
                    ilGen.Emit(OpCodes.Ldloc, requestBody);
                    ilGen.Emit(OpCodes.Ldc_I4, i);
                    ilGen.Emit(OpCodes.Ldstr, parameters[i].Name);
                    ilGen.Emit(OpCodes.Ldarg, i + 1);
                    if (!parameters[i].ParameterType.IsClass)
                    {
                        ilGen.Emit(OpCodes.Box, parameters[i].ParameterType);
                    }

                    ilGen.Emit(OpCodes.Callvirt, setMethod);
                }
            }

            // call the base Invoke method
            ilGen.Emit(OpCodes.Ldarg_0); // base
            ilGen.Emit(OpCodes.Ldc_I4, interfaceDescriptionId); // interfaceId
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId

            if (parameters.Length > 0)
            {
                ilGen.Emit(OpCodes.Ldloc, requestBody);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
            }

            ilGen.EmitCall(OpCodes.Call, this.invokeMethodInfo, null);
        }

        internal virtual void AddInterfaceImplementations(
            TypeBuilder classBuilder,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            foreach (var item in interfaceDescriptions)
            {
                var interfaceDescription = item;

                foreach (var methodDescription in interfaceDescription.Methods)
                {
                    if (TypeUtility.IsTaskType(methodDescription.ReturnType))
                    {
                        this.AddAsyncMethodImplementation(
                            classBuilder,
                            interfaceDescription.Id,
                            methodDescription,
                            interfaceDescription.InterfaceType.FullName);
                    }
                    else if (TypeUtility.IsVoidType(methodDescription.ReturnType))
                    {
                        this.AddVoidMethodImplementation(
                            classBuilder,
                            interfaceDescription.Id,
                            methodDescription,
                            interfaceDescription.InterfaceType.FullName);
                    }
                }
            }
        }

        protected abstract TProxyGenerator CreateProxyGenerator(
            Type proxyInterfaceType,
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

        private Type BuildProxyType(
            CodeBuilderContext context,
            Type proxyInterfaceType,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            var classBuilder = CodeBuilderUtils.CreateClassBuilder(
                context.ModuleBuilder,
                ns: context.AssemblyNamespace,
                className: this.CodeBuilder.Names.GetProxyClassName(proxyInterfaceType),
                baseType: this.proxyBaseType,
                interfaces: interfaceDescriptions.Select(p => p.InterfaceType).ToArray());

            this.AddGetReturnValueMethod(classBuilder, interfaceDescriptions);
            this.AddInterfaceImplementations(classBuilder, interfaceDescriptions);

            return classBuilder.CreateTypeInfo().AsType();
        }

        private void AddVoidMethodImplementation(
            TypeBuilder classBuilder,
            int interfaceDescriptionId,
            MethodDescription methodDescription,
            string interfaceName)
        {
            var interfaceMethod = methodDescription.MethodInfo;

            var methodBuilder = CodeBuilderUtils.CreateExplitInterfaceMethodBuilder(
                classBuilder,
                interfaceMethod);

            var ilGen = methodBuilder.GetILGenerator();

            this.AddVoidMethodImplementation2(
                ilGen,
                interfaceDescriptionId,
                methodDescription,
                interfaceName);

            ilGen.Emit(OpCodes.Ret);
        }

        private void AddGetReturnValueMethod(
            TypeBuilder classBuilder,
            IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            var methodBuilder = CodeBuilderUtils.CreateProtectedMethodBuilder(
                classBuilder,
                "GetReturnValue",
                typeof(object), // return value from the reponseBody
                typeof(int), // interfaceId
                typeof(int), // methodId
                typeof(object)); // responseBody

            var ilGen = methodBuilder.GetILGenerator();

            foreach (var item in interfaceDescriptions)
            {
                var interfaceDescription = item;

                foreach (var methodDescription in interfaceDescription.Methods)
                {
                    if (methodDescription.ReturnType == null)
                    {
                        continue;
                    }

                    var elseLabel = ilGen.DefineLabel();

                    this.AddIfInterfaceIdAndMethodIdReturnRetvalBlock(
                        ilGen,
                        elseLabel,
                        interfaceDescription.Id,
                        methodDescription.Id,
                        methodDescription.ReturnType);

                    ilGen.MarkLabel(elseLabel);
                }
            }

            // return null; (if method id's and interfaceId do not match)
            ilGen.Emit(OpCodes.Ldnull);
            ilGen.Emit(OpCodes.Ret);
        }

        private void AddAsyncMethodImplementation(
            TypeBuilder classBuilder,
            int interfaceId,
            MethodDescription methodDescription,
            string interfaceName)
        {
            var interfaceMethod = methodDescription.MethodInfo;
            var parameters = interfaceMethod.GetParameters();

            var methodBuilder = CodeBuilderUtils.CreateExplitInterfaceMethodBuilder(
                classBuilder,
                interfaceMethod);

            var ilGen = methodBuilder.GetILGenerator();

            // Create Dictionary
            var parameterLength = parameters.Length;
            if (methodDescription.HasCancellationToken)
            {
                // Cancellation token is tracked locally and should not be serialized and sent
                // as a part of the request body.
                parameterLength = parameterLength - 1;
            }

            LocalBuilder requestMessage = null;
            if (parameters.Length > 0)
            {
                ilGen.Emit(OpCodes.Ldarg_0); // base
                requestMessage = ilGen.DeclareLocal(typeof(IServiceRemotingRequestMessageBody));
                ilGen.Emit(OpCodes.Ldstr, interfaceName);
                ilGen.Emit(OpCodes.Ldstr, methodDescription.Name);
                ilGen.Emit(OpCodes.Ldc_I4, parameterLength);

                ilGen.EmitCall(OpCodes.Call, this.createMessage, null);
                ilGen.Emit(OpCodes.Stloc, requestMessage);

                var setMethod = typeof(IServiceRemotingRequestMessageBody).GetMethod("SetParameter");

                // Add to Dictionary
                for (var i = 0; i < parameterLength; i++)
                {
                    ilGen.Emit(OpCodes.Ldloc, requestMessage);
                    ilGen.Emit(OpCodes.Ldc_I4, i);
                    ilGen.Emit(OpCodes.Ldstr, parameters[i].Name);
                    ilGen.Emit(OpCodes.Ldarg, i + 1);
                    if (!parameters[i].ParameterType.IsClass)
                    {
                        ilGen.Emit(OpCodes.Box, parameters[i].ParameterType);
                    }

                    ilGen.Emit(OpCodes.Callvirt, setMethod);
                }
            }

            var objectTask = ilGen.DeclareLocal(typeof(Task<IServiceRemotingResponseMessageBody>));

            // call the base InvokeAsync method
            ilGen.Emit(OpCodes.Ldarg_0); // base
            ilGen.Emit(OpCodes.Ldc_I4, interfaceId); // interfaceId
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId

            if (requestMessage != null)
            {
                ilGen.Emit(OpCodes.Ldloc, requestMessage);
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
            if ((TypeUtility.IsTaskType(methodDescription.ReturnType) &&
                 methodDescription.ReturnType.GetTypeInfo().IsGenericType))
            {
                var retvalType = methodDescription.ReturnType.GetGenericArguments()[0];

                ilGen.Emit(OpCodes.Ldarg_0); // base pointer
                ilGen.Emit(OpCodes.Ldloc, objectTask); // task<IServiceRemotingResponseMessageBody>
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
            ilGen.Emit(OpCodes.Ldloc, castedResponseBody);
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
