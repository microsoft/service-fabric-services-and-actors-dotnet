// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;

    internal class RCTxExecutor
    {
        private static readonly string TraceType = typeof(RCTxExecutor).Name;
        private IEnumerable<IExceptionHandler> exceptionHandlers;
        private Func<ITransaction> txFactory;
        private string traceId;

        public RCTxExecutor(Func<ITransaction> txFactory, string traceId)
            : this (
                new List<IExceptionHandler>
                {
                    new RCTxExceptionHandler(),
                },
                txFactory,
                traceId)
        {
        }

        public RCTxExecutor(IEnumerable<IExceptionHandler> exceptionHandlers, Func<ITransaction> txFactory, string traceId)
        {
            this.exceptionHandlers = exceptionHandlers;
            this.traceId = traceId;
            this.txFactory = txFactory;
        }

        public async Task ExecuteWithRetriesAsync(
            Func<ITransaction, CancellationToken, Task> asyncFunc,
            string funcTag,
            CancellationToken cancellationToken)
        {
            Func<ITransaction, CancellationToken, Task<object>> asyncFunc2 =
                (tx, token) =>
                {
                    asyncFunc.Invoke(tx, token);
                    return null;
                };

            await this.ExecuteWithRetriesAsync(asyncFunc2, funcTag, MigrationConstants.MaxBackoffForTransientErrors, MigrationConstants.MaxRetryCountForTransientErrors, cancellationToken);
        }

        public async Task<T> ExecuteWithRetriesAsync<T>(
            Func<ITransaction, CancellationToken, Task<T>> asyncFunc,
            string funcTag,
            CancellationToken cancellationToken)
        {
            return await this.ExecuteWithRetriesAsync(asyncFunc, funcTag, MigrationConstants.MaxBackoffForTransientErrors, MigrationConstants.MaxRetryCountForTransientErrors, cancellationToken);
        }

        public async Task ExecuteWithRetriesAsync(
           Func<ITransaction, CancellationToken, Task> asyncFunc,
           string funcTag,
           TimeSpan backoffInterval,
           int retries,
           CancellationToken cancellationToken)
        {
            Func<ITransaction, CancellationToken, Task<object>> asyncFunc2 =
                (tx, token) =>
                {
                    asyncFunc.Invoke(tx, token);
                    return null;
                };

            await this.ExecuteWithRetriesAsync(asyncFunc2, funcTag, backoffInterval, retries, cancellationToken);
        }

        public async Task<T> ExecuteWithRetriesAsync<T>(
            Func<ITransaction, CancellationToken, Task<T>> asyncFunc,
            string funcTag,
            TimeSpan backoffInterval,
            int retries,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var tx = this.txFactory.Invoke())
            {
                try
                {
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        this.traceId,
                        $"Invoking migration func - {funcTag}");
                    return await asyncFunc.Invoke(tx, cancellationToken);
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException)
                    {
                        var aggEx = ex as AggregateException;
                        foreach (var inner in aggEx.Flatten().InnerExceptions)
                        {
                            foreach (var handler in this.exceptionHandlers)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                if (retries > 0 && handler.TryHandleException(ex, out var isTransient))
                                {
                                    if (isTransient)
                                    {
                                        ActorTrace.Source.WriteWarningWithId(
                                            TraceType,
                                            this.traceId,
                                            $"Transient exception occured while invoking migration func - {funcTag}, retriesLeft : {retries}, ex.Type : {ex.GetType().Name}, ex.Message : {ex.Message}");

                                        await Task.Delay(backoffInterval);
                                        return await this.ExecuteWithRetriesAsync(asyncFunc, funcTag, backoffInterval, --retries, cancellationToken);
                                    }
                                }
                            }
                        }

                        ActorTrace.Source.WriteWarningWithId(
                            TraceType,
                            this.traceId,
                            $"Exception occured while invoking migration func - {funcTag}, ex.Type : {ex.GetType().Name}, ex.Message : {ex.Message}");

                        throw ex;
                    }
                    else
                    {
                        foreach (var handler in this.exceptionHandlers)
                        {
                            if (retries > 0 && handler.TryHandleException(ex, out var isTransient))
                            {
                                if (isTransient)
                                {
                                    ActorTrace.Source.WriteWarningWithId(
                                           TraceType,
                                           this.traceId,
                                           $"Transient exception occured while invoking migration func - {funcTag}, retriesLeft : {retries}, ex.Type : {ex.GetType().Name}, ex.Message : {ex.Message}");

                                    await Task.Delay(backoffInterval);
                                    return await this.ExecuteWithRetriesAsync(asyncFunc, funcTag, backoffInterval, --retries, cancellationToken);
                                }
                            }
                        }

                        ActorTrace.Source.WriteWarningWithId(
                            TraceType,
                            this.traceId,
                            $"Exception occured while invoking migration func - {funcTag}, ex.Type : {ex.GetType().Name}, ex.Message : {ex.Message}");

                        throw ex;
                    }
                }
                finally
                {
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        this.traceId,
                        $"Completed migration func - {funcTag}");
                }
            }
        }
    }
}
