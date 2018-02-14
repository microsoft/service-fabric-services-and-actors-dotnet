// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    internal class BuildResult
    {
        private readonly CodeBuilderContext buildContext;

        protected BuildResult(CodeBuilderContext buildContext)
        {
            this.buildContext = buildContext;
        }

        public CodeBuilderContext BuildContext
        {
            get
            {
                return this.buildContext;
            }
        }
    }
}
