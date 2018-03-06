// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V1.Builder
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    internal class MethodDispatcherBuilder<TMethodDispatcher> : CodeBuilderModule
        where TMethodDispatcher : MethodDispatcherBaseWithSerializer
    {
        private readonly Type methodDispatcherBaseType;
        private readonly MethodInfo continueWithResultMethodInfo;
        private readonly MethodInfo continueWithMethodInfo;

        public MethodDispatcherBuilder(ICodeBuilder codeBuilder)
            : base(codeBuilder)
        {
            this.methodDispatcherBaseType = typeof(TMethodDispatcher);

            this.continueWithResultMethodInfo = this.methodDispatcherBaseType.GetMethod(
                "ContinueWithResult",
                BindingFlags.Instance | BindingFlags.NonPublic);

            this.continueWithMethodInfo = this.methodDispatcherBaseType.GetMethod(
                "ContinueWith",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public MethodDispatcherBuildResult Build(InterfaceDescription interfaceDescription)
        {
            var context = new CodeBuilderContext(
                assemblyName: this.CodeBuilder.Names.GetMethodDispatcherAssemblyName(interfaceDescription
                    .InterfaceType),
                assemblyNamespace: this.CodeBuilder.Names.GetMethodDispatcherAssemblyNamespace(interfaceDescription
                    .InterfaceType),
                enableDebugging: CodeBuilderAttribute.IsDebuggingEnabled(interfaceDescription.InterfaceType));

            var result = new MethodDispatcherBuildResult(context);

            // ensure that the method body types are built
            var methodBodyTypesBuildResult =
                this.CodeBuilder.GetOrBuildMethodBodyTypes(interfaceDescription.InterfaceType);

            // build dispatcher class
            var classBuilder = CodeBuilderUtils.CreateClassBuilder(
                context.ModuleBuilder,
                ns: context.AssemblyNamespace,
                className: this.CodeBuilder.Names.GetMethodDispatcherClassName(interfaceDescription.InterfaceType),
                baseType: this.methodDispatcherBaseType);

            this.AddCreateResponseBodyMethod(classBuilder, interfaceDescription, methodBodyTypesBuildResult);
            this.AddOnDispatchAsyncMethod(classBuilder, interfaceDescription, methodBodyTypesBuildResult);
            this.AddOnDispatchMethod(classBuilder, interfaceDescription, methodBodyTypesBuildResult);

            var methodNameMap = GetMethodNameMap(interfaceDescription);

            // create the dispatcher type, instantiate and initialize it
            result.MethodDispatcherType = classBuilder.CreateTypeInfo().AsType();
            result.MethodDispatcher = (TMethodDispatcher)Activator.CreateInstance(result.MethodDispatcherType);
            var v1MethodDispatcherBase = (MethodDispatcherBaseWithSerializer)result.MethodDispatcher;
            v1MethodDispatcherBase.Initialize(
                interfaceDescription,
                methodNameMap,
                methodBodyTypesBuildResult.GetRequestBodyTypes(),
                methodBodyTypesBuildResult.GetResponseBodyTypes());

            context.Complete();
            return result;
        }

        private void AddCreateResponseBodyMethod(
            TypeBuilder classBuilder,
            InterfaceDescription interfaceDescription,
            MethodBodyTypesBuildResult methodBodyTypesBuildResult)
        {
            var methodBuilder = CodeBuilderUtils.CreateProtectedMethodBuilder(
                classBuilder,
                "CreateResponseBody",
                typeof(object), // responseBody - return value
                typeof(int), // methodId
                typeof(object)); // retval from the invoked method on the remoted object

            var ilGen = methodBuilder.GetILGenerator();

            foreach (var methodDescription in interfaceDescription.Methods)
            {
                var methodBodyTypes = methodBodyTypesBuildResult.MethodBodyTypesMap[methodDescription.Name];
                if (methodBodyTypes.ResponseBodyType == null)
                {
                    continue;
                }

                var elseLabel = ilGen.DefineLabel();

                this.AddIfMethodIdCreateResponseBlock(
                    ilGen,
                    elseLabel,
                    methodDescription.Id,
                    methodBodyTypes.ResponseBodyType);

                ilGen.MarkLabel(elseLabel);
            }

            // return null; (if method id's do not match)
            ilGen.Emit(OpCodes.Ldnull);
            ilGen.Emit(OpCodes.Ret);
        }

        private void AddIfMethodIdCreateResponseBlock(
            ILGenerator ilGen,
            Label elseLabel,
            int methodId,
            Type responseType)
        {
            // if (methodId == <methodid>)
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldc_I4, methodId);
            ilGen.Emit(OpCodes.Bne_Un_S, elseLabel);

            var ctorInfo = responseType.GetConstructor(Type.EmptyTypes);
            if (ctorInfo != null)
            {
                var localBuilder = ilGen.DeclareLocal(responseType);

                // new <ResponseBodyType>
                ilGen.Emit(OpCodes.Newobj, ctorInfo);
                ilGen.Emit(OpCodes.Stloc, localBuilder);
                ilGen.Emit(OpCodes.Ldloc, localBuilder);

                // responseBody.retval = (<retvaltype>)retval;
                var fInfo = responseType.GetField(this.CodeBuilder.Names.RetVal);
                ilGen.Emit(OpCodes.Ldarg_2);
                ilGen.Emit(
                    fInfo.FieldType.GetTypeInfo().IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any,
                    fInfo.FieldType);
                ilGen.Emit(OpCodes.Stfld, fInfo);
                ilGen.Emit(OpCodes.Ldloc, localBuilder);
                ilGen.Emit(OpCodes.Ret);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
                ilGen.Emit(OpCodes.Ret);
            }
        }

        private void AddOnDispatchAsyncMethod(
            TypeBuilder classBuilder,
            InterfaceDescription interfaceDescription,
            MethodBodyTypesBuildResult methodBodyTypesBuildResult)
        {
            var dispatchMethodImpl = CodeBuilderUtils.CreateProtectedMethodBuilder(
                classBuilder,
                "OnDispatchAsync",
                typeof(Task<object>),
                typeof(int), // methodid
                typeof(object), // remoted object
                typeof(object), // requestBody
                typeof(CancellationToken)); // CancellationToken

            var ilGen = dispatchMethodImpl.GetILGenerator();

            var castedObject = ilGen.DeclareLocal(interfaceDescription.InterfaceType);
            ilGen.Emit(OpCodes.Ldarg_2); // load remoted object
            ilGen.Emit(OpCodes.Castclass, interfaceDescription.InterfaceType);
            ilGen.Emit(OpCodes.Stloc, castedObject); // store casted result to local 0

            foreach (var methodDescription in interfaceDescription.Methods)
            {
                if (!TypeUtility.IsTaskType(methodDescription.ReturnType))
                {
                    continue;
                }

                var elseLable = ilGen.DefineLabel();

                this.AddIfMethodIdInvokeAsyncBlock(
                    ilGen: ilGen,
                    elseLabel: elseLable,
                    castedObject: castedObject,
                    methodDescription: methodDescription,
                    methodBodyTypes: methodBodyTypesBuildResult.MethodBodyTypesMap[methodDescription.Name]);

                ilGen.MarkLabel(elseLable);
            }

            ilGen.ThrowException(typeof(MissingMethodException));
        }

        private void AddIfMethodIdInvokeAsyncBlock(
            ILGenerator ilGen,
            Label elseLabel,
            LocalBuilder castedObject,
            MethodDescription methodDescription,
            MethodBodyTypes methodBodyTypes)
        {
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id);
            ilGen.Emit(OpCodes.Bne_Un_S, elseLabel);

            var invokeTask = ilGen.DeclareLocal(methodDescription.ReturnType);

            LocalBuilder castedRequestBody = null;
            if (methodBodyTypes.RequestBodyType != null)
            {
                // cast the request body
                // var castedRequestBody = (<RequestBodyType>)requestBody;
                castedRequestBody = ilGen.DeclareLocal(methodBodyTypes.RequestBodyType);
                ilGen.Emit(OpCodes.Ldarg_3);
                ilGen.Emit(OpCodes.Castclass, methodBodyTypes.RequestBodyType);
                ilGen.Emit(OpCodes.Stloc, castedRequestBody);
            }

            // now invoke the method on the casted object
            ilGen.Emit(OpCodes.Ldloc, castedObject);

            if (methodBodyTypes.RequestBodyType != null)
            {
                foreach (var field in methodBodyTypes.RequestBodyType.GetFields())
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    // castedRequestBody is set to non-null in the previous if check on the same condition
                    ilGen.Emit(OpCodes.Ldloc, castedRequestBody);
                    ilGen.Emit(OpCodes.Ldfld, field);
                }
            }

            if (methodDescription.HasCancellationToken)
            {
                ilGen.Emit(OpCodes.Ldarg, 4);
            }

            ilGen.EmitCall(OpCodes.Callvirt, methodDescription.MethodInfo, null);
            ilGen.Emit(OpCodes.Stloc, invokeTask);

            // call the base method to return continuation task
            if (methodBodyTypes.ResponseBodyType != null)
            {
                // the return is Task<T>
                var continueWithGenericMethodInfo = this.continueWithResultMethodInfo.MakeGenericMethod(
                    methodDescription.ReturnType.GenericTypeArguments[0]);

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id);
                ilGen.Emit(OpCodes.Ldloc, invokeTask);
                ilGen.EmitCall(OpCodes.Call, continueWithGenericMethodInfo, null);
                ilGen.Emit(OpCodes.Ret);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldloc, invokeTask);
                ilGen.EmitCall(OpCodes.Call, this.continueWithMethodInfo, null);
                ilGen.Emit(OpCodes.Ret);
            }
        }

        private void AddOnDispatchMethod(
            TypeBuilder classBuilder,
            InterfaceDescription interfaceDescription,
            MethodBodyTypesBuildResult methodBodyTypesBuildResult)
        {
            var dispatchMethodImpl = CodeBuilderUtils.CreateProtectedMethodBuilder(
                classBuilder,
                "OnDispatch",
                typeof(void),
                typeof(int), // method id
                typeof(object), // remote object
                typeof(object)); // message body

            var ilGen = dispatchMethodImpl.GetILGenerator();

            var castedObject = ilGen.DeclareLocal(interfaceDescription.InterfaceType);
            ilGen.Emit(OpCodes.Ldarg_2); // load object
            ilGen.Emit(OpCodes.Castclass, interfaceDescription.InterfaceType);
            ilGen.Emit(OpCodes.Stloc, castedObject); // store casted result to local 0

            foreach (var methodDescription in interfaceDescription.Methods)
            {
                if (!TypeUtility.IsVoidType(methodDescription.ReturnType))
                {
                    continue;
                }

                var elseLable = ilGen.DefineLabel();

                this.AddIfMethodIdInvokeBlock(
                    ilGen: ilGen,
                    elseLabel: elseLable,
                    castedObject: castedObject,
                    methodDescription: methodDescription,
                    requestBodyType: methodBodyTypesBuildResult.MethodBodyTypesMap[methodDescription.Name].RequestBodyType);

                ilGen.MarkLabel(elseLable);
            }

            ilGen.ThrowException(typeof(MissingMethodException));
        }

        private void AddIfMethodIdInvokeBlock(
            ILGenerator ilGen,
            Label elseLabel,
            LocalBuilder castedObject,
            MethodDescription methodDescription,
            Type requestBodyType)
        {
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldc_I4, methodDescription.Id);
            ilGen.Emit(OpCodes.Bne_Un_S, elseLabel);

            LocalBuilder castedRequestBody = null;
            if (requestBodyType != null)
            {
                // cast the request body
                // var castedRequestBody = (<RequestBodyType>)requestBody;
                castedRequestBody = ilGen.DeclareLocal(requestBodyType);
                ilGen.Emit(OpCodes.Ldarg_3);
                ilGen.Emit(OpCodes.Castclass, requestBodyType);
                ilGen.Emit(OpCodes.Stloc, castedRequestBody);
            }

            // now invoke the method on the casted subscriber
            ilGen.Emit(OpCodes.Ldloc, castedObject);

            if (requestBodyType != null)
            {
                foreach (var field in requestBodyType.GetFields())
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    // castedEventBody is set to non-null in the previous if check on the same condition
                    ilGen.Emit(OpCodes.Ldloc, castedRequestBody);
                    ilGen.Emit(OpCodes.Ldfld, field);
                }
            }

            ilGen.EmitCall(OpCodes.Callvirt, methodDescription.MethodInfo, null);
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
