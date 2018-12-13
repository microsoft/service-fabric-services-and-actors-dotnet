// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Fabric;
    using System.Fabric.Common;
    using System.Fabric.Description;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Represents a delegate to create a FabricClient object.
    /// </summary>
    /// <returns>FabricClient</returns>
    public delegate FabricClient CreateFabricClientDelegate();

    /// <summary>
    /// <para>
    /// Implements the Service partition resolver class that uses the <see cref="System.Fabric.FabricClient">FabricClient's </see> <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" /> method for service resolution
    /// and implements a back-off/retry mechanism on errors from that method.
    /// </para>
    /// </summary>
    public class ServicePartitionResolver : IServicePartitionResolver
    {
        /// <summary>
        /// The default resolve timeout per try used by the ResolveAsync method of <see cref="ServicePartitionResolver"/> when it is
        /// invoked without explicitly specifying the resolveTimeoutPerTry argument. The default value is 30 seconds.
        /// </summary>
        public static readonly TimeSpan DefaultResolveTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The default maximum back-off time used by ServicePartitionResolver's ResolveAsync method before retrying, when it is
        /// invoked without explicitly specifying the maxRetryBackoffInterval argument. The default value is 5 seconds.
        /// </summary>
        public static readonly TimeSpan DefaultMaxRetryBackoffInterval = TimeSpan.FromSeconds(5);

        private static readonly object StaticLock = new object();
        private static readonly Random Rand = new Random();

        private static ServicePartitionResolver defaultResolver;

        private readonly object thisLock = new object();
        private readonly RandomGenerator randomGenerator = new RandomGenerator();
        private readonly CreateFabricClientDelegate createFabricClient;
        private readonly CreateFabricClientDelegate recreateFabricClient;
        private FabricClient fabricClient;
        private ConcurrentDictionary<Uri, bool> registrationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionResolver"/> class.
        /// </summary>
        /// <param name="createFabricClient">Delegate to create the fabric client.</param>
        /// <param name="recreateFabricClient">Delegate to re-create the fabric client.</param>
        /// <remarks>The first delegate is used to create <see cref="System.Fabric.FabricClient">FabricClient.</see>.
        /// During partition resolution if FabricClient object gets disposed and second delegate is provided,
        /// it uses the second delegate to create the FabricClient again. The second delegate provides a way to specify
        /// an alternate way to get or create FabricClient if FabricClient created with first delegate get disposed.
        /// </remarks>
        public ServicePartitionResolver(
            CreateFabricClientDelegate createFabricClient,
            CreateFabricClientDelegate recreateFabricClient)
        {
            this.createFabricClient = createFabricClient;
            this.recreateFabricClient = recreateFabricClient ?? createFabricClient;
            this.UseNotification = true;
            this.registrationCache = new ConcurrentDictionary<Uri, bool>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionResolver"/> class.
        /// The constructor invokes the given delegate to create an instance of <see cref="System.Fabric.FabricClient">FabricClient</see>
        /// that is used for connecting to a Service Fabric cluster and perform service resolution.
        /// </summary>
        /// <param name="createFabricClient">Delegate to create fabric client.</param>
        public ServicePartitionResolver(
            CreateFabricClientDelegate createFabricClient)
            : this(createFabricClient, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionResolver"/> class.
        /// The constructor uses given connectionEndpoints to create an instance of <see cref="System.Fabric.FabricClient">FabricClient</see>
        /// that is used for connecting to a Service Fabric cluster and perform service resolution.
        /// </summary>
        /// <param name="connectionEndpoints">Array of management endpoints of the cluster.</param>
        public ServicePartitionResolver(params string[] connectionEndpoints)
            : this(() => new FabricClient(connectionEndpoints))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionResolver"/> class.
        /// The constructor uses given settings and connectionEndpoints to create an instance of <see cref="System.Fabric.FabricClient">FabricClient</see>
        /// that is used for connecting to a Service Fabric cluster and perform service resolution.
        /// </summary>
        /// <param name="settings">Fabric client Settings.</param>
        /// <param name="connectionEndpoints">Array of management endpoints of the cluster.</param>
        public ServicePartitionResolver(
            FabricClientSettings settings,
            params string[] connectionEndpoints)
            : this(() => new FabricClient(settings, connectionEndpoints))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionResolver"/> class.
        /// The constructor uses the given security credentials and the connectionEndpoints to create an instance of <see cref="System.Fabric.FabricClient">FabricClient</see>
        /// that is used for connecting to a Service Fabric cluster and perform service resolution.
        /// </summary>
        /// <param name="credential">Security credentials for the fabric client.</param>
        /// <param name="connectionEndpoints">Array of management endpoints of the cluster.</param>
        public ServicePartitionResolver(
            SecurityCredentials credential,
            params string[] connectionEndpoints)
            : this(() => new FabricClient(credential, connectionEndpoints))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicePartitionResolver"/> class.
        /// The constructor uses the given security credentials, settings and the connectionEndpoints to create an instance of <see cref="System.Fabric.FabricClient">FabricClient</see>
        /// that is used for connecting to a Service Fabric cluster and perform service resolution.
        /// </summary>
        /// <param name="credential">Security credentials for the fabric client.</param>
        /// <param name="settings">Fabric client Settings.</param>
        /// <param name="connectionEndpoints">Array of management endpoints of the cluster.</param>
        public ServicePartitionResolver(
            SecurityCredentials credential,
            FabricClientSettings settings,
            params string[] connectionEndpoints)
            : this(() => new FabricClient(credential, settings, connectionEndpoints))
        {
        }

        internal bool UseNotification { get; set; }

        /// <summary>
        /// Updates the default ServicePartitionResolver.
        /// </summary>
        /// <param name="defaultServiceResolver">The new default value</param>
        public static void SetDefault(ServicePartitionResolver defaultServiceResolver)
        {
            lock (StaticLock)
            {
                defaultResolver = defaultServiceResolver;
            }
        }

        /// <summary>
        /// Gets the default ServicePartitionResolver.
        /// <remarks>
        /// <para>
        /// The default service partition resolver instance uses the local <see href="https://docs.microsoft.com/en-us/dotnet/api/system.fabric.fabricclient#System_Fabric_FabricClient__ctor">fabric client</see>.
        /// If you are using the ServicePartitionResolver to resolve services that are running on a remote cluster, the recommended practice is to create a ServicePartitionResolver using the appropriate endpoints or FabricClient and then update
        /// the default ServicePartitionResolver.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <returns>Default <see cref="ServicePartitionResolver"/></returns>
        public static ServicePartitionResolver GetDefault()
        {
            lock (StaticLock)
            {
                if (defaultResolver == null)
                {
                    defaultResolver = new ServicePartitionResolver(
                        () => new FabricClient(
                            new FabricClientSettings()
                            {
                                PartitionLocationCacheBucketCount = 4096,
                                PartitionLocationCacheLimit = 4096,
                            }));
                }

                return defaultResolver;
            }
        }

        /// <summary>
        /// <para>
        /// Resolves a partition of the specified service by invoking FabricClient's
        /// <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method. This uses the default settings for
        /// <see cref="DefaultResolveTimeout">timeout</see> and <see cref="DefaultMaxRetryBackoffInterval">back-off retry</see> intervals.
        /// </para>
        /// </summary>
        /// <param name="serviceUri">Name of the service instance to resolve.</param>
        /// <param name="partitionKey">
        /// <para>
        /// <see cref="ServicePartitionKey">Key</see> that determines the target partition of the service instance. The <see cref="ServicePartitionKind">partitioning scheme</see>
        /// specified in the key should match the partitioning scheme used to create the service instance.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>
        /// The CancellationToken that this operation is observing. It is used to notify the operation that it should be canceled.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result from
        /// the task is the <see cref="System.Fabric.ResolvedServicePartition" /> object, that contains the information
        /// about the resolved service partition including the service endpoints.
        /// </returns>
        /// <exception cref="System.Fabric.FabricServiceNotFoundException">
        /// <para>
        /// This method can throw a FabricServiceNotFoundExcepion if there is no service instance in the cluster matching the specified serviceUri.
        /// </para>
        /// </exception>
        /// <exception cref="System.Fabric.FabricException">
        /// <para>
        /// This method can throw a FabricException if the scheme specified in the ServicePartitionKey doesn't match the scheme used to create the service instance.
        /// See also <see href="https://azure.microsoft.com/documentation/articles/service-fabric-errors-and-exceptions/">Errors and Exceptions</see> for handling common FabricClient failures.
        /// </para>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method retries on all transient exceptions. For cases where you want to limit the max execution time of this method, you should create a <see href="https://docs.microsoft.com/en-us/dotnet/core/api/system.threading.cancellationtokensource#System_Threading_CancellationTokenSource__ctor_System_TimeSpan_">cancellation token associated with that max execution time</see>
        /// and pass that cancellation token to this method.
        /// </para>
        /// </remarks>
        public Task<ResolvedServicePartition> ResolveAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            CancellationToken cancellationToken)
        {
            return this.ResolveAsync(
                serviceUri,
                partitionKey,
                DefaultResolveTimeout,
                DefaultMaxRetryBackoffInterval,
                cancellationToken);
        }

        /// <summary>
        /// Resolves a partition of the specified service by invoking FabricClient's
        /// <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" /> method with the given timeout and back-off/retry on retry-able errors.
        /// </summary>
        /// <param name="serviceUri">Name of the service instance to resolve.</param>
        /// <param name="partitionKey">
        /// <para>
        /// <see cref="ServicePartitionKey">Key</see> that determines the target partition of the service instance. The <see cref="ServicePartitionKind">partitioning scheme</see>
        /// specified in the key should match the partitioning scheme used to create the service instance.
        /// </para>
        /// </param>
        /// <param name="resolveTimeoutPerTry">The timeout passed to FabricClient's <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method.</param>
        /// <param name="maxRetryBackoffInterval">
        /// <para>
        /// The max interval to back-off before retrying when FabricClient's <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method fails with a retry-able exception.
        /// The actual back off interval is a random time interval which is less than or equal to the specified maxRetryBackoffInterval.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>
        /// The CancellationToken that this operation is observing. It is used to notify the operation that it should be canceled.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result from
        /// the task is the <see cref="System.Fabric.ResolvedServicePartition" /> object, that contains the information
        /// about the resolved service partition including the service endpoints.
        /// </returns>
        /// <exception cref="System.Fabric.FabricServiceNotFoundException">
        /// <para>
        /// This method can throw a FabricServiceNotFoundExcepion if there is no service instance in the cluster matching the specified serviceUri.
        /// </para>
        /// </exception>
        /// <exception cref="System.Fabric.FabricException">
        /// <para>
        /// This can throw a FabricException if the scheme specified in the ServicePartitionKey doesn't match the scheme used to create the service instance.
        /// See also <see href="https://azure.microsoft.com/documentation/articles/service-fabric-errors-and-exceptions/">Errors and Exceptions</see> for more information.
        /// </para>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method retries on all transient exceptions. For cases where you want to limit the max execution time of this method, you should create a <see href="https://docs.microsoft.com/en-us/dotnet/core/api/system.threading.cancellationtokensource#System_Threading_CancellationTokenSource__ctor_System_TimeSpan_">cancellation token associated with that max execution time</see>
        /// and pass that cancellation token to this method.
        /// </para>
        /// </remarks>
        public Task<ResolvedServicePartition> ResolveAsync(
            Uri serviceUri,
            ServicePartitionKey partitionKey,
            TimeSpan resolveTimeoutPerTry,
            TimeSpan maxRetryBackoffInterval,
            CancellationToken cancellationToken)
        {
            if (partitionKey == null)
            {
                partitionKey = ServicePartitionKey.Singleton;
            }

            switch (partitionKey.Kind)
            {
                case ServicePartitionKind.Singleton:
                    {
                        return this.ResolveHelperAsync(
                            (client, prevRsp, timeout, cancellation) => ResolveSingletonPartitionAsync(
                                client,
                                serviceUri,
                                prevRsp,
                                timeout,
                                cancellation),
                            null,
                            resolveTimeoutPerTry,
                            maxRetryBackoffInterval,
                            cancellationToken,
                            serviceUri);
                    }

                case ServicePartitionKind.Named:
                    {
                        return this.ResolveHelperAsync(
                            (client, prevRsp, timeout, cancellation) => ResolveNamedPartitionAsync(
                                client,
                                serviceUri,
                                (string)partitionKey.Value,
                                prevRsp,
                                timeout,
                                cancellation),
                            null,
                            resolveTimeoutPerTry,
                            maxRetryBackoffInterval,
                            cancellationToken,
                            serviceUri);
                    }

                case ServicePartitionKind.Int64Range:
                    {
                        return this.ResolveHelperAsync(
                            (client, prevRsp, timeout, cancellation) => ResolveInt64PartitionAsync(
                                client,
                                serviceUri,
                                (long)partitionKey.Value,
                                prevRsp,
                                timeout,
                                cancellation),
                            null,
                            resolveTimeoutPerTry,
                            maxRetryBackoffInterval,
                            cancellationToken,
                            serviceUri);
                    }

                default:
                    throw new ArgumentOutOfRangeException("partitionKey");
            }
        }

        /// <summary>
        /// Resolves a partition of the specified service by invoking FabricClient's
        /// <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method with back-off/retry on retry-able errors. This takes in
        /// the resolved service partition that was got via an earlier invocation of the ResolveAsync() method.
        /// This method overload is used in cases where the client knows that the resolved service partition that it has is no longer valid.
        /// </summary>
        /// <param name="previousRsp">The resolved service partition that the client got from the earlier invocation of the ResolveAsync() method.</param>
        /// <param name="cancellationToken">
        /// <para>
        /// The CancellationToken that this operation is observing. It is used to notify the operation that it should be canceled.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result from
        /// the task is the <see cref="System.Fabric.ResolvedServicePartition" /> object, that contains the information
        /// about the resolved service partition including the service endpoints.
        /// </returns>
        /// <exception cref="System.Fabric.FabricServiceNotFoundException">
        /// <para>
        /// This method can throw a FabricServiceNotFoundExcepion if the service which was resolved previously is no longer present in the cluster.
        /// </para>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method retries on all transient exceptions. For cases where you want to limit the max execution time of this method, you should create a <see href="https://docs.microsoft.com/en-us/dotnet/core/api/system.threading.cancellationtokensource#System_Threading_CancellationTokenSource__ctor_System_TimeSpan_">cancellation token associated with that max execution time</see>
        /// and pass that cancellation token to this method.
        /// </para>
        /// </remarks>
        public Task<ResolvedServicePartition> ResolveAsync(
            ResolvedServicePartition previousRsp,
            CancellationToken cancellationToken)
        {
            return this.ResolveAsync(
                previousRsp,
                DefaultResolveTimeout,
                DefaultMaxRetryBackoffInterval,
                cancellationToken);
        }

        /// <summary>
        /// Resolves a partition of the specified service by invoking FabricClient's
        /// <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method with back-off/retry on retry-able errors. This takes in
        /// the resolved service partition that was got via an earlier invocation of the ResolveAsync() method.
        /// This method overload is used in cases where the client knows that the resolved service partition that it has is no longer valid.
        /// </summary>
        /// <param name="previousRsp">The resolved service partition that the client got from the earlier invocation of the ResolveAsync() method.</param>
        /// <param name="resolveTimeoutPerTry">The timeout passed to FabricClient's <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method </param>
        /// <param name="maxRetryBackoffInterval">
        /// <para>
        /// The max interval to back-off before retrying when FabricClient's <see cref="FabricClient.ServiceManagementClient.ResolveServicePartitionAsync(System.Uri)" />method fails with a retry-able exception.
        /// The actual back off interval is a random time interval which is less than or equal to the specified maxRetryBackoffInterval.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">
        /// <para>
        /// The CancellationToken that this operation is observing. It is used to notify the operation that it should be canceled.
        /// </para>
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation. The result from
        /// the task is the <see cref="System.Fabric.ResolvedServicePartition" /> object, that contains the information
        /// about the resolved service partition including the service endpoints.
        /// </returns>
        /// <exception cref="System.Fabric.FabricServiceNotFoundException">
        /// <para>
        /// This method can throw a FabricServiceNotFoundExcepion if the service which was resolved previously is no longer present in the cluster.
        /// </para>
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method retries on all transient exceptions. For cases where you want to limit the max execution time of this method, you should create a <see href="https://docs.microsoft.com/en-us/dotnet/core/api/system.threading.cancellationtokensource#System_Threading_CancellationTokenSource__ctor_System_TimeSpan_">cancellation token associated with that max execution time</see>
        /// and pass that cancellation token to this method.
        /// </para>
        /// </remarks>
        public Task<ResolvedServicePartition> ResolveAsync(
            ResolvedServicePartition previousRsp,
            TimeSpan resolveTimeoutPerTry,
            TimeSpan maxRetryBackoffInterval,
            CancellationToken cancellationToken)
        {
            var serviceName = previousRsp.ServiceName;
            switch (previousRsp.Info.Kind)
            {
                case ServicePartitionKind.Singleton:
                    {
                        return this.ResolveHelperAsync(
                            (client, prevRsp, timeout, cancellation) => ResolveSingletonPartitionAsync(
                                client,
                                serviceName,
                                prevRsp,
                                timeout,
                                cancellation),
                            previousRsp,
                            resolveTimeoutPerTry,
                            maxRetryBackoffInterval,
                            cancellationToken,
                            serviceName);
                    }

                case ServicePartitionKind.Named:
                    {
                        var partitionName = ((NamedPartitionInformation)previousRsp.Info).Name;
                        return this.ResolveHelperAsync(
                            (client, prevRsp, timeout, cancellation) => ResolveNamedPartitionAsync(
                                client,
                                serviceName,
                                partitionName,
                                prevRsp,
                                timeout,
                                cancellation),
                            previousRsp,
                            resolveTimeoutPerTry,
                            maxRetryBackoffInterval,
                            cancellationToken,
                            serviceName);
                    }

                case ServicePartitionKind.Int64Range:
                    {
                        var partitionKey = ((Int64RangePartitionInformation)previousRsp.Info).LowKey;
                        return this.ResolveHelperAsync(
                            (client, prevRsp, timeout, cancellation) => ResolveInt64PartitionAsync(
                                client,
                                serviceName,
                                partitionKey,
                                prevRsp,
                                timeout,
                                cancellation),
                            previousRsp,
                            resolveTimeoutPerTry,
                            maxRetryBackoffInterval,
                            cancellationToken,
                            serviceName);
                    }

                default:
                    throw new ArgumentOutOfRangeException("previousRsp");
            }
        }

        private static Task<ResolvedServicePartition> ResolveSingletonPartitionAsync(
            FabricClient client,
            Uri serviceName,
            ResolvedServicePartition previousRsp,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            return client.ServiceManager.ResolveServicePartitionAsync(
                serviceName,
                previousRsp,
                timeout,
                cancellationToken);
        }

        private static Task<ResolvedServicePartition> ResolveNamedPartitionAsync(
            FabricClient client,
            Uri serviceName,
            string partitionKey,
            ResolvedServicePartition previousRsp,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            return client.ServiceManager.ResolveServicePartitionAsync(
                serviceName,
                partitionKey,
                previousRsp,
                timeout,
                cancellationToken);
        }

        private static Task<ResolvedServicePartition> ResolveInt64PartitionAsync(
            FabricClient client,
            Uri serviceName,
            long partitionKey,
            ResolvedServicePartition previousRsp,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            return client.ServiceManager.ResolveServicePartitionAsync(
                serviceName,
                partitionKey,
                previousRsp,
                timeout,
                cancellationToken);
        }

        private async Task<ResolvedServicePartition> ResolveHelperAsync(
            Func<FabricClient, ResolvedServicePartition,
            TimeSpan,
            CancellationToken,
            Task<ResolvedServicePartition>> resolveFunc,
            ResolvedServicePartition previousRsp,
            TimeSpan resolveTimeout,
            TimeSpan maxRetryInterval,
            CancellationToken cancellationToken,
            Uri serviceUri)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                var totaltime = new TimeoutHelper(resolveTimeout);
                var client = this.GetClient();
                ResolvedServicePartition rsp = null;

                // resolve and get the rsp
                try
                {
                    rsp = await resolveFunc.Invoke(
                        client,
                        previousRsp,
                        totaltime.GetRemainingTime(),
                        CancellationToken.None);
                }
                catch (AggregateException ae)
                {
                    ae.Handle(
                        x =>
                        {
                            if ((x is FabricTransientException) ||
                                (x is TimeoutException) ||
                                (x is OperationCanceledException))
                            {
                                return true;
                            }

                            return false;
                        });
                }
                catch (FabricTransientException)
                {
                }
                catch (TimeoutException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (FabricObjectClosedException)
                {
                    // retry on the different client
                    this.ReportFaulted(client);
                }

                // check if the rsp is valid
                try
                {
                    if (rsp != null)
                    {
                        rsp.GetEndpoint();
                        try
                        {
                            // Registering for Notification only for the first request for a service uri.
                            if (this.UseNotification && !this.registrationCache.ContainsKey(serviceUri))
                            {
                                var added = this.registrationCache.TryAdd(serviceUri, true);
                                if (added)
                                {
                                    ServiceNotificationFilterDescription filter = new ServiceNotificationFilterDescription(
                                       name: serviceUri,
                                       matchNamePrefix: true,
                                       matchPrimaryChangeOnly: false);
                                    await client.ServiceManager.RegisterServiceNotificationFilterAsync(filter, totaltime.GetRemainingTime(), CancellationToken.None);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // Remove the Entry so that in next call we can again try registeration
                            bool res;
                            this.registrationCache.TryRemove(serviceUri, out res);
                        }

                        return rsp;
                    }
                }
                catch (FabricException)
                {
                    // retry if no suitable endpoints found from the RSP
                }

                previousRsp = rsp;

                // wait before retry
                await Task.Delay(
                       new TimeSpan((long)(this.randomGenerator.NextDouble() * maxRetryInterval.Ticks)),
                       cancellationToken);
            }
        }

        private FabricClient GetClient()
        {
            lock (this.thisLock)
            {
                if (this.fabricClient == null)
                {
                    this.fabricClient = this.createFabricClient.Invoke();
                }

                return this.fabricClient;
            }
        }

        private void ReportFaulted(FabricClient client)
        {
            lock (this.thisLock)
            {
                if (ReferenceEquals(client, this.fabricClient))
                {
                    this.fabricClient = this.recreateFabricClient.Invoke();
                }
            }
        }
    }
}
