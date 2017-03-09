// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    internal abstract class CodeBuilderModule
    {
        private readonly ICodeBuilder codeBuilder;

        protected CodeBuilderModule(ICodeBuilder codeBuilder)
        {
            this.codeBuilder = codeBuilder;
        }

        protected ICodeBuilder CodeBuilder
        {
            get { return this.codeBuilder; }
        }
    }
}