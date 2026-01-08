// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Utilities
{
    using System;
    using System.Threading.Tasks;

    internal class ConcurrentOperationsRunner<T>
    {
        private readonly Func<T, Task> runOperation;
        private readonly TryGetNextOperationParameters tryGetNextOperationParameters;
        private readonly int concurrencyCount;

        public ConcurrentOperationsRunner(
            Func<T, Task> runOperation,
            TryGetNextOperationParameters tryGetNextOperationParameters,
            int concurrencyCount)
        {
            this.runOperation = runOperation;
            this.tryGetNextOperationParameters = tryGetNextOperationParameters;
            this.concurrencyCount = concurrencyCount;
        }

        public delegate bool TryGetNextOperationParameters(out T parameters);

        public async Task RunAll()
        {
            var concurrentOperations = new Task[this.concurrencyCount];
            for (var i = 0; i < this.concurrencyCount; i++)
            {
                concurrentOperations[i] = this.RunOperationsSerially();
            }

            await Task.WhenAll(concurrentOperations);
        }

        private async Task RunOperationsSerially()
        {
            while (this.tryGetNextOperationParameters(out var parameters))
            {
                await this.runOperation(parameters);
            }
        }
    }
}
