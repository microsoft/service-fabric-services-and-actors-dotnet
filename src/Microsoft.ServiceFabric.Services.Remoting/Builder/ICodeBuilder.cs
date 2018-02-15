// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;

    internal interface ICodeBuilder
    {
        ICodeBuilderNames Names { get; }

        MethodDispatcherBuildResult GetOrBuilderMethodDispatcher(Type interfaceType);

        MethodBodyTypesBuildResult GetOrBuildMethodBodyTypes(Type interfaceType);

        ProxyGeneratorBuildResult GetOrBuildProxyGenerator(Type interfaceType);
    }
}
