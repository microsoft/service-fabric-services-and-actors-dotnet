// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System.Reflection.Emit;

    class CodeBuilderContext
    {
        private readonly string assemblyNamespace;
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;
        private readonly bool enableDebugging;

        public CodeBuilderContext(string assemblyName, string assemblyNamespace, bool enableDebugging = false)
        {
            this.assemblyNamespace = assemblyNamespace;
            this.enableDebugging = enableDebugging;

            this.assemblyBuilder = CodeBuilderUtils.CreateAssemblyBuilder(assemblyName, this.enableDebugging);
            this.moduleBuilder = CodeBuilderUtils.CreateModuleBuilder(this.assemblyBuilder, assemblyName, this.enableDebugging);
        }

        public AssemblyBuilder AssemblyBuilder
        {
            get { return this.assemblyBuilder; }
        }

        public ModuleBuilder ModuleBuilder
        {
            get { return this.moduleBuilder; }
        }

        public string AssemblyNamespace
        {
            get
            {
                return this.assemblyNamespace;
            }
        }

        public void Complete()
        {
            if (this.enableDebugging)
            {
                this.assemblyBuilder.Save(this.assemblyBuilder.GetName().Name + ".dll");
            }
        }
    }
}