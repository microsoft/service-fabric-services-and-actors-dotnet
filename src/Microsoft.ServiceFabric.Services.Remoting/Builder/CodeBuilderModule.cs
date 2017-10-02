// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

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

        protected static IReadOnlyDictionary<int, string> GetMethodNameMap(InterfaceDescription interfaceDescription)
        {
            var methodNameMap = interfaceDescription.Methods.ToDictionary(
                methodDescription => methodDescription.Id,
                methodDescription => methodDescription.Name);

            return new ReadOnlyDictionary<int, string>(methodNameMap);
        }
    }
}