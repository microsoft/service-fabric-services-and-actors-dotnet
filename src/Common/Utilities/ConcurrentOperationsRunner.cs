// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.Common.Utilities
{
    sealed class ConcurrentOperationsRunner<T>
    {
        readonly Func<T, Task> runOperation;
        readonly TryGetNextOperationParameters tryGetNextOperationParameters;
        readonly int concurrencyCount;

        internal ConcurrentOperationsRunner(Func<T, Task> runOperation, TryGetNextOperationParameters tryGetNextOperationParameters, int concurrencyCount)
        {
            this.runOperation = runOperation;
            this.tryGetNextOperationParameters = tryGetNextOperationParameters;
            this.concurrencyCount = concurrencyCount;
        }

        internal delegate bool TryGetNextOperationParameters(out T parameters);

        internal async Task RunAll()
        {
            var concurrentOperations = new Task[concurrencyCount];
            for (var i = 0; i < concurrencyCount; i++)
                concurrentOperations[i] = RunOperationsSerially();

            await Task.WhenAll(concurrentOperations);
        }

        async Task RunOperationsSerially()
        {
            while (tryGetNextOperationParameters(out T parameters))
                await runOperation(parameters);
        }
    }
}
