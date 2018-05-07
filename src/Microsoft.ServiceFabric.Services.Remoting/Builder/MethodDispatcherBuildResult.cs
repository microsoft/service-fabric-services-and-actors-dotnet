// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;

    internal class MethodDispatcherBuildResult : BuildResult
    {
        public MethodDispatcherBuildResult(CodeBuilderContext buildContext)
            : base(buildContext)
        {
        }

        public Type MethodDispatcherType { get; set; }

        public MethodDispatcherBase MethodDispatcher { get; set; }
    }
}
