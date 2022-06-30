// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;

    internal class RCTxExecutor
    {
        private static readonly string TraceType = typeof(RCTxExecutor).Name;
        private IEnumerable<IExceptionHandler> exceptionHandlers;
        private string traceId;

        public RCTxExecutor(string traceId)
            : this (
                new List<IExceptionHandler>
                {
                    new RCTxExceptionHandler(),
                }, traceId)
        {
        }

        public RCTxExecutor(IEnumerable<IExceptionHandler> exceptionHandlers, string traceId)
        {
            this.exceptionHandlers = exceptionHandlers;
            this.traceId = traceId;
        }

        public async Task ExecuteWithRetriesAsync(
            Func<ITransaction, Task> asyncFunc,
            Func<ITransaction> txFactory,
            string funcTag)
        {
            Func<ITransaction, Task<object>> asyncFunc2 =
                tx =>
                {
                    asyncFunc.Invoke(tx);
                    return null;
                };

            await this.ExecuteWithRetriesAsync(asyncFunc2, txFactory, funcTag, MigrationConstants.MaxBackoffForTransientErrors, MigrationConstants.MaxRetryCountForTransientErrors);
        }

        public async Task<T> ExecuteWithRetriesAsync<T>(
            Func<ITransaction, Task<T>> asyncFunc,
            Func<ITransaction> txFactory,
            string funcTag)
        {
            return await this.ExecuteWithRetriesAsync(asyncFunc, txFactory, funcTag, MigrationConstants.MaxBackoffForTransientErrors, MigrationConstants.MaxRetryCountForTransientErrors);
        }

        public async Task ExecuteWithRetriesAsync(
           Func<ITransaction, Task> asyncFunc,
           Func<ITransaction> txFactory,
           string funcTag,
           TimeSpan backoffInterval,
           int retries)
        {
            Func<ITransaction, Task<object>> asyncFunc2 =
                tx =>
                {
                    asyncFunc.Invoke(tx);
                    return null;
                };

            await this.ExecuteWithRetriesAsync(asyncFunc2, txFactory, funcTag, backoffInterval, retries);
        }

        public async Task<T> ExecuteWithRetriesAsync<T>(
            Func<ITransaction, Task<T>> asyncFunc,
            Func<ITransaction> txFactory,
            string funcTag,
            TimeSpan backoffInterval,
            int retries)
        {
            using (var tx = txFactory.Invoke())
            {
                try
                {
                    ActorTrace.Source.WriteNoiseWithId(
                        TraceType,
                        this.traceId,
                        $"Invoking migration func - {funcTag}");
                    return await asyncFunc.Invoke(tx);
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
                                if (retries > 0 && handler.TryHandleException(ex, out var isTransient))
                                {
                                    if (isTransient)
                                    {
                                        ActorTrace.Source.WriteWarningWithId(
                                            TraceType,
                                            this.traceId,
                                            $"Transient exception occured while invoking migration func - {funcTag}, retriesLeft : {retries}, ex.Type : {ex.GetType().Name}, ex.Message : {ex.Message}");

                                        await Task.Delay(backoffInterval);
                                        return await this.ExecuteWithRetriesAsync(asyncFunc, txFactory, funcTag, backoffInterval, --retries);
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
                                    return await this.ExecuteWithRetriesAsync(asyncFunc, txFactory, funcTag, backoffInterval, --retries);
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
