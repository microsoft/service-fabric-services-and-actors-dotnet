// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Runtime
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    internal class RuntimeContext : IDisposable
    {
        private static readonly object SharedContextLock = new object();
        private static RuntimeContext sharedContext;

        public FabricRuntime Runtime { get; private set; }

        public ICodePackageActivationContext CodePackageContext { get; private set; }

        public NodeContext NodeContext { get; private set; }

        public static async Task<RuntimeContext> GetOrCreateAsync(
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            // check if the context exist using double lock pattern if so return it.
            // shared context is never set to null, so the following check is safe
            if (sharedContext != null)
            {
                return sharedContext;
            }

            lock (SharedContextLock)
            {
                if (sharedContext != null)
                {
                    return sharedContext;
                }
            }

            // shared context does not exist, create it
            ICodePackageActivationContext codePackageContext = null;
            NodeContext nodeContext;
            FabricRuntime fabricRuntime = null;
            try
            {
                nodeContext = await FabricRuntime.GetNodeContextAsync(timeout, cancellationToken).ConfigureAwait(false);
                codePackageContext = await FabricRuntime.GetActivationContextAsync(timeout, cancellationToken).ConfigureAwait(false);
                fabricRuntime = await FabricRuntime.CreateAsync(timeout, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                if (fabricRuntime != null)
                {
                    fabricRuntime.Dispose();
                }

                if (codePackageContext != null)
                {
                    codePackageContext.Dispose();
                }

                throw;
            }

            // set the shared context
            lock (SharedContextLock)
            {
                if (sharedContext == null)
                {
                    sharedContext = new RuntimeContext()
                    {
                        Runtime = fabricRuntime,
                        CodePackageContext = codePackageContext,
                        NodeContext = nodeContext,
                    };
                }
            }

            // dispose the newly created runtime and context if they do not become the shared
            if (!object.ReferenceEquals(sharedContext.Runtime, fabricRuntime))
            {
                fabricRuntime.Dispose();
            }

            if (!object.ReferenceEquals(sharedContext.CodePackageContext, codePackageContext))
            {
                codePackageContext.Dispose();
            }

            return sharedContext;
        }

        public void Dispose()
        {
            this.Runtime?.Dispose();
            this.CodePackageContext?.Dispose();
        }
    }
}
