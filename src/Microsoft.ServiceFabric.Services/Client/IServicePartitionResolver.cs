// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Client
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// <para>
    /// Defines the interface for the Service partition resolver.
    /// Service resolution is the process of looking up the set of endpoints for the replicas in a partition. A service partition resolver
    /// implements the logic for service resolution.
    /// </para>
    /// </summary>
    public interface IServicePartitionResolver
    {
        /// <summary>
        /// Resolves a partition of the specified service with specified back-off/retry settings on retry-able errors.
        /// </summary>
        /// <param name="serviceUri">Name of the service instance to resolve.</param>
        /// <param name="partitionKey">
        /// <para>
        /// <see cref="ServicePartitionKey">Key</see> that determines the target partition of the service instance. The <see cref="ServicePartitionKind">partitioning scheme</see>
        /// specified in the key should match the partitioning scheme used to create the service instance.
        /// </para>
        /// </param>
        /// <param name="resolveTimeoutPerTry">The timeout per resolve try.</param>
        /// <param name="maxRetryBackoffInterval">
        /// The interval to back-off before retrying the resolution after a failure due to retry-able exception.
        /// </param>
        /// <param name="cancellationToken">
        /// <para>
        /// The CancellationToken that this operation is observing. It is used to notify the operation that it should be canceled.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents the outstanding service resolution operation. The result from
        /// the Task is the <see cref="System.Fabric.ResolvedServicePartition" /> object, that contains the information
        /// about the resolved service partition including the service endpoints.
        /// </returns>
        Task<ResolvedServicePartition> ResolveAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TimeSpan resolveTimeoutPerTry,
            TimeSpan maxRetryBackoffInterval,
            CancellationToken cancellationToken);

        /// <summary>
        /// <para>
        /// Re-resolves a previously resolved partition of the specified service with specified back-off/retry settings 
        /// on retry-able errors. This method overload is used in cases where the client knows that the resolved service partition that it has is no longer valid.
        /// </para>
        /// </summary>
        /// <param name="previousRsp">The resolved service partition that the client got from the earlier invocation of the ResolveAsync() method.</param>
        /// <param name="resolveTimeoutPerTry">The timeout per resolve try.</param>
        /// <param name="maxRetryBackoffInterval">
        /// The interval to back-off before retrying the resolution after a failure due to retry-able exception.
        /// </param>
        /// <param name="cancellationToken">
        /// <para>
        /// The CancellationToken that this operation is observing. It is used to notify the operation that it should be canceled.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding service resolution operation. The result from
        /// the Task is the <see cref="System.Fabric.ResolvedServicePartition" /> object, that contains the information
        /// about the resolved service partition including the service endpoints.
        /// </returns>
        Task<ResolvedServicePartition> ResolveAsync(
            ResolvedServicePartition previousRsp,
            TimeSpan resolveTimeoutPerTry,
            TimeSpan maxRetryBackoffInterval,
            CancellationToken cancellationToken);
    }
}
