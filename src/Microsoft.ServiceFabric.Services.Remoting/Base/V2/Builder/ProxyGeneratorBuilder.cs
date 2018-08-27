// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Base.Description;

    internal abstract class ProxyGeneratorBuilder<TProxyGenerator, TProxy> : Base.Builder.ProxyGeneratorBuilder<TProxyGenerator, TProxy>
        where TProxyGenerator : ProxyGenerator
        where TProxy : ProxyBase
    {
        private readonly Type proxyBaseType;
        private readonly MethodInfo createMessage;
        private readonly MethodInfo invokeAsyncMethodInfo;
        private readonly MethodInfo invokeMethodInfo;
        private readonly MethodInfo continueWithResultMethodInfo;
        private readonly MethodInfo continueWithMethodInfo;
        private readonly MethodInfo checkIfitsWrapped;

        public ProxyGeneratorBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
            this.proxyBaseType = typeof(TProxy);

            this.invokeAsyncMethodInfo = this.proxyBaseType.GetMethod(
                "InvokeAsyncV2",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(int), typeof(int), typeof(string), typeof(IServiceRemotingRequestMessageBody), typeof(CancellationToken) },
                null);

            this.checkIfitsWrapped = this.proxyBaseType.GetMethod(
               "CheckIfItsWrappedRequest",
               BindingFlags.Instance | BindingFlags.NonPublic,
               null,
               CallingConventions.Any,
               new[] { typeof(IServiceRemotingRequestMessageBody) },
               null);

            this.createMessage = this.proxyBaseType.GetMethod(
                "CreateRequestMessageBodyV2",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] { typeof(string), typeof(string), typeof(int), typeof(object) },
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
                new[] { typeof(int), typeof(int), typeof(Task<IServiceRemotingResponseMessageBody>) },
                null);

            this.continueWithMethodInfo = this.proxyBaseType.GetMethod(
                "ContinueWith",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected Type ProxyBaseType => this.proxyBaseType;

        internal static LocalBuilder CreateWrappedRequestBody(
        MethodDescription methodDescription,
        MethodBodyTypes methodBodyTypes,
        ILGenerator ilGen,
        ParameterInfo[] parameters)
        {
            var parameterLength = parameters.Length;
            if (methodDescription.HasCancellationToken)
            {
                // Cancellation token is tracked locally and should not be serialized and sent
                // as a part of the request body.
                parameterLength = parameterLength - 1;
            }

            if (parameterLength == 0)
            {
                return null;
            }

            LocalBuilder wrappedRequestBody = ilGen.DeclareLocal(methodBodyTypes.RequestBodyType);
            var requestBodyCtor = methodBodyTypes.RequestBodyType.GetConstructor(Type.EmptyTypes);

            if (requestBodyCtor != null)
            {
                ilGen.Emit(OpCodes.Newobj, requestBodyCtor);
                ilGen.Emit(OpCodes.Stloc, wrappedRequestBody);

                var argsLength = parameters.Length;
                if (methodDescription.HasCancellationToken)
                {
                    // Cancellation token is tracked locally and should not be serialized and sent
                    // as a part of the request body.
                    argsLength = argsLength - 1;
                }

                for (var i = 0; i < argsLength; i++)
                {
                    ilGen.Emit(OpCodes.Ldloc, wrappedRequestBody);
                    ilGen.Emit(OpCodes.Ldarg, i + 1);
                    ilGen.Emit(OpCodes.Stfld, methodBodyTypes.RequestBodyType.GetField(parameters[i].Name));
                }
            }

            return wrappedRequestBody;
        }

        internal void AddVoidMethodImplementation2(
            ILGenerator ilGen,
            int interfaceDescriptionId,
            MethodDescription methodDescription,
            LocalBuilder wrappedRequestBody,
            string interfaceName)
        {
            var interfaceMethod = methodDescription.MethodInfo;

            var parameters = interfaceMethod.GetParameters();

            LocalBuilder requestBody = null;

            if (parameters.Length > 0)
            {
                // create IServiceRemotingRequestMessageBody message
                requestBody = this.CreateRequestRemotingMessageBody(
                    methodDescription,
                    interfaceName,
                    ilGen,
                    parameters.Length,
                    wrappedRequestBody);

                // Check if requestMessage is not implementing WrappedMessage , then call SetParam
                this.SetParameterIfNeeded(ilGen, requestBody, parameters.Length, parameters);
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

        protected override TProxyGenerator CreateProxyGenerator(
            Type proxyInterfaceType,
            IDictionary<InterfaceDescription, MethodBodyTypesBuildResult> methodBodyTypesResultsMap,
            Type proxyActivatorType)
        {
            return this.CreateProxyGenerator(proxyInterfaceType, proxyActivatorType);
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

                    if (TypeUtility.IsTaskType(methodDescription.ReturnType))
                    {
                        this.AddAsyncMethodImplementation(
                            classBuilder,
                            interfaceDescription.Id,
                            methodDescription,
                            methodBodyTypes,
                            interfaceDescription.InterfaceType.FullName);
                    }
                    else if (TypeUtility.IsVoidType(methodDescription.ReturnType))
                    {
                        this.AddVoidMethodImplementation(
                            classBuilder,
                            interfaceDescription.Id,
                            methodDescription,
                            methodBodyTypes,
                            interfaceDescription.InterfaceType.FullName);
                    }
                }
            }
        }

        protected abstract TProxyGenerator CreateProxyGenerator(
            Type proxyInterfaceType,
            Type proxyActivatorType);

        private void AddAsyncMethodImplementation(
        TypeBuilder classBuilder,
        int interfaceId,
        MethodDescription methodDescription,
        MethodBodyTypes methodBodyTypes,
        string interfaceName)
        {
            var interfaceMethod = methodDescription.MethodInfo;
            var parameters = interfaceMethod.GetParameters();
            var methodBuilder = CodeBuilderUtils.CreateExplitInterfaceMethodBuilder(
                classBuilder,
                interfaceMethod);
            var ilGen = methodBuilder.GetILGenerator();
            var parameterLength = parameters.Length;
            if (methodDescription.HasCancellationToken)
            {
                // Cancellation token is tracked locally and should not be serialized and sent
                // as a part of the request body.
                parameterLength = parameterLength - 1;
            }

            LocalBuilder requestMessage = null;
            if (parameterLength > 0)
            {
                // Create Wrapped Message
                // create requestBody and assign the values to its field from the arguments
                var wrappedRequestBody = CreateWrappedRequestBody(methodDescription, methodBodyTypes, ilGen, parameters);

                // create IServiceRemotingRequestMessageBody message
                requestMessage = this.CreateRequestRemotingMessageBody(methodDescription, interfaceName, ilGen, parameterLength, wrappedRequestBody);

                // Check if requestMessage is not implementing WrappedMessage , then call SetParam
                this.SetParameterIfNeeded(ilGen, requestMessage, parameterLength, parameters);
            }

            var objectTask = ilGen.DeclareLocal(typeof(Task<IServiceRemotingResponseMessageBody>));

            // call the base InvokeAsync method
            ilGen.Emit(OpCodes.Ldarg_0); // base
            ilGen.Emit(OpCodes.Ldc_I4, interfaceId); // interfaceId
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId
            ilGen.Emit(OpCodes.Ldstr, methodDescription.Name); // method name

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
                ilGen.Emit(OpCodes.Ldc_I4, interfaceId); // interfaceId
                ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id); // methodId
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

        private void SetParameterIfNeeded(
            ILGenerator ilGen,
            LocalBuilder requestMessage,
            int parameterLength,
            ParameterInfo[] parameters)
        {
            var boolres = ilGen.DeclareLocal(typeof(bool));
            var boolres2 = ilGen.DeclareLocal(typeof(bool));
            ilGen.Emit(OpCodes.Ldarg_0); // base
            ilGen.Emit(OpCodes.Ldloc_1, requestMessage);
            ilGen.Emit(OpCodes.Call, this.checkIfitsWrapped);
            ilGen.Emit(OpCodes.Stloc, boolres);
            ilGen.Emit(OpCodes.Ldloc_2);
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Ceq);
            ilGen.Emit(OpCodes.Stloc, boolres2);
            ilGen.Emit(OpCodes.Ldloc_3, boolres2);
            var elseLabel = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brfalse, elseLabel);

            // if false ,Call SetParamater
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

            ilGen.MarkLabel(elseLabel);
        }

        private LocalBuilder CreateRequestRemotingMessageBody(
            MethodDescription methodDescription,
            string interfaceName,
            ILGenerator ilGen,
            int parameterLength,
            LocalBuilder wrappedRequestBody)
        {
            LocalBuilder requestMessage;
            ilGen.Emit(OpCodes.Ldarg_0); // base
            requestMessage = ilGen.DeclareLocal(typeof(IServiceRemotingRequestMessageBody));
            ilGen.Emit(OpCodes.Ldstr, interfaceName);
            ilGen.Emit(OpCodes.Ldstr, methodDescription.Name);
            ilGen.Emit(OpCodes.Ldc_I4, parameterLength);
            ilGen.Emit(OpCodes.Ldloc, wrappedRequestBody);
            ilGen.EmitCall(OpCodes.Call, this.createMessage, null);
            ilGen.Emit(OpCodes.Stloc, requestMessage);
            return requestMessage;
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
            ilGen.Emit(OpCodes.Bne_Un, elseLabel);

            // if (methodId == <methodId>)
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldc_I4, methodId);
            ilGen.Emit(OpCodes.Bne_Un, elseLabel);

            var castedResponseBody = ilGen.DeclareLocal(responseBodyType);
            ilGen.Emit(OpCodes.Ldarg_3); // load responseBody object
            ilGen.Emit(OpCodes.Castclass, responseBodyType); // cast it to responseBodyType
            ilGen.Emit(OpCodes.Stloc, castedResponseBody); // store casted result to castedResponseBody local variable
            ilGen.Emit(OpCodes.Ldloc, castedResponseBody);
            ilGen.Emit(OpCodes.Ret);
        }

        private void AddVoidMethodImplementation(
            TypeBuilder classBuilder,
            int interfaceDescriptionId,
            MethodDescription methodDescription,
            MethodBodyTypes methodBodyTypes,
            string interfaceName)
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
                interfaceDescriptionId,
                methodDescription,
                wrappedRequestBody,
                interfaceName);

            ilGen.Emit(OpCodes.Ret);
        }
    }
}
