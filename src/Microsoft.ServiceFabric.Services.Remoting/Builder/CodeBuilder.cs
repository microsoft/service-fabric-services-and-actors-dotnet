// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;
    using System.Collections.Generic;

    internal abstract class CodeBuilder : ICodeBuilder
    {
        private readonly Dictionary<Type, MethodBodyTypesBuildResult> methodBodyTypesBuildResultMap;
        private readonly Dictionary<Type, MethodDispatcherBuildResult> methodDispatcherBuildResultMap;
        private readonly Dictionary<Type, ProxyGeneratorBuildResult> proxyGeneratorBuildResultMap;

        private readonly ICodeBuilderNames codeBuilderNames;

        protected CodeBuilder(ICodeBuilderNames codeBuilderNames)
        {
            this.codeBuilderNames = codeBuilderNames;

            this.methodBodyTypesBuildResultMap = new Dictionary<Type, MethodBodyTypesBuildResult>();
            this.methodDispatcherBuildResultMap = new Dictionary<Type, MethodDispatcherBuildResult>();
            this.proxyGeneratorBuildResultMap = new Dictionary<Type, ProxyGeneratorBuildResult>();
        }

        ICodeBuilderNames ICodeBuilder.Names
        {
            get { return this.codeBuilderNames; }
        }

        MethodDispatcherBuildResult ICodeBuilder.GetOrBuilderMethodDispatcher(Type interfaceType)
        {
            MethodDispatcherBuildResult result;
            if (this.TryGetMethodDispatcher(interfaceType, out result)) return result;

            result = this.BuildMethodDispatcher(interfaceType);
            this.UpdateMethodDispatcherBuildMap(interfaceType, result);

            return result;
        }

        protected void UpdateMethodDispatcherBuildMap(Type interfaceType, MethodDispatcherBuildResult result)
        {
            this.methodDispatcherBuildResultMap.Add(interfaceType, result);
        }

        protected bool TryGetMethodDispatcher(Type interfaceType,
            out MethodDispatcherBuildResult builderMethodDispatcher)
        {
            MethodDispatcherBuildResult result;
            if (this.methodDispatcherBuildResultMap.TryGetValue(interfaceType, out result))
            {
                {
                    builderMethodDispatcher = result;
                    return true;
                }
            }
            builderMethodDispatcher = null;
            return false;
        }

        MethodBodyTypesBuildResult ICodeBuilder.GetOrBuildMethodBodyTypes(Type interfaceType)
        {
            MethodBodyTypesBuildResult result;
            if (this.methodBodyTypesBuildResultMap.TryGetValue(interfaceType, out result))
            {
                return result;
            }

            result = this.BuildMethodBodyTypes(interfaceType);
            this.methodBodyTypesBuildResultMap.Add(interfaceType, result);

            return result;
        }

        ProxyGeneratorBuildResult ICodeBuilder.GetOrBuildProxyGenerator(Type interfaceType)
        {
            ProxyGeneratorBuildResult result;
            if (this.TryGetProxyGenerator(interfaceType, out result)) return result;

            result = this.BuildProxyGenerator(interfaceType);
            this.UpdateProxyGeneratorMap(interfaceType, result);

            return result;
        }

        protected void UpdateProxyGeneratorMap(Type interfaceType, ProxyGeneratorBuildResult result)
        {
            this.proxyGeneratorBuildResultMap.Add(interfaceType, result);
        }

        protected bool TryGetProxyGenerator(Type interfaceType, out ProxyGeneratorBuildResult orBuildProxyGenerator)
        {
            ProxyGeneratorBuildResult result;
            if (this.proxyGeneratorBuildResultMap.TryGetValue(interfaceType, out result))
            {
                {
                    orBuildProxyGenerator = result;
                    return true;
                }
            }
            orBuildProxyGenerator = null;
            return false;
        }

        protected abstract MethodDispatcherBuildResult BuildMethodDispatcher(Type interfaceType);

        protected abstract MethodBodyTypesBuildResult BuildMethodBodyTypes(Type interfaceType);

        protected abstract ProxyGeneratorBuildResult BuildProxyGenerator(Type interfaceType);
    }
}